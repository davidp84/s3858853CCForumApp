using s3858853CCForumApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Google.Api.Gax;
using Google.Cloud.Storage.V1;

namespace s3858853CCForumApp.Controllers
{
    //Mask URL
    [Route("/Register")]
    public class RegisterController : Controller
    {


        public IActionResult Register() => View();

        //attempt user registration
        [HttpPost]
        public async Task<IActionResult> Register(string loginID, string username, string password, string imageName, string imagePath, IFormFile image)
        {

            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("user");

            Key key = _keyFactory.CreateKey("default");

            Query query = new Query("user")
            {
                Filter = Filter.Equal("id", loginID)
            };
            //find login id
            var login = _context.RunQueryLazilyAsync(query);

            if (login != null)
            {
                ModelState.AddModelError("RegistrationFailure", "The ID already exists");
                return View();
            }

            Query usernameQuery = new Query("user")
            {
                Filter = Filter.Equal("user_name", username)
            };

            //find username
            var userName = _context.RunQueryLazilyAsync(usernameQuery);

            if (userName != null)
            {
                ModelState.AddModelError("RegistrationFailure", "The username already exists");
                return View();
            }

            string imageString = "gs://s3858853-a1-storage/" + imageName;

            Entity user = new Entity()
            {
                Key = _context.CreateKeyFactory("user").CreateKey("default"),
                ["id"] = loginID,
                ["user_name"] = username,
                ["password"] = password,
                ["image"] = imageString
            };

            await _context.InsertAsync(user);

            // Prepare bucket and image for upload

            var client = StorageClient.Create();

            var bucket = client.GetBucketAsync("s3858853-a1-storage");

            var content = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

            // Upload file to bucket
            if (imageName.Contains(".png"))
            {
                var obj1 = client.UploadObjectAsync("s3858853-a1-storage", imageName, "image/png", content);
            }
            else
            {
                var obj1 = client.UploadObjectAsync("s3858853-a1-storage", imageName, "image/jpg", content);
            }

            return RedirectToAction("Login", "User");

        }

    }
}
