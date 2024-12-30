using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharpCompress.Common;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class DownloadInventoryItemPageModel : PageModel
{
    private readonly ILogger<DownloadInventoryItemPageModel> _logger;

    public DownloadInventoryItemPageModel(ILogger<DownloadInventoryItemPageModel> logger)
    {
        _logger = logger;
    }

    public FileContentResult OnGet()
    {
        var fileBytes = CreateSpreadsheetWorkbook();

        List<AppUser> appUsers = new List<AppUser>();
        appUsers.Add(new AppUser { Username = "asda,zxc" });
        appUsers.Add(new AppUser { Username = "zxc\"\"234" });
        appUsers.Add(new AppUser { Username = "qwe,zxc" });

        fileBytes = CsvServices.ExportToCsvBytes(appUsers);

        return File(fileBytes
            , System.Net.Mime.MediaTypeNames.Text.Csv
            , "TestFile.csv");


        //return File(fileBytes
        //    , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //    , "TestFile.xlsx");




        //return File(System.Text.Encoding.UTF8.GetBytes("Hello test file"), "plain/text", "TestFile.txt");
    }


    static byte[] CreateSpreadsheetWorkbook()
    {
        using MemoryStream ms = new MemoryStream();
        // Create a spreadsheet document by supplying the filepath.
        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);

        // Add a WorkbookPart to the document.
        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
        sheets.Append(sheet);

        

        workbookPart.Workbook.Save();

        // Dispose the document.
        spreadsheetDocument.Dispose();

        ms.Close();
        return ms.ToArray();
    }
}
