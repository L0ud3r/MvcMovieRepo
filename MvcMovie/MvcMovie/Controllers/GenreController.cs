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
        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Genre/Create
        public IActionResult Create()
        {
            return View();
        }
    }
}
