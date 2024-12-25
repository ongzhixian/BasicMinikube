using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.RazorPages;

using QRCoder;

namespace MobileWebApp.Pages;

public class QrCodePageModel : PageModel
{
    private readonly ILogger<QrCodePageModel> logger;

    [BindProperty, Required]
    public string QRCodeContent { get; set; } = null!;

    public string QRCodeImage { get; set; } = null!;

    public QrCodePageModel(ILogger<QrCodePageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
        // Any server-side logic can go here
        GenerateQrCode("https://telera-mobile-app.azurewebsites.net/login?ReturnUrl=%2F");
    }

    public void OnPostGenerate()
    {
        GenerateQrCode(QRCodeContent);
    }

    void GenerateQrCode(string qrCodeContent)
    {
        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        QRCodeImage = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
    }
}
