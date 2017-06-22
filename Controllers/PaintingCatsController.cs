using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HeedeMoestrup.Models;
using PagedList;

namespace HeedeMoestrup.Controllers
{
    public class PaintingCatsController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: PaintingCats
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
            var category = from n in db.PaintingCats
                              select n;
            if (!String.IsNullOrEmpty(searchString))
            {
                category = category.Where(n => n.Name.Contains(searchString));
            }

            category = category.OrderBy(n => n.Name);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(category.ToPagedList(pageNumber, pageSize));
        }

        // GET: PaintingCats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaintingCat paintingCat = db.PaintingCats.Find(id);
            if (paintingCat == null)
            {
                return HttpNotFound();
            }
            return View(paintingCat);
        }

        // GET: PaintingCats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaintingCats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Create([Bind(Include = "Id,Name")] PaintingCat paintingCat)
        {
            if (ModelState.IsValid)
            {
                db.PaintingCats.Add(paintingCat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paintingCat);
        }

        // GET: PaintingCats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaintingCat paintingCat = db.PaintingCats.Find(id);
            if (paintingCat == null)
            {
                return HttpNotFound();
            }
            return View(paintingCat);
        }

        // POST: PaintingCats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] PaintingCat paintingCat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paintingCat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paintingCat);
        }

        // GET: PaintingCats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaintingCat paintingCat = db.PaintingCats.Find(id);
            if (paintingCat == null)
            {
                return HttpNotFound();
            }
            return View(paintingCat);
        }

        // POST: PaintingCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            PaintingCat paintingCat = db.PaintingCats.Find(id);
            db.PaintingCats.Remove(paintingCat);
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
