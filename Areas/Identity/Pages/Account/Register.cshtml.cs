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
using WarbandOfTheSpiritborn.Areas.Identity;

namespace WarbandOfTheSpiritborn.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterModel> _logger;
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

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Load external login providers.
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // Default to the home page when no return URL is supplied.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Reload providers if the page is redisplayed.

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password); // Create the new user account.

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description); // Show user creation errors.
                }

                return Page();
            }

            _logger.LogInformation("User created a new account with password.");

            var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User); // Assign the default User role.

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    _logger.LogError("Failed to assign default role to user {Email}: {Error}", user.Email, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(user); // Remove the user if role assignment fails.
                return Page();
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generate the email confirmation token.
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackPath = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new
                {
                    area = "Identity",
                    userId = user.Id,
                    code,
                    returnUrl
                });

            var publicBaseUrl = _configuration["AppSettings:PublicBaseUrl"];

            if (string.IsNullOrWhiteSpace(publicBaseUrl))
            {
                publicBaseUrl = $"{Request.Scheme}://{Request.Host}"; // Fall back to the current host if no public URL is configured.
            }

            var callbackUrl = $"{publicBaseUrl}{callbackPath}";

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Send the confirmation email.

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new
                {
                    email = Input.Email,
                    returnUrl
                });
            }

            await _signInManager.SignInAsync(user, isPersistent: false); // Sign in immediately when confirmation is not required.
            return LocalRedirect(returnUrl);
        }
    }
}