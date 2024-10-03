using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OidcServerWebApp.Models;
using OidcServerWebApp.Services;

namespace OidcServerWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizeResultService _authorizeResultService;
    private readonly ICodeStoreService _codeStoreService;

    public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor,
        IAuthorizeResultService authorizeResultService,
            ICodeStoreService codeStoreService)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _authorizeResultService = authorizeResultService;
        _codeStoreService = codeStoreService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Authorize(AuthorizationRequest authorizationRequest)
    {
        var result = _authorizeResultService.AuthorizeRequest(_httpContextAccessor, authorizationRequest);

        if (result.HasError)
            return RedirectToAction("Error", new { error = result.Error });

        var loginModel = new OpenIdConnectLoginRequest
        {
            RedirectUri = result.RedirectUri,
            Code = result.Code,
            RequestedScopes = result.RequestedScopes,
            Nonce = result.Nonce
        };


        return View("Login", loginModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(OpenIdConnectLoginRequest loginRequest)
    {
        // here I have to check if the username and passowrd is correct
        // and I will show you how to integrate the ASP.NET Core Identity
        // With our framework

        var result = _codeStoreService.UpdatedClientDataByCode(loginRequest.Code, loginRequest.RequestedScopes,
            loginRequest.UserName, nonce: loginRequest.Nonce);
        if (result != null)
        {

            loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
            return Redirect(loginRequest.RedirectUri);
        }
        return RedirectToAction("Error", new { error = "invalid_request" });
    }

    public JsonResult Token()
    {
        var result = _authorizeResultService.GenerateToken(_httpContextAccessor);

        if (result.HasError)
            return Json("0");

        return Json(result);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
