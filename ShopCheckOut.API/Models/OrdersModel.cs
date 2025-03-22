using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCheckOut.API.Models
{
    public class OrdersModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItems> OrderItems { get; set; }
        public int TotalAmount { get; set; }
    }
}
