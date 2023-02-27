using Microsoft.AspNetCore.Mvc;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MvcMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace MvcMovie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public UserController(IConfiguration configuration, IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("GetUserByToken")]
        public async Task<User> GetUserByToken(string token)
        {
            var tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var claimMail = tokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userMail = claimMail?.Value;

            // Validate that we have a bearer token.

            //string emailCliente = User.FindFirstValue(ClaimTypes.Email);

            return _userRepository.Get().Where(x => x.Email == userMail && x.Active == true).SingleOrDefault();
        }

        // POST: UserController/Create
        [HttpPost("Register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Email,Password")] UserViewModel user)
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

                    string jsonObject = JsonSerializer.Serialize(
                    _userRepository.Get().OrderBy(x => x.Id).LastOrDefault(),
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    });

                    return Json(jsonObject);
                }

                return BadRequest();
            }
            catch
            {
                return NotFound();
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

        [HttpPost("Edit")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Username,Email,Password")] UserViewModel user)
        {
            //if (id != user.Id)
            //    return NotFound();

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

                    string jsonObject = JsonSerializer.Serialize(
                        userEdit,
                        new JsonSerializerOptions
                        {
                            ReferenceHandler = ReferenceHandler.Preserve
                        });

                    return Json(jsonObject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }

        // POST: User/Remove
        [HttpPost("Remove"), ActionName("Remove")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            if (_userRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.User' is null.");
            }

            var user = _userRepository.Get().Where(x => x.Id == id && x.Active == true).SingleOrDefault();

            if (user != null)
            {
                user.Active = false;

                //Inativar todas as outras ligacoes?

                _userRepository.Update(user);
                _userRepository.Save();

                return new JsonResult(true) { StatusCode = 200 };
            }

            return NotFound();
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

            // Disable issuer and audience validation
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = key
            };

            // Verify if the token is valid
            var handler = new JwtSecurityTokenHandler();
            var result = handler.ValidateToken(jwt, tokenValidationParameters, out var validatedToken);

            return jwt;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([Bind("Email, Password")] LoginViewModel account)
        {
            //Procurar na database user com email
            var user = _userRepository.Get().Where(x => x.Email == account.Email && x.Active == true).FirstOrDefault();

            if (user != null)
            {
                if (!VerifyPasswordHash(account.Password, Convert.FromBase64String(user.PassHash), Convert.FromBase64String(user.PassSalt)))
                {
                    return new JsonResult(false) { StatusCode = 400, Value = "Wrong Password" };
                }

                string token = CreateToken(user);

                // Armazenar token de sessão
                HttpContext.Session.SetString("token", token);

                return new JsonResult(true) { StatusCode = 200, Value = token };
            }

            return new JsonResult(false) { StatusCode = 400, Value = "Wrong Email" };
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Check if the session token exists
            if (HttpContext.Session.TryGetValue("token", out byte[] token))
            {
                // Remove the session token
                HttpContext.Session.Remove("token");
            }

            // Redirect to the login page or any other page as needed
            return new JsonResult(true) { StatusCode = 200 };
        }

        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([Bind("Email, Password")] LoginViewModel account)
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

                return new JsonResult(true) { StatusCode = 200 };
            }

            return BadRequest("Wrong email");
        }

        #endregion
    }
}
