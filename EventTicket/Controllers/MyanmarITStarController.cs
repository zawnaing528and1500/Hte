using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;

namespace EventTicket.Controllers
{
    public class MyanmarITStarController : Controller
    {
        DBBase db = new DBBase();
        // GET: MyanmarITStar
        public ActionResult Index()
        {
            return View();
        }
        #region For Contacting Shop
        public ActionResult AddContactShop()
        {
            return View();
        }
        public ActionResult ProcessAddContactShop()
        {
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            db.ChangeByQuery("insert into ContactShop values(N'"+Name+"',N'"+Phone+"',null,'new',null,'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");
            return RedirectToAction("AddContactShop", "MyanmarITStar");
        }
        #endregion

    }
}