using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovieDAL.Entities;

namespace MvcMovie.Controllers
{
    public class SharedController : Controller
    {
        // GET: SharedController
        public ActionResult Index()
        {
            var errorModel = new ErrorViewModel
            {
                ErrorText = string.Empty,
            };

            return View(errorModel);
        }

        // GET: SharedController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SharedController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SharedController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: SharedController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SharedController/Edit/5
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

        // GET: SharedController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SharedController/Delete/5
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
    }
}
