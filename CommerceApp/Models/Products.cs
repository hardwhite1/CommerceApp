
namespace CommerceApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        // Catalogue
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Brand { get; set; }
        public required string Category { get; set; }

        // Pricing
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }

        // Inventory
        public int StockQuantity { get; set; }

        // Presentation
        public required string PictureUrl { get; set; }

        // Status
        public bool InStock { get; set; }
    }

}
