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
        public IActionResult AddMovie(Movie model)
        {
            try
            {
                _context.Movies.Add(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi add: " + ex.Message);
                return View(model);
            }
            return RedirectToAction("ManageMovies");
        }
        public IActionResult EditMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            return View(movie);
        }
        [HttpPost]
        public IActionResult EditMovie(Movie model)
        {
            var movie = _context.Movies.Find(model.Id);
            try
            {
                _context.Movies.Update(movie);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi edit: " + ex.Message);
                return View(model);
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
