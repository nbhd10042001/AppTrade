
using System.ComponentModel.DataAnnotations;

namespace App.Models;
public class UploadOneFile
{
    [Required(ErrorMessage = "Phai chon 1 file de upload")]
    [DataType(DataType.Upload)]
    [FileExtensions(Extensions = "png, jpg, jpeg, gif")]
    [Display(Name = "Chon File Upload")]
    public IFormFile FileUpload {set; get;}
}