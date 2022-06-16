using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }
        [MaxLength(1500)]
        public string SubDesc { get; set; }
        public double SalePrice { get; set; }
        public double CostPrice { get; set; }
        public double DiscountPercent { get; set; }
        public int PageSize { get; set; }
        public bool IsAvailable { get; set; }
        public byte Rate { get; set; }

        public Genre Genre { get; set; }
        public Author Author { get; set; }
        public List<BookImage> BookImages { get; set; }
    }
}
