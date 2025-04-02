using System.ComponentModel.DataAnnotations;

namespace ve_xem_phim.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public int Rom {  get; set; }
        public bool Available { get; set; } = true;
        public int MovieId { get; set; }
        public DateTime Date { get; set; }
        public Movie Movie { get; set; }
        public decimal Price { get; set; }
    }
}
