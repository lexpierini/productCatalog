using System.Text.Json.Serialization;

namespace backendMinimalApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Inventory { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Relationship Properties
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
