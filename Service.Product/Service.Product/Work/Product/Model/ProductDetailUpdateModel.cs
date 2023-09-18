using Service.Product.Work.Utility;
using System.ComponentModel.DataAnnotations;

namespace Service.Product.Work.Model
{
    public class ProductDetailUpdateModel
    {
        [Required]
        [ValidateUniqueIdentifier]
        public int CategoryID { get; set; }

        [Required]
        [ValidateUniqueIdentifier]
        public int ProductID { get; set; }

        [Display(Name = "Product")]
        [ValidateProductName]
        public string ProductName { get; set; }

        [Required]
        [ValidateUniqueIdentifier]
        public int SupplierID { get; set; }

        [Required]
        [ValidateUnitInStock]
        public short UnitsInStock { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [ValidateUnitPrice]
        public decimal UnitPrice { get; set; }

    }
}