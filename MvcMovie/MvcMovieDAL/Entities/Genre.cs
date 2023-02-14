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
    public class Genre : DefaultEntity
    {
        public Genre()
        {
            Movies = new HashSet<Movie>();
        }

        public string Name { get; set; }
        public virtual ICollection<Movie> Movies { get; set;}
    }
}
