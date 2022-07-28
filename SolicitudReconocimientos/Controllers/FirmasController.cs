using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Catrinas.Tags;
using Model.Commons;
using Rec.Models;
using SolicitudReconocimientos.Models;
using SolicitudReconocimientos.mx.gob.edomex.qasistemas2;

namespace SolicitudReconocimientos.Controllers
{
    [Permiso(Permiso = RolesPermisos.vista_supeadministrador)]
    public class FirmasController : Controller
    {
        private ReconocimientosContext db = new ReconocimientosContext();

        // GET: Firmas
        public ActionResult Index()
        {
            ViewBag.usuario = db.Usuario.ToList();
            return View(db.Firma.ToList());
        }

     

        // GET: Firmas/Details/5
        public ActionResult Details(long? id)
        {
            //documentoVO documento = new documentoVO();
            //solicitudVO solicitud = new solicitudVO();
            //solicitud.asunto=
            //    solicitud.firmantes
             
            ViewBag.usuario = db.Usuario.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Firma firma = db.Firma.Find(id);
            if (firma == null)
            {
                return HttpNotFound();
            }
            return View(firma);
        }

        // GET: Firmas/Create
        public ActionResult Create()
        {
            ViewBag.usuario = db.Usuario.ToList();
            return View();
        }

        // POST: Firmas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFirma,Nombre")] Firma firma)
        {
            if (ModelState.IsValid)
            {
                db.Firma.Add(firma);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(firma);
        }

        // GET: Firmas/Edit/5
        public ActionResult Edit(long? id)
        {
            ViewBag.usuario = db.Usuario.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Firma firma = db.Firma.Find(id);
            if (firma == null)
            {
                return HttpNotFound();
            }
            return View(firma);
        }

        // POST: Firmas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFirma,Nombre")] Firma firma)
        {
            ViewBag.usuario = db.Usuario.ToList();
            if (ModelState.IsValid)
            {
                db.Entry(firma).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(firma);
        }

        // GET: Firmas/Delete/5
        public ActionResult Delete(long? id)
        {
            ViewBag.usuario = db.Usuario.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Firma firma = db.Firma.Find(id);
            if (firma == null)
            {
                return HttpNotFound();
            }
            return View(firma);
        }

        // POST: Firmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ViewBag.usuario = db.Usuario.ToList();
            Firma firma = db.Firma.Find(id);
            db.Firma.Remove(firma);
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
