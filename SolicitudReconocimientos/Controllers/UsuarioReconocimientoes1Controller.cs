using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SolicitudReconocimientos.Models;
using Helper;

namespace SolicitudReconocimientos.Controllers
{
    public class UsuarioReconocimientoes1Controller : Controller
    {
        private ReconocimientosContext db = new ReconocimientosContext();

        // GET: UsuarioReconocimientoes1
        public ActionResult Index()

        {
            var reconocimientos = db.Reconocimientos.Include(r => r.TipoDocumento);
            var id = SessionHelper.GetUser();
            ViewBag.total = db.Reconocimientos.Where(x => x.IdUsuario == id).ToList();

            ViewBag.lista = db.Reconocimientos.Where(u => u.IdUsuario == id);



            var usuarioReconocimiento = db.UsuarioReconocimiento.Include(u => u.Reconocimientos).Include(u => u.Usuario);
            return View(usuarioReconocimiento.ToList());
        }

        // GET: UsuarioReconocimientoes1/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioReconocimiento usuarioReconocimiento = db.UsuarioReconocimiento.Find(id);
            if (usuarioReconocimiento == null)
            {
                return HttpNotFound();
            }
            return View(usuarioReconocimiento);
        }

        // GET: UsuarioReconocimientoes1/Create
        public ActionResult Create()
        {
            ViewBag.Reconocimiento = new SelectList(db.Reconocimientos, "Identificador", "Nombre");
            ViewBag.UsuarioId = new SelectList(db.Usuario, "id", "NombreTipo");
            return View();
        }

        // POST: UsuarioReconocimientoes1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Indentificador,UsuarioId,Reconocimiento")] UsuarioReconocimiento usuarioReconocimiento)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioReconocimiento.Add(usuarioReconocimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Reconocimiento = new SelectList(db.Reconocimientos, "Identificador", "Nombre", usuarioReconocimiento.Reconocimiento);
            ViewBag.UsuarioId = new SelectList(db.Usuario, "id", "NombreTipo", usuarioReconocimiento.UsuarioId);
            return View(usuarioReconocimiento);
        }

        // GET: UsuarioReconocimientoes1/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioReconocimiento usuarioReconocimiento = db.UsuarioReconocimiento.Find(id);
            if (usuarioReconocimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.Reconocimiento = new SelectList(db.Reconocimientos, "Identificador", "Nombre", usuarioReconocimiento.Reconocimiento);
            ViewBag.UsuarioId = new SelectList(db.Usuario, "id", "NombreTipo", usuarioReconocimiento.UsuarioId);
            return View(usuarioReconocimiento);
        }

        // POST: UsuarioReconocimientoes1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Indentificador,UsuarioId,Reconocimiento")] UsuarioReconocimiento usuarioReconocimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioReconocimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Reconocimiento = new SelectList(db.Reconocimientos, "Identificador", "Nombre", usuarioReconocimiento.Reconocimiento);
            ViewBag.UsuarioId = new SelectList(db.Usuario, "id", "NombreTipo", usuarioReconocimiento.UsuarioId);
            return View(usuarioReconocimiento);
        }

        // GET: UsuarioReconocimientoes1/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioReconocimiento usuarioReconocimiento = db.UsuarioReconocimiento.Find(id);
            if (usuarioReconocimiento == null)
            {
                return HttpNotFound();
            }
            return View(usuarioReconocimiento);
        }

        // POST: UsuarioReconocimientoes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            UsuarioReconocimiento usuarioReconocimiento = db.UsuarioReconocimiento.Find(id);
            db.UsuarioReconocimiento.Remove(usuarioReconocimiento);
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
