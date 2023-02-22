using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MvcMovie.Controllers
{
    public class UserController : Controller
    {
        //private readonly IConfiguration _configuration;
        //private readonly IRepository<User> _userRepository;

        //public UserController(IConfiguration configuration, IRepository<User> userRepository)
        //{
        //    _configuration = configuration;
        //    _userRepository = userRepository;
        //}

        //// ATUALIZAR METODO PARA NAO ACEDER À BD
        //// METER NO JS
        //// GET: UserController
        //public async Task<IActionResult> Index()
        //{
        //    var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("token"));

        //    var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        //    string userMail = claimMail?.Value;

        //    //var user = await GetUserByEmail(userMail);


        //    //if (user == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //var userModel = new UserViewModel
        //    //{
        //    //    Id = user.Id,
        //    //    Username = user.Username,
        //    //    Email = user.Email
        //    //};

        //    //return View(userModel);
        //    return View();
        //}

        //// ATUALIZAR METODO PARA NAO ACEDER À BD
        //// PREENCHER CAMPOS NO JS
        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    if (_userRepository.Get() == null)
        //        return NotFound();

        //    var user = _userRepository.Get().Where(x => x.Id == id && x.Active == true).SingleOrDefault();

        //    if (user == null)
        //        return null;

        //    return View(new UserViewModel()
        //    {
        //        Id = user.Id,
        //        Username = user.Username,
        //        Email = user.Email,
        //        Password = ""
        //    });
        //}

        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> RecoverPassword()
        {
            return View();
        }
    }
}
