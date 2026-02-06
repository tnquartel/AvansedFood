namespace Domain.Models
{
    public class PackageProducts
    {
        public int PackageId { get; set; }

        public int ProductId { get; set; }

        public Package Package { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}