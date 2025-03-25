namespace ve_xem_phim.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public bool Available { get; set; } = true;
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public decimal Price { get; set; }
    }
}
