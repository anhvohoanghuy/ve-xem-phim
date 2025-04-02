using System.ComponentModel.DataAnnotations;

namespace ve_xem_phim.ViewModel
{
    public class AddTicketViewModel
    {
        [Required(ErrorMessage = "Phòng chiếu là bắt buộc.")]
        public int Rom { get; set; }

        [Required(ErrorMessage = "Ngày chiếu là bắt buộc.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Giá vé là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá vé phải là một số hợp lệ.")]
        public decimal Price { get; set; }
    }
}
