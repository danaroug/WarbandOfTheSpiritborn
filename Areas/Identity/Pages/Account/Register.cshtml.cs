// ASP.NET Core and Identity dependencies.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WarbandOfTheSpiritborn.Areas.Identity.Pages.Account
{
    // Allows unauthenticated users to access the registration page.
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        // Manages user sign-in operations.
        private readonly SignInManager<IdentityUser> _signInManager;

        // Manages user creation and Identity operations.
        private readonly UserManager<IdentityUser> _userManager;

        // Reads application configuration values.
        private readonly IConfiguration _configuration;

        // Writes diagnostic logs for registration events.
        private readonly ILogger<RegisterModel> _logger;

        // Sends confirmation and other Identity emails.
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _emailSender = emailSender;
        }

        // Binds registration form input to the page model.
        [BindProperty]
        public InputModel Input { get; set; }

        // Stores the return URL for redirect after registration.
        public string ReturnUrl { get; set; }

        // Stores available external login providers.
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Defines the registration form input fields and validation rules.
        public class InputModel
        {
            // Stores the user's email address.
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // Stores the user's password.
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            // Confirms that the user entered the same password twice.
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        // Loads the registration page and available external login options.
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // Handles user registration, role assignment, and email confirmation.
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Default to the home page when no return URL is provided.
            returnUrl = returnUrl ?? Url.Content("~/");

            // Reload external login options if the page needs to be redisplayed.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Create a new Identity user from the submitted email address.
                var user = new IdentityUser
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };

                // Create the user in the database.
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Assign the default User role to the new account.
                    var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);

                    // Delete the user if role assignment fails to avoid inconsistent data.
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            _logger.LogError("Failed to assign default role to user {Email}: {Error}", user.Email, error.Description);
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        await _userManager.DeleteAsync(user);
                        return Page();
                    }

                    // Generate and encode the email confirmation token.
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // Build the confirmation path first.
                    var callbackPath = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            area = "Identity",
                            userId = user.Id,
                            code = code,
                            returnUrl = returnUrl
                        });

                    // Use the public site URL for email links.
                    var publicBaseUrl = _configuration["AppSettings:PublicBaseUrl"];

                    if (string.IsNullOrWhiteSpace(publicBaseUrl))
                    {
                        publicBaseUrl = $"{Request.Scheme}://{Request.Host}";
                    }

                    var callbackUrl = $"{publicBaseUrl}{callbackPath}";

                    // Send the confirmation email to the new user.
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    );

                    // Redirect to the confirmation page when confirmed accounts are required.
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new
                        {
                            email = Input.Email,
                            returnUrl = returnUrl
                        });
                    }
                    else
                    {
                        // Sign the user in immediately when confirmation is not required.
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                // Add user creation errors to the page model state.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Redisplay the form if validation or registration fails.
            return Page();
        }
    }
}