using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace ve_xem_phim.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public string Url { get; set; }
        public Category Category { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
