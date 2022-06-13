namespace backendMinimalApi.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        // Relationship Properties (1:N)
        public ICollection<Product>? Products { get; set; }
    }
}
