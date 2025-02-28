﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project.Models;

namespace project.Controllers
{
    public class ComplaintsController : Controller
    {
        private dbProject db = new dbProject();

        // GET: Complaints
        public ActionResult Index()
        {
            
            var complaints = db.Complaints.Include(c => c.city).Include(c => c.Citzen).Include(c => c.Complaint_Catgories).Include(c => c.Entity_Branchs).Include(c => c.Official).Include(c => c.Official1);
            return View(complaints.ToList());
        }

        // GET: Complaints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Complaint complaint = db.Complaints.Find(id);
            if (complaint == null)
            {
                return HttpNotFound();
            }
            return View(complaint);
        }

        // GET: Complaints/Create
        public ActionResult Create()
        {
            if(Session["id"]!=null)
            {
                ViewBag.comGov = new SelectList(db.Governments, "id", "name");
                ViewBag.comCity = new SelectList(db.cities, "id", "name");
                ViewBag.comCitzen = new SelectList(db.Citzens, "id", "fName");
                ViewBag.comCategory = new SelectList(db.Complaint_Catgories, "id", "Cat_Name");
                ViewBag.comEntitybranch = new SelectList(db.Entity_Branchs, "id", "title");
                ViewBag.comEntity = new SelectList(db.Entities, "id", "Title");
                ViewBag.solveby = new SelectList(db.Officials, "id", "fName");
                ViewBag.readby = new SelectList(db.Officials, "id", "fName");
                return View();
            }
            else
            {
                return RedirectToAction("login", "Citzens");
            }

        }

        // POST: Complaints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "comNumber,comTitle,comDescription,comFile,comFile2,comDate,comType,comCategory,comCity,comEntitybranch,comCitzen,isreaded,readby,solveby,isSolved,solveDescription")] Complaint complaint, HttpPostedFileBase comFile1, HttpPostedFileBase comFile2)
        {
            if (Session["id"]!=null)
            {

                if (ModelState.IsValid)
                {
                    int id = int.Parse(Session["id"].ToString());
                    complaint.comDate = DateTime.Now;
              
                    string citzenId = Session["id"].ToString();
                    string comNumber = DateTime.Now.Year.ToString() + citzenId.ToString();
                    complaint.comNumber = int.Parse(comNumber);
                    complaint.isreaded = false;
                    complaint.isSolved = false;
                    complaint.comCitzen = int.Parse(Session["id"].ToString());
                    if (complaint.comFile != null)
                    {
                        string image = System.IO.Path.GetFileName(comFile1.FileName);
                        string myPath = Server.MapPath("~/images/" + image);
                        comFile1.SaveAs(myPath);
                        complaint.comFile = image;
                    }

                    if (complaint.comFile2 != null)
                    {
                        string image2 = System.IO.Path.GetFileName(comFile2.FileName);
                        string myPath2 = Server.MapPath("~/images/" + image2);
                        comFile2.SaveAs(myPath2);
                        complaint.comFile2 = image2;
                    }
                  

                    db.Complaints.Add(complaint);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.comGov = new SelectList(db.Governments, "id", "name");
                ViewBag.comCity = new SelectList(db.cities, "id", "name", complaint.comCity);
                ViewBag.comCitzen = new SelectList(db.Citzens, "id", "fName", complaint.comCitzen);
                ViewBag.comCategory = new SelectList(db.Complaint_Catgories, "id", "Cat_Name", complaint.comCategory);
                ViewBag.comEntitybranch = new SelectList(db.Entity_Branchs, "id", "title", complaint.comEntitybranch);
                ViewBag.comEntity= new SelectList(db.Entities, "id", "title", complaint.comEntitybranch);
                ViewBag.solveby = new SelectList(db.Officials, "id", "fName", complaint.solveby);
                ViewBag.readby = new SelectList(db.Officials, "id", "fName", complaint.readby);
                return View(complaint);
            }
            else { return RedirectToAction("login", "Citzens"); }

        }
        public ActionResult Search()
        {
            if (Session["id"] != null)
            {
                int id = int.Parse(Session["id"].ToString());
                var userComplaints = db.Complaints.Where(c => c.comCitzen == id).ToList();
                return View(userComplaints);
            }
            else
            {
                return RedirectToAction("login", "Citzens");
            }
        }
    
        // GET: Complaints/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Complaint complaint = db.Complaints.Find(id);
            if (complaint == null)
            {
                return HttpNotFound();
            }
            ViewBag.comGov = new SelectList(db.Governments, "id", "name");
            ViewBag.comCity = new SelectList(db.cities, "id", "name", complaint.comCity);
            ViewBag.comCitzen = new SelectList(db.Citzens, "id", "fName", complaint.comCitzen);
            ViewBag.comCategory = new SelectList(db.Complaint_Catgories, "id", "Cat_Name", complaint.comCategory);
            ViewBag.comEntitybranch = new SelectList(db.Entity_Branchs, "id", "title", complaint.comEntitybranch);
            ViewBag.comEntity = new SelectList(db.Entities, "id", "title", complaint.comEntitybranch);
            ViewBag.solveby = new SelectList(db.Officials, "id", "fName", complaint.solveby);
            ViewBag.readby = new SelectList(db.Officials, "id", "fName", complaint.readby);
            return View(complaint);
        }

        // POST: Complaints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,comNumber,comTitle,comDescription,comFile,comFile2,comDate,comType,comCategory,comCity,comEntitybranch,comCitzen,isreaded,readby,solveby,isSolved,solveDescription")] Complaint complaint,HttpPostedFileBase comFile1,HttpPostedFileBase comFile2)
        {
            if (ModelState.IsValid)
            {
                if (complaint.comFile != null)
                {
                    string image = System.IO.Path.GetFileName(comFile1.FileName);
                    string myPath = Server.MapPath("~/images/" + image);
                    comFile1.SaveAs(myPath);
                    complaint.comFile = image;
                }

                if (complaint.comFile2 != null) {
                    string image2 = System.IO.Path.GetFileName(comFile2.FileName);
                    string myPath2 = Server.MapPath("~/images/" + image2);
                    comFile2.SaveAs(myPath2);
                    complaint.comFile2 = image2;
                }

                db.Entry(complaint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.comGov = new SelectList(db.Governments, "id", "name");
            ViewBag.comCity = new SelectList(db.cities, "id", "name", complaint.comCity);
            ViewBag.comCitzen = new SelectList(db.Citzens, "id", "fName", complaint.comCitzen);
            ViewBag.comCategory = new SelectList(db.Complaint_Catgories, "id", "Cat_Name", complaint.comCategory);
            ViewBag.comEntitybranch = new SelectList(db.Entity_Branchs, "id", "title", complaint.comEntitybranch);
            ViewBag.comEntity= new SelectList(db.Entities, "id", "title", complaint.comEntitybranch);
            ViewBag.solveby = new SelectList(db.Officials, "id", "fName", complaint.solveby);
            ViewBag.readby = new SelectList(db.Officials, "id", "fName", complaint.readby);
            return View(complaint);
        }

        // GET: Complaints/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Complaint complaint = db.Complaints.Find(id);
            if (complaint == null)
            {
                return HttpNotFound();
            }
            return View(complaint);
        }

        // POST: Complaints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Complaint complaint = db.Complaints.Find(id);
            db.Complaints.Remove(complaint);
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
