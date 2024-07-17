using System.ComponentModel.DataAnnotations;

namespace App.Areas.Order.Models;

public class CheckoutInforModel
{
    public string UserName {set; get;}
    public string Email {set; get;}
    public string OrderId {set; get;}
    public string PaymentMethod {set; get;}

    [Required]
    [Phone]
    public string Phone { get; set; }

    [Required]
    [StringLength(400)]
    public string Address { get; set; }
}
