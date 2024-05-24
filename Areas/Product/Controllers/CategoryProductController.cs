using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Product;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.CodeAnalysis;
using App.Utilities;

namespace App.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/categoryproduct/category/[action]/{id?}/{oldparentid?}")]
    [Authorize(Roles = RoleName.Administrator)]

    public class CategoryProductController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryProductController(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage {set; get;}
        [TempData]
        public string TypeStatusMessage {set; get;}

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            var  qr = (from c in _context.CategoryProducts select c)
                        .Include(c => c.ParentCategory)
                        .Include(c => c.ChildrenCategory);
            
            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();

            return View(categories);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoryProduct = await _context.CategoryProducts
                                    .Include(c => c.ParentCategory)
                                    .FirstOrDefaultAsync(m => m.Id == id);

            if (categoryProduct == null) return NotFound();

            return View(categoryProduct);
        }

        // tuy bien cay thu muc select category
        private void CreateSelectItems(List<CategoryProductModel> source, List<CategoryProductModel> newCateProd, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("----", level));

            foreach (var cate in source)
            {
                // cate.Title = prefix + " " + cate.Title;
                newCateProd.Add(new CategoryProductModel(){
                    Id = cate.Id,
                    Title = prefix + " " + cate.Title
                });

                if (cate.ChildrenCategory?.Count > 0)
                {
                    CreateSelectItems(cate.ChildrenCategory.ToList(), newCateProd, level + 1);
                }
            }
        }

        private List<CategoryProductModel> SelectCategories ()
        {
            var  qr = (from c in _context.CategoryProducts select c)
                        .Include(c => c.ParentCategory)
                        .Include(c => c.ChildrenCategory);
            
            var categories = qr.ToList()
                            .Where(c => c.ParentCategory == null)
                            .ToList();

            categories.Insert(0, new CategoryProductModel(){
                Id = -1,
                Title = "Khong co danh muc cha"
            });

            var newCateProd = new List<CategoryProductModel>();
            CreateSelectItems(categories, newCateProd, 0);
            return newCateProd;
        }

        // GET: Blog/Category/Create
        public IActionResult Create()
        {
            var items = SelectCategories();
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View();
        }

        // POST: Blog/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentCategoryId,Title,Content,Slug")] CategoryProductModel CategoryProductModel)
        {
            if (ModelState.IsValid)
            {
                if (await _context.CategoryProducts.AnyAsync(cp => cp.Slug == CategoryProductModel.Slug))
                {
                    ModelState.AddModelError("Slug", "Nhap chuoi URL khac");
                    return View(CategoryProductModel);
                }

                if (CategoryProductModel.Slug == null)
                    CategoryProductModel.Slug = AppUtilities.GenerateSlug(CategoryProductModel.Title);

                if(CategoryProductModel.ParentCategoryId == -1)
                    CategoryProductModel.ParentCategoryId = null;

                _context.Add(CategoryProductModel);
                await _context.SaveChangesAsync();

                StatusMessage = @$"Ban vua tao CMSP moi ""{CategoryProductModel.Title}""";
                TypeStatusMessage = TypeName.Success;
                return RedirectToAction(nameof(Index));
            }

            var items = SelectCategories();
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View(CategoryProductModel);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id, int? parentid)
        {
            if (id == null) return NotFound();

            var categoryProduct = await _context.CategoryProducts.FindAsync(id);
            if (categoryProduct == null) return NotFound();

            var items = SelectCategories();
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View(categoryProduct);
        }

        // kiem tra parentid select co thuoc id children cua category do
        public bool CheckIfParentIdSelectSameChildren (List<CategoryProductModel> cateChilds, int? parentid_select)
        {
            foreach (var cateChild in cateChilds)
            {
                if (cateChild.Id == parentid_select)
                    return true;

                var cateChilds_1 = (from c in _context.CategoryProducts select c)
                                .Where(c => c.ParentCategoryId == cateChild.Id).ToList();
                                
                if (cateChilds_1.Count > 0)
                    // thuc hien de quy neu phat hien childrens trong childCate
                    return CheckIfParentIdSelectSameChildren(cateChilds_1, parentid_select);
            }
            return false;
        }

        // thay doi parent id cua cac category children
        public void SwitchParent (List<CategoryProductModel> cateChilds, int? parentid)
        {
            // lap qua cac child category
            foreach(var cateChild in cateChilds)
            {
                // gan idparent cua cateChild == idparent categoryEdit
                cateChild.ParentCategoryId = parentid;
                _context.Update(cateChild);
            }
            _context.SaveChanges();
        }

        // POST: Blog/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentCategoryId,Title,Content,Slug")] CategoryProductModel categoryProduct, int? oldparentid)
        {
            if (id != categoryProduct.Id) return NotFound();

            if (categoryProduct.ParentCategoryId == categoryProduct.Id)
            {
                ModelState.AddModelError(string.Empty, "Phai chon thu muc cha khac!");
            }

            if (ModelState.IsValid && (categoryProduct.ParentCategoryId != categoryProduct.Id))
            {
                try
                {
                    var isError = false;
                    // lay danh sach cac categoryChild cua categoryProduct
                    var cateChilds = (from c in _context.CategoryProducts select c)
                                    .Where(c => c.ParentCategoryId == categoryProduct.Id).ToList();

                    if (cateChilds.Count > 0)
                    {
                        isError = CheckIfParentIdSelectSameChildren(cateChilds, categoryProduct.ParentCategoryId);
                        if (isError == true)
                            // ModelState.AddModelError(string.Empty, "Phai chon thu muc cha khac!");
                            SwitchParent(cateChilds, oldparentid);
                    }


                    if (categoryProduct.ParentCategoryId == -1)
                        categoryProduct.ParentCategoryId = null;

                    _context.Update(categoryProduct);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryProductModelExists(categoryProduct.Id)) return NotFound();
                    else throw;
                }

                StatusMessage = @$"Ban vua cap nhat CMSP ""{categoryProduct.Title}""";
                TypeStatusMessage = TypeName.Success;
                return RedirectToAction(nameof(Index));
            }

            var items = SelectCategories();
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View(categoryProduct);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoryProduct = await _context.CategoryProducts
                                .Include(c => c.ParentCategory)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoryProduct == null) return NotFound();

            return View(categoryProduct);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryProduct = await _context.CategoryProducts
                                .Include(c => c.ChildrenCategory) // lay ra cac categories children cua category bi xoa
                                .FirstOrDefaultAsync(c => c.Id == id); // lay ra category co Id == id can xoa

            if (categoryProduct == null) return NotFound();

            // duyet cac cate child va thiet lap parent cua cate child la parent cua cate bi xoa
            foreach(var childrenCate in categoryProduct.ChildrenCategory)
            {
                childrenCate.ParentCategoryId = categoryProduct.ParentCategoryId;
            }

            _context.CategoryProducts.Remove(categoryProduct);

            await _context.SaveChangesAsync();

            StatusMessage = @$"Ban vua xoa CMSP ""{categoryProduct.Title}""";
            TypeStatusMessage = TypeName.Danger;
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryProductModelExists(int id)
        {
            return _context.CategoryProducts.Any(e => e.Id == id);
        }
    }
}
