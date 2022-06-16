using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            List<Genre> genres = _context.Genres.Include(x=>x.Books).ToList();
            return View(genres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid)
                return View();
            if(_context.Genres.Any(x=>x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "genre already exists");
                return View();
            }

            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(X => X.Id == id);
            if (genre == null)
                return Content("error");
            return View(genre);
        }[HttpPost]
        public IActionResult Edit(Genre genre)
        {
            Genre entity = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);
            if (entity == null)
                return Content("errror");
            if(_context.Genres.Any(x=>x.Id !=genre.Id && x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "genre already exists");
            }
            if (!ModelState.IsValid)
                return View();
            entity.Name = genre.Name;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
            if (genre == null)
                return NotFound();

            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return Ok();

        }
    }

}
