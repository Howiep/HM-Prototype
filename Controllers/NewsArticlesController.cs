using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HeedeMoestrup.Models;
using System.IO;
using PagedList;
using HtmlAgilityPack;

namespace HeedeMoestrup.Controllers
{
    public class NewsArticlesController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: NewsArticles
        public ActionResult Index(string searchString, string currentFilter, int? page)
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
            return View(newsArticle.ToPagedList(pageNumber, pageSize));
        }

        // GET: NewsArticles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.NewsArticle.Find(id);
            ViewBag.imgSrc = newsArticle.FilePaths.First(f => f.FileType == FileType.NewsPhoto).FileName;

            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // GET: NewsArticles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Create([Bind(Include = "Id,Title,SubTitle,Author,BodyText")] NewsArticle newsArticle, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                //sets the image path  
                string imgName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);

                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new FilePath
                    {
                        FileName = imgName,
                        FileType = FileType.NewsPhoto
                    };
                    newsArticle.FilePaths = new List<FilePath>();
                    newsArticle.FilePaths.Add(photo);

                }
                //string imgPath = "images/" + imgName;
                var path = Path.Combine(Server.MapPath("~/images/"), imgName);
                //validates the posted file before saving  
                if (upload != null && upload.FileName != "")
                {
                    //then save it to the Folder
                    upload.SaveAs(path);
                }

                newsArticle.Date = DateTime.Now;
                db.NewsArticle.Add(newsArticle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsArticle);
        }

        // GET: NewsArticles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.NewsArticle.Find(id);
            ViewBag.imgSrc = newsArticle.FilePaths.First(f => f.FileType == FileType.NewsPhoto).FileName;

            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // POST: NewsArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit([Bind(Include = "Id,Title,SubTitle,Date,Author,BodyText")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsArticle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsArticle);
        }

        // GET: NewsArticles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.NewsArticle.Find(id);
            ViewBag.imgSrc = newsArticle.FilePaths.First(f => f.FileType == FileType.NewsPhoto).FileName;

            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // POST: NewsArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsArticle newsArticle = db.NewsArticle.Find(id);
            List<FilePath> fList = db.FilePaths.Where(p => p.NewsArticle.Id == newsArticle.Id).ToList();

            db.FilePaths.RemoveRange(fList);
            db.NewsArticle.Remove(newsArticle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
