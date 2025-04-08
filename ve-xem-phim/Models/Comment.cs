using System.ComponentModel.DataAnnotations;

namespace ve_xem_phim.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Range(0, 10)]
        public int Score { get; set; }
        public string Description { get; set; }
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
}
