using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ExternalLoginCallback;
using Stubbl.Identity.Notifications.Email;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class ExternalLoginCallbackController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public ExternalLoginCallbackController(IEmailSender emailSender,
            IIdentityServerInteractionService interactionService, SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public ExternalLoginCallbackViewModel BuildExternalLoginCallbackViewModel(ExternalLoginCallbackInputModel inputModel,
            string loginProvider, string returnUrl)
        {
            return new ExternalLoginCallbackViewModel
            (
                loginProvider,
                returnUrl
            )
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                EmailAddress = inputModel.EmailAddress
            };
        }

        [HttpGet("/external-login-callback", Name = "ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl,
            [FromQuery] string remoteError, CancellationToken cancellationToken)
        {
            if (remoteError != null)
            {
                return View("Error");
            }

            await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                return View("Error");
            }

            var signInResult =
                await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, false,
                    false);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return RedirectToRoute("LockedOut");
                }

                if (signInResult.IsNotAllowed)
                {
                    var user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);

                    if (_signInManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id, returnUrl });
                    }

                    return View("Error");
                }

                if (signInResult.RequiresTwoFactor)
                {
                    // TODO LoginTwoFactor

                    return View("Error");
                    //return RedirectToRoute("LoginTwoFactor", new { returnUrl });
                }

                var emailAddress = Regex.Replace(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email), @"\+[^@]+", "");
                var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name)?.Split(" ");

                var model = new ExternalLoginCallbackInputModel
                {
                    FirstName = name?.First(),
                    LastName = name?.Length > 1 ? name.Last() : null,
                    EmailAddress = emailAddress
                };

                ModelState.SetModelValue(nameof(model.FirstName), model.FirstName, model.FirstName);
                ModelState.SetModelValue(nameof(model.LastName), model.LastName, model.LastName);
                ModelState.SetModelValue(nameof(model.EmailAddress), model.EmailAddress, model.EmailAddress);

                TryValidateModel(model);

                return await ExternalLoginCallback(model, returnUrl, true, cancellationToken);
            }

            if (_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("Home");
        }

        [HttpPost("/external-login-callback", Name = "ExternalLoginCallback")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginCallback([FromForm] ExternalLoginCallbackInputModel inputModel,
            [FromQuery] string returnUrl, [FromQuery] bool isEmailAddressConfirmed, CancellationToken cancellationToken)
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                return View("Error");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = BuildExternalLoginCallbackViewModel(inputModel, externalLoginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            if (await _userManager.FindByEmailAsync(inputModel.EmailAddress) != null)
            {
                ModelState.AddModelError(nameof(inputModel.EmailAddress), "This email address has already been registered");

                var viewModel = BuildExternalLoginCallbackViewModel(inputModel, externalLoginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            var user = new StubblUser
            {
                EmailAddress = inputModel.EmailAddress,
                EmailAddressConfirmed = isEmailAddressConfirmed,
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Username = inputModel.EmailAddress
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            result = await _userManager.AddLoginAsync(user, externalLoginInfo);

            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return View("Error");
            }

            await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

            if (!isEmailAddressConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.RouteUrl("ConfirmEmailAddress", new { userId = user.Id, token, returnUrl },
                    Request.Scheme);
                var email = new ConfirmEmailAddressEmail
                (
                    user.NewEmailAddress,
                    callbackUrl
                );

                await _emailSender.SendEmailAsync(email, cancellationToken);

                if (_signInManager.Options.SignIn.RequireConfirmedEmail)
                {
                    return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id, returnUrl });
                }

                return View("Error");
            }

            await _signInManager.SignInAsync(user, false);

            if (_interactionService.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("Home");
        }
    }
}