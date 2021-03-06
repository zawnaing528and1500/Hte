﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;


namespace EventTicket.Controllers
{
    public class LoginController : Controller
    {
        DBBase b = new DBBase();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoginForm()
        {
            return View();
        }
        public ActionResult CustomerRegistrationForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckUsernameAndPassword()
        {
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            Username = Username.Replace(" ", string.Empty);
            Username = Username.Replace("\'", string.Empty);
            Password = Password.Replace(" ", string.Empty);
            Password = Password.Replace("\'", string.Empty);

            System.Diagnostics.Debug.WriteLine(Password);

            LoginAuthorization l = new LoginAuthorization();
            if (l.checkUser(Username, Password))
            {
                
                int UserID = l.getUserID();
                int AccessLevel = l.getAccessLevel();
                //System.Diagnostics.Debug.WriteLine(UserID);
                //System.Diagnostics.Debug.WriteLine(AccessLevel);

                Session["CurrentUserID"] = UserID;
                Session["LoginSession"] = null;

                //Access Level 1 is CR Manager
                //Access Level 2 is Hte Shop
                //Access Level 3 is MyanmarItStar

                if (AccessLevel == 1)
                {
                    return RedirectToAction("AddNewCustomerForm", "Staff");//Changes_second parioty
                }
                else if (AccessLevel == 2)
                {
                    return RedirectToAction("CheckHoleSaleForDifferent", "Shop");//Most Important
                }
                else if (AccessLevel == 3)
                {
                    return RedirectToAction("AllShop", "MyanmarITStar");//Most Important
                }
                else
                {
                    return RedirectToAction("Index", "Customer");
                }
            }
            else
            {
                Session["LoginSession"] = "error";
                return RedirectToAction("LoginForm", "Login");
            }
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            Response.Redirect("/Login/LoginForm");
            return View();
        }
    }
}