using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace App.Models;

public class OrderModel
{
    [Key]
    public int Id {set; get;}
    public string OrderId {set; get;}
    public string UserId {set; get;}
    public string UserName {set; get;}
    public int ProductId {set; get;}
    public string ProductName {set; get;}
    public int Quantity {set; get;}
    public double UnitPrice {set; get;}
    public DateTime DateCreate {set; get;}
    public string CodePayment {set; get;}
    public string PaymentMethod {set; get;}
    public string Phone {set; get;}
    public string Address {set; get;}
    public string Status {set; get;}

}