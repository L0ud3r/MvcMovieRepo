using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcMovie.Models
{
    public class MovieAllGenresViewModel : MovieViewModel
    {
        public MovieAllGenresViewModel(SelectList genres, int Id, string Title, decimal Price, DateTime ReleaseDate, string Rating)
        {
            Genres = genres;
            this.Id = Id;
            this.Title = Title;
            this.Price = Price;
            this.ReleaseDate = ReleaseDate;
            this.Rating = Rating;
        }

        public MovieAllGenresViewModel(SelectList genres)
        {
            Genres = genres;
        }

        public SelectList? Genres { get; set; }
    }
}
