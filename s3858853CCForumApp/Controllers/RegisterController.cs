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
        public async Task<IActionResult> Register(string id, string user_name, string Password, IFormFile Image)
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
                Filter = Filter.Equal("id", id)
            };
            //find login id
            var login = _context.RunQueryLazilyAsync(query);

            var loginCheck = true;

            await login.ForEachAsync(x =>
            {
                if (x == null)
                {
                    loginCheck = false;
                }
            });

            if (loginCheck == false)
            {
                ModelState.AddModelError("RegistrationFailure", "The ID already exists");
                return View();
            }

            Query usernameQuery = new Query("user")
            {
                Filter = Filter.Equal("user_name", user_name)
            };

            //find username
            var userName = _context.RunQueryLazilyAsync(usernameQuery);

            bool confirmed = false;

            await userName.ForEachAsync(x =>
            {
                if (x != null)
                {
                    confirmed = true;
                }
            });


            if (confirmed == true)
            {
                ModelState.AddModelError("RegistrationFailure", "The username already exists");
                return View();
            }

            var imageName = Image.FileName;

            string imageString = "gs://s3858853-a1-storage/" + imageName;

            KeyFactory keyFactory = _context.CreateKeyFactory("user");

            Entity user = new Entity()
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["id"] = id,
                ["user_name"] = user_name,
                ["password"] = Password,
                ["image"] = imageString
            };

            await _context.InsertAsync(user);

            // Prepare bucket and image for upload

            var client = StorageClient.Create();

            var bucket = client.GetBucketAsync("s3858853-a1-storage");

            var obj1 = "a";

            if (imageName.Contains(".png"))
            {
                obj1 = "image/png";
            }
            else
            {
                obj1 = "image/jpg";
            }

            // Upload file to bucket
            using (var memoryStream = new MemoryStream())
            {
                await Image.CopyToAsync(memoryStream);
                var dataObject = await client.UploadObjectAsync("s3858853-a1-storage", imageName, obj1, memoryStream);                
            }

            return RedirectToAction("Login", "Login");

        }

    }
}
