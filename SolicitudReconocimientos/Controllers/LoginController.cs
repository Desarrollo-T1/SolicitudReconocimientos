using ConcursoCatrinas.Tags;
using Helper;
using Model;
using SeguimientoPredios.ViewModel;
using SolicitudReconocimientos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SolicitudReconocimientos.Controllers
{
   
    public class LoginController : Controller
    {
        private Usuario um = new Usuario();

       
        public ActionResult Salir()
        {
            SessionHelper.DestroyUserSession();
            return Redirect("~/Home/");
        }
        public ActionResult Index()
        {
            return View();
        }

        [NoLogin]
        public JsonResult Autenticar(LoginViewModel model)
        {
            var rm = new ResponseModel();

            if (ModelState.IsValid)
            {
                this.um.UserName = model.UserName;
                this.um.Contraseña = model.Contraseña;

                rm = um.Autenticar();




                if (rm.response)
                {

                    //if (!FrontUser.TienePermiso(RolesPermisos.Vista_administrador))
                    //{
                    //    rm.href = Url.Content("~/Usuarios/Index");
                    //}



                    rm.href = Url.Content("~/Reconocimientos/index");
                }
            }
            else
            {
                rm.SetResponse(false, "Debe llenar los campos para poder autenticarse.");
            }

            return Json(rm);


        }
        // GET: Login
    }
}