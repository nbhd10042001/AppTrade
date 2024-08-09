using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using Microsoft.AspNetCore.Identity;
using App.Utilities;
using App.Areas.Product.Models;
using App.Models.Product;

namespace App.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/productmanage/{action}/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Member)]
    public class ProductManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage {set; get;}
        [TempData]
        public string TypeStatusMessage {set; get;}

        // GET
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage) // du lieu URL co query la p se binding vao bien currentPage
        {
            var products = _context.Products
                                    .Include(p => p.Author)
                                    .Include(p => p.ProductCategoryProducts)
                                    .ThenInclude(pc => pc.CategoryProduct)
                                    .OrderByDescending(p => p.DateUpdated);

            const int ITEMS_PER_PAGE = 10;

            int totalproducts = await products.CountAsync();
            int countPages = (int)Math.Ceiling((double)totalproducts / ITEMS_PER_PAGE);

            if (currentPage > countPages)  currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new {p = pageNumber})
            };

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalproducts = totalproducts;
            ViewBag.productIndex = (currentPage - 1) * ITEMS_PER_PAGE; // danh STT cac products

            var productsInPage = await products.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                                        .Take(ITEMS_PER_PAGE)
                                        .ToListAsync();

            return View(productsInPage);
        }

        // GET: /product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: product/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categoriesSelect"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published, CategoryIDs, Price, FileProductUpload")] CreateProductModel product)
        {
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categoriesSelect"] = new MultiSelectList(categories, "Id", "Title");

            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Nhap chuoi URL khac");
                return View(product);
            }

            if (product.Slug == null)
                product.Slug = AppUtilities.GenerateSlug(product.Title);

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                product.DateCreated = product.DateUpdated = DateTime.Now;
                product.AuthorId = user.Id;
                _context.Add(product);

                if (product.CategoryIDs != null)
                {
                    foreach (var CateId in product.CategoryIDs)
                    {
                        _context.Add(new ProductCategoryProduct(){
                            CategoryProductID = CateId,
                            Product = product
                        });
                    }
                }
                await _context.SaveChangesAsync();

                if(product.FileProductUpload != null)
                {
                    // phat sinh ten file ngau nhien de tranh bi trung
                    var fileNameRandom = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                                + Path.GetExtension(product.FileProductUpload.FileName);
                    
                    // ~/Uploads/Products/img.png
                    var filePath = Path.Combine("Uploads", "Products", fileNameRandom);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.FileProductUpload.CopyToAsync(fileStream);
                    }

                    var pathFile = Path.Combine("/contents/Products", fileNameRandom);
                    _context.ProductPhotos.Add(new ProductPhotoModel(){
                        ProductID = product.ProductId,
                        FileName = pathFile
                    });
                }
                await _context.SaveChangesAsync();

                StatusMessage = @$"Ban vua tao San pham moi: ""{product.Title}""";
                TypeStatusMessage = TypeMessage.Success;

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // var post = await _context.Products.FindAsync(id);
            var product = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            var productEdit = new CreateProductModel(){
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Slug = product.Slug,
                CategoryIDs = product.ProductCategoryProducts.Select(pcp => pcp.CategoryProductID).ToArray(),
                Price = product.Price
            };

            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categoriesSelect"] = new MultiSelectList(categories, "Id", "Title");

            return View(productEdit);
        }

        // POST: product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Published, CategoryIDs, Price")] CreateProductModel product)
        {
            if (id != product.ProductId) return NotFound();

            if (product.Slug == null)
                product.Slug = AppUtilities.GenerateSlug(product.Title);

            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug && p.ProductId != id))
            {
                ModelState.AddModelError("Slug", "Nhap chuoi URL khac");
                return View(product);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productUpdate = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
                    if (productUpdate == null) return NotFound();

                    productUpdate.Title = product.Title;
                    productUpdate.Description = product.Description;
                    productUpdate.Slug = product.Slug;
                    productUpdate.DateUpdated = DateTime.Now;
                    productUpdate.Price = product.Price;

                    // update productcategories
                    if (product.CategoryIDs == null) product.CategoryIDs = new int[] {};

                    var oldCateIDs = productUpdate.ProductCategoryProducts.Select(c => c.CategoryProductID).ToArray();
                    var newCateIDs = product.CategoryIDs;

                    var removeCateProducts = from productCate in productUpdate.ProductCategoryProducts
                                            where (!newCateIDs.Contains(productCate.CategoryProductID))
                                            select productCate;

                    _context.ProductCategoryProducts.RemoveRange(removeCateProducts);

                    var addCateIDs = from CateId in newCateIDs
                                        where !oldCateIDs.Contains(CateId)
                                        select CateId;

                    // tao ra moi quan he giua Product va CateProduct
                    foreach (var CateId in addCateIDs) {
                        _context.ProductCategoryProducts.Add(new ProductCategoryProduct(){
                            ProductID = id,
                            CategoryProductID = CateId
                        });
                    }

                    _context.Update(productUpdate);
                    await _context.SaveChangesAsync();

                    StatusMessage = @$"Ban vua cap nhat San pham: ""{productUpdate.Title}""";
                    TypeStatusMessage = TypeMessage.Success;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categoriesSelect"] = new MultiSelectList(categories, "Id", "Title");

            return View(product);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);

            // delete all photo of product
            var photos = _context.ProductPhotos.Where(p => p.ProductID == product.ProductId).ToList();
            _context.ProductPhotos.RemoveRange(photos);
            foreach(var photo in photos)
            {
                var filePath = photo.FileName;
                filePath = filePath.Replace("/contents", "Uploads");
                System.IO.File.Delete(filePath);
            }

            await _context.SaveChangesAsync();
            StatusMessage = @$"Ban da xoa San pham: ""{product.Title}""";
            TypeStatusMessage = TypeMessage.Danger;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        [HttpGet]
        public IActionResult UploadPhoto(int id)
        {
            var product = _context.Products.Where(p => p.ProductId == id)
                                            .Include(p => p.Photos)
                                            .FirstOrDefault();
            if (product == null) return NotFound("Khong tim thay san pham");

            ViewBag.product = product;
            return View(new UploadOneFile());
        }

        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int id, [Bind("FileUpload")] UploadOneFile fileUp)
        {
            var product = _context.Products.Where(p => p.ProductId == id)
                                            .Include(p => p.Photos)
                                            .FirstOrDefault();
            if (product == null) return NotFound("Khong tim thay san pham");

            if( fileUp != null)
            {
                // phat sinh ten file ngau nhien de tranh bi trung
                var fileNameRandom = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(fileUp.FileUpload.FileName);
                
                // ~/Uploads/Products/img.png
                var filePath = Path.Combine("Uploads", "Products", fileNameRandom);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUp.FileUpload.CopyToAsync(fileStream);
                }

                var pathFile = Path.Combine("/contents/Products", fileNameRandom);
                _context.ProductPhotos.Add(new ProductPhotoModel(){
                    ProductID = product.ProductId,
                    FileName = pathFile
                });
                await _context.SaveChangesAsync();
            }

            ViewBag.product = product;
            return View(fileUp);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoAPI(int id, [Bind("FileUpload")] UploadOneFile fileUp)
        {
            var product = _context.Products.Where(p => p.ProductId == id)
                                            .Include(p => p.Photos)
                                            .FirstOrDefault();
            if (product == null) return NotFound("Khong tim thay san pham");

            if( fileUp != null)
            {
                // phat sinh ten file ngau nhien de tranh bi trung
                var fileNameRandom = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(fileUp.FileUpload.FileName);
                
                // ~/Uploads/Products/img.png
                var filePath = Path.Combine("Uploads", "Products", fileNameRandom);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUp.FileUpload.CopyToAsync(fileStream);
                }

                var pathFile = Path.Combine("/contents/Products", fileNameRandom);
                _context.ProductPhotos.Add(new ProductPhotoModel(){
                    ProductID = product.ProductId,
                    FileName = pathFile
                });
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ListPhotos(int id)
        {
            var product = _context.Products.Where(p => p.ProductId == id)
                                            .Include(p => p.Photos)
                                            .FirstOrDefault();

            if (product == null) return Json(
                new {
                    success = 0,
                    message = "Khong tim thay san pham"
                }
            );

            var listPhotos = product.Photos.Select(photo => new {
                id = photo.Id,
                path = photo.FileName
            });

            return Json(
                new {
                    success = 1,
                    photos = listPhotos
                }
            );
        }

        [HttpPost]
        public IActionResult DeletePhoto (int id)
        {
            var photo = _context.ProductPhotos.Where(p => p.Id == id).FirstOrDefault();
            if (photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();
                // var filePath = "Uploads/Products/" + photo.FileName;
                var filePath = photo.FileName;
                filePath = filePath.Replace("/contents", "Uploads");
                System.IO.File.Delete(filePath);
            }
            return Ok();
        }
    }
}
