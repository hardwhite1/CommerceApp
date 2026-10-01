namespace CommerceApp.DTOs.Products
{
    public class UpdateProductDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Brand { get; set; }

        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }

        public int StockQuantity { get; set; }
        public required string PictureUrl { get; set; }

    }
}
