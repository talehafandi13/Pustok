using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            this._context = context;
            this._env = env;
        }
        public IActionResult Index()
        {
            List<Book> books = _context.Books.Include(x => x.Genre).Include(x => x.Author).ToList();
            return View(books);
        }
        public IActionResult Create()
        {
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
                ModelState.AddModelError("AuthorId", "author not found");
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
                ModelState.AddModelError("GenreId", "author not found");

            PosterImageCheck(book);
            HoverPosterImageCheck(book);
            ImageFilesCheck(book);
            CheckTagId(book);
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View();
            }

            BookImage image = new BookImage
            {
                Name = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterFile),
                PosterStatus = true
            };
            book.BookImages.Add(image);

            BookImage HoverImage = new BookImage
            {
                Name = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverFile),
                PosterStatus = false
            };
            book.BookImages.Add(HoverImage);

            AddImages(book, book.ImageFiles);

            if (book.TagIds != null)
            {
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag
                    {
                        TagId = item
                    };
                    book.bookTags.Add(bookTag);
                }
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.bookTags).FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                return RedirectToAction("error", "dashboard");
            }
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            book.TagIds = book.bookTags.Select(x => x.TagId).ToList();
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            Book entity = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == book.Id);
            if (book == null)
                RedirectToAction("error", "dashboard");

            if (entity.AuthorId != book.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
                ModelState.AddModelError("AuthorId", "author not found");

            if (entity.GenreId != book.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
                ModelState.AddModelError("GenreId", "author not found");

            if (book.PosterFile != null)
                EditPosterImageCheck(book);
            if (book.HoverFile != null)
                EditHoverPosterFileCheck(book);

            ImageFilesCheck(book);
            CheckTagId(book);

            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View();
            }
            List<string> deletedFiles = new List<string>();
            if (book.PosterFile != null)
            {
                BookImage poster = entity.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                deletedFiles.Add(poster.Name);
                poster.Name = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterFile);
            }
            if (book.HoverFile != null)
            {
                BookImage hoverPoster = entity.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                deletedFiles.Add(hoverPoster.Name);
                hoverPoster.Name = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverFile);
            }

            AddImages(entity, book.ImageFiles);
            entity.bookTags.RemoveAll(bt => !book.TagIds.Contains(bt.TagId));
            foreach (var item in book.TagIds.Where(x => !entity.bookTags.Any(bt => bt.TagId == x)))
            {
                BookTag bookTag = new BookTag
                {
                    TagId = item
                };
                entity.bookTags.Add(bookTag);
            }

            entity.Name = book.Name;
            entity.Desc = book.Desc;
            entity.SubDesc = book.SubDesc;
            entity.SalePrice = book.SalePrice;
            entity.CostPrice = book.CostPrice;
            entity.DiscountPercent = book.DiscountPercent;
            entity.IsAvailable = book.IsAvailable;
            entity.PageSize = book.PageSize;
            entity.Rate = book.Rate;
            entity.AuthorId = book.AuthorId;
            entity.GenreId = book.GenreId;
            _context.SaveChanges();
            FileManager.DeleteAll(_env.WebRootPath, "uploads/books", deletedFiles);
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();
            return Ok();
        }

        public void PosterImageCheck(Book book)
        {
            if (book.PosterFile != null)
            {
                EditPosterImageCheck(book);
            }
            else
            {
                ModelState.AddModelError("PosterFile", "poster file is required");
            }
        }
        public void EditPosterImageCheck(Book book)
        {
            if (book.PosterFile.ContentType != "image/jpeg" && book.PosterFile.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterFile", "file format must be jpeg or png");
            }
            if (book.PosterFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterFile", "file size must ne less than 2 mb");
            }
        }
        public void HoverPosterImageCheck(Book book)
        {
            if (book.HoverFile != null)
            {
                EditHoverPosterFileCheck(book);
            }
            else
            {
                ModelState.AddModelError("HoverFile", "hover poster image is required");
            }

        }
        public void EditHoverPosterFileCheck(Book book)
        {
            if (book.HoverFile.ContentType != "image/jpeg" && book.HoverFile.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterFile", "file format must be jpeg or png");
            }
            if (book.HoverFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterFile", "file size must ne less than 2 mb");
            }
        }
        public void ImageFilesCheck(Book book)
        {
            if (book.ImageFiles != null)
            {
                foreach (var item in book.ImageFiles)
                {
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "file format must be jpeg or png");
                    }
                    if (item.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "file size must ne less than 2 mb");
                    }
                }
            }
        }
        public void AddImages(Book book, List<IFormFile> formFiles)
        {
            if (formFiles != null)
            {
                foreach (var file in formFiles)
                {
                    BookImage bookImage = new BookImage
                    {
                        Name = FileManager.Save(_env.WebRootPath, "uploads/books", file)
                    };
                    book.BookImages.Add(bookImage);
                }
            }
        }
        public void CheckTagId(Book book)
        {
            if (book.TagIds!=null)
            {
                foreach (var tagId in book.TagIds)
                {
                    if (!_context.Tags.Any(x=>x.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", "Tag was not found");
                        return;
                    }
                }
            }
        }
    }
}
