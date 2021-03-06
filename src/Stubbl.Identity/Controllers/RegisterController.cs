﻿using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.Register;
using Stubbl.Identity.Notifications.Email;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public RegisterController(IEmailSender emailSender, IIdentityServerInteractionService interactionService,
            SignInManager<StubblUser> signInManager, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public RegisterViewModel BuildRegisterViewModel(string returnUrl, string emailAddress = null,
            RegisterInputModel inputModel = null)
        {
            return new RegisterViewModel
            (
                returnUrl
            )
            {
                EmailAddress = emailAddress ?? inputModel?.EmailAddress,
                Password = inputModel?.Password,
            };
        }

        [HttpGet("/register", Name = "Register")]
        [AllowAnonymous]
        public IActionResult Register([FromQuery] string emailAddress, [FromQuery] string returnUrl)
        {
            ModelState.Clear();

            var viewModel = BuildRegisterViewModel(returnUrl, emailAddress);

            return View(viewModel);
        }

        [HttpPost("/register", Name = "Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterInputModel inputModel,
            [FromQuery] string returnUrl, CancellationToken cancellationToken)
        {
            RegisterViewModel viewModel;

            if (!ModelState.IsValid)
            {
                viewModel = BuildRegisterViewModel(returnUrl);

                return View(viewModel);
            }

            var emailAddress = Regex.Replace(inputModel.EmailAddress, @"\+[^@]+", "");

            if (await _userManager.FindByEmailAsync(emailAddress) != null)
            {
                ModelState.AddModelError(nameof(inputModel.EmailAddress), "This email address has already been registered");

                viewModel = BuildRegisterViewModel(returnUrl);

                return View(viewModel);
            }

            var user = new StubblUser
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                EmailAddress = emailAddress,
                Username = inputModel.EmailAddress
            };

            var result = await _userManager.CreateAsync(user, inputModel.Password);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ConfirmEmailAddress", new {userId = user.Id, token, returnUrl},
                Request.Scheme);
            var email = new ConfirmEmailAddressEmail
            (
                user.EmailAddress,
                callbackUrl
            );

            await _emailSender.SendEmailAsync(email, cancellationToken);

            if (_signInManager.Options.SignIn.RequireConfirmedEmail)
            {
                return RedirectToRoute("EmailAddressConfirmationSent", new {userId = user.Id, returnUrl});
            }

            await _signInManager.SignInAsync(user, false);

            if (_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("Home");
        }
    }
}