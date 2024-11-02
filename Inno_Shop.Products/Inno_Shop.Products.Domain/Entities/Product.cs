namespace Inno_Shop.Products.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public Guid UserId { get; set; }
    }
}
