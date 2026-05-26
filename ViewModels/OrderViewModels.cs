using GiftStoreMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace GiftStoreMVC.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash on Delivery";
    }
}
