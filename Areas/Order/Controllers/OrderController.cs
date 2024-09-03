
using App.Areas.Product.Models.Services;
using App.Data;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Order.Controllers;

[Area("Order")]
[Route("/order/{action}")]
[Authorize]
public class OrderController : Controller
{
    private readonly CartService _cartService;
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly UrlHelperService _urlHelperService;
    private readonly PaypalClient _paypalClient;
    private readonly NotificationService _notifService;

    public OrderController(
                            AppDbContext context,
                            CartService cartService,
                            UrlHelperService urlHelperService,
                            UserManager<AppUser> userManager,
                            PaypalClient paypalClient,
                            NotificationService notifService  )
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

    [HttpGet("/my-order")]
    public async Task<IActionResult> ViewOrder ()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Content("Not found user!");
        
        var orders = _context.Orders.Where(o => o.UserId == user.Id).ToList();
        if(orders == null) return Content("List Order Not Found!");

        ViewBag.orders = orders;
        return View();
    }

    [HttpGet("/my-order/detail")]
    public async Task<IActionResult> DetailOrder (string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Content("Not found user!");
        
        var order = _context.Orders.Where(o => o.OrderId == id && o.UserId == user.Id).FirstOrDefault();
        var fileName = _context.ProductPhotos.Where(p => p.ProductID == order.ProductId).FirstOrDefault().FileName;
        ViewBag.filename = fileName;
        return View(order);
    }

    [HttpPost("/my-order/update-order")]
    public  async Task<IActionResult> UpdateInformOrderAPI(string inform)
    {
        string[] words = inform.Split('+');
        var id = "";
        var name = "";
        var value = "";

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0) id = words[i];
            else if (i == 1) name = words[i];
            else if (i == 2) value = words[i];
        }

        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Order Not Found!");

        if(name == "address")
            order.Address = value;
        else if (name == "phone")
            order.Phone = value;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return Json(new {
            name = name,
            value = value
        });
    }

    [HttpPost("/my-order/delete")]
    public async Task<IActionResult> DeleteOrder (string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return BadRequest("User not found!");

        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Order Not Found!");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        StatusMessage = $@"Bạn vừa hủy đơn hàng: {order.OrderId} - {order.ProductName}";
        TypeStatusMessage = TypeMessage.Success;

        string message = $@"Bạn đã hủy thành công đơn hàng *{order.ProductName}*";
        _notifService.CreateNotification(TypeNotification.CancelOrder, message, user.Id, user.UserName);

        return RedirectToAction(nameof(ViewOrder));
    }

    [HttpPost("/my-order/confirm")]
    public async Task<ActionResult> OrderConfirmation(string id)
    {
        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Order Not Found!");

        order.Status = StatusOrder.Shipping;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        
        return Json(new {
            success = true
        });
    }

    #region Manager Order For Admin
    [HttpGet("/manager-order")]
    public IActionResult ManagerOrder ()
    {
        var orders = _context.Orders.ToList();
        if(orders == null) return Content("List Order Not Found!");

        ViewBag.orders = orders;
        return View();
    }

    [HttpPost("/api/get-orders")]
    public ActionResult GetOrders_API (string sort)
    {
        var orders = _context.Orders.ToList();
        if(orders == null){
            return Json(new {
                success = false
            });
        }

        if(sort == "Pending"){
            orders = orders.Where(o => o.Status == StatusOrder.Pending).ToList();
        }
        else if(sort == "Shipping"){
            orders = orders.Where(o => o.Status == StatusOrder.Shipping).ToList();
        }
        else if(sort == "Success"){
            orders = orders.Where(o => o.Status == StatusOrder.Success).ToList();
        }

        return Json(new {
            success = true,
            orders = orders
        });
    }

    [HttpGet("/manager-order/detail")]
    public IActionResult ManagerDetailOrder (string id)
    {
        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Not Found Order!");

        var fileName = _context.ProductPhotos.Where(p => p.ProductID == order.ProductId).FirstOrDefault().FileName;
        ViewBag.filename = fileName;
        return View(order);
    }

    [HttpPost("/manager-order/delete")]
    public async Task<IActionResult> ManagerDeleteOrder (string id)
    {
        Console.WriteLine(id);
        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Order Not Found!");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        StatusMessage = $@"Bạn vừa hủy đơn hàng: {order.OrderId} - {order.ProductName}";
        TypeStatusMessage = TypeMessage.Success;

        return RedirectToAction(nameof(ManagerOrder));
    }

    [HttpPost("/manager-order/success")]
    public async Task<ActionResult> ManagerOrderSuccess(string id)
    {
        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        if(order == null) return Content("Order Not Found!");

        order.Status = StatusOrder.Success;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        
        return Json(new {
            success = true
        });
    }
    #endregion
}   