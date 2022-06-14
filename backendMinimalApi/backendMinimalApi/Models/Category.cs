using System.Text.Json.Serialization;

namespace backendMinimalApi.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        // Relationship Properties (1:N)
        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
