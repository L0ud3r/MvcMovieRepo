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

        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public UserController(IConfiguration configuration, IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return _userRepository.Get().Where(x => x.Email == email && x.Active == true).SingleOrDefault();
        }

        // GET: UserController
        public async Task<IActionResult> Index()
        {
            var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("token"));

            var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userMail = claimMail?.Value;

            var user = await GetUserByEmail(userMail);

            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return View(userModel);
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
                    return RedirectToAction("Login");
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (_userRepository.Get() == null)
                return NotFound();

            var user = _userRepository.Get().Where(x => x.Id == id && x.Active == true).SingleOrDefault();

            if (user == null)
                return null;

            return View(new UserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Password")] UserViewModel user)
        {
            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    User userEdit = _userRepository.Get().FirstOrDefault(x => x.Id == user.Id && x.Active == true);

                    if (userEdit == null)
                        return NotFound();

                    userEdit.Username = user.Username;
                    userEdit.Email = user.Email;

                    userEdit.UpdatedDate = DateTime.Now;

                    _userRepository.Update(userEdit);
                    _userRepository.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                // Redirects to the list of movies (Movies/Index)
                return RedirectToAction("Index", "User");
            }
            return View(user);
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

        // POST: User/Remove/{id}
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            if (_userRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.User' is null.");
            }

            var user = _userRepository.Get().Where(x => x.Id == id).SingleOrDefault();
            
            if (user != null)
            {
                user.Active = false;

                //Inativar todas as outras ligacoes?

                _userRepository.Update(user);
            }

            _userRepository.Save();
            return RedirectToAction("Login", "User");
        }

        #region Login and Logout
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
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

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

        [HttpPost]
        public IActionResult Login([Bind("Email, Password")] UserViewModel account)
        {
            //Procurar na database user com email
            var user = _userRepository.Get().Where(x => x.Email == account.Email && x.Active == true).FirstOrDefault();

            if(user != null)
            {
                if (!VerifyPasswordHash(account.Password, Convert.FromBase64String(user.PassHash), Convert.FromBase64String(user.PassSalt)))
                {
                    return new JsonResult("Wrong Pass");
                }

                string token = CreateToken(user);

                // Armazenar token de sessão
                HttpContext.Session.SetString("token", token);

                return RedirectToAction("Index", "Movies");
            }

            return new JsonResult("Wrong Email");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Check if the session token exists
            if (HttpContext.Session.TryGetValue("token", out byte[] token))
            {
                // Remove the session token
                HttpContext.Session.Remove("token");
            }

            // Redirect to the login page or any other page as needed
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<IActionResult> RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword([Bind("Email, Password")] UserViewModel account)
        {
            //Procurar na database user com email
            var user = _userRepository.Get().Where(x => x.Email == account.Email && x.Active == true).FirstOrDefault();

            if (user != null)
            {
                CreatePasswordHash(account.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PassHash = Convert.ToBase64String(passwordHash);
                user.PassSalt = Convert.ToBase64String(passwordSalt);

                user.UpdatedDate = DateTime.Now;

                _userRepository.Update(user);
                _userRepository.Save();

                return RedirectToAction("Login", "User");
            }

            return View();
        }

        #endregion

    }
}
