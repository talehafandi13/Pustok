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
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            List<Author> authors = _context.Authors.Include(x => x.Books).ToList();
            return View(authors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid)
                return View();
           
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);
            if (author == null)
                return Content("error");
            return View(author);
        }
        [HttpPost]
        public IActionResult Edit(Author author)
        {
            Author entity = _context.Authors.FirstOrDefault(x => x.Id == author.Id);
            if (entity == null)
                return Content("errror");
            if (!ModelState.IsValid)
                return View();
            entity.FullName = author.FullName;
            entity.BirthDate = author.BirthDate;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);
            if (author == null)
                return NotFound();

            _context.Authors.Remove(author);
            _context.SaveChanges();
            return Ok();
        }
    }
}
