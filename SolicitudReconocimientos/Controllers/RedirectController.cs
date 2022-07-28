using ConcursoCatrinas.Tags;
using Helper;
using Model.Commons;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SolicitudReconocimientos.Controllers
{
    [Autenticado]
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Index()
        {
            if (FrontUser.TienePermiso(RolesPermisos.vista_usuario))
            {
               
                return Redirect("~/Home/Index/");
            }
            if (FrontUser.TienePermiso(RolesPermisos.vista_administrador))
            {
                return Redirect("~/Home/Index");
            }
            if (FrontUser.TienePermiso(RolesPermisos.vista_supeadministrador))
            {
                return Redirect("~/Home/Index");
            }


            return Redirect("~/Home");
        }
        public ActionResult Salir()
        {
            SessionHelper.DestroyUserSession();
            return Redirect("~/Login");
        }

    }
}