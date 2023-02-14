using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers
{
    public class HelloWorldController : Controller
    {
        // https://localhost:{PORT}/HelloWorld
        // GET: /HelloWorld/
        public IActionResult Index()
        {
            return View();
        }

        // https://localhost:{PORT}/HelloWorld/Welcome
        // GET: /HelloWorld/WelcomeBasic/ 
        public string WelcomeBasic()
        {
            return HtmlEncoder.Default.Encode("This is the Welcome action method...");
        }

        // https://localhost:{PORT}/HelloWorld/Welcome?name={name}&numtimes={numTimes}
        // GET: /HelloWorld/Welcome/ 
        public IActionResult WelcomeNumTimes(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }

        // https://localhost:{PORT}/HelloWorld/WelcomeID/{ID}?name={name}
        // GET: /HelloWorld/WelcomeID/
        public string WelcomeID(string name = "User", int ID = 1)
        {
            return HtmlEncoder.Default.Encode($"Hello {name}, ID: {ID}");
        }
    }
}
