
using App.Areas.Product.Models;
using App.Areas.Product.Models.Services;
using App.Data;
using App.Models;
using App.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Product.Controllers
{
    [Area("Product")]
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

        [Route("product/{categoryslug?}")]
        public ActionResult Index(string categoryslug, [FromQuery(Name = "p")]int currentPage)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;

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

            // lay ra cac post co idcate == id cateSelect
            if (categoryChoosed != null)
            {
                var ids = new List<int>();
                categoryChoosed.GetChildCategoryIDs(ids, null);
                ids.Add(categoryChoosed.Id);
                products = products.Where(p => p.ProductCategoryProducts.Where(pc => ids.Contains(pc.CategoryProductID)).Any());
            }

            products = products.OrderByDescending(p => p.DateUpdated);

            // pagingModel------------------------------------------------------------
            const int ITEMS_PER_PAGE = 6;
            int totalProducts = products.Count();
            int countPages = (int)Math.Ceiling((double)totalProducts / ITEMS_PER_PAGE);

            if (currentPage > countPages)  currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new {p = pageNumber})
            };

            var productsInPage = products.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                                    .Take(ITEMS_PER_PAGE);

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalProducts = totalProducts;

            ViewBag.categoryChoosed = categoryChoosed;

            return View(productsInPage.ToList());
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
        [Route ("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart ([FromRoute] int productid) {

            var product = _context.Products
                .Where (p => p.ProductId == productid)
                .FirstOrDefault ();
            if (product == null)
                return NotFound ("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            } else {
                //  Thêm mới
                cart.Add (new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction (nameof (Cart));
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
            TypeStatusMessage = TypeName.Success;
            return RedirectToAction(nameof(Cart));
        }
    }
}
