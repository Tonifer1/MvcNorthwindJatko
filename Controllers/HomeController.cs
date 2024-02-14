using MvcNorthwindJatko.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcNorthwindJatko.Controllers
{
    public class HomeController : Controller
    {
        private NorthwindOriginalEntities db = new NorthwindOriginalEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //public ActionResult Products()
        //{
        //    if (Session["Username"] == null)
        //    {
        //        ViewBag.LoggedStatus = "Out";
        //    }
        //    else ViewBag.LoggedStatus = "In";

        //    return View();//Hiiri tähän päälle ja kakkosella luo uuden näkymän.
        //}


        

        public ActionResult Login()//Controllerin Metodi.Palauttaa Metodin nimisen näkymän oletuksena.
        {
            return View();
        }


        [HttpPost]
        public ActionResult Authorize(Logins LoginModel)
        {

            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == LoginModel.UserName && x.PassWord == LoginModel.PassWord);
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                ViewBag.LoginError = 0;
                Session["UserName"] = LoggedUser.UserName;
                return RedirectToAction("Index", "Home"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa --> Home/Index
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                ViewBag.LoginError = 1;
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Login", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
            return RedirectToAction("Index", "Home");//Uloskirjautumisen jälkeen Pääsivulle
        }
    }
}