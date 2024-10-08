using App.Areas.Order.Models;
using App.Areas.Product.Models.Services;
using App.Data;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Order.Controllers;

[Area("Order")]
[Authorize]
public class CheckoutController : Controller
{
    private readonly CartService _cartService;
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly UrlHelperService _urlHelperService;
    private readonly PaypalClient _paypalClient;
    private readonly NotificationService _notifService;

    public CheckoutController(
                            AppDbContext context,
                            CartService cartService,
                            UrlHelperService urlHelperService,
                            UserManager<AppUser> userManager,
                            PaypalClient paypalClient,
                            NotificationService notifService )
    {
        _context = context;
        _cartService = cartService;
        _urlHelperService = urlHelperService;
        _userManager = userManager;
        _paypalClient = paypalClient;
        _notifService = notifService;
    }


    [TempData]
    public string StatusMessage{set; get;}
    [TempData]
    public string TypeStatusMessage{set; get;}

    // gui don hang
    [Route ("/checkout")]
    public async Task<IActionResult> Checkout ()
    {
        var currUser = await _userManager.GetUserAsync(User);
        var carts = _cartService.GetCartItems();
        var user = new {
            name = currUser.UserName,
            email = currUser.Email,
            phone = currUser.PhoneNumber,
            address = currUser.HomeAddress,
        };
        ViewBag.user = user;
        ViewBag.carts = carts;
        ViewBag.PaypalClientId = _paypalClient.ClientId;

        return View();
    }

    [HttpPost("/completedpayment")]
    public async Task<IActionResult> CompletedPayment(CheckoutInforModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return BadRequest("User not found!");

        var carts = _cartService.GetCartItems();
        foreach (var item in carts)
        {
            var status = StatusOrder.Pending;
            if(model.PaymentMethod == PaymentMethod.Paypal)
                status = StatusOrder.Shipping;
                
            _context.Orders.Add(new OrderModel {
                OrderId = Guid.NewGuid().ToString(),
                UserId = user.Id,
                UserName = user.UserName,
                ProductId = item.product.ProductId,
                ProductName = item.product.Title,
                Quantity = item.quantity,
                UnitPrice = item.product.Price,
                DateCreate = DateTime.Now,
                CodePayment = model.OrderId,
                PaymentMethod = model.PaymentMethod,
                Phone = model.Phone,
                Address = model.Address,
                Status = status
            });
            string message = $@"Bạn đã thanh toán thanh công đơn hàng *{item.product.Title}*
                                với số lượng {item.quantity} và tổng tiền {item.quantity*item.product.Price}    
                            ";
            _notifService.CreateNotification(TypeNotification.Order, message, user.Id, user.UserName);

        }
        await _context.SaveChangesAsync();

        _cartService.ClearCart();
        return View(model);
        // return Json(new {
        //     url = _urlHelperService.GetLink("Cart", "ViewProduct", "Product")
        // });
    }

    [HttpPost("/checkout/create-paypal-order")]
    public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
    {
        var total = 0;
        var carts = _cartService.GetCartItems();
        foreach(var item in carts)
        {
            total += item.product.Price * item.quantity;
        }
        var currency_code = "USD";
        var code = "Order " + DateTime.Now.Ticks.ToString();

        try{
            var response = await _paypalClient.CreateOrder(total.ToString(), currency_code, code);
            return Ok(response);
        }
        catch (Exception ex){
            var error = new {ex.GetBaseException().Message};
            return BadRequest(error);
        }
    }

    [HttpPost("/checkout/capture-paypal-order")]
    public async Task<IActionResult> CapturePaypalOrder(string orderId, CancellationToken cancellationToken)
    {
        try {
            var response = await _paypalClient.CaptureOrder(orderId);
            // luu database
            return Ok(response);
        }
        catch (Exception ex){
            var error = new {ex.GetBaseException().Message};
            return BadRequest(error);
        }
    }
}