using ConcursoCatrinas.Tags;
using Model.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SolicitudReconocimientos.Controllers
{
    [NoLogin]
    public class HomeController : Controller
    {
        // GET: Home
       

        public ActionResult Index()
        {
            return View();
        }
    }
}