using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Product;

public class ProductCategoryProduct
    {
        public int ProductID {set; get;}

        public int CategoryProductID {set; get;}

        [ForeignKey("ProductID")]
        public ProductModel Product {set; get;}

        [ForeignKey("CategoryProductID")]
        public CategoryProductModel CategoryProduct {set; get;}
        
    }