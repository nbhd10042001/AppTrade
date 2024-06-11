
using App.Areas.Product.Models;
using App.Areas.Product.Models.Services;
using App.Data;
using App.Models;
using App.Models.Product;
using App.Services;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace App.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("viewproduct/{action}")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly CartService _cartService;
        private readonly AppDbContext _context;

        public ViewProductController(ILogger<ViewProductController> logger,
                                     AppDbContext context,
                                     CartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
        }

        [TempData]
        public string StatusMessage{set; get;}
        [TempData]
        public string TypeStatusMessage{set; get;}

        [HttpPost]
        [Route("product/filter")]
        public IActionResult ListFilter ([Bind("selectFilters, selectCategories, SearchBar")] SelectFilters filters)
        {
            List<int> filterIDs = new List<int>();
            List<int> filterCateIDs = new List<int>();

            if(filters.selectCategories != null)
                foreach (var filter in filters.selectCategories)
                    filterCateIDs.Add(Int32.Parse(filter));

            if(filters.selectFilters != null)
                foreach (var filter in filters.selectFilters)
                    filterIDs.Add(Int32.Parse(filter));

            return RedirectToAction(nameof(Index), new {f = filterIDs, fc = filterCateIDs, search = filters.SearchBar});
        }

        [Route("product/{categoryslug?}")]
        public ActionResult Index(string categoryslug, 
                                [FromQuery(Name = "p")]int currentPage, 
                                [FromQuery(Name = "f")]List<int> filters, 
                                [FromQuery(Name = "fc")]List<int> filterCates,
                                [FromQuery(Name = "search")]string searchBar
                                )
        {
            CategoryProductModel categoryChoosed = null; 
            // kiem tra neu url co query categoryslug thi thuc hien lay category 
            if (!string.IsNullOrEmpty(categoryslug))
            {
                categoryChoosed = _context.CategoryProducts.Where(c => c.Slug == categoryslug)
                                            .Include(c => c.ChildrenCategory)
                                            .FirstOrDefault();

                if (categoryChoosed == null) return NotFound("Khong tim thay chuyen muc");
            }

            var products = _context.Products.Include(p => p.Author)
                                        .Include(p => p.Photos)
                                        .Include(p => p.ProductCategoryProducts)
                                        .ThenInclude(pc => pc.CategoryProduct)
                                        .AsQueryable();   

            // search bar
            if (!string.IsNullOrEmpty(searchBar))
            {
                products = products.Where(p => p.Title.Contains(searchBar));
            }

            // lay ra cac product co idcategory == id categorySelect
            if (categoryChoosed != null)
            {
                var ids = new List<int>();
                categoryChoosed.GetChildCategoryIDs(ids, null);
                ids.Add(categoryChoosed.Id);
                products = products.Where(p => p.ProductCategoryProducts.Where(pc => ids.Contains(pc.CategoryProductID)).Any());
            }

            // custome filter and sort product
            if (filters.Count == 0)
                filters.Add(101); // neu khong co filters nao thi mac dinh filters them vao la dateUpdated
            
            foreach(var f in filters)
            {
                if (f == 101){
                    products = products.OrderByDescending(p => p.DateUpdated);
                    break;
                }
                else if (f == 102){
                    products = products.OrderBy(p => p.DateUpdated);
                    break;
                }
                else if (f == 103){
                    products = products.OrderByDescending(p => p.Price);
                    break;
                }
                else if (f == 104){
                    products = products.OrderBy(p => p.Price);
                    break;
                }
            }
            
            // sort by category
            if(filterCates.Count > 0)
                products = products.Where(p => p.ProductCategoryProducts.Where(pc => filterCates.Contains(pc.CategoryProductID)).Any());

            // pagingModel------------------------------------------------------------
            const int ITEMS_PER_PAGE = 10;
            int totalProducts = products.Count();
            int countPages = (int)Math.Ceiling((double)totalProducts / ITEMS_PER_PAGE);

            if (currentPage > countPages)  currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new {p = pageNumber, f = filters, fc = filterCates, search = searchBar})
            };

            var productsInPage = products.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                                    .Take(ITEMS_PER_PAGE);

            // ViewBag
            var categories = GetCategories();
            ViewBag.categories = categories;

            ViewBag.categoryslug = categoryslug;
            ViewBag.pagingModel = pagingModel;
            ViewBag.totalProducts = totalProducts;

            // save thong tin lua chon cac filters (neu co)
            var allcategories = _context.CategoryProducts.ToList();
            ViewBag.MSLCategories = new MultiSelectList(allcategories, "Id", "Title", filterCates);
            ViewBag.filterSelected = filters;

            ViewBag.categoryChoosed = categoryChoosed;
            ViewBag.productsInPage = productsInPage.ToList();
            ViewBag.searchBar = searchBar;

            return View();
        }

        [Route("product/{productslug}.html")]
        public ActionResult Detail(string productslug)
        {
            // var categories = GetCategories();
            // ViewBag.categories = categories;

            var product = _context.Products.Where(p => p.Slug == productslug)
                                        .Include(p => p.Author)
                                        .Include(p => p.Photos)
                                        .Include(p => p.ProductCategoryProducts)
                                        .ThenInclude(pc => pc.CategoryProduct)
                                        .FirstOrDefault();
                                        
            if (product == null) return NotFound("Khong tim thay product");

            // CategoryProductModel category = product.ProductCategoryProducts.FirstOrDefault()?.CategoryProduct;
            // ViewBag.category = category;
            // ViewBag.categoryslug = category.Slug;
            
            return View(product);
        }


        // other method -------------------------------------------------
        private List<CategoryProductModel> GetCategories()
        {
            var categories = _context.CategoryProducts
                                .Include(c => c.ChildrenCategory)
                                .AsEnumerable()
                                .Where(c => c.ParentCategory == null) // chi can lay cate cha vi trong query ta da include cac cate child
                                .ToList();

            return categories;
        }


        /// Thêm sản phẩm vào cart
        [HttpPost]
        public IActionResult AddToCart (int id) 
        {
            var product = _context.Products
                .Where (p => p.ProductId == id)
                .FirstOrDefault ();
            if (product == null) return NotFound();
            
            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find (p => p.product.ProductId == id);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            } else {
                //  Thêm mới
                cart.Add (new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);
            
            return Json(new {
                success = 1,
                message = id
            });
        }

        public IActionResult LoadCartCount ()
        {
            var cart = _cartService.GetCartItems();

            return Json(new {
                success = 1,
                count = cart.Count
            });
        }

        // Hiện thị giỏ hàng
        [Route ("/cart", Name = "cart")]
        public IActionResult Cart () 
        {
            return View (_cartService.GetCartItems());
        }

        /// xóa item trong cart
        [Route ("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart ([FromRoute] int productid) {
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction (nameof (Cart));
        }

        /// Cập nhật
        [Route ("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart ([FromForm] int productid, [FromForm] int quantity) {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            _cartService.SaveCartSession (cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        // gui don hang
        [Route ("/checkout")]
        public IActionResult Checkout ()
        {
            var cart = _cartService.GetCartItems();
            // .... code xu li du lieu gui don hang

            _cartService.ClearCart();

            StatusMessage = "Ban da gui don hang";
            TypeStatusMessage = TypeMessage.Success;
            return RedirectToAction(nameof(Cart));
        }
    }
}
