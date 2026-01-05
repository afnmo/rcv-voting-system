using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Models;

namespace VotingSystem.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("/account/register")]
    public IActionResult Register() => View();

    [HttpPost("/account/register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/account/login")]
    public IActionResult Login() => View();

    [HttpPost("/account/login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized("Invalid login");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}