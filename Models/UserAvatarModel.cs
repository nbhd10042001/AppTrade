using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table("UserAvatar")]
public class UserAvatarModel
{
    [Key]
    public int Id {set; get;}

    public string AvatarName {set; get;}

    public string UserID {set; get;}  

    [ForeignKey("UserID")]
    public AppUser User {set; get;} 
}