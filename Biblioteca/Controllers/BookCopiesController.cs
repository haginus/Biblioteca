using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Biblioteca.Models;

namespace Biblioteca.Controllers
{
    public class BookCopiesController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: BookCopies
        public ActionResult Index()
        {
            var bookCopies = db.BookCopies.Include(b => b.Book).Include(b => b.Publisher);
            return View(bookCopies.ToList());
        }

        // GET: BookCopies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookCopy bookCopy = db.BookCopies.Find(id);
            if (bookCopy == null)
            {
                return HttpNotFound();
            }
            return View(bookCopy);
        }

        // GET: BookCopies/Create/<ParentBookId>
        public ActionResult Create(int id)
        {
            ViewBag.BookID = new SelectList(db.Books, "BookID", "Name", id);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name");
            ViewBag.ParentId = id;
            return View();
        }

        // POST: BookCopies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookID,PublisherID,Pages")] BookCopy bookCopy)
        {
            if (ModelState.IsValid)
            {
                db.BookCopies.Add(bookCopy);
                db.SaveChanges();
                return RedirectToAction("Details", "Books", new { id = bookCopy.BookID });
            }

            ViewBag.BookID = new SelectList(db.Books, "BookID", "Name", bookCopy.BookID);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", bookCopy.PublisherID);
            return View(bookCopy);
        }

        // GET: BookCopies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookCopy bookCopy = db.BookCopies.Find(id);
            if (bookCopy == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookID = new SelectList(db.Books, "BookID", "Name", bookCopy.BookID);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", bookCopy.PublisherID);
            return View(bookCopy);
        }

        // POST: BookCopies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CopyID,BookID,PublisherID,Pages")] BookCopy bookCopy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookCopy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Books", new { id = bookCopy.BookID });
            }
            ViewBag.BookID = new SelectList(db.Books, "BookID", "Name", bookCopy.BookID);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", bookCopy.PublisherID);
            return View(bookCopy);
        }

        // GET: BookCopies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookCopy bookCopy = db.BookCopies.Find(id);
            if (bookCopy == null)
            {
                return HttpNotFound();
            }
            return View(bookCopy);
        }

        // POST: BookCopies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookCopy bookCopy = db.BookCopies.Find(id);
            db.BookCopies.Remove(bookCopy);
            db.SaveChanges();
            return RedirectToAction("Details", "Books", new { id = bookCopy.BookID });
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
