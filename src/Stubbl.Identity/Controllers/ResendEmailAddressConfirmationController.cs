using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Notifications.Email;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class ResendEmailAddressConfirmationController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<StubblUser> _userManager;

        public ResendEmailAddressConfirmationController(IEmailSender emailSender, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpPost("/resend-email-address-confirmation/{userId}", Name = "ResendEmailAddressConfirmation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail([FromRoute] string userId,
            [FromQuery] string returnUrl, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToRoute("Home");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ConfirmEmailAddress", new {userId = user.Id, token, returnUrl},
                Request.Scheme);
            var email = new ConfirmEmailAddressEmail
            (
                user.NewEmailAddress,
                callbackUrl
            );

            await _emailSender.SendEmailAsync(email, cancellationToken);

            return RedirectToRoute("EmailAddressConfirmationSent", new {userId = user.Id, resent = true, returnUrl});
        }
    }
}