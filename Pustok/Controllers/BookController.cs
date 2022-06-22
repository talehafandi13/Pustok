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
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            this._context = context;
        }
        public IActionResult Detail(int id)
        {
            BookDetailViewModel bookDetailVM =  GetBookDetailVM(id);
            if (bookDetailVM == null)
                return RedirectToAction("error", "home");
            return View(bookDetailVM);
        }
        private BookDetailViewModel GetBookDetailVM(int id)
        {
            Book book = _context.Books.Include(x=>x.Author).Include(x=>x.Genre).Include(x=>x.BookImages)
                .Include(x=>x.bookTags).ThenInclude(x=>x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (book == null)
                return null;
            BookDetailViewModel bookDetailVM = new BookDetailViewModel
            {
                Book = book,
                ReletedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.GenreId == book.GenreId).ToList(),
                BookCommentVM = new BookCommentViewModel { BookId = id}
            };
            return bookDetailVM;
        }

        public IActionResult Comment(BookCommentViewModel vm)
        {
            //if (!ModelState.IsValid)
            //    return 


            return Ok(vm);
        }

    }
}
