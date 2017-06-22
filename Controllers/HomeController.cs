using HeedeMoestrup.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace HeedeMoestrup.Controllers
{
    public class HomeController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: Index
        [AllowAnonymous]
        public ActionResult Index()
        {
            var paintings = db.Paintings.Where(c => c.Artist.Featured).ToList();
            ViewBag.Title = "Forside";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;

            return View(paintings);
        }

        [AllowAnonymous]
        public ActionResult Artists(string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var artists = from a in db.Artists
                          select a;
            if (!String.IsNullOrEmpty(searchString))
            {
                artists = artists.Where(a => a.Name.Contains(searchString));
            }

            artists = artists.OrderBy(n => n.Name);
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.page = pageNumber;
            ViewBag.Title = "Kunstnere";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;

            return View(artists.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult News(string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var newsArticle = from n in db.NewsArticle
                              select n;
            if (!String.IsNullOrEmpty(searchString))
            {
                newsArticle = newsArticle.Where(n => n.Title.Contains(searchString)
                || n.SubTitle.Contains(searchString) || n.Author.Contains(searchString));
            }

            newsArticle = newsArticle.OrderBy(t => t.Date);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.page = pageNumber;
            ViewBag.Title = "Nyheder";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;

            return View(newsArticle.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult NewsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.NewsArticle.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = newsArticle.FilePaths.First(f => f.FileType == FileType.NewsPhoto).FileName;

            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        [AllowAnonymous]
        public ActionResult ArtistsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = artist.FilePaths.First(f => f.FileType == FileType.ArtistAvatar).FileName;

            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }
        [AllowAnonymous]
        public ActionResult Paintings(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "artist_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var paintings = from p in db.Paintings
                            select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                paintings = paintings.Where(p => p.Artist.Name.Contains(searchString)
                || p.Title.Contains(searchString) || p.PaintingCat.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    paintings = paintings.OrderByDescending(p => p.Title);
                    break;
                case "artist_desc":
                    paintings = paintings.OrderByDescending(p => p.Artist.Name);
                    break;
                default:
                    paintings = paintings.OrderBy(p => p.Artist.Name);
                    break;
            }

            int pageSize = 24;
            int pageNumber = (page ?? 1);
            ViewBag.page = pageNumber;
            ViewBag.Title = "Kunst";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;
            return View(paintings.ToPagedList(pageNumber, pageSize));

        }

        [AllowAnonymous]
        public ActionResult PaintingDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Painting paintings = db.Paintings.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = paintings.FilePaths.First(f => f.FileType == FileType.ItemPhoto).FileName;

            if (paintings == null)
            {
                return HttpNotFound();
            }
            return View(paintings);
        }

        [AllowAnonymous]
        public ActionResult Publications()
        {
            var artists = db.Artists.Where(h => h.HMPublication).ToList();
            ViewBag.Title = "Udgivelser";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;

            return View(artists);
        }

        [AllowAnonymous]
        public ActionResult Organizations()
        {
            ViewBag.Title = "Kunstforeninger";
            var pageHeader = db.PageHeaders.ToList();
            ViewBag.Header = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().Title;
            ViewBag.Body = pageHeader.Where(t => t.SelectedView.ToString().Contains(ViewBag.Title)).First().PageHeaderText;

            return View();

        }
    }
}