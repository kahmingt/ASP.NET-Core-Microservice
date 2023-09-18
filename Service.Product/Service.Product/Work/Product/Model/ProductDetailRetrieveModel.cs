using System.ComponentModel.DataAnnotations;

namespace Service.Product.Work.Model
{
    public class ProductDetailRetrieveModel
    {
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        public int ProductID { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        [Display(Name = "Supplier")]
        public string? SupplierName { get; set; }

        [Display(Name = "Units In Stock")]
        public short UnitsInStock { get; set; }

        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

    }
}