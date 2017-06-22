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
    public class ArtistsController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: Artists
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

            var artists = from a in db.Artists
                          select a;
            if (!String.IsNullOrEmpty(searchString))
            {
                artists = artists.Where(a => a.Name.Contains(searchString));
            }

            artists = artists.OrderBy(n => n.Name);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(artists.ToPagedList(pageNumber, pageSize));
        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
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

        // GET: Artists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Create([Bind(Include = "Id,Name,Desc,Featured,HMPublication")] Artist artist, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                //sets the image path  
                string imgName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(upload.FileName);

                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new FilePath
                    {
                        FileName = imgName,
                        FileType = FileType.ArtistAvatar
                    };
                    artist.FilePaths = new List<FilePath>();
                    artist.FilePaths.Add(photo);
                }
                //string imgPath = "images/" + imgName;
                var path = Path.Combine(Server.MapPath("~/images/"), imgName);
                //validates the posted file before saving  
                if (upload != null && upload.FileName != "")
                {
                    //then save it to the Folder
                    upload.SaveAs(path);
                }
                db.Artists.Add(artist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artist);
        }

        // GET: Artists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = artist.FilePaths.First(f => f.FileType == HeedeMoestrup.Models.FileType.ArtistAvatar).FileName;
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit([Bind(Include = "Id,Name,Desc,Featured,HMPublication")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artist);
        }

        // GET: Artists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);
            ViewBag.imgSrc = artist.FilePaths.First(f => f.FileType == HeedeMoestrup.Models.FileType.ArtistAvatar).FileName;
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Artist artist = db.Artists.Include(i => i.FilePaths).SingleOrDefault(i => i.Id == id);

            List<Painting> pList = db.Paintings.Where(p => p.Artist.Id == artist.Id).ToList();
            List<FilePath> fList = db.FilePaths.Where(p => p.Artist.Id == artist.Id).ToList();

            db.FilePaths.RemoveRange(fList);
            db.Paintings.RemoveRange(pList);
            db.Artists.Remove(artist);
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
