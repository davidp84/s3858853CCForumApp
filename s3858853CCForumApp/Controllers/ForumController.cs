using Google.Api.Gax;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using s3858853CCForumApp.Data;
using s3858853CCForumApp.Models;
using System.Linq;
using System.Text;
using System.Security.Claims;

namespace s3858853CCForumApp.Controllers
{
    public class ForumController : Controller
    {
        private string UserID => HttpContext.Session.GetString("sessionID");

        private string UserName => HttpContext.Session.GetString("user_name");

        public ForumController()
        {
        }

        public IActionResult Index()
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
                Filter = Filter.Equal("id", UserID)
            };

            //lazy loading
            var user = _context.RunQueryLazilyAsync(query);
            return View(user);
        }

        public async Task<IActionResult> Forum(int id)
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
                Filter = Filter.Equal("id", UserID)
            };

            //lazy loading
            var user = _context.RunQueryLazilyAsync(query);
            return View(user);
        }

        public async Task<IActionResult> NewPost()
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("post");

            //Key key = _keyFactory.CreateKey("default");

            Query query = new Query("post")
            {
                Filter = Filter.Equal("User", UserName)
            };

            var customer = _context.RunQueryLazilyAsync(query);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(string subject, string messageText, string imageName, IFormFile image, string imagePath)
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory keyFactory = _context.CreateKeyFactory("post");

            string imageString = "gs://s3858853-a1-storage/" + imageName;

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

            Entity update = new Entity
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["subject"] = subject,
                ["messageText"] = messageText,
                ["User"] = UserID,
                ["postTimeUTC"] = DateTime.Now,
                ["Image"] = imageString
            };
            await _context.InsertAsync(update);

            return RedirectToAction("Forum", "User");

        }
    }
}
