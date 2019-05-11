using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;

namespace EventTicket.Controllers
{
    public class ShopController : Controller
    {
        DBBase db = new DBBase();
        // GET: Shop
        public ActionResult Index()
        {
            return View();
        }

        #region Customer Form
        public ActionResult CustomerForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessCustomerForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            db.ChangeByQuery("delete from Customer where ID not in (select CustomerID from HteNumber)");
            db.ChangeByQuery("insert into Customer values(" + ShopID + ",N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "',2)");
            int CustomerID = db.getIntByQuery("select * from Customer where ShopID=" + ShopID + " and Name=N'" + Name + "' and Phone=N'" + Phone + "'", "ID");
            Session["CustomerID"] = CustomerID;
            return RedirectToAction("HteNumberForm", "Shop");
        }
        #endregion

        #region Hte Number Form
        public ActionResult HteNumberForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessHteNumberForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int CustomerID = Convert.ToInt32(Session["CustomerID"]);
            int HteTypeID = 2;
            string No = Request.Form["No"];

            No = No.Replace(" ", string.Empty);
            No = No.Replace("-", string.Empty);
            No = No.Replace("_", string.Empty);
            No = No.Replace("\'", string.Empty);
            No = No.Replace("-", string.Empty);

            int CostPrice = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID, "CostPrice");
            int Profit = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID,"SellingPrice")- CostPrice;
            if (!(db.CheckByQuery("select * from HteNumber where CustomerID= "+CustomerID+" and No LIKE N'" + No + "%'")))
            {
                db.ChangeByQuery("insert into HteNumber values(" + ShopID + "," + CustomerID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + ","+Profit+","+ CostPrice + ")");
                db.ChangeByQuery("update HteAttribute set Quantity = Quantity-1 where ShopID=" + ShopID + " and HteTypeID=2");
            }
            return RedirectToAction("HteNumberForm", "Shop");
        }
        #endregion

        //Still Unused
        #region Download Customer Monthly Hte Number
        public ActionResult ProcessCustomerHteNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            CustomerHteNumber();
            return View();
        }
        private void CustomerHteNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            string DownloadedDate = "";
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            DataTable dt = db.getAllByQuery("select No,Date from HteNumber where ShopID=" + ShopID + " and MONTH(Date) = MONTH(GetDate())AND YEAR(Date) = YEAR(GetDate())");//your datatable
            string attachment = "attachment; filename=လကုန္​ထိုးထား​ေသာစာရင္​း.txt";

            Response.Buffer = true;
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/plain";

            string tab = "\t"; string newline = "\r\n";
            Response.Write("စဥ္​" + tab);
            Response.Write("ထီနံပါတ္" + newline);

            int i = 1;
            foreach (DataRow row in dt.Rows)
            {
                tab = "\t";
                Response.Write(i);
                Response.Write(tab);
                Response.Write(row["No"].ToString());
                DownloadedDate = row["Date"].ToString();
                Response.Write(newline);
                i++;
            }
            Response.Write(DownloadedDate);
            Response.End();
        }
        #endregion

        #region Hte Attribute
        public ActionResult HteAttribute()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ChangeHteAttribute()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int HteAttributeID = Convert.ToInt32(Request.QueryString["HteAttributeID"]);
            ViewBag.HteAttributeID = HteAttributeID;
            return View();
        }
        public ActionResult ProcessChangeHteAttribute()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int HteAttributeID = Convert.ToInt32(Request.Form["HteAttributeID"]);
            int CostPrice = Convert.ToInt32(Request.Form["CostPrice"]);
            int SellingPrice = Convert.ToInt32(Request.Form["SellingPrice"]);
            int Quantity = Convert.ToInt32(Request.Form["Quantity"]);
            //Check Date with Date in Customer. If It is, do not allowed...Left it

            db.ChangeByQuery("update HteAttribute set CostPrice= " + CostPrice + ", SellingPrice= " + SellingPrice + ", Quantity= " + Quantity + ", Date='"+ DateTime.Now.ToString("MM.dd.yyyy") + "' where ID= " + HteAttributeID + "and ShopID=" + ShopID);
            return RedirectToAction("HteAttribute", "Shop");
        }
        #endregion

        #region Sale Force
        public ActionResult SaleForce()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }

            if (TempData["FromDate"] != null)
            {
                ViewBag.FromDate = TempData["FromDate"].ToString();
                ViewBag.ToDate = TempData["ToDate"].ToString();
            }
            else
            {
                ViewBag.FromDate = null;
                ViewBag.ToDate = null;
            }
            return View();
        }
        public ActionResult ProcessSaleForce()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }

            string FromDate = Request.Form["FromDate"];
            string ToDate = Request.Form["ToDate"];

            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;

            return RedirectToAction("SaleForce", "Shop");
        }
        #endregion

        public ActionResult DeleteHteNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            //Add to HteAttribute First, then delte
            int HteTypeID = db.getIntByQuery("select * from HteNumber where ID="+ID, "HteTypeID");
            db.ChangeByQuery("update HteAttribute set Quantity = Quantity+1 where ShopID=" + ShopID + " and HteTypeID="+HteTypeID);
            db.ChangeByQuery("delete from HteNumber where ID="+ID+" and ShopID="+ShopID);
            return RedirectToAction("HteNumberForm", "Shop");
        }
        public ActionResult ViewTodayCustomer()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ViewPreviousCustomerHteNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult CustomerSuccessNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ViewMonthlyCustomerHteNumber()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ShowAggreement()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ShowSuccessNumber()
        {
            return View();
        }
    }
}