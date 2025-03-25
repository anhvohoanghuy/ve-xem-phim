using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;

namespace ve_xem_phim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ManageMovies()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }
        public IActionResult AddMovie()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddMovie(string title, string description)
        {
            var movie = new Movie { Title = title, Description = description, IsActive = true };
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction("ManageMovies");
        }
        public IActionResult EditMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            return View(movie);
        }
        [HttpPost]
        public IActionResult EditMovie(int id, string title, string description)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                movie.Title = title;
                movie.Description = description;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMovies");
        }
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                movie.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMovies");
        }
    }
}
