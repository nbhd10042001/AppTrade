using System.ComponentModel.DataAnnotations;

namespace App.Areas.Product.Models;

public class SelectFilters
{
    [Display(Name = "Filter")]
    public string[] selectFilters {set; get;}
    
    [Display(Name = "Category")]
    public string[] selectCategories {set; get;}

    [Display(Name = "Search")]
    public string SearchBar {set; get;}
}