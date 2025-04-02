using System.ComponentModel.DataAnnotations;

namespace ve_xem_phim.ViewModel
{
    public class EditTicketViewModel
    {
        public int MovieId { get; set; }
        public int Rom { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
