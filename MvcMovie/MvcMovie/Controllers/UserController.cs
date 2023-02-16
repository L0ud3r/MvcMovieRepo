using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MvcMovie.Controllers
{
    public class UserController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public UserController(IConfiguration configuration, IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        // GET: UserController
        public ActionResult Index()
        {

            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View(new UserViewModel());
        }

        #region REGISTAR

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Username,Email,Password")] UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User userInsert = new User();

                    userInsert.Username = user.Username;
                    userInsert.Email = user.Email;

                    CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    userInsert.PassHash = Convert.ToBase64String(passwordHash);
                    userInsert.PassSalt = Convert.ToBase64String(passwordSalt);

                    userInsert.CreatedDate = DateTime.Now;
                    userInsert.UpdatedDate = DateTime.Now;
                    userInsert.Active = true;

                    _userRepository.Insert(userInsert);
                    _userRepository.Save();
                    return RedirectToAction(nameof(Index));
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


        #endregion

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        #region Login
        public static bool VerifyAccount(User account)
        {
            //Verificar se existe user na databse com mail e ativo   
            //se for null -> return false
            // verficar pass:
            /*
                if (!MvcMovie.HashSaltPW.VerifyPasswordHash(account.Pass, Convert.FromBase64String(cliente.Pass), Convert.FromBase64String(cliente.PassSalt)))
                {
                    throw new ArgumentException("Password Errada.", "account");
                }
            */
            return true;
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(100),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new UserViewModel());
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login([Bind("Username, Password")] UserViewModel account)
        {
            //Procurar na database user com email
            var user = _userRepository.Get().Where(x => x.Email == account.Email).FirstOrDefault();

            if(user != null)
            {
                bool check = VerifyAccount(user);
                string token = String.Empty;

                if (check)
                {
                    token = CreateToken(user);
                    Console.WriteLine(token);
                }
                else
                    return new JsonResult("Wrong Pass");

                return RedirectToAction(nameof(Index));
            }

            return new JsonResult("Wrong Email");
        }

        #endregion

    }
}
