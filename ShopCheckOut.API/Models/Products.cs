using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCheckOut.API.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public string Name { get; set; }
        public string Brand { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string PriceUnit { get; set; }
    }
}
