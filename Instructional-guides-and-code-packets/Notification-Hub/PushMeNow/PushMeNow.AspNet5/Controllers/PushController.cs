using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PushMeNow.AspNet5.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PushMeNow.AspNet5.Controllers
{
    public class PushController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var registrations = await GetRegistrationsAsync();
            return View(registrations);
        }

        [HttpGet]
        public IActionResult Send([Bind("Tags,RegistrationType")] Registration registration)
        {
            return View(new Message(registration));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([Bind("Tags,Text,RegistrationType")] Message message)
        {
            if (ModelState.IsValid)
            {
                NotificationHubApiClient notificationHubApiClient = GetClient();
                await notificationHubApiClient.SendNotificationAsync(message);
                return RedirectToAction("Index");
            }

            return View(message);
        }

        private async Task<IEnumerable<Registration>> GetRegistrationsAsync()
        {
            var notificationHubApiClient = GetClient();
            var registrationTags = await notificationHubApiClient.GetRegistrationsAsync();
            return registrationTags;
        }

        private static NotificationHubApiClient GetClient()
        {
            string hubName = "<HUB-NAME>";
            string fullConnectionString = "<HUB-FULL-CONNECTION_STRING>";
            return new NotificationHubApiClient(hubName, fullConnectionString);
        }
    }
}
