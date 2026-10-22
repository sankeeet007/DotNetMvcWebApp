using System.ComponentModel.DataAnnotations;

namespace MVCwebApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Price { get; set; } = null!;

        public string Color { get; set; } = null!;

    }
}
