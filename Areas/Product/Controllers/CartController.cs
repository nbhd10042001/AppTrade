using App.Areas.Product.Models;
using App.Areas.Product.Models.Services;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Product.Controllers;

[Area("Product")]
[Route("cart/{action}")]
public class CartController : Controller
{
    private readonly ILogger<ViewProductController> _logger;
    private readonly CartService _cartService;
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly UrlHelperService _urlHelperService;

    public CartController(ILogger<ViewProductController> logger,
                                    AppDbContext context,
                                    CartService cartService,
                                    UrlHelperService urlHelperService,
                                    UserManager<AppUser> userManager)
    {
        _logger = logger;
        _context = context;
        _cartService = cartService;
        _urlHelperService = urlHelperService;
        _userManager = userManager;
    }

    [TempData]
    public string StatusMessage{set; get;}
    [TempData]
    public string TypeStatusMessage{set; get;}

    /// Thêm sản phẩm vào cart
    [HttpPost]
    public async Task<IActionResult> AddToCart (int id) 
    {
        var user = await _userManager.GetUserAsync(this.User);
        if(user == null) return Json(new{
                success = 0,
                url = _urlHelperService.GetLink("Login", "Account", "Identity")
            });

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
            message = id,
            productname = product.Title
        });
    }

    public IActionResult LoadCartCount ()
    {
        var cart = _cartService.GetCartItems();

        return Json(new {
            success = "success",
            count = cart.Count,
            url = _urlHelperService.GetLink("Cart", "Cart", "Product")
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

    public IActionResult ClearCart()
    {
        _cartService.ClearCart();
        return RedirectToAction(nameof(Cart));
    }
}