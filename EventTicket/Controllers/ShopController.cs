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
            No = No.Replace("\\", string.Empty);
            No = No.Replace("\\/", string.Empty);
            No = No.Replace("။", string.Empty);
            No = No.Replace("၊", string.Empty);
            No = No.Replace(":", string.Empty);
            No = No.Replace(";", string.Empty);

            No = No.Replace("0","၀");
            No = No.Replace("1", "၁");
            No = No.Replace("2", "၂");
            No = No.Replace("3", "၃");
            No = No.Replace("4", "၄");
            No = No.Replace("5", "၅");
            No = No.Replace("6", "၆");
            No = No.Replace("7", "၇");
            No = No.Replace("8", "၈");
            No = No.Replace("9", "၉");

            int CostPrice = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID+" and ShopID="+ShopID, "CostPrice");
            int Profit = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID+" and ShopID="+ShopID,"SellingPrice")- CostPrice;
            if (!(db.CheckByQuery("select * from HteNumber where CustomerID= "+CustomerID+" and No LIKE N'" + No + "%'")))
            {
                db.ChangeByQuery("insert into HteNumber values(" + ShopID + "," + CustomerID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + ","+Profit+","+ CostPrice + ")");
                db.ChangeByQuery("update HteAttribute set Quantity = Quantity-1 where ShopID=" + ShopID + " and HteTypeID=2");
            }
            return RedirectToAction("HteNumberForm", "Shop");
        }
        #endregion

        #region 5-9-10 Hte Pre Record
        public ActionResult DifferentHteRecordForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            if (Request.QueryString["Type"] != null)
            {
                int Type = Convert.ToInt32(Request.QueryString["Type"]);
                Session["Type"] = Type;
            }
            return View();
        }
        public ActionResult ProcessDifferentHteRecordForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            String selectedHteNumber = Request.Form["SelectedHteNumber"]; int Type = Convert.ToInt32(Request.Form["Type"]);
            string[] separater = { "," };
            string[] SelectedHteNumber = selectedHteNumber.Split(separater, StringSplitOptions.RemoveEmptyEntries);

            if (SelectedHteNumber.Length > 0)
            {
                int HteTypeID = Convert.ToInt32(Session["HteTypeID"]);
                int ShopID = Convert.ToInt32(Session["CurrentUserID"]);

                //Outer Looping is for 5-9-10 Hte
                for(int k = 0; k < Type; k++)
                {
                    for(int j=0;j< SelectedHteNumber.Length; j++)
                    {
                        String HteNo = SelectedHteNumber[j];
                        HteNo = HteNo.Remove(HteNo.Length - 1, 1) + k;
                        SelectedHteNumber[j] = HteNo;
                    }
                    //Insert First Hte and last Hte into  DiiferentHte Table
                    string FirstNo = SelectedHteNumber[0];
                    string LastNo = SelectedHteNumber.Last();
                    db.ChangeByQuery("insert into DifferentHte values(" + ShopID + ",N'" + FirstNo + "',N'" + LastNo + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
                    int DifferentHteID = db.getIntByQuery("SELECT * FROM DifferentHte where ShopID=" + ShopID + " ORDER BY ID ASC", "ID");

                    for (int i = 0; i < SelectedHteNumber.Length; i++)
                    {
                        string No = SelectedHteNumber[i];

                        No = No.Replace("0", "၀");
                        No = No.Replace("1", "၁");
                        No = No.Replace("2", "၂");
                        No = No.Replace("3", "၃");
                        No = No.Replace("4", "၄");
                        No = No.Replace("5", "၅");
                        No = No.Replace("6", "၆");
                        No = No.Replace("7", "၇");
                        No = No.Replace("8", "၈");
                        No = No.Replace("9", "၉");

                        db.ChangeByQuery("insert into DifferentHteNo Values(" + DifferentHteID + "," + ShopID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
                    }
                }

            }

            return RedirectToAction("DifferentHteRecordForm", "Shop");
        }
        #endregion

        #region Mix Hte Form

        #region checking Is Hole Sale
        public ActionResult CheckHoleSaleForMix()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            if (db.CheckByQuery("select * from Shop where isHoleSale='False' and ID=" + ShopID))
            {
                Session["HteTypeID"] = 2;
                return RedirectToAction("MixHteNumberForm", "Shop");
            }
            else
            {
                return RedirectToAction("ChooseSaleTypeForMix", "Shop");
            }
        }
        public ActionResult ChooseSaleTypeForMix()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessChooseSaleTypeForMix()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int HteTypeID = Convert.ToInt32(Request.QueryString["HteTypeID"]);
            Session["HteTypeID"] = HteTypeID;
            return RedirectToAction("MixHteNumberForm", "Shop");
        }
        #endregion

        #region Mix Form
        public ActionResult MixHteNumberForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        #endregion

        #endregion


        #region Different Hte Record

        #region for checking Hole Sale or not
        public ActionResult CheckHoleSaleForDifferentHte()
        {

            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }

            int Type = Convert.ToInt32(Request.QueryString["Type"]);
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);

            if (Type == 5)
            {
                TempData["Type"] = 5;
            }
            else if (Type == 9)
            {
                TempData["Type"] = 9;
            }
            else if (Type == 10)
            {
                TempData["Type"] = 10;
            }

            if (db.CheckByQuery("select * from Shop where isHoleSale='False' and ID=" + ShopID))
            {
                Session["HteTypeID"] = 2;
                return RedirectToAction("DifferentHteRecordSellForm", "Shop");
            }
            else
            {
                //Left
                return RedirectToAction("ChooseSaleTypeForDifferentHte", "Shop");
            }
        }
        #endregion

        //For Not Hole Sale Shop
        #region Form and Backend
        public ActionResult DifferentHteRecordSellForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            ViewBag.Type = TempData["Type"].ToString();

            return View();
        }
        public ActionResult ProcessDifferentHteRecordSellForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int DifferentHteID = Convert.ToInt32(Request.Form["DifferentHteID"]);
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];

            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int HteTypeID = Convert.ToInt32(Session["HteTypeID"]);

            Phone = Phone.Replace("၀", "0");
            Phone = Phone.Replace("၁", "1"); Phone = Phone.Replace("၂", "2"); Phone = Phone.Replace("၃", "3"); Phone = Phone.Replace("၄", "4");
            Phone = Phone.Replace("၅", "5"); Phone = Phone.Replace("၆", "6"); Phone = Phone.Replace("၇", "7"); Phone = Phone.Replace("၈", "8");
            Phone = Phone.Replace("၉", "9");

            //Insert into Customer
            db.ChangeByQuery("insert into Customer values(" + ShopID + ",N'" + Name + "','" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + ")");
            int CustomerID = db.getIntByQuery("SELECT * FROM Customer where ShopID=" + ShopID + " ORDER BY ID ASC", "ID");

            int CostPrice = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID + " and ShopID=" + ShopID, "CostPrice");
            int Profit = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID + " and ShopID=" + ShopID, "SellingPrice") - CostPrice;

            DataTable dbDifferntHteNo = db.getAllByQuery("select * from DifferentHteNo where DifferentHteID=" + DifferentHteID + " and ShopID=" + ShopID);
            foreach (DataRow rows in dbDifferntHteNo.Rows)
            {
                string No = rows["No"].ToString();
                db.ChangeByQuery("insert into HteNumber values(" + ShopID + "," + CustomerID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + "," + Profit + "," + CostPrice + ")");
                db.ChangeByQuery("update HteAttribute set Quantity = Quantity-1 where ShopID=" + ShopID + " and HteTypeID=" + HteTypeID);

                db.ChangeByQuery("delete from DifferentHteNo where ID=" + Convert.ToInt32(rows["ID"]));
            }
            db.ChangeByQuery("delete from DifferentHte where ID=" + DifferentHteID);

            return RedirectToAction("ViewTodayCustomer", "Shop");
        }

        #endregion

        //For Hole Sale Shop
        #region Choose Form and Backend
        public ActionResult ChooseSaleTypeForDifferentHte()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            Session["Type"] = TempData["Type"];
            
            return View();
        }
        public ActionResult ProcessChooseSaleTypeForDifferentHte()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int HteTypeID = Convert.ToInt32(Request.QueryString["HteTypeID"]);
            Session["HteTypeID"] = HteTypeID;
            TempData["Type"] = Session["Type"];
            return RedirectToAction("DifferentHteRecordSellForm", "Shop");
        }
        #endregion

        #endregion

        #region Pair, Special, Couple

        #region for checking Is Hole Sale

        public ActionResult CheckHoleSaleForDifferent()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            if (db.CheckByQuery("select * from Shop where isHoleSale='False' and ID=" + ShopID))
            {
                Session["HteTypeID"] = 2;
                return RedirectToAction("DifferentHteNumberForm", "Shop");
            }
            else
            {
                return RedirectToAction("ChooseSaleType", "Shop");
            }
        }
        public ActionResult ChooseSaleType()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessChooseSaleType()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            int HteTypeID = Convert.ToInt32(Request.QueryString["HteTypeID"]);
            Session["HteTypeID"] = HteTypeID;
            return RedirectToAction("DifferentHteNumberForm", "Shop");
        }

        #endregion

        #region Different Hte Number Form
        public ActionResult DifferentHteNumberForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessDifferentHteNumberForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            String selectedHteNumber = Request.Form["SelectedHteNumber"];
            string[] separater = { "," };
            string[] SelectedHteNumber = selectedHteNumber.Split(separater, StringSplitOptions.RemoveEmptyEntries);

            if (SelectedHteNumber.Length > 0)
            {
                int HteTypeID = Convert.ToInt32(Session["HteTypeID"]);
                int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
                string Name = Request.Form["Name"];
                string Phone = Request.Form["Phone"];
                string Address = Request.Form["Address"];

                //၁,၂,၃,၄,၅,၆,၇,၈,၉,၀ - Zaw Gyi Font

                Phone = Phone.Replace("၀", "0");
                Phone = Phone.Replace("၁", "1"); Phone = Phone.Replace("၂", "2"); Phone = Phone.Replace("၃", "3"); Phone = Phone.Replace("၄", "4");
                Phone = Phone.Replace("၅", "5"); Phone = Phone.Replace("၆", "6"); Phone = Phone.Replace("၇", "7"); Phone = Phone.Replace("၈", "8");
                Phone = Phone.Replace("၉", "9");

                db.ChangeByQuery("insert into Customer values(" + ShopID + ",N'" + Name + "','" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + ")");
                int CustomerID = db.getIntByQuery("SELECT * FROM Customer where ShopID=" + ShopID + " ORDER BY ID ASC", "ID");

                int CostPrice = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID + " and ShopID=" + ShopID, "CostPrice");
                int Profit = db.getIntByQuery("select * from HteAttribute where HteTypeID=" + HteTypeID + " and ShopID=" + ShopID, "SellingPrice") - CostPrice;

                for (int i = 0; i < SelectedHteNumber.Length; i++)
                {
                    string No = SelectedHteNumber[i];

                    No = No.Replace("0", "၀");
                    No = No.Replace("1", "၁");
                    No = No.Replace("2", "၂");
                    No = No.Replace("3", "၃");
                    No = No.Replace("4", "၄");
                    No = No.Replace("5", "၅");
                    No = No.Replace("6", "၆");
                    No = No.Replace("7", "၇");
                    No = No.Replace("8", "၈");
                    No = No.Replace("9", "၉");

                    db.ChangeByQuery("insert into HteNumber values(" + ShopID + "," + CustomerID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "'," + HteTypeID + "," + Profit + "," + CostPrice + ")");
                    db.ChangeByQuery("update HteAttribute set Quantity = Quantity-1 where ShopID=" + ShopID + " and HteTypeID=" + HteTypeID);
                }
            }

            return RedirectToAction("CheckHoleSaleForDifferent", "Shop");
        }
        #endregion

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

        #region for entering 5-9-10 Hte

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