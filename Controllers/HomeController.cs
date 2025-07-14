using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PayrollToyHRD.Models;

namespace PayrollToyHRD.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        string? remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Normalize IPv6 loopback and IPv4-mapped IPv6 addresses
        if (remoteIpAddress == "::1")
        {
            remoteIpAddress = "127.0.0.1";
        }
        else if (remoteIpAddress != null && remoteIpAddress.StartsWith("::ffff:"))
        {
            remoteIpAddress = remoteIpAddress.Substring(7);
        }

        Console.WriteLine("Client IP: " + remoteIpAddress);

        var allowedIPs = new List<string>
    {
        "127.0.0.1",       // Local dev
        "192.168.1.101",
        "192.168.1.110",
        "192.168.1.200"
    };

        bool isAllowed = allowedIPs.Contains(remoteIpAddress);
        Console.WriteLine("Is Allowed: " + isAllowed);

        ViewBag.ClientIP = remoteIpAddress;
        ViewBag.IsAllowed = isAllowed;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
