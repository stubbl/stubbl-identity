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

        private ConsentViewModel BuildConsentViewModel(ConsentInputModel model, string returnUrl,
            AuthorizationRequest request, Client client, Resources resources)
        {
            var viewModel = new ConsentViewModel
            {
                AllowRememberConsent = client.AllowRememberConsent,
                ClientLogoUrl = client.LogoUri,
                ClientName = client.ClientName ?? client.ClientId,
                ClientUrl = client.ClientUri,
                RememberConsent = model?.RememberConsent ?? true,
                ReturnUrl = returnUrl,
                ScopesConsented = model?.ScopesConsented ?? new string[0]
            };

            viewModel.IdentityScopes = resources.IdentityResources.Select(ir =>
                    new Scope
                    {
                        Checked = viewModel.ScopesConsented.Contains(ir.Name) || model == null || ir.Required,
                        Description = ir.Description,
                        DisplayName = ir.DisplayName,
                        Emphasize = ir.Emphasize,
                        Name = ir.Name,
                        Required = ir.Required
                    })
                .ToList();
            viewModel.ResourceScopes = resources.ApiResources.SelectMany(ar => ar.Scopes).Select(s =>
                    new Scope
                    {
                        Checked = viewModel.ScopesConsented.Contains(s.Name) || model == null || s.Required,
                        Description = s.Description,
                        DisplayName = s.DisplayName,
                        Emphasize = s.Emphasize,
                        Name = s.Name,
                        Required = s.Required
                    })
                .ToList();

            if (ConsentConfig.EnableOfflineAccess && resources.OfflineAccess)
            {
                viewModel.ResourceScopes = viewModel.ResourceScopes.Union(new[]
                    {
                        new Scope
                        {
                            Checked = viewModel.ScopesConsented.Contains(IdentityServerConstants.StandardScopes
                                          .OfflineAccess) || model == null,
                            Description = "Access to your applications and resources, even when you are offline",
                            DisplayName = "Offline access",
                            Name = IdentityServerConstants.StandardScopes.OfflineAccess,
                            Emphasize = true
                        }
                    })
                    .ToList();
            }

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
                        return BuildConsentViewModel(model, returnUrl, request, client, resources);
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
        public async Task<IActionResult> Consent(string returnUrl)
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
        public async Task<IActionResult> Consent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            ConsentResponse grantedConsent = null;

            switch (model.Button)
            {
                case "no":
                    grantedConsent = ConsentResponse.Denied;
                    break;
                case "yes":
                    if (model.ScopesConsented != null && model.ScopesConsented.Any())
                    {
                        var scopes = model.ScopesConsented;

                        if (ConsentConfig.EnableOfflineAccess == false)
                        {
                            scopes = scopes.Where(s => s != IdentityServerConstants.StandardScopes.OfflineAccess);
                        }

                        grantedConsent = new ConsentResponse
                        {
                            RememberConsent = model.RememberConsent,
                            ScopesConsented = scopes.ToArray()
                        };
                    }
                    else
                    {
                        result.ValidationError = "You must pick at least one permission";
                    }

                    break;
                default:
                    result.ValidationError = "Invalid selection";
                    break;
            }

            if (grantedConsent != null)
            {
                var authorizationRequest = await _interactionService.GetAuthorizationContextAsync(model.ReturnUrl);

                if (authorizationRequest != null)
                {
                    await _interactionService.GrantConsentAsync(authorizationRequest, grantedConsent);

                    result.RedirectUri = model.ReturnUrl;
                }
            }
            else
            {
                result.ViewModel = await BuildConsentViewModelAsync(model.ReturnUrl, model);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Consent", result.ViewModel);
            }

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            // TODO Custom exception.
            throw new Exception();
        }
    }
}