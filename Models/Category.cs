using System.ComponentModel.DataAnnotations;

namespace GiftStoreMVC.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
