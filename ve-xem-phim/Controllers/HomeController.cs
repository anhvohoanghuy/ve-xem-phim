using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var movies = _context.Movies.Include(m => m.Tickets).Where(m => m.IsActive).ToList();
            return View(movies);
        }

        public IActionResult MovieDetails(int id)
        {
            var movie = _context.Movies.Include(m => m.Tickets).FirstOrDefault(m => m.Id == id);
            return View(movie);
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
