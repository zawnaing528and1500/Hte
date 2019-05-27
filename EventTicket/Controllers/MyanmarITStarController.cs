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

        #region For Contacting Shop (Used By Internship Student)
        public ActionResult AddContactShop()
        {
            return View();
        }
        public ActionResult ProcessAddContactShop()
        {
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];

            if(!(db.CheckByQuery("select * from ContactShop where Phone='" + Phone + "'")))
            {
                int InternStaffID = Convert.ToInt32(Request.Form["InternStaffID"]);
                db.ChangeByQuery("insert into ContactShop values(N'" + Name + "',N'" + Phone + "',null,'new',null,'" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + InternStaffID + ")");

            }
            return RedirectToAction("AddContactShop", "MyanmarITStar");
        }
        #endregion

        #region Myanmar IT Page
        public ActionResult AllShop()
        {
            return View();
        }
        public ActionResult DeleteShop()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from Customer where ShopID=" + ID);
            db.ChangeByQuery("delete from HteNumber where ShopID=" + ID);
            db.ChangeByQuery("delete from HteAttribute where ShopID=" + ID);

            db.ChangeByQuery("delete from Login where AllID=" + ID);
            db.ChangeByQuery("delete from StaffShop where ShopID=" + ID);
            db.ChangeByQuery("delete from Shop where ID=" + ID);
            return RedirectToAction("AllShop", "MyanmarITStar");
        }
        #endregion
    }
}