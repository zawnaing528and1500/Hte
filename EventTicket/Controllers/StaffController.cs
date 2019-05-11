using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;

namespace EventTicket.Controllers
{
    public class StaffController : Controller
    {
        DBBase stock = new DBBase();
        // GET: Staff
        public ActionResult Index()
        {
            return View();
        }
        #region add customer
        public ActionResult AddNewCustomerForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessNewCustomerForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            string Name = Request.Form["Name"];
            string Address = Request.Form["Address"];
            string Phone = Request.Form["Phone"];
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            string ExpiryDate = Request.Form["ExpiryDate"];
            int StaffID = Convert.ToInt32(Session["CurrentUserID"]);

            if (stock.CheckByQuery("select * from Shop where Name=N'" + Name + "' and Phone=N'" + Phone + "'") == false)
            {
                if (stock.CheckByQuery("select * from Login where Username=N'" + Username + "'") == false)
                {
                    stock.ChangeByQuery("insert into Shop values(N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "',null)");
                    int AllID = stock.getIntByQuery("select * from Shop where Name=N'" + Name + "' and Phone=N'" + Phone + "'", "ID");
                    stock.ChangeByQuery("insert into Login values(" + AllID + ",N'" + Username + "',N'" + Password + "',2,'True','" + ExpiryDate + "')");
                    stock.ChangeByQuery("insert into StaffShop values(" + StaffID + "," + AllID + ",'" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
                    stock.ChangeByQuery("insert into HteAttribute values("+AllID+",2,950,1000,500,'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");

                }
            }
            return RedirectToAction("AddNewCustomerForm", "Staff");
        }
        public ActionResult DeleteCustomer()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Customer where ShopID=" + ID);
            stock.ChangeByQuery("delete from HteNumber where ShopID=" + ID);
            stock.ChangeByQuery("delete from HteAttribute where ShopID=" + ID);


            stock.ChangeByQuery("delete from Login where AllID=" + ID);
            stock.ChangeByQuery("delete from StaffShop where ShopID=" + ID);
            stock.ChangeByQuery("delete from Shop where ID=" + ID);
            return RedirectToAction("AddNewCustomerForm", "Staff");
        }
        #endregion

        #region For Contact Shop
        public ActionResult SeeContactShop()
        {
            return View();
        }
        public ActionResult ProcessSeeContactShop()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            string Address = Request.Form["Address"];
            string Description = Request.Form["Description"];
            string Status = Request.Form["Status"];
            stock.ChangeByQuery("update ContactShop set Address=N'"+Address+"' , Description =N'"+Description+"' , Status =N'"+Status+"'  where ID="+ID);
            return RedirectToAction("SeeContactShop", "Staff");
        }
        #endregion
        
        public ActionResult ShowAggreement()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
    }
}