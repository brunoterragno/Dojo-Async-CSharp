using AsyncMVC.Models;
using AsyncMVC.Repository;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AsyncMVC.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var repo = new AuthorRepository();
            IEnumerable<Author> authors = await repo.GetAuthorsAsync();

            watcher.Stop();
            ViewBag.ElapsedTime = watcher.Elapsed.ToString();
            return View("Index", authors);
        }

        public async Task<ActionResult> Title()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var repo = new TitleRepository();            
            IEnumerable<Title> titles = await repo.GetTitlesAsync();

            watcher.Stop();
            ViewBag.ElapsedTime = watcher.Elapsed.ToString();
            return View("Title", titles);
        }

        public async Task<ActionResult> Full()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var authorRepo = new AuthorRepository();
            var titleRepo = new TitleRepository();

            var authorsTask = authorRepo.GetAuthorsAsync();
            var titlesTask = titleRepo.GetTitlesAsync();

            await Task.WhenAll(authorsTask, titlesTask);

            IEnumerable<Author> authors = authorsTask.Result;
            IEnumerable<Title> titles = titlesTask.Result;

            watcher.Stop();
            ViewBag.ElapsedTime = watcher.Elapsed.ToString();
            return View("Full", new FullViewModel { Authors = authors, Titles = titles });
        }
    }
}