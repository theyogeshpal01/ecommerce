using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStoreMVC.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Packed,
        Shipped,
        OutForDelivery,
        Delivered,
        Cancelled
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string OrderNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        public string ShippingAddress { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string PaymentMethod { get; set; } = "Cash on Delivery";

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
