﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Auth
{
    public class AuthController: Controller
    {
        public SignInManager<WorldUser> signInManager { get; set; }

        public AuthController(SignInManager<WorldUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await this.signInManager.PasswordSignInAsync(vm.Username,
                                                                          vm.Password,
                                                                          true, false);

                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToAction("Trips", "App");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("", "Username or password incorrect");
                }
            }

            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await this.signInManager.SignOutAsync();
            }

            return RedirectToAction("Index","App");
        }
    }
}
