namespace LibraryProjectModule12.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;


public class AccountController
{
    //private readonly UserManager<User> _userManager;
    //private readonly SignInManager<User> _signInManager;
    //private readonly RoleManager<IdentityRole> _roleManager;

    //public AccountController(
    //    UserManager<User> userManager,
    //    SignInManager<User> signInManager,
    //    RoleManager<IdentityRole> roleManager)
    //{
    //    _userManager = userManager;
    //    _signInManager = signInManager;
    //    _roleManager = roleManager;
    //}

    //// ===============================
    //// РЕГИСТРАЦИЯ - форма и обработка
    //// ===============================

    ///// <summary>
    ///// GET: /Account/Register
    ///// Показва формата за регистрация
    ///// </summary>
    //[HttpGet]
    //public IActionResult Register()
    //{
    //    // Ако потребителят вече е влязъл, не му трябва регистрация
    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        return RedirectToAction("Index", "Home");
    //    }

    //    return View();
    //}

    ///// <summary>
    ///// POST: /Account/Register
    ///// Обработва регистрацията
    ///// </summary>
    //[HttpPost]
    //[ValidateAntiForgeryToken]  // CSRF защита
    //public async Task<IActionResult> Register(RegisterViewModel model)
    //{
    //    // СТЪПКА 1: Проверка на валидацията
    //    if (!ModelState.IsValid)
    //    {
    //        // Ако има грешки ([Required], [StringLength] и т.н.) връщаме формата с грешките
    //        return View(model);
    //    }

    //    // СТЪПКА 2: Проверка дали email-ът вече съществува
    //    var existingUser = await _userManager.FindByEmailAsync(model.Email);
    //    if (existingUser != null)
    //    {
    //        ModelState.AddModelError("Email", "Този email вече е регистриран");
    //        return View(model);
    //    }

    //    // СТЪПКА 3: Създаване на нов ApplicationUser обект
    //    var user = new ApplicationUser
    //    {
    //        UserName = model.Username,
    //        Email = model.Email,
    //        EmailConfirmed = true  // За простота директно потвърждаваме
    //    };

    //    // СТЪПКА 4: Създаване на потребителя в базата данни

    //    // UserManager автоматично:
    //    // - Хешира паролата с PBKDF2
    //    // - Проверява изискванията за парола
    //    // - Добавя потребителя в AspNetUsers таблица
    //    var result = await _userManager.CreateAsync(user, model.Password);

    //    if (result.Succeeded)
    //    {
    //        // СТЪПКА 5: Уверяваме се че ролите съществуват
    //        await EnsureRolesCreated();

    //        // СТЪПКА 6: Добавяме потребителя в роля "User"
    //        // Всички нови потребители са с роля User, а потребителят Admin се създава от Seed данните
    //        await _userManager.AddToRoleAsync(user, "User");

    //        // СТЪПКА 7: Автоматично влизаме потребителя
    //        // isPersistent: false -> Session cookie (изтрива се при затваряне на браузъра)
    //        await _signInManager.SignInAsync(user, isPersistent: false);

    //        // СТЪПКА 8: Пренасочване към началната страница
    //        return RedirectToAction("Index", "Home");
    //    }

    //    // АКО ИМА ГРЕШКИ при създаване
    //    // Добавяме всички грешки от Identity към ModelState
    //    foreach (var error in result.Errors)
    //    {
    //        ModelState.AddModelError(string.Empty, error.Description);
    //    }

    //    return View(model);
    //}

    //// ============
    //// LOGIN (ВХОД)
    //// =============

    ///// <summary>
    ///// GET: /Account/Login
    ///// Показва формата за вход
    ///// </summary>
    //[HttpGet]
    //public IActionResult Login()
    //{
    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        return RedirectToAction("Index", "Home");
    //    }

    //    return View();
    //}

    ///// <summary>
    ///// POST: /Account/Login
    ///// Обработва входа
    ///// </summary>
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Login(LoginViewModel model)
    //{
    //    // СТЪПКА 1: Валидация
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    // СТЪПКА 2: Опит за вход

    //    // SignInManager:
    //    // - Намира потребителя по UserName
    //    // - Сравнява хеширани пароли
    //    // - Създава authentication cookie
    //    // - Проверява за lockout
    //    var result = await _signInManager.PasswordSignInAsync(
    //        userName: model.Username,
    //        password: model.Password,
    //        isPersistent: model.RememberMe,  // Persistent cookie?
    //        lockoutOnFailure: true           // Lockout след грешки?
    //    );

    //    // РЕЗУЛТАТ: Успешен вход
    //    if (result.Succeeded)
    //    {
    //        return RedirectToAction("Index", "Home");
    //    }

    //    // РЕЗУЛТАТ: Акаунтът е заключен (lockout)
    //    if (result.IsLockedOut)
    //    {
    //        ModelState.AddModelError(string.Empty,
    //            "Акаунтът ви е временно заключен поради много грешни опити. " +
    //            "Моля опитайте отново след 5 минути.");
    //        return View(model);
    //    }

    //    // РЕЗУЛТАТ: Грешно потребителско име или парола
    //    ModelState.AddModelError(string.Empty, "Невалидно потребителско име или парола.");
    //    return View(model);
    //}

    //// ==============
    //// LOGOUT (ИЗХОД)
    ////===============

    ///// <summary>
    ///// POST: /Account/Logout
    ///// Изход от системата
    ///// </summary>
    //[HttpPost]
    //[Authorize]  // Само влезли потребители могат да излязат
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Logout()
    //{
    //    // SignInManager.SignOutAsync():
    //    // - Изтрива authentication cookie
    //    // - Изчиства claims
    //    await _signInManager.SignOutAsync();

    //    return RedirectToAction("Index", "Home");
    //}


    //// ACCESS DENIED (403 FORBIDDEN)
    ///// <summary>
    ///// GET: /Account/AccessDenied
    ///// Показва се когато потребител няма права за действие
    ///// Пример: Обикновен потребител опита да влезе в Admin панел
    ///// </summary>
    //[HttpGet]
    //public IActionResult AccessDenied()
    //{
    //    return View();
    //}


    //// HELPER МЕТОДИ
    ///// <summary>
    ///// Помощен метод за създаване на роли
    ///// Извиква се при регистрация
    ///// </summary>
    //private async Task EnsureRolesCreated()
    //{
    //    // Проверяваме и създаваме роля Admin
    //    if (!await _roleManager.RoleExistsAsync("Admin"))
    //    {
    //        await _roleManager.CreateAsync(new IdentityRole("Admin"));
    //    }

    //    // Проверяваме и създаваме роля User
    //    if (!await _roleManager.RoleExistsAsync("User"))
    //    {
    //        await _roleManager.CreateAsync(new IdentityRole("User"));
    //    }

    //}
}
