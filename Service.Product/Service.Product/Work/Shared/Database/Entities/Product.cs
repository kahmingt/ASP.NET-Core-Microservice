using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Product.Shared.Database.Entity
{
    public partial class dbProduct
    {
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; } = null!;
        [Column("SupplierID")]
        public int SupplierId { get; set; }
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Product")]
        public virtual dbCategory Category { get; set; } = null!;
    }
}
