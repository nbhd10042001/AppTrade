using App.Data;
using App.Models;
using App.Models.Product;
using App.Utilities;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Operators;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/{action}")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbManageController(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage {set; get;}
        [TempData]
        public string TypeStatusMessage {set; get;}


        // GET: DbManageController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult DeleteDB()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> DeleteDBAsync()
        {
            var result = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = result ? "Xoa database thanh cong" : "Khong xoa duoc";
            TypeStatusMessage = result ? TypeMessage.Warning : TypeMessage.Danger;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MigrateAsync()
        {
            await _dbContext.Database.MigrateAsync();
            TypeStatusMessage = TypeMessage.Success;
            StatusMessage = "Da tao (cap nhat) database thanh cong";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeedAdminAsync()
        {
            // tao ra cac roles duoc dinh nghia san trong Data\RoleNames.cs
            // var roles = typeof(RoleName).GetFields().ToList();
            // foreach (var role in roles)
            // {
            //     var roleName = (string)role.GetRawConstantValue();
            //     var isHasRole = await _roleManager.FindByNameAsync(roleName);
            //     if(isHasRole == null)
            //     {
            //         await _roleManager.CreateAsync(new IdentityRole(roleName));
            //     }
            // }

            // tao ra user Admin
            var userAdmin = await _userManager.FindByEmailAsync($"admin@example.com");
            if (userAdmin == null)
            {
                userAdmin = new AppUser()
                {
                    UserName = $"admin",
                    Email = $"admin@example.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(userAdmin, "123123");
                await _userManager.AddToRoleAsync(userAdmin, RoleName.Administrator);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> SeedDataAsync()
        {
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return this.Forbid();
            var rolesUser = await _userManager.GetRolesAsync(user);
            
            if(!rolesUser.Any(r => r == RoleName.Administrator))
            {
                return this.Forbid();
            }

            // SeedProductCategory();
            SeedProducts();

            StatusMessage = "Ban vua seed database";
            TypeStatusMessage = TypeMessage.Success;

            return RedirectToAction("Index");
        }

        private void SeedProducts()
        {
            // delete category seed
            _dbContext.CategoryProducts.RemoveRange(_dbContext.CategoryProducts.Where(c => c.Content.Contains("[fakeData]")));
            _dbContext.Products.RemoveRange(_dbContext.Products.Where(p => p.Description.Contains("[fakeData]")));
            _dbContext.SaveChanges();

            // create seed data categories
            var pathProduct = Path.Combine("Uploads", "Products");
            var pathProductWeapon = Path.Combine(pathProduct, "weapon");
            var ipadPath = Path.Combine(pathProduct, "ipad");
            var laptopPath = Path.Combine(pathProduct, "laptop");
            var watchPath = Path.Combine(pathProduct, "watch");
            var phonePath = Path.Combine(pathProduct, "phone");
            var wp_mg_Path = Path.Combine(pathProductWeapon, "mg");
            var wp_rifle_Path = Path.Combine(pathProductWeapon, "rifle");
            var wp_smg_Path = Path.Combine(pathProductWeapon, "smg");
            var wp_snip_Path = Path.Combine(pathProductWeapon, "snip");

            Dictionary<string, string> dictPath = new Dictionary<string, string>();
            dictPath.Add("wp_mg", wp_mg_Path);
            dictPath.Add("wp_rifle", wp_rifle_Path);
            dictPath.Add("wp_smg", wp_smg_Path);
            dictPath.Add("wp_snip", wp_snip_Path);
            dictPath.Add("ipad", ipadPath);
            dictPath.Add("laptop", laptopPath);
            dictPath.Add("watch", watchPath);
            dictPath.Add("phone", phonePath);

            var user = _userManager.GetUserAsync(this.User).Result;
            Random rand = new Random();
            List<ProductModel> products = new List<ProductModel>();
            List<ProductCategoryProduct> product_categories = new List<ProductCategoryProduct>();
            List<string> FileNames = new List<string>();

            foreach( KeyValuePair<string, string> kvp in dictPath )
            {
                // add category in database
                var CPmodel = new CategoryProductModel(){
                    Title = kvp.Key,
                    Content = $"Description for category {kvp.Key} [fakeData]",
                    Slug = AppUtilities.GenerateSlug(kvp.Key)
                };
                _dbContext.CategoryProducts.Add(CPmodel);

                // initial varible
                DirectoryInfo di = new DirectoryInfo(kvp.Value);
                FileInfo[] Images = di.GetFiles("*.*");
                FileNames.Clear();
                foreach (var img in Images)
                {
                    Console.WriteLine("img"+img);
                    FileNames.Add(img.Name);
                }

                // create seed data products
                products.Clear();
                product_categories.Clear();

                foreach(var filename in FileNames)
                {
                    var name = Path.GetFileNameWithoutExtension(filename);
                    var product = new ProductModel(){
                        Title = name,
                        Description = $"description for : {name} [fakeData]",
                        AuthorId = user.Id,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Price = rand.Next(500),
                        Slug = AppUtilities.GenerateSlug(name),
                    };
                    products.Add(product);
                    product_categories.Add(new ProductCategoryProduct(){
                        Product = product,
                        CategoryProduct = CPmodel
                    });
                }
                _dbContext.AddRange(products);
                _dbContext.AddRange(product_categories);
                _dbContext.SaveChanges();

                foreach(var filename in FileNames)
                {
                    var name = Path.GetFileNameWithoutExtension(filename);
                    var product = _dbContext.Products.Where(p => p.Title == name)
                                                .Include(p => p.Photos)
                                                .FirstOrDefault();

                    var changePath = kvp.Value.Replace("Uploads", "/contents");
                    var path = Path.Combine(changePath, filename);

                    _dbContext.ProductPhotos.Add(new ProductPhotoModel(){
                        ProductID = product.ProductId,
                        FileName = path
                    });
                }
                _dbContext.SaveChanges();
            }   
        }

        private void SeedProductCategory()
        {
            // de dam bao ko seed them nhieu category du thua, ta thuc hien xoa cac category fake da khoi tao truoc do
            _dbContext.CategoryProducts.RemoveRange(_dbContext.CategoryProducts.Where(c => c.Content.Contains("[fakeData]")));
            _dbContext.Products.RemoveRange(_dbContext.Products.Where(p => p.Description.Contains("[fakeData]")));
            _dbContext.SaveChanges();

            //-----------------Phat sinh Categories Product----------------------------------

            var fakerCategory = new Faker<CategoryProductModel>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"Nhom SP {cm++} " + fk.Lorem.Sentence(1,2).Trim('.'));
            fakerCategory.RuleFor(c => c.Content, fk => fk.Lorem.Sentences(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate_1 = fakerCategory.Generate();
                var cate_1_1 = fakerCategory.Generate();
                var cate_1_2 = fakerCategory.Generate();
            var cate_2 = fakerCategory.Generate();
                var cate_2_1 = fakerCategory.Generate();
                    var cate_2_1_1 = fakerCategory.Generate();

            cate_1_1.ParentCategory = cate_1;
            cate_1_2.ParentCategory = cate_1;
            cate_2_1.ParentCategory = cate_2;
                cate_2_1_1.ParentCategory = cate_2_1;

            var categories = new CategoryProductModel[] { cate_1, cate_1_1, cate_1_2, cate_2, cate_2_1, cate_2_1_1};
            _dbContext.CategoryProducts.AddRange(categories);


            //----------------------Phat sinh Products----------------------------------

            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;

            var fakerProduct = new Faker<ProductModel>();
            fakerProduct.RuleFor(p => p.AuthorId, fk => user.Id);
            fakerProduct.RuleFor(p => p.Description, fk => fk.Commerce.ProductDescription() + "[fakeData]");
            fakerProduct.RuleFor(p => p.DateCreated, fk => fk.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 12, 30)));
            fakerProduct.RuleFor(p => p.Slug, fk => fk.Lorem.Slug());
            fakerProduct.RuleFor(p => p.Title, fk => $"SP {bv++} " + fk.Commerce.ProductName());
            fakerProduct.RuleFor(p => p.Price, fk => int.Parse(fk.Commerce.Price(500, 10000, 0)));


            List<ProductModel> products = new List<ProductModel>();
            List<ProductCategoryProduct> product_categories = new List<ProductCategoryProduct>();

            for (int i = 0; i < 40; i++)
            {
                var product = fakerProduct.Generate();
                product.DateUpdated = product.DateCreated;
                products.Add(product);
                
                product_categories.Add(new ProductCategoryProduct(){
                    Product = product,
                    CategoryProduct = categories[rCateIndex.Next(categories.Length - 1)] // random ngau nhien 1 trong cac categories duoc phat sinh
                });
            }

            _dbContext.AddRange(products);
            _dbContext.AddRange(product_categories);

            _dbContext.SaveChanges();
        }

        
    }
}
