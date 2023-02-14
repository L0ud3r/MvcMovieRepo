using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MvcMovieDAL.Default;

namespace MvcMovieDAL.Entities
{
    public class Movie : DefaultEntity
    {
        public string? Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public string? Rating { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
