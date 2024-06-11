using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace App.Models;

public class AppUser : IdentityUser
{
    [Column(TypeName ="nvarchar")] [StringLength(400)]
    public string HomeAddress {set;get;}

    [DataType(DataType.Date)]
    public DateTime? BirthDate {set; get;}

    [Display(Name = "Avatar")]
    public UserAvatarModel Avatar {set; get;}

}