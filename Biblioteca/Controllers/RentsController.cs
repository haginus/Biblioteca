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
    public class RentsController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: Rents
        public ActionResult Index()
        {
            var rents = db.Rents.Include(r => r.BookCopy).Include(r => r.Customer);
            return View(rents.ToList());
        }

        // GET: Rents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            return View(rent);
        }

        // GET: Rents/Create
        public ActionResult Create()
        {   
            var bookCopyFields = db.BookCopies.Where(bc => bc.Rents.Count() == 0 || bc.Rents.OrderByDescending(p => p.RentID).Select(r => r.IsReturned).FirstOrDefault())
                .Select(bc => new { CopyId = bc.CopyID, Details = bc.Book.Name + " (#" + bc.CopyID + ", " + bc.Publisher.Name + ") " });
            ViewBag.CopyID = new SelectList(bookCopyFields, "CopyID", "Details");
            var customerFields = db.Customers.Select(c => new { CustomerId = c.CustomerID, Details = c.FirstName + " " + c.LastName + " (" + c.CNP + ")" });
            ViewBag.CustomerID = new SelectList(customerFields, "CustomerID", "Details");
            return View();
        }

        public ActionResult CreateByCustomer(int customerId)
        {
            var bookCopyFields = db.BookCopies.Where(bc => bc.Rents.Count() == 0 || bc.Rents.OrderByDescending(p => p.RentID).Select(r => r.IsReturned).FirstOrDefault())
               .Select(bc => new { CopyId = bc.CopyID, Details = bc.Book.Name + " (#" + bc.CopyID + ", " + bc.Publisher.Name + ") " });
            ViewBag.CopyID = new SelectList(bookCopyFields, "CopyID", "Details");
            var customerFields = db.Customers.Select(c => new { CustomerId = c.CustomerID, Details = c.FirstName + " " + c.LastName + " (" + c.CNP + ")" });
            ViewBag.CustomerID = new SelectList(customerFields, "CustomerID", "Details", customerId);
            return View("Create");
        }

        public ActionResult CreateByBook(int copyId)
        {
            var bookCopyFields = db.BookCopies.Where(bc => bc.Rents.Count() == 0 || bc.Rents.OrderByDescending(p => p.RentID).Select(r => r.IsReturned).FirstOrDefault())
               .Select(bc => new { CopyId = bc.CopyID, Details = bc.Book.Name + " (#" + bc.CopyID + ", " + bc.Publisher.Name + ") " });
            ViewBag.CopyID = new SelectList(bookCopyFields, "CopyID", "Details", copyId);
            var customerFields = db.Customers.Select(c => new { CustomerId = c.CustomerID, Details = c.FirstName + " " + c.LastName + " (" + c.CNP + ")" });
            ViewBag.CustomerID = new SelectList(customerFields, "CustomerID", "Details");
            return View("Create");
        }

        // POST: Rents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CopyID,CustomerID,DateStart,DateEnd")] Rent rent)
        {
            if (ModelState.IsValid)
            {
                db.Rents.Add(rent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CopyID = new SelectList(db.BookCopies, "CopyID", "CopyID", rent.CopyID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CNP", rent.CustomerID);
            return View(rent);
        }

        // GET: Rents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            ViewBag.CopyID = new SelectList(db.BookCopies, "CopyID", "CopyID", rent.CopyID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CNP", rent.CustomerID);
            return View(rent);
        }

        // POST: Rents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RentID,CopyID,CustomerID,DateStart,DateEnd,IsReturned,DateReturned")] Rent rent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CopyID = new SelectList(db.BookCopies, "CopyID", "CopyID", rent.CopyID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CNP", rent.CustomerID);
            return View(rent);
        }

        // GET: Rents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            return View(rent);
        }

        // POST: Rents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rent rent = db.Rents.Find(id);
            db.Rents.Remove(rent);
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
