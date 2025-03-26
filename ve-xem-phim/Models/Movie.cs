using System.Net.Sockets;

namespace ve_xem_phim.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string Url { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
