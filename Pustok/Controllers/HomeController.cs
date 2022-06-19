using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            HomeViewModel homeVM = new HomeViewModel
            {
                Sliders = sliders,
                DiscountedBooks = _context.Books.Include(x => x.BookImages).Include(x=>x.Author).Where(x => x.DiscountPercent > 0).Take(10).ToList(),
                NewBooks = _context.Books.Include(x => x.BookImages).Include(x=>x.Author).Where(x => x.IsNew).Take(10).ToList(),
                FeaturedBooks = _context.Books.Include(x => x.BookImages).Include(x=>x.Author).Where(x => x.IsFeatured).Take(10).ToList()
            };

            return View(homeVM);
        }

    }
}
