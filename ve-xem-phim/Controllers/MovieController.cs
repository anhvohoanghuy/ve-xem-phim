using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var categories = _context.Categories.ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var item in categories)
            {
                SelectListItem selectListItem = new SelectListItem { Value = item.Id.ToString(), Text = item.Name };
                selectList.Add(selectListItem);
            }
            ViewBag.SelectList = selectList;
            var movies = _context.Movies.ToList();
            return View(movies);
        }
        [HttpPost]
        public IActionResult ManageMovies(string name,string idCategory)
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
        public IActionResult AddMovie()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult AddMovie(Movie model)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
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
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            var movie = _context.Movies.Find(id);
            return View(movie);
        }
        [HttpPost]
        public IActionResult EditMovie(Movie model)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            var movie = _context.Movies.Find(model.Id);
            if (movie == null)
            {
                return NotFound();
            }

            try
            {
                // Gán giá trị mới từ model
                movie.Title = model.Title;
                movie.Description = model.Description;
                movie.CategoryId = model.CategoryId;
                movie.Url = model.Url;
                movie.IsActive = model.IsActive;

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
        public IActionResult Active(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                movie.IsActive = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMovies");
        }
    }
}
