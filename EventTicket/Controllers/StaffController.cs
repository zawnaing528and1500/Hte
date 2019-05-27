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
            Boolean IsHoleSale = false;Boolean IsTest = true;
            int HteTypeID = Convert.ToInt32(Request.Form["HteTypeID"]);
            int Test = Convert.ToInt32(Request.Form["isTest"]);

            if (HteTypeID == 1)
            {
                IsHoleSale = true;
            }
            if (Test == 1)
            {
                IsTest = false;
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
                    stock.ChangeByQuery("insert into Shop values(N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "',null,'"+ IsHoleSale + "','"+IsTest+"')");
                    int AllID = stock.getIntByQuery("select * from Shop where Name=N'" + Name + "' and Phone=N'" + Phone + "'", "ID");
                    stock.ChangeByQuery("insert into Login values(" + AllID + ",N'" + Username + "',N'" + Password + "',2,'True','" + ExpiryDate + "')");
                    stock.ChangeByQuery("insert into StaffShop values(" + StaffID + "," + AllID + ",'" + DateTime.Now.ToString("MM.dd.yyyy") + "')");             
                    stock.ChangeByQuery("insert into HteAttribute values("+AllID+",2,950,1000,500,'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");
                    if (IsHoleSale == true)
                    {
                        stock.ChangeByQuery("insert into HteAttribute values(" + AllID + ",1,900,940,500,'" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
                    }
                }
            }
            return RedirectToAction("AddNewCustomerForm", "Staff");
        }
        #endregion

        #region For Contact Shop
        public ActionResult SeeContactShop()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult DeleteContactShop()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from ContactShop where ID=" + ID);
            return RedirectToAction("SeeContactShop", "Staff");
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

        #region Delete Wrong Hte
        public ActionResult SeeHteNumberToDelete()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ShowHteNumberToDelete()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
            ViewBag.CustomerID = CustomerID;
            return View();
        }
        public ActionResult DeleteCustomer()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int CustomerID = Convert.ToInt32(Request.QueryString["CustomerID"]);
            stock.ChangeByQuery("delete from HteNumber where CustomerID="+CustomerID);
            stock.ChangeByQuery("delete from Customer where ID=" + CustomerID);
            return RedirectToAction("AddNewCustomerForm", "Staff");
        }
        #endregion

        #region Enter Success Number By Staff
        public ActionResult EnterSuccessNumberForm()
        {
            return View();
        }
        public ActionResult ProcessEnterSuccessNumberForm()
        {
            string No = Request.Form["No"];
            string Amount = Request.Form["Amount"];
            stock.ChangeByQuery("insert into Success values(N'"+No+"',N'"+Amount+"')");
            return RedirectToAction("EnterSuccessNumberForm", "Staff");
        }
        public ActionResult DeleteSuccessNumber()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Success where ID="+ID);
            return RedirectToAction("EnterSuccessNumberForm", "Staff");
        }
        #endregion

        #region Show Aggrement
        public ActionResult ShowAggreement()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        #endregion

    }
}