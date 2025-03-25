using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;

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
        public IActionResult AddTickets()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddTickets(int movieId, int quantity)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
                return RedirectToAction("Login");

            var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie != null)
            {
                movie.Tickets.AddRange(GenerateSeats(movieId, quantity));
                _context.SaveChanges();
            }
            return RedirectToAction("Admin");
        }

        private List<Ticket> GenerateSeats(int movieId, int quantity)
        {
            List<Ticket> seats = new List<Ticket>();
            string[] rows = { "A", "B", "C", "D", "E", "F", "G" };
            int count = 0;
            foreach (var row in rows)
            {
                for (int num = 1; num <= 12; num++)
                {
                    seats.Add(new Ticket { Row = row, Number = num, MovieId = movieId, Available = true });
                    count++;
                    if (count >= quantity) return seats;
                }
            }
            return seats;
        }
        public IActionResult ManageTickets()
        {
            var tickets = _context.Tickets.Include(t => t.Movie).ToList();
            return View(tickets);
        }
        public IActionResult EditTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            return View(ticket);
        }
        [HttpPost]
        public IActionResult EditTicket(int id, string row, int number, decimal price, bool available)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket != null)
            {
                ticket.Row = row;
                ticket.Number = number;
                ticket.Price = price;
                ticket.Available = available;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageTickets");
        }
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageTickets");
        }
    }
}
