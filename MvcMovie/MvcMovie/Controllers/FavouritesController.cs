using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using MvcMovie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MvcMovieDAL.Migrations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MvcMovie.Controllers
{
    public class FavouritesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
