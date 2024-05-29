using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Contacts;

#nullable disable

public class ContactModel
{
    [Key]
    public int Id {set; get;}

    [Required(ErrorMessage = "Phai nhap ho ten")]
    [Column(TypeName ="nvarchar")] [StringLength(50)]
    [Display(Name = "Ho ten")]

    public string FullName {set; get;}

    [Required(ErrorMessage = "Phai nhap {0}")]
    [StringLength(100)][EmailAddress]
    public string Email {set; get;}

    [StringLength(50)]
    [Phone(ErrorMessage = "Phai nhap {0}")]
    [Display(Name = "So dien thoai")]
    public string Phone {set; get;}

    public DateTime DateSent {set; get;}

    [Display(Name = "Noi dung")]
    public string Message {set; get;}
}