using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Biblioteca.Models;

namespace Biblioteca.Controllers
{
    public class BooksController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: Books
        public ActionResult Index()
        {
            return View(db.Books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            var serializer = new JavaScriptSerializer();
            var list = db.Authors.ToList().Select(x => new { name = x.FirstName + ' ' + x.LastName, id = x.AuthorID } );
            var json = serializer.Serialize(list);
            ViewBag.authors = json;
            ViewBag.selectedAuthors = "[]";
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,YearReleased,Description")] Book book, string Authors)
        {
            string[] authors = Authors.Split(',');
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                foreach (var authorId in authors)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO BookAuthor VALUES (" + book.BookID + ", " + authorId + ")");
                }
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var serializer = new JavaScriptSerializer();
            var list = db.Authors.ToList().Select(x => new { name = x.FirstName + ' ' + x.LastName, id = x.AuthorID });
            var json = serializer.Serialize(list);
            ViewBag.authors = json;
            var selectedList = book.Authors.Select(x => x.AuthorID);
            var jsonSelected = serializer.Serialize(selectedList);
            ViewBag.selectedAuthors = jsonSelected;
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,Name,YearReleased,Description")] Book book, string Authors)
        {
            string[] authors = Authors.Split(',');
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("DELETE FROM BookAuthor WHERE BookID = " + book.BookID);
                foreach (var authorId in authors)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO BookAuthor VALUES (" + book.BookID + ", " + authorId + ")");
                }
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
