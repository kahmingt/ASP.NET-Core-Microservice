using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Product.Shared.Database.Entity
{
    public partial class dbCategory
    {
        public dbCategory()
        {
            Product = new HashSet<dbProduct>();
        }

        [Key]
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [StringLength(50)]
        public string CategoryName { get; set; } = null!;
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<dbProduct> Product { get; set; }
    }
}
