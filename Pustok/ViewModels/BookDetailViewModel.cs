using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class BookDetailViewModel
    {
        public Book Book { get; set; }
        public List<Book> ReletedBooks { get; set; }
        public BookCommentViewModel BookCommentVM { get; set; }
    }
}
