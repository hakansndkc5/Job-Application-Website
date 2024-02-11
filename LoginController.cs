using stajdeneme.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace stajdeneme.Controllers
{
    public class LoginController : Controller
    {
        stajdenemeEntities3 db = new stajdenemeEntities3();
        // GET: Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
           
                FormsAuthentication.SignOut();
                return View();
           
            

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(tbl_admin model)
        {
            var datavalue = db.tbl_admin.FirstOrDefault(m=>m.user_name==model.user_name && m.user_pass==model.user_pass);
            if (datavalue!=null)
            {
                FormsAuthentication.SetAuthCookie(datavalue.user_name,false);
                return RedirectToAction("Index","Basvuru");
            }


            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }


    }
}
    
