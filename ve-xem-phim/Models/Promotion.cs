namespace ve_xem_phim.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
