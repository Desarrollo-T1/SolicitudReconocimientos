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
using Model;
using System.Net.Mail;


namespace SolicitudReconocimientos.Controllers
{
    [Autenticado]
    public class ReconocimientosController : Controller
    {
        private ReconocimientosContext db = new ReconocimientosContext();

        // GET: Reconocimientos

        public ActionResult Index()
        {
            var reconocimientos = db.Reconocimientos.Include(r => r.TipoDocumento);
            var id = SessionHelper.GetUser();
            ViewBag.total = db.Reconocimientos.Where(x => x.IdUsuario == id).ToList();
            ViewBag.todos = db.UsuarioReconocimiento.ToList();
            ViewBag.lista = db.UsuarioReconocimiento.Include(u => u.Usuario.Unidad).Where(u => u.Reconocimientos.IdUsuario == id).OrderBy(i => i.Reconocimientos.Identificador);
            ViewBag.usuario = db.Usuario.ToList();
            /*ViewBag.UltimoRegistro = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == id).Select(u => u.Reconocimientos.Folio).Max();*///selecciona los folios por undiad
            ViewBag.UltimoRegistro = db.UsuarioReconocimiento.Select(u => u.Reconocimientos.Folio).Max();

            return View(reconocimientos.ToList());
        }
        [Autenticado]
        public ActionResult Pdf()
        {
            return new Rotativa.ActionAsPdf("Print");
        }

        [Autenticado]
        public ActionResult prueba()
        {
            var reconocimientos = db.Reconocimientos.Include(r => r.TipoDocumento);
            var id = SessionHelper.GetUser();
            ViewBag.total = db.Reconocimientos.Where(x => x.IdUsuario == id).ToList();
            ViewBag.todos = db.UsuarioReconocimiento.ToList();
            ViewBag.lista = db.UsuarioReconocimiento.Include(u => u.Usuario.Unidad).Where(u => u.Reconocimientos.IdUsuario == id).OrderBy(i => i.Reconocimientos.Identificador);
            ViewBag.UltimoRegistro = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == id).Select(u => u.Reconocimientos.Folio).Max();


            return View(reconocimientos.ToList());
        }


        // GET: Reconocimientos/Details/5
        public ActionResult Details(long? id)
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



        // GET: Reconocimientos/Create
        [Permiso(Permiso = RolesPermisos.vista_usuario)]
        public ActionResult Create()
        {
            var id = SessionHelper.GetUser();
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre");
            //var ultimo = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == id).Select(u => u.Reconocimientos.Folio).Count(); sacar folios por usuario
            var ultimo = db.UsuarioReconocimiento.Select(u => u.Reconocimientos.Folio).Count();
            int ul = Convert.ToInt32(ultimo);
            //ViewBag.UltimoRegistro = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == id).Select(u => u.Reconocimientos.Folio).Max();
            ViewBag.UltimoRegistro = ul + 1;
            return View();
        }


        //[PermisoAttribute(Permiso = RolesPermisos.vista_usuario)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int Identificador, string FOLIO, string Libro, string Foja,int IdQuien ,int IdUsuario, string Nombre, int IdTipoDocumento, string Motivo, string FechaEvento,int IdFirma, string FechaRegistro)
        {
            //, string Autorizado, string Archivo, string AcuseEntrega

            Reconocimientos reconocimientos = new Reconocimientos();
            UsuarioReconocimiento usuariorec = new UsuarioReconocimiento();
            if (ModelState.IsValid)
            {
                using (var transact = db.Database.BeginTransaction()) {

                    try
                    {


                        var ultimo = db.UsuarioReconocimiento.Select(u => u.Reconocimientos.Folio).Count();
                        int ul = Convert.ToInt32(ultimo);
                        var UltimoRegistro = ul + 1;

                        var total = UltimoRegistro;
                        var ceros = "";
                        if (total<=9) {
                            ceros = "00";
                        }
                        if (total > 9 && total <=99)
                        {
                            ceros = "0";
                        }
                        if (total >= 100 && total <= 1000)
                        {
                            ceros = "";
                        }
                        
                        var completo = ceros + total;

                        reconocimientos.Identificador = reconocimientos.Identificador;
                        reconocimientos.IdUsuario = IdUsuario;
                        //reconocimientos.Folio = "0";
                        reconocimientos.Folio = completo;
                        reconocimientos.Libro = Libro;
                        reconocimientos.Foja = Foja;
                        reconocimientos.IdQuien=IdQuien;
                        reconocimientos.Nombre = Nombre;
                        reconocimientos.IdTipoDocumento = IdTipoDocumento;
                        reconocimientos.Motivo = Motivo;
                        reconocimientos.FechaEvento = FechaEvento;
                        reconocimientos.FechaRegistro = FechaRegistro;
                        reconocimientos.IdFirma = IdFirma;
                        reconocimientos.Autorizado = "Pendiente";
                        reconocimientos.Archivo = " ";
                        reconocimientos.AcuseEntrega = " ";

                        var rm = new ResponseModel();
                        var mail = new MailMessage();
                        mail.From = new MailAddress("abogado.general@umb.mx");
                        mail.To.Add("abogado.general@umb.mx");
                        //mail.From = new MailAddress("desarrollo.ti@umb.mx");
                        //mail.To.Add("desarrollo.ti@umb.mx");
                        mail.CC.Add("ti@umb.mx");
                        mail.CC.Add("desarrollo.ti@umb.mx");
                        mail.Subject = "SISTEMA DE SOLICITUD DE RECONOCIMIENTOS";
                        mail.IsBodyHtml = true;
                        //usuario.Contraseña = PasswordCode.Generate(5, 5);C:\Users\Desarrollo TI\Desktop\Reconocimientos edit\SolicitudReconocimientos\SolicitudReconocimientos\Imagenes\umb.PNG
                        string Mensaje = "<html><body><b>Solicitud de Reconocimientos</b></body></html>";
                        string html3 = "<html><body><img src=\"~/Imagenes/umb.png\"></body></html>";
                        string html1 = "<html><body><p> El usuario </body></html>";
                        //string html2 = "<html><body><p> CONTRASEÑA: </body></html>";
                        string html = "<html><body><p>CLICK PARA COMENZAR: <a href=\"http://reconocimientos.umb.edu.mx:8085\" > INICIAR SESIÓN</a></body></html>";
                        int idUsuario = IdUsuario;
                        var usuario = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Nombre).ToList();
                        var unidad = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Unidad.Nombre).ToList();
                        var documento = db.UsuarioReconocimiento.Where(u => u.Usuario.id == idUsuario).Select(u => u.Reconocimientos.TipoDocumento.Nombre).ToList();


                        mail.Body = "" + html3 + " " + Mensaje + " " + html1 + usuario[0] + " " + "de" + " " + unidad[0] + " " + "Solicito uno o varios" + " " + "Reconocimientos" + "<hr />" + "Inresa a " + " " + html + " " + " Para gestionarlos";//model.Mensaje;

                        var SmtpServer = new SmtpClient("smtp.gmail.com"); // or "smtp.gmail.com"
                        SmtpServer.Port = 587;//587
                        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpServer.UseDefaultCredentials = false;

                        // Agrega tu correo y tu contraseña, hemos usado el servidor de Outlook.
                        SmtpServer.Credentials = new NetworkCredential("reconocimientosnotreply@umb.mx", "reconocimientos.");
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                        rm.function = "CerrarContacto();";
                        rm.function = "Mensaje();";




                        db.Reconocimientos.Add(reconocimientos);
                        db.SaveChanges();


                        var iduser = SessionHelper.GetUser();
                        var ultrec = db.Reconocimientos.Where(u => u.IdUsuario == iduser).Select(u => u.Identificador).Max();


                        usuariorec.UsuarioId = iduser;
                        usuariorec.Reconocimiento = ultrec;
                        db.UsuarioReconocimiento.Add(usuariorec);
                        db.SaveChanges();

                        transact.Commit();

                        return RedirectToAction("Index");

                    }
                    catch {
                        
                        transact.Rollback();
                    }

                }





            }

            //ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            //ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargaMasiva(string[] Libro, string[] Foja, string[] Nombre, int[] Documento, string[] Motivo, string[] FechaEvento)
        {




            if (ModelState.IsValid)
            {
                using (var transact = db.Database.BeginTransaction())
                {

                    try
                    {

                        Reconocimientos reconocimientos = new Reconocimientos();
                        UsuarioReconocimiento usuariorec = new UsuarioReconocimiento();

                        var reg = Nombre.Count();
                        var IdUsuario = SessionHelper.GetUser();
                        for (int i = 0; i <= reg - 1; i++)
                        {
                            int f;
                            var ultimo = db.UsuarioReconocimiento.Where(u => u.Reconocimientos.IdUsuario == IdUsuario).Count();
                            int ul = Convert.ToInt32(ultimo);

                            f = i + 1;
                            int id = ul + f;
                            ViewBag.UltimoRegistro = id;


                            var total = ViewBag.UltimoRegistro;
                            var ceros = "000";
                            var completo = ceros + total;

                            //reconocimientos.Identificador = reconocimientos.Identificador;
                            reconocimientos.IdUsuario = IdUsuario;
                            reconocimientos.Folio = completo;
                            reconocimientos.Libro = Libro[i];
                            reconocimientos.Foja = Foja[i];
                            reconocimientos.Nombre = Nombre[i];
                            reconocimientos.IdTipoDocumento = Documento[i];
                            reconocimientos.Motivo = Motivo[i];
                            reconocimientos.FechaEvento = FechaEvento[i];
                            reconocimientos.FechaRegistro = DateTime.Today.ToLongDateString();
                            reconocimientos.Autorizado = "Pendiente";
                            reconocimientos.Archivo = " ";
                            reconocimientos.AcuseEntrega = " ";

                            db.Reconocimientos.Add(reconocimientos);
                            db.SaveChanges();




                        }
                        var reg1 = Nombre.Count();

                        for (int a = 1; a <= reg1; a++)
                        {


                            var ultrec = db.Reconocimientos.Where(u => u.IdUsuario == IdUsuario).Select(u => u.Identificador).Max();


                            usuariorec.UsuarioId = IdUsuario;
                            usuariorec.Reconocimiento = ultrec + 1 - a;
                            db.UsuarioReconocimiento.Add(usuariorec);
                            db.SaveChanges();
                        }

                        transact.Commit();

                        return Redirect("~/Home");

                    }
                    catch
                    {

                        transact.Rollback();
                    }

                }
            }


            return View();
        }

        [Permiso(Permiso = RolesPermisos.Vista_Foliador)]
        //Agregar Folio
        public ActionResult AgregarFolio(long? id)
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
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }

        // POST: Reconocimientos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Permiso(Permiso = RolesPermisos.Vista_Foliador)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarFolio(int Identificador, string Folio, string Libro, string Foja, int IdUsuario, string Nombre, int IdTipoDocumento, string Motivo,
            string FechaEvento, string FechaRegistro, string Autorizado, string Comentario, string Archivo, string AcuseEntrega)
        {
            Reconocimientos reconocimientos = new Reconocimientos();
            if (ModelState.IsValid)
            {
                reconocimientos.Identificador = Identificador;
                reconocimientos.IdUsuario = IdUsuario;
                reconocimientos.Folio = Folio;
                reconocimientos.Foja = Foja;
                reconocimientos.Libro = Libro;
                reconocimientos.Nombre = Nombre;
                reconocimientos.IdTipoDocumento = IdTipoDocumento;
                reconocimientos.Motivo = Motivo;
                reconocimientos.FechaEvento = FechaEvento;
                reconocimientos.FechaRegistro = FechaRegistro;
                reconocimientos.Autorizado = Autorizado;
                reconocimientos.Comentario = Comentario;
                reconocimientos.Archivo = Archivo;
                reconocimientos.AcuseEntrega = AcuseEntrega;

                if (Autorizado == "Autorizado")
                {
                    var email = db.Usuario.Where(u => u.id == IdUsuario).Select(u => u.UserName).ToList();
                    var rm = new ResponseModel();
                    var mail = new MailMessage();
                    mail.From = new MailAddress("gabriela.figueroa@umb.mx");
                    mail.To.Add("desarrollo.ti@umb.mx");
                    mail.CC.Add("ti@umb.mx");
                    mail.Subject = "SISTEMA DE SOLICITUD DE RECONOCIMIENTOS";
                    mail.IsBodyHtml = true;
                    //usuario.Contraseña = PasswordCode.Generate(5, 5);C:\Users\Desarrollo TI\Desktop\Reconocimientos edit\SolicitudReconocimientos\SolicitudReconocimientos\Imagenes\umb.PNG
                    string Mensaje = "<html><body><b>Solicitud de Reconocimientos</b></body></html>";
                    string html3 = "<html><body><img src=< img src='~/Imagenes/umb.png'/></body></html>";

                    //string html1 = "<html><body><p> El usuario </body></html>";
                    //string html2 = "<html><body><p> CONTRASEÑA: </body></html>";
                    string html = "<html><body><p><a href=\"http://reconocimientos.umb.edu.mx:8085\" > INICIAR SESIÓN</a></body></html>";
                    int idUsuario = IdUsuario;
                    var usuario = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Nombre).ToList();
                    var unidad = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Unidad.Nombre).ToList();
                    var documento = db.UsuarioReconocimiento.Where(u => u.Usuario.id == idUsuario).Select(u => u.Reconocimientos.TipoDocumento.Nombre).ToList();


                    mail.Body = "" + "<img src='/Imagenes/umb.PNG' />" + " " + Mensaje + " " + "El reconocimiento a nombre de" + " " + "<strong>" + Nombre + " </strong>" + " " + "solicitado por" + " " + usuario[0] + " " + "de" + " " + unidad[0] + "esta listo para su impresión" + "<hr style='border: 0px' />" + "Inresa a " + " " + html + " " + " para revisar";//model.Mensaje;

                    var SmtpServer = new SmtpClient("smtp.gmail.com"); // or "smtp.gmail.com"
                    SmtpServer.Port = 587;//587
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.UseDefaultCredentials = false;

                    // Agrega tu correo y tu contraseña, hemos usado el servidor de Outlook.
                    SmtpServer.Credentials = new NetworkCredential("reconocimientosnotreply@umb.mx", "reconocimientos.");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    rm.function = "CerrarContacto();";
                    rm.function = "Mensaje();";
                }



                db.Entry(reconocimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }
        // GET: Reconocimientos/Edit/5
        public ActionResult EditUser(long? id)
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
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }

        // POST: Reconocimientos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Permiso(Permiso = RolesPermisos.vista_usuario)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(int Identificador, string Folio, string Libro, string Foja, int IdUsuario, string Nombre, int IdTipoDocumento, string Motivo,
            string FechaEvento, string FechaRegistro, string Autorizado, string Comentario, string Archivo, string AcuseEntrega,int IdQuien, int IdFirma)
        {
            Reconocimientos reconocimientos = new Reconocimientos();
            if (ModelState.IsValid)
            {
                reconocimientos.Identificador = Identificador;
                reconocimientos.IdUsuario = IdUsuario;
                reconocimientos.Folio = Folio;
                reconocimientos.Foja = Foja;
                reconocimientos.IdQuien = IdQuien;
                reconocimientos.IdFirma = IdFirma;
                reconocimientos.Libro = Libro;
                reconocimientos.Nombre = Nombre;
                reconocimientos.IdTipoDocumento = IdTipoDocumento;
                reconocimientos.Motivo = Motivo;
                reconocimientos.FechaEvento = FechaEvento;
                reconocimientos.FechaRegistro = FechaRegistro;
                reconocimientos.Autorizado = Autorizado;
                reconocimientos.Comentario = Comentario;
                reconocimientos.Archivo = Archivo;
                reconocimientos.AcuseEntrega = AcuseEntrega;

                var rm = new ResponseModel();
                var mail = new MailMessage();
                mail.From = new MailAddress("abogado.general@umb.mx ");
                mail.To.Add("abogado.general@umb.mx");
                mail.CC.Add("desarrollo.ti@umb.mx");
                mail.Subject = "SISTEMA DE SOLICITUD DE RECONOCIMIENTOS";
                mail.IsBodyHtml = true;
                //usuario.Contraseña = PasswordCode.Generate(5, 5);C:\Users\Desarrollo TI\Desktop\Reconocimientos edit\SolicitudReconocimientos\SolicitudReconocimientos\Imagenes\umb.PNG
                string Mensaje = "<html><body><b>Solicitud de Reconocimientos</b></body></html>";
                string html3 = "<html><body><img src=\"~/Imagenes/umb.png\"></body></html>";
                string html1 = "<html><body><p> El usuario </body></html>";
                //string html2 = "<html><body><p> CONTRASEÑA: </body></html>";
                string html = "<html><body><p>CLICK PARA COMENZAR: <a href=\"http://reconocimientos.umb.edu.mx:8085\" > INICIAR SESIÓN</a></body></html>";
                int idUsuario = IdUsuario;
                var usuario = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Nombre).ToList();
                var unidad = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Unidad.Nombre).ToList();
                var documento = db.UsuarioReconocimiento.Where(u => u.Usuario.id == idUsuario).Select(u => u.Reconocimientos.TipoDocumento.Nombre).ToList();


                mail.Body = "" + "<img src='/Imagenes/umb.PNG' />" + " " + Mensaje + " " + "Los cambios solicitados en el reconocimiento a nombre de" + " " + "<strong>" + Nombre + " </strong>" + " " + "solicitado por" + " " + usuario[0] + " " + "de" + " " + unidad[0] + " ya se han realizado" + "<hr style='border: 0px' />" + "Inresa a " + " " + html + " " + " Para verificarlos";//model.Mensaje;


                var SmtpServer = new SmtpClient("smtp.gmail.com"); // or "smtp.gmail.com"
                SmtpServer.Port = 587;//587
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;

                // Agrega tu correo y tu contraseña, hemos usado el servidor de Outlook.
                SmtpServer.Credentials = new NetworkCredential("reconocimientosnotreply@umb.mx", "reconocimientos.");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                rm.function = "CerrarContacto();";
                rm.function = "Mensaje();";

                db.Entry(reconocimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }


       
        [Permiso(Permiso = RolesPermisos.vista_administrador)] 
      
        // GET: Reconocimientos/Edit/5
        public ActionResult Edit(long? id)
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
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }

        // POST: Reconocimientos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Permiso(Permiso = RolesPermisos.vista_administrador)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Identificador,string Folio, string Libro, string Foja ,int IdUsuario, string Nombre, int IdTipoDocumento, string Motivo,
            string FechaEvento, string FechaRegistro, string Autorizado, string Comentario, string Archivo, string AcuseEntrega, int IdQuien, int IdFirma)
        {
            Reconocimientos reconocimientos = new Reconocimientos();
            if (ModelState.IsValid)
            {
                reconocimientos.Identificador = Identificador;
                reconocimientos.IdUsuario = IdUsuario;
                reconocimientos.Folio = Folio;
                reconocimientos.Foja = Foja;
                reconocimientos.Libro = Libro;
                reconocimientos.IdQuien = IdQuien;
                reconocimientos.IdFirma = IdFirma;
                reconocimientos.Nombre = Nombre;
                reconocimientos.IdTipoDocumento = IdTipoDocumento;
                reconocimientos.Motivo = Motivo;
                reconocimientos.FechaEvento = FechaEvento;
                reconocimientos.FechaRegistro = FechaRegistro;
                reconocimientos.Autorizado = Autorizado;
                reconocimientos.Comentario = Comentario;
                reconocimientos.Archivo = Archivo;
                reconocimientos.AcuseEntrega = AcuseEntrega;
              
                var nomb = db.Usuario.Where(u => u.id == IdUsuario).Select(u => u.Nombre).ToList();
                if (Comentario != "" && Autorizado=="Pendiente" )
                {
                    var email = db.Usuario.Where(u => u.id == IdUsuario).Select(u => u.UserName).ToList();
                    var rm = new ResponseModel();
                    var mail = new MailMessage();
                    mail.From = new MailAddress(email[0]);
                    mail.To.Add(email[0]);
                    mail.Subject = "SISTEMA DE SOLICITUD DE RECONOCIMIENTOS";
                    mail.IsBodyHtml = true;
                    //usuario.Contraseña = PasswordCode.Generate(5, 5);C:\Users\Desarrollo TI\Desktop\Reconocimientos edit\SolicitudReconocimientos\SolicitudReconocimientos\Imagenes\umb.PNG
                    string Mensaje = "<html><body><b>Solicitud de Reconocimientos</b></body></html>";
                    string html3 = "<html><body><img src=\"~/Imagenes/umb.png\"></body></html>";
                    string html1 = "<html><body><p> El usuario </body></html>";
                    //string html2 = "<html><body><p> CONTRASEÑA: </body></html>";
                    string html = "<html><body><p>CLICK PARA COMENZAR: <a href=\"http://reconocimientos.umb.edu.mx:8085\" > INICIAR SESIÓN</a></body></html>";
                    int idUsuario = SessionHelper.GetUser();
                    var usuario = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Nombre).ToList();

                    var documento = db.UsuarioReconocimiento.Where(u => u.Usuario.id == idUsuario).Select(u => u.Reconocimientos.TipoDocumento.Nombre).ToList();


                    mail.Body = "" + html3 + " " + Mensaje + " " + html1 + usuario[0] + " " + "Solicito realizar los siguientes cambios:" + "<hr/>" + Comentario + "<hr />" + "En el reconocimiento solicitado a nombre de" +
                    " " + "<strong>" + Nombre + "</strong>" + "Inresa a " + " " + html + " " + " Para gestionarlos";//model.Mensaje;

                    var SmtpServer = new SmtpClient("smtp.gmail.com"); // or "smtp.gmail.com"
                    SmtpServer.Port = 587;//587
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.UseDefaultCredentials = false;

                    // Agrega tu correo y tu contraseña, hemos usado el servidor de Outlook.
                    SmtpServer.Credentials = new NetworkCredential("reconocimientosnotreply@umb.mx", "reconocimientos.");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    rm.function = "CerrarContacto();";
                    rm.function = "Mensaje();";


                }
                else {
                    if (Autorizado == "Autorizado") {
                        var email = db.Usuario.Where(u => u.id == IdUsuario).Select(u => u.UserName).ToList();
                        var rm = new ResponseModel();
                        var mail = new MailMessage();
                        mail.From = new MailAddress("gabriela.figueroa@umb.mx");
                        //mail.To.Add("gabriela.figueroa@umb.mx");
                        mail.CC.Add("desarrollo.ti@umb.mx");
                        mail.Subject = "SISTEMA DE SOLICITUD DE RECONOCIMIENTOS";
                        mail.IsBodyHtml = true;
                        //usuario.Contraseña = PasswordCode.Generate(5, 5);C:\Users\Desarrollo TI\Desktop\Reconocimientos edit\SolicitudReconocimientos\SolicitudReconocimientos\Imagenes\umb.PNG
                        string Mensaje = "<html><body><b>Solicitud de Reconocimientos</b></body></html>";
                        string html3 = "<html><body><img src=< img src='~/Imagenes/umb.png'/></body></html>";

                        //string html1 = "<html><body><p> El usuario </body></html>";
                        //string html2 = "<html><body><p> CONTRASEÑA: </body></html>";
                        string html = "<html><body><p><a href=\"http://reconocimientos.umb.edu.mx:8085\" > INICIAR SESIÓN</a></body></html>";
                        int idUsuario = IdUsuario;
                        var usuario = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Nombre).ToList();
                        var unidad = db.Usuario.Where(u => u.id == idUsuario).Select(u => u.Unidad.Nombre).ToList();
                        var documento = db.UsuarioReconocimiento.Where(u => u.Usuario.id == idUsuario).Select(u => u.Reconocimientos.TipoDocumento.Nombre).ToList();


                        mail.Body = "" + "<img src='~/Imagenes/umb.PNG' />" + " " + Mensaje + " " + "El reconocimiento a nombre de" + " " + "<strong>" + Nombre + " </strong>" + " " + "solicitado por" + " " + usuario[0] + " " + "de" + " " + unidad[0] + "Esta listo para su impresión" + "<hr style='border: 0px' />" + "Inresa a " + " " + html + " " + " Para Imprimir";//model.Mensaje;

                        var SmtpServer = new SmtpClient("smtp.gmail.com"); // or "smtp.gmail.com"
                        SmtpServer.Port = 587;//587
                        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpServer.UseDefaultCredentials = false;

                        // Agrega tu correo y tu contraseña, hemos usado el servidor de Outlook.
                        SmtpServer.Credentials = new NetworkCredential("reconocimientosnotreply@umb.mx", "reconocimientos.");
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                        rm.function = "CerrarContacto();";
                        rm.function = "Mensaje();";
                    } 
                }



                db.Entry(reconocimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }


        //subir archivos
        public ActionResult SubirArchivos(long? id)
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
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Solo se admiten achivos de excel");
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;
                    //var conf = new ExcelDataSetConfiguration
                    //{
                    //    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    //    {
                    //        UseHeaderRow = true,

                    //    }
                    //};



                    DataSet result = reader.AsDataSet();
                    reader.Close();
                  
                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor carga un archivo");
                }
            }
            return View();
        }



        //subir archivos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubirArchivos(int Identificador, string Folio, string Libro, string Foja, int IdUsuario, string Nombre, int IdTipoDocumento, string Motivo,
        string FechaEvento, string FechaRegistro, string Autorizado, string Comentario, HttpPostedFileBase Archivo, HttpPostedFileBase AcuseEntrega, int IdQuien, int IdFirma)
        {
            Reconocimientos reconocimientos = new Reconocimientos();
            if (ModelState.IsValid)
            {
                reconocimientos.Identificador = Identificador;
                reconocimientos.Folio = Folio;
                reconocimientos.Libro = Libro;
                reconocimientos.Foja = Foja;
                reconocimientos.Quien = reconocimientos.Quien;
                reconocimientos.IdUsuario = IdUsuario;
                reconocimientos.IdQuien = IdQuien;
                reconocimientos.IdFirma = IdFirma;
                reconocimientos.Nombre = Nombre;
                reconocimientos.IdTipoDocumento = IdTipoDocumento;
                reconocimientos.Motivo = Motivo;
                reconocimientos.FechaEvento = FechaEvento;
                reconocimientos.FechaRegistro = FechaRegistro;
                reconocimientos.Autorizado = Autorizado;
                reconocimientos.Firma = reconocimientos.Firma;
                reconocimientos.Comentario = Comentario;
                if (Archivo != null)
                {
                    // Nombre del archivo, es decir, lo renombramos para que no se repita nunca
                    string archivo = DateTime.Now.ToString("yyyyMMddHHmmss") + Archivo.FileName + Path.GetExtension(Archivo.FileName);

                    // La ruta donde lo vamos guardar
                    Archivo.SaveAs(HttpContext.Server.MapPath("~/Archivos/" + archivo));

                    // Establecemos en nuestro modelo el nombre del archivo
                    reconocimientos.Archivo = archivo;
                }

                if (AcuseEntrega != null)
                {
                    // Nombre del archivo, es decir, lo renombramos para que no se repita nunca
                    string acuse = DateTime.Now.ToString("yyyyMMddHHmmss") + AcuseEntrega.FileName + Path.GetExtension(AcuseEntrega.FileName);

                    // La ruta donde lo vamos guardar
                    AcuseEntrega.SaveAs(HttpContext.Server.MapPath("~/Acuses/" + acuse));

                    // Establecemos en nuestro modelo el nombre del archivo
                    reconocimientos.AcuseEntrega = acuse;
                }
               
                db.Entry(reconocimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoDocumento = new SelectList(db.TipoDocumento, "Identificador", "Nombre", reconocimientos.IdTipoDocumento);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "id", "NombreTipo", reconocimientos.IdUsuario);
            return View(reconocimientos);
        }



        // POST: Reconocimientos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // GET: Reconocimientos/Delete/5
        public ActionResult Delete(long? id)
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

        // POST: Reconocimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Reconocimientos reconocimientos = db.Reconocimientos.Find(id);
            db.Reconocimientos.Remove(reconocimientos);
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
