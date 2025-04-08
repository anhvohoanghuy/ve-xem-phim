using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;

namespace ve_xem_phim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult AddPromotion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPromotion( decimal discountPercentage)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
                return RedirectToAction("Login");

            var promotion = new Promotion { DiscountPercentage = discountPercentage };
            _context.Promotions.Add(promotion);
            _context.SaveChanges();
            return RedirectToAction("Admin");
        }
        public IActionResult ManagePromotions()
        {
            var promotions = _context.Promotions.Include(p => p.Tickets).ToList();
            return View(promotions);
        }
        public IActionResult EditPromotion(int id)
        {
            var promotion = _context.Promotions.Find(id);
            return View(promotion);
        }
        [HttpPost]
        public IActionResult EditPromotion(int id, decimal discountPercentage)
        {
            var promotion = _context.Promotions.Find(id);
            if (promotion != null)
            {
                promotion.DiscountPercentage = discountPercentage;
                _context.SaveChanges();
            }
            return RedirectToAction("ManagePromotions");
        }

        public IActionResult DeletePromotion(int id)
        {
            var promotion = _context.Promotions.Find(id);
            if (promotion != null)
            {
                promotion.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ManagePromotions");
        }
        
    }
}
