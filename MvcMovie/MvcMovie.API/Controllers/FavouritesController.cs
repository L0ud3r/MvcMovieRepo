using Microsoft.AspNetCore.Mvc;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MvcMovie.API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcMovie.Models;
using System.Text.Json;

namespace MvcMovie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavouritesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Favourite> _favouriteRepository;
        private readonly IRepository<User> _userRepository;

        public FavouritesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository,
            IRepository<Favourite> favouriteRepository, IRepository<User> userRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _favouriteRepository = favouriteRepository;
            _userRepository = userRepository;
        }

        private User GetUserByToken(string token)
        {
            var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userMail = claimMail?.Value;

            return _userRepository.Get().Where(x => x.Email == userMail && x.Active == true).SingleOrDefault();
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(int movieId, string token)
        {
            //    var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(token);

            //    var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            //    string userMail = claimMail?.Value;

            var user = GetUserByToken(token);

            var favorite = _favouriteRepository.Get().SingleOrDefault(f => f.Movie.Id == movieId && f.User.Id == user.Id);

            if (favorite == null)
            {
                var newFavorite = new Favourite { };

                newFavorite.Movie = _movieRepository.Get().Where(x => x.Id == movieId && x.Active == true).SingleOrDefault();
                newFavorite.User = _userRepository.Get().Where(x => x.Id == user.Id && x.Active == true).SingleOrDefault();

                _favouriteRepository.Insert(newFavorite);
                _favouriteRepository.Save();

            }
            //else if (favorite != null && action == "remove")
            //{
            //    // Remove the movie from the user's favorites
            //    _favouriteRepository.Delete(favorite.Id);
            //    _favouriteRepository.Save();
            //}

            return Json(new { success = true });
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> Remove(int movieId, string token)
        {
            //var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("token"));

            //var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            //string userMail = claimMail?.Value;

            var user = GetUserByToken(token);

            var favorite = _favouriteRepository.Get().SingleOrDefault(f => f.Movie.Id == movieId && f.User.Id == user.Id);

            if (favorite != null)
            {
                // Remove the movie from the user's favorites
                _favouriteRepository.Delete(favorite.Id);
                _favouriteRepository.Save();
            }


            return Json(new { success = true });
        }

        //[HttpGet]
        //public async Task<IActionResult<bool>> IsFavourite(int movieId)
        //{
        //    var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("token"));

        //    var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        //    string userMail = claimMail?.Value;

        //    var user = await GetUserByEmail(userMail);

        //    var isFavourite = _favouriteRepository.Get().Any(f => f.Movie.Id == movieId && f.User.Id == user.Id);

        //    return Json(new { success = isFavourite });
        //}

        // POST: FavouritesController/Delete/5
        [HttpPost("Delete"), ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string token)
        {
            if (_favouriteRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Favourites' is null.");
            }

            var user = GetUserByToken(token);

            var movieFav = _favouriteRepository.Get().Where(x => x.Movie.Id == id && x.User.Id == user.Id).SingleOrDefault();

            if (movieFav != null)
            {
                _favouriteRepository.Delete(movieFav.Id);
            }

            _movieRepository.Save();
            return Json(new { success = true });
        }

        [HttpPost("Paginate")]
        public async Task<JsonResult> Paginate([FromBody] BootstrapModel model)
        {
            model.Limit = model.Limit.HasValue && model.Limit != 0 ? model.Limit.Value : int.MaxValue;

            var token = model.Search.FirstOrDefault(x => x.Name == "Token");

            var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(token.Value.ToString());

            var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userMail = claimMail?.Value;

            var user = GetUserByToken(token.Value.ToString());

            var query = _favouriteRepository.Get().Where(x => x.User.Id == user.Id).Include(x => x.Movie).AsQueryable();

            var genreName = model.Search.FirstOrDefault(x => x.Name == "Genre");

            if (genreName != null && !string.IsNullOrEmpty(genreName.Value))
            {
                query = query.Where(x => x.Movie.Genre.Name.ToUpper().StartsWith(genreName.Value.ToUpper()));
            }

            var title = model.Search.FirstOrDefault(x => x.Name == "Title");
            if (title != null && !string.IsNullOrEmpty(title.Value))
            {
                query = query.Where(x => x.Movie.Title.ToUpper().StartsWith(title.Value.ToUpper()));
            }

            query = query.Include(x => x.Movie.Genre);

            var result = await query.Skip(model.Offset).Take(model.Limit.Value).Select(x => new
            {
                x.Movie.Id,
                x.Movie.Title,
                x.Movie.ReleaseDate,
                GenreName = x.Movie.Genre.Name,
                x.Movie.Price,
                x.Movie.Rating
            }).ToListAsync();

            var _count = query.Count();

            return Json(new
            {
                rows = result,
                total = _count,
                //To fix
                totalNotFiltered = _count
            });
        }
    }
}
