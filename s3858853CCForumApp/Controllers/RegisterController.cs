using Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace s3858853CCForumApp.Controllers
{
    //Mask URL
    [Route("/Register")]
    public class RegisterController : Controller
    {

       // public RegisterController(MCAWebAppContext context)
        {
            _context = context;
        }


    public IActionResult Register() => View();

    //attempt user registration
    [HttpPost]
    public async Task<IActionResult> Register(string loginID, string username, string password)
    {
        //find login id
        var login = await _context.user.FindAsync(loginID);


        if (login != null)
        {
            ModelState.AddModelError("RegistrationFailure", "The ID already exists");
            return View(new Register { LoginID = loginID });
        }

        //find username
        var userName = await _context.user.FindAsync(username); // This may not work

        if (userName != null)
        {
            ModelState.AddModelError("RegistrationFailure", "The username already exists");
            return View(new Register { Username = username });
        }

        //createUser(string loginID, string username, string password); // This may be a better approach.
        Entity user = new Entity()
        {
            KeyNotFoundException = _context.CreateKeyFactory("User").CreateKey("userKey"),
            ["id"] = loginID,
            ["user_name"] = username,
            ["password"] = password
        };

        await _context.InsertAsync(user);

        // File uploaded to Cloud Storage

        return RedirectToAction("Login", "User");

    }

    //public async Task<IActionResult> createUser(string loginID, string username, string password)
    //{

    //    _context.User.Add(
    //        new User
    //        {
    //            id = loginID,
    //            user_name = username,
    //            password = password
    //        });

    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("Index", "User");
    //}

}
}
