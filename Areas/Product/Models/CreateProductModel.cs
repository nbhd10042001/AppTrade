using System.ComponentModel.DataAnnotations;
using App.Models.Product;

namespace App.Areas.Product.Models;

public class CreateProductModel : ProductModel
{
    [Display(Name = "Chuyen muc")]
    [Required(ErrorMessage = "Select your categories!")]
    public int[] CategoryIDs {set; get;}

    [Display(Name = "Upload file Product")]
    public IFormFile FileProductUpload {set; get;}
}