namespace ve_xem_phim.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
