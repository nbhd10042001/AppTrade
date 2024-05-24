using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Product;

[Table("ProductPhoto")]
public class ProductPhotoModel
{
    [Key]
    public int Id {set; get;}

    public string FileName {set; get;}

    public int ProductID {set; get;}    
    
    [ForeignKey("ProductID")]
    public ProductModel Product {set; get;}
}