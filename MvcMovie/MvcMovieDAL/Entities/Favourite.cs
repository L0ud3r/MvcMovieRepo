using MvcMovieDAL.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcMovieDAL.Entities
{
    public class Favourite : DefaultEntity
    {
        public virtual Movie Movie { get; set; }
        public virtual User User { get; set; }
    }
}
