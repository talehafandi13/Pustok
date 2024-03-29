﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }
        public Genre Genre { get; set; }
        public Author Author { get; set; }
        public List<BookTag> bookTags { get; set; } = new List<BookTag>();
        [NotMapped]
        public List<int> TagIds { get; set; }
        public List<BookImage> BookImages { get; set; } = new List<BookImage>();
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        [NotMapped]
        public IFormFile PosterFile { get; set; }
        [NotMapped]
        public IFormFile HoverFile { get; set; }
        public List<BookComment> BookComments { get; set; }
    }
}
