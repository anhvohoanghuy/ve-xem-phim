namespace ve_xem_phim.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Ticket> Tickets { get; set; }
    }
}
