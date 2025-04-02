using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;

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

        public IActionResult Ticket(int movieId)
        {
            var tickets = _context.Tickets.Where(t => t.MovieId == movieId).ToList();
            return View(tickets);
        }
        [HttpPost]
        public IActionResult Ticket(int movieId, string row, string seat, string rom)
        {
            int number;
            int.TryParse(seat,out number);
            int romNumber;
            int.TryParse(rom, out romNumber);
            List<Ticket> tickets = _context.Tickets.Where(t => t.MovieId == movieId).ToList();
            if (row != "0")
            {
                tickets=tickets.Where(t=>t.Row==row).ToList();
            }
            if (number > 0)
            {
                tickets=tickets.Where(t=>t.Number==number).ToList();
            }
            if(romNumber > 0)
            {
                tickets=tickets.Where(t=>t.Rom==romNumber).ToList();
            }
            return View(tickets);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult BuyTicket(string userId, int ticketId)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            var user = _context.Users.Include(u => u.Cart).ThenInclude(c => c.Tickets).FirstOrDefault(u => u.Id == userId);
            if (ticket != null && ticket.Available && user != null)
            {
                ticket.Available = false;
                user.Cart.Tickets.Add(ticket);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
