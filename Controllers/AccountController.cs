using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Models;
using VotingSystem.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "تم إيقاف الحساب مؤقتًا بسبب محاولات فاشلة متكررة");
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            ModelState.AddModelError("", "حدث خطأ غير متوقع أثناء تسجيل الدخول");
            return View(model);
        }
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!model.AcceptTerms)
        {
            ModelState.AddModelError(
                "AcceptTerms",
                "يجب الموافقة على الشروط"
            );
        }
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", MapIdentityError(error));
                }

                return View(model);
            }

            await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));


            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register failed");
            ModelState.AddModelError("", "حدث خطأ أثناء إنشاء الحساب");
            return View(model);
        }
    }

    
    private string MapIdentityError(IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateUserName" => "هذا البريد مستخدم بالفعل",
            "DuplicateEmail" => "هذا البريد مستخدم بالفعل",
            "PasswordTooShort" => "كلمة المرور قصيرة جدًا",
            "PasswordRequiresDigit" => "كلمة المرور يجب أن تحتوي على رقم",
            "PasswordRequiresUpper" => "كلمة المرور يجب أن تحتوي على حرف كبير",
            "PasswordRequiresLower" => "كلمة المرور يجب أن تحتوي على حرف صغير",
            "PasswordRequiresNonAlphanumeric" => "كلمة المرور يجب أن تحتوي على رمز",
            _ => "حدث خطأ أثناء إنشاء الحساب"
        };
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
