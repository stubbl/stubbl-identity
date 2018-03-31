using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stubbl.Identity.Models.Consent;
using Scope = Stubbl.Identity.Models.Consent.Scope;

namespace Stubbl.Identity.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly ILogger<ConsentController> _logger;
        private readonly IResourceStore _resourceStore;

        public ConsentController(IClientStore clientStore,
            IIdentityServerInteractionService interactionService, ILogger<ConsentController> logger,
            IResourceStore resourceStore)
        {
            _clientStore = clientStore;
            _interactionService = interactionService;
            _logger = logger;
            _resourceStore = resourceStore;
        }

        private ConsentViewModel BuildConsentViewModel(ConsentInputModel model, Client client,
            Resources resources, string returnUrl)
        {
            var scopesConsented = (model?.ScopesConsented ?? new string[0]).ToList();

            var identityScopes = resources.IdentityResources.Select(ir =>
                new Scope
                (
                    ir.Name,
                    ir.DisplayName,
                    ir.Description,
                    scopesConsented.Contains(ir.Name) || model == null || ir.Required,
                    ir.Emphasize,
                    ir.Required
                ))
                .ToList();

            var resourceScopes = resources.ApiResources.SelectMany(ar => ar.Scopes).Select(s =>
                new Scope
                (
                    s.Name,
                    s.DisplayName,
                    s.Description,
                    scopesConsented.Contains(s.Name) || model == null || s.Required,
                    s.Emphasize,
                    s.Required
                ))
                .ToList();

            if (ConsentConfig.EnableOfflineAccess && resources.OfflineAccess)
            {
                resourceScopes = resourceScopes.Union(new[]
                    {
                        new Scope
                        (
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                            "Offline access",
                            "Access to your applications and resources, even when you are offline",
                            scopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model == null,
                            true,
                            false
                        )
                    })
                    .ToList();
            }

            var viewModel = new ConsentViewModel
            (
                client.ClientName ?? client.ClientId,
                client.ClientUri,
                client.LogoUri,
                identityScopes,
                resourceScopes,
                client.AllowRememberConsent
            )
            {
                RememberConsent = model?.RememberConsent ?? true,
                ReturnUrl = returnUrl,
                ScopesConsented = scopesConsented
            };


            return viewModel;
        }

        private async Task<ConsentViewModel> BuildConsentViewModelAsync(string returnUrl,
            ConsentInputModel model = null)
        {
            var request = await _interactionService.GetAuthorizationContextAsync(returnUrl);

            if (request != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);

                if (client != null)
                {
                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        return BuildConsentViewModel
                        (
                            model,
                            client,
                            resources,
                            returnUrl
                        );
                    }

                    _logger.LogError("No scopes matching: {0}",
                        request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                }
                else
                {
                    _logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return null;
        }

        [HttpGet("/consent", Name = "Consent")]
        public async Task<IActionResult> Consent([FromQuery] string returnUrl)
        {
            var viewModel = await BuildConsentViewModelAsync(returnUrl);

            if (viewModel == null)
            {
                return View("Error");
            }

            return View("Consent", viewModel);
        }

        [HttpPost("/consent", Name = "Consent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consent([FromForm] ConsentInputModel inputModel)
        {
            ConsentResponse grantedConsent = null;

            string returnUrl = null;
            string validationError = null;
            ConsentViewModel viewModel = null;

            if (inputModel.GrantConsent)
            {
                if (inputModel.ScopesConsented != null && inputModel.ScopesConsented.Any())
                {
                    var scopes = inputModel.ScopesConsented;

                    if (ConsentConfig.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(s => s != IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = inputModel.RememberConsent,
                        ScopesConsented = scopes.ToList()
                    };
                }
                else
                {
                    validationError = "You must pick at least one permission";
                }
            }
            else
            {
                grantedConsent = ConsentResponse.Denied;
            }

            if (grantedConsent != null)
            {
                var authorizationRequest = await _interactionService.GetAuthorizationContextAsync(inputModel.ReturnUrl);

                if (authorizationRequest != null)
                {
                    await _interactionService.GrantConsentAsync(authorizationRequest, grantedConsent);

                    returnUrl = inputModel.ReturnUrl;
                }
            }
            else
            {
                viewModel = await BuildConsentViewModelAsync(inputModel.ReturnUrl, inputModel);
            }

            if (viewModel != null)
            {
                if (validationError != null)
                {
                    ModelState.AddModelError("", "Error granting consent");
                }

                return View("Consent", viewModel);
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }

            return View("Error");
        }
    }
}