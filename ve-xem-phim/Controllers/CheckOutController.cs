﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;

namespace ve_xem_phim.Controllers
{
    [Authorize]
    public class CheckOutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckOutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Checkout(string userId)
        {
            var user = _context.Users.Include(u => u.Cart).ThenInclude(c => c.Tickets).FirstOrDefault(u => u.Id == userId);
            if (user != null && user.Cart.Tickets.Any())
            {
                var totalPrice = user.Cart.Tickets.Sum(t => t.Price);
                var invoice = new Invoice
                {
                    UserId = user.Id,
                    Tickets = new List<Ticket>(user.Cart.Tickets),
                    TotalPrice = totalPrice
                };
                _context.Invoices.Add(invoice);
                user.Cart.Tickets.Clear();
                _context.SaveChanges();
                return View("Invoice", invoice);
            }
            return RedirectToAction("Index");
        }

        
    }
}
