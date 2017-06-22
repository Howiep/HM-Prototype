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

namespace HeedeMoestrup.Controllers
{
    public class PaintingsController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: Paintings
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
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

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(paintings.ToPagedList(pageNumber, pageSize));
        }

        // GET: Paintings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Painting painting = db.Paintings.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = painting.FilePaths.First(f => f.FileType == FileType.ItemPhoto).FileName;

            if (painting == null)
            {
                return HttpNotFound();
            }
            return View(painting);
        }

        // GET: Paintings/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
            ViewBag.PaintingCatId = new SelectList(db.PaintingCats, "Id", "Name");
            return View();
        }

        // POST: Paintings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Create([Bind(Include = "Id,Title,Desc,Height,Width,Price,Materials,Age,PaintingCatId,ArtistId,HMPublication")] Painting painting, HttpPostedFileBase upload)
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
                        FileType = FileType.ItemPhoto
                    };
                    painting.FilePaths = new List<FilePath>();
                    painting.FilePaths.Add(photo);
                }
                //string imgPath = "images/" + imgName;
                var path = Path.Combine(Server.MapPath("~/images/"), imgName);
                //validates the posted file before saving  
                if (upload != null && upload.FileName != "")
                {
                    //then save it to the Folder
                    upload.SaveAs(path);
                }
                db.Paintings.Add(painting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", painting.ArtistId);
            ViewBag.PaintingCatId = new SelectList(db.PaintingCats, "Id", "Name", painting.PaintingCatId);
            return View(painting);
        }

        // GET: Paintings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Painting painting = db.Paintings.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = painting.FilePaths.First(f => f.FileType == FileType.ItemPhoto).FileName;
            if (painting == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", painting.ArtistId);
            ViewBag.PaintingCatId = new SelectList(db.PaintingCats, "Id", "Name", painting.PaintingCatId);
            return View(painting);
        }

        // POST: Paintings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit([Bind(Include = "Id,Title,Desc,Height,Width,Price,Materials,Age,PaintingCatId,ArtistId,HMPublication")] Painting painting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(painting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", painting.ArtistId);
            ViewBag.PaintingCatId = new SelectList(db.PaintingCats, "Id", "Name", painting.PaintingCatId);
            return View(painting);
        }

        // GET: Paintings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Painting painting = db.Paintings.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = painting.FilePaths.First(f => f.FileType == FileType.ItemPhoto).FileName;
            if (painting == null)
            {
                return HttpNotFound();
            }
            return View(painting);
        }

        // POST: Paintings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Painting painting = db.Paintings.Find(id);
            List<FilePath> fList = db.FilePaths.Where(p => p.Painting.Id == painting.Id).ToList();

            db.FilePaths.RemoveRange(fList);

            db.Paintings.Remove(painting);
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
