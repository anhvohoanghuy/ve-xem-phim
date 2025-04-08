using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;
using ve_xem_phim.ViewModel;

namespace ve_xem_phim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ListMovie()
        {
            var categories = _context.Categories.ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var item in categories)
            {
                SelectListItem selectListItem = new SelectListItem { Value = item.Id.ToString(), Text = item.Name };
                selectList.Add(selectListItem);
            }
            ViewBag.SelectList = selectList;
            var movies = _context.Movies.Where(m => m.IsActive).ToList();
            return View(movies);
        }
        [HttpPost]
        public IActionResult ListMovie(string name, string idCategory)
        {
            var categoryId = int.Parse(idCategory);
            var categories = _context.Categories.ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var item in categories)
            {
                SelectListItem selectListItem = new SelectListItem { Value = item.Id.ToString(), Text = item.Name };
                selectList.Add(selectListItem);
            }
            ViewBag.SelectList = selectList;
            List<Movie> movies;
            if (categoryId > 0 && !string.IsNullOrEmpty(name))
            {
                movies = _context.Movies.Where(m => m.IsActive && m.Title.ToLower().Contains(name.ToLower().Trim()) && m.CategoryId == categoryId).ToList();
            }
            else if (categoryId > 0)
            {
                movies = _context.Movies.Where(m => m.IsActive && m.CategoryId == categoryId).ToList();
            }
            else if (categoryId == 0)
            {
                movies = _context.Movies.Where(m => m.IsActive && m.Title.ToLower().Contains(name.ToLower().Trim())).ToList();
            }
            else
            {
                movies = _context.Movies.Where(m => m.IsActive).ToList();
            }
            return View(movies);
        }
        public IActionResult AddTickets(int movieId)
         {
            ViewBag.MovieId=movieId;
            return View();
        }
        [HttpPost]
        public IActionResult AddTickets(int movieId, int rom, DateTime date, decimal price)
        {
            // Kiểm tra xem model có hợp lệ không
            if (ModelState.IsValid)
            {
                var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);

                if (movie != null)
                {
                    // Thêm vé vào bộ sưu tập của phim
                    movie.Tickets.AddRange(GenerateSeats(movieId, rom, date, price));
                    _context.SaveChanges();
                    return RedirectToAction("ListMovie");
                }
                else
                {
                    // Nếu không tìm thấy phim, trả lại lỗi và thông báo người dùng
                    ModelState.AddModelError("", "Phim không tồn tại.");
                }
            }
            // Nếu dữ liệu không hợp lệ, trả lại trang thêm vé với thông báo lỗi
            return View();
        }

        private List<Ticket> GenerateSeats(int movieId, int rom, DateTime date, decimal price)
        {
            List<Ticket> seats = new List<Ticket>();
            string[] rows = { "A", "B", "C", "D", "E", "F", "G" };
            foreach (var row in rows)
            {
                for (int num = 1; num <= 12; num++)
                {
                    seats.Add(new Ticket { Row = row, Number = num, Rom = rom, Date = date, MovieId = movieId, Available = true, Price = price });
                }
            }
            return seats;
        }
        public IActionResult ManageTickets()
        {
            var tickets = _context.Tickets.Include(t => t.Movie).ToList();
            return View(tickets);
        }
        public IActionResult EditTicket(EditTicketViewModel model)
        {
            var tickets = _context.Tickets
                .Where(t => t.MovieId == model.MovieId && t.Rom == model.Rom && t.Date == model.Date)
                .FirstOrDefault(); // Lấy vé đầu tiên phù hợp

            if (tickets == null)
            {
                return NotFound();
            }

            var viewModel = new EditTicketViewModel
            {
                MovieId = tickets.MovieId,
                Rom = tickets.Rom,
                Date = tickets.Date,
                Price = tickets.Price
            };

            ViewBag.Movie = _context.Movies.FirstOrDefault(m => m.Id == model.MovieId);

            return View(viewModel); // Trả về View với ViewModel chứa thông tin gốc
        }
        [HttpPost]
        public IActionResult EditTicket(EditTicketViewModel model,DateTime newDate)
        {
            if (ModelState.IsValid)
            {
                // Lấy danh sách tất cả vé có cùng MovieId, Rom và Date
                var tickets = _context.Tickets
                    .Where(t => t.MovieId == model.MovieId && t.Rom == model.Rom && t.Date == model.Date)
                    .ToList();

                if (!tickets.Any()) // Kiểm tra nếu không có vé nào
                {
                    return NotFound();
                }

                // Cập nhật tất cả vé trong danh sách
                foreach (var ticket in tickets)
                {
                    ticket.Date = newDate;
                    ticket.Price = model.Price;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Chuyển hướng về trang danh sách suất chiếu của phim
                return RedirectToAction("GetRoomsAndShowtimes", "Ticket", new { movieId = model.MovieId });
            }

            return View(model); // Trả lại view nếu dữ liệu không hợp lệ
        }

        public IActionResult DeleteTicket(EditTicketViewModel model)
        {
            // Lấy tất cả các vé có cùng MovieId, Rom và Date
            var tickets = _context.Tickets
                .Where(t => t.MovieId == model.MovieId && t.Rom == model.Rom && t.Date == model.Date)
                .ToList();

            if (tickets.Any())
            {
                // Duyệt qua tất cả vé và thay đổi trạng thái Available thành false
                foreach (var ticket in tickets)
                {
                    ticket.Available = false;
                }

                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction("GetRoomsAndShowtimes", "Ticket", new { movieId = model.MovieId}); // Quay lại trang quản lý vé
        }

        [HttpGet]
        public IActionResult GetRoomsAndShowtimes(int movieId)
        {
            var showtimes = _context.Tickets
                .Where(t => t.MovieId == movieId&& t.Available==true)
                .GroupBy(t => new { t.Rom, t.Date })
                .Select(g => new RoomShowtimeViewModel
                {
                    Rom = g.Key.Rom,
                    Date = g.Key.Date,
                    Quantity= g.Count()
                })
                .OrderBy(s => s.Date)
                .ToList();

            ViewBag.Movie = _context.Movies.FirstOrDefault(m => m.Id == movieId); // Lấy thông tin phim để hiển thị

            return View(showtimes); // Trả về View
        }
        public IActionResult AddPromotionToTicket(int movieId, int rom, DateTime date)
        {
            ViewBag.MovieId=movieId;
            ViewBag.Rom=rom;
            ViewBag.Date=date;
            var promotions = _context.Promotions.ToList();
            return View(promotions);
        }
        [HttpPost]
        public IActionResult AddPromotionToTicket(int movieId, int rom,DateTime date, int promotionId)
        {
            var tickets = _context.Tickets
                .Where(t => t.MovieId == movieId && t.Rom == rom && t.Date == date)
                .ToList();
            if (tickets.Any())
            {
                // Duyệt qua tất cả vé và thay đổi trạng thái Available thành false
                foreach (var ticket in tickets)
                {
                    ticket.PromotionId = promotionId;
                }

                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction("GetRoomsAndShowtimes", "Ticket", new { movieId = movieId });
        }
    }
}
