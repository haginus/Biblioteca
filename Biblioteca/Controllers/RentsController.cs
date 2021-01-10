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
            ViewBag.DateStart = DateTime.Today;
            return View();
        }

        public ActionResult CreateByCustomer(int customerId)
        {
            var bookCopyFields = db.BookCopies.Where(bc => bc.Rents.Count() == 0 || bc.Rents.OrderByDescending(p => p.RentID).Select(r => r.IsReturned).FirstOrDefault())
               .Select(bc => new { CopyId = bc.CopyID, Details = bc.Book.Name + " (#" + bc.CopyID + ", " + bc.Publisher.Name + ") " });
            ViewBag.CopyID = new SelectList(bookCopyFields, "CopyID", "Details");
            var customerFields = db.Customers.Select(c => new { CustomerId = c.CustomerID, Details = c.FirstName + " " + c.LastName + " (" + c.CNP + ")" });
            ViewBag.CustomerID = new SelectList(customerFields, "CustomerID", "Details", customerId);
            ViewBag.DateStart = DateTime.Today;
            return View("Create");
        }

        public ActionResult CreateByBook(int copyId)
        {
            var bookCopyFields = db.BookCopies.Where(bc => bc.Rents.Count() == 0 || bc.Rents.OrderByDescending(p => p.RentID).Select(r => r.IsReturned).FirstOrDefault())
               .Select(bc => new { CopyId = bc.CopyID, Details = bc.Book.Name + " (#" + bc.CopyID + ", " + bc.Publisher.Name + ") " });
            ViewBag.CopyID = new SelectList(bookCopyFields, "CopyID", "Details", copyId);
            var customerFields = db.Customers.Select(c => new { CustomerId = c.CustomerID, Details = c.FirstName + " " + c.LastName + " (" + c.CNP + ")" });
            ViewBag.CustomerID = new SelectList(customerFields, "CustomerID", "Details");
            ViewBag.DateStart = DateTime.Today;
            return View("Create");
        }

        // POST: Rents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CopyID,CustomerID,DateStart,DateEnd")] Rent rent)
        {
            rent.DateStart = DateTime.Today;
            if(rent.DateEnd < rent.DateStart)
            {
                ViewBag.Errors = "Date of end can't be earlier than today.";
                return Create();
            }
            if (ModelState.IsValid)
            {   
                db.Rents.Add(rent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return Create();
        }

        public ActionResult Extend(int? id)
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
            if(rent.IsReturned)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(rent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Extend([Bind(Include = "RentID,DateEnd")] Rent rent)
        {
            Rent oldRent = db.Rents.Find(rent.RentID);
            if (oldRent == null)
            {
                return HttpNotFound();
            }
            if (oldRent.DateEnd >= rent.DateEnd)
            {
                ViewBag.Errors = "The new date should be bigger than the last one.";
                return View(oldRent);
            }
            oldRent.DateEnd = rent.DateEnd;

            db.Entry(oldRent).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Rents/Return/5
        public ActionResult Return(int? id)
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

        // POST: Rents/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnConfirmed(int id)
        {
            Rent rent = db.Rents.Find(id);
            if(rent == null)
            {
                return HttpNotFound();
            }
            if(rent.IsReturned)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rent.IsReturned = true;
            rent.DateReturned = DateTime.Today;
            db.Entry(rent).State = EntityState.Modified;
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
