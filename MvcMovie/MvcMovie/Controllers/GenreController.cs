using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using System.Web;


namespace MvcMovie.Controllers
{
    public class GenreController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;

        public GenreController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;

        }

        // GET: Genre/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GenreViewModel genre)
        {
            if (ModelState.IsValid)
            {
                Genre genreInsert = new Genre();

                var genreDb = _genreRepository.Get().Where(x => x.Name == genre.Name).SingleOrDefault();

                if (genreDb == null)
                {
                    genreInsert.Name = genre.Name;

                    genreInsert.CreatedDate = DateTime.Now;
                    genreInsert.UpdatedDate = DateTime.Now;
                    genreInsert.Active = true;

                    _genreRepository.Insert(genreInsert);
                    _genreRepository.Save();
                }
                else
                {
                    /*
                     * 
                     * 
                        ALERT USER!!!
                     *
                     *
                     */
                }

                return RedirectToAction("Index", "Movies");
            }
            return View(genre);
        }
    }
}
