using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Product;

[Table("Product")]
public class ProductModel
{
    [Key]
    public int ProductId {set; get;}

    [Required(ErrorMessage = "Phải có tiêu đề bài viết")]
    [Display(Name = "Tên Sản Phẩm")]
    [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
    public string Title {set; get;}

    [Display(Name = "Mô tả ngắn")]
    public string Description {set; get;}

    [Display(Name="Chuỗi định danh (url)", Prompt = "Nhập hoặc để trống tự phát sinh theo Title")]
    [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
    [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
    public string Slug {set; get;}

    [Display(Name = "Người đăng")]
    public string AuthorId {set; get;}
    
    [ForeignKey("AuthorId")]
    [Display(Name = "Người đăng")]
    public AppUser Author {set; get;}
    
    [Display(Name = "Ngày tạo")]
    public DateTime DateCreated {set; get;}

    [Display(Name = "Ngày cập nhật")]
    public DateTime DateUpdated {set; get;}

    [Display(Name = "Giá sản phẩm")]
    public int Price {set; get;}

    public List<ProductCategoryProduct>  ProductCategoryProducts { get; set; }

    public List<ProductPhotoModel> Photos {set; get;}

}