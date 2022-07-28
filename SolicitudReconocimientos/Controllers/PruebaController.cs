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
using ConcursoCatrinas.Tags;
using Catrinas.Tags;
using Model.Commons;
using System.IO;
using Excel;
using Rotativa;

namespace SolicitudReconocimientos.Controllers
{
    
    public class PruebaController : Controller
    {
        private ReconocimientosContext db = new ReconocimientosContext();
        // GET: Prueba
        public ActionResult Index()
        {
            var id = SessionHelper.GetUser();
            ViewBag.total = db.Reconocimientos.Where(x => x.IdUsuario == id).ToList();
            ViewBag.todos = db.UsuarioReconocimiento.ToList();
            ViewBag.lista = db.UsuarioReconocimiento.Include(u => u.Usuario.Unidad).Where(u => u.Reconocimientos.IdUsuario == id).OrderBy(i => i.Reconocimientos.Identificador);
            ViewBag.UltimoRegistro = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == id).Select(u => u.Reconocimientos.Folio).Max();
            return View();
           
        }

        [Autenticado]
        public ActionResult Pdf(int id)
        {
            return new ActionAsPdf("Print/"+id) {
                PageOrientation=Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 2, Bottom = -2, Right = 2, Top = 2 },
                PageSize = Rotativa.Options.Size.Letter,

            }
            ;
        }
        
        public ActionResult print(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reconocimientos reconocimientos = db.Reconocimientos.Find(id);
            if (reconocimientos == null)
            {
                return HttpNotFound();
            }
            return View(reconocimientos);
        }
    }
}

   
    