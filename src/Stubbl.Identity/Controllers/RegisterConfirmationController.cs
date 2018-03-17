﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.RegisterConfirmation;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class RegisterConfirmationController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<StubblUser> _userManager;

        public RegisterConfirmationController(IEmailSender emailSender, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet("/register-confirmation/{userId}", Name = "RegisterConfirmation")]
        public async Task<IActionResult> RegisterConfirmation(string userId, string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToRoute("Home");
            }

            var viewModel = new RegisterConfirmationViewModel
            {
                EmailAddress = user.EmailAddress,
                ReturnUrl = returnUrl,
                UserId = userId
            };

            return View(viewModel);
        }
    }
}