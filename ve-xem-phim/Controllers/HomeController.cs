using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;
using ve_xem_phim.ViewModel;

namespace ve_xem_phim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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
        public IActionResult Index(string name, string idCategory)
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
        public IActionResult MovieDetails(int id)
        {
            var movie = _context.Movies.Include(m => m.Tickets).Include(m => m.Category).FirstOrDefault(m => m.Id == id);
            return View(movie);
        }

        public IActionResult Ticket(int movieId, int rom, DateTime date)
        {
            var tickets = _context.Tickets.Where(t => t.MovieId == movieId && t.Rom== rom&& t.Date==date).Include(t=>t.Promotion).ToList();
            ViewBag.Date=date;
            ViewBag.Rom = rom;
            return View(tickets);
        }
        [HttpPost]
        public IActionResult Ticket(int movieId, string row, string seat, DateTime date)
        {
            int number;
            int.TryParse(seat,out number);
            List<Ticket> tickets = _context.Tickets.Where(t => t.MovieId == movieId && t.Date==date).Include(t => t.Promotion).ToList();
            if (row != "0")
            {
                tickets=tickets.Where(t=>t.Row==row).ToList();
            }
            if (number > 0)
            {
                tickets=tickets.Where(t=>t.Number==number).ToList();
            }
            return View(tickets);
        }
        [Authorize(Roles = "Admin,User")]
        public IActionResult BuyTicket(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            var user = _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.Tickets)
                .FirstOrDefault(u => u.Id == userId);

            if (ticket != null && ticket.Available && user != null)
            {
                // Ensure the Cart exists
                if (user.Cart == null)
                {
                    user.Cart = new Cart
                    {
                        Tickets = new List<Ticket>()
                    };
                    _context.Carts.Add(user.Cart); // Optional if you have cascade set up
                }

                ticket.Available = false;
                user.Cart.Tickets.Add(ticket);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult RemoveTicket(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.Tickets)
                .FirstOrDefault(u => u.Id == userId);

            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (ticket != null && user?.Cart != null && user.Cart.Tickets.Contains(ticket))
            {
                user.Cart.Tickets.Remove(ticket);
                ticket.Available = true;
                _context.SaveChanges();
            }

            return RedirectToAction("Index"); // hoặc tên view đang hiển thị vé đã mua
        }
        [HttpGet]
        public IActionResult GetRoomsAndShowtimes(int movieId)
        {
            var showtimes = _context.Tickets
                .Where(t => t.MovieId == movieId && t.Available == true && t.Date> DateTime.Today)
                .GroupBy(t => new { t.Rom, t.Date })
                .Select(g => new RoomShowtimeViewModel
                {
                    Rom = g.Key.Rom,
                    Date = g.Key.Date,
                    Quantity = g.Count()
                })
                .OrderBy(s => s.Date)
                .ToList();

            ViewBag.Movie = _context.Movies.FirstOrDefault(m => m.Id == movieId); // Lấy thông tin phim để hiển thị

            return View(showtimes); // Trả về View
        }
        
        public async Task<IActionResult> MyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cart = await _context.Carts
                .Include(c => c.Tickets)
                .ThenInclude(t => t.Movie) // nếu Ticket có liên kết với Movie
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Tickets.Any())
            {
                ViewBag.Message = "Bạn chưa mua vé nào.";
                return View(new List<Ticket>());
            }

            return View(cart.Tickets);
        }
    }
}
