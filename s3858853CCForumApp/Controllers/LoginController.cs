using s3858853CCForumApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace s3858853CCForumApp.Controllers
{
    //Mask URL
    [Route("/Login")]
    public class LoginController : Controller
    {

       // public LoginController(MCAWebAppContext context)
        //{
        //    _context = context;
        //}


    public IActionResult Login() => View();

    //attempt logging in
    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        //find login id
        var login = await _context.user.FindAsync(loginID);

        //attempt password check
        //password would typically be hashed
        if (login == null || login.password != password)
        {
            ModelState.AddModelError("LoginFailure", "ID or password is invalid");
            return View(new Login { LoginID = loginID });
        }

        //Customer login
        HttpContext.Session.SetInt32(nameof(User.id), login.id);
        HttpContext.Session.SetString(nameof(User.user_name), login.User.user_name);

        return RedirectToAction("Index", "User");
    }

    [Route("LoggingOut")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }




}
}