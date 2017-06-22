using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HeedeMoestrup.Models;

namespace HeedeMoestrup.Controllers
{
    public class PageHeadersController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: PageHeaders
        public ActionResult Index()
        {
            return View(db.PageHeaders.ToList());
        }

        // GET: PageHeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PageHeader pageHeader = db.PageHeaders.Find(id);
            if (pageHeader == null)
            {
                return HttpNotFound();
            }
            return View(pageHeader);
        }

        // GET: PageHeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PageHeader pageHeader = db.PageHeaders.Find(id);
            if (pageHeader == null)
            {
                return HttpNotFound();
            }
            return View(pageHeader);
        }

        // POST: PageHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,PageHeaderText,SelectedView")] PageHeader pageHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pageHeader).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pageHeader);
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
