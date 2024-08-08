using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Contact;

public class ContactModel
{
    [Key]
    public int Id {set; get;}

    [Required(ErrorMessage = "Required Full Name")]
    [Column(TypeName ="nvarchar")] [StringLength(50)]
    [Display(Name = "Full Name")]
    public string FullName {set; get;}

    [Required(ErrorMessage = "Required {0}")]
    [StringLength(100)][EmailAddress]
    public string Email {set; get;}

    [StringLength(50)]
    [Phone(ErrorMessage = "Required {0}")]
    [Display(Name = "Phone Number")]
    public string Phone {set; get;}

    public DateTime DateSent {set; get;}

    [Required]
    public string Message {set; get;}
}