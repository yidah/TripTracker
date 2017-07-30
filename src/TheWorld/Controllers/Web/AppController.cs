
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController: Controller
    {
        public IMailService mailService { get; set; }

        public IConfigurationRoot config { get; set; }

        public IWorldRepository repository { get; set; }

        public ILogger<AppController> logger { get; set; }

        public AppController(IMailService mailService, 
            IConfigurationRoot config, 
            IWorldRepository repository,
            ILogger<AppController> logger)
        {
            this.mailService = mailService;
            this.config = config;
            this.repository = repository;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();

        }

        [Authorize]
        public IActionResult Trips()
        {
            //We wont use this code as angular will handle this
            //try
            //{
            //    var data = this.repository.GetAllTrips();
            //    return View(data);
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Failed to get trips in Index page{ex.Message }");
            //    return Redirect("/error");
            //}

            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (model.Email.Contains("aol.com"))
            {
                // as I am using Email property the error will appear next to the field linked to the property
                ModelState.AddModelError("Email", "We dont support aol" );
                //ModelState.AddModelError("", "We dont support aol");
            }
            if (ModelState.IsValid)
            {
                mailService.SendMail(config["MailSettings:ToAddress"], model.Email, "From Jordi", model.Message);
                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent";
            }
            
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
