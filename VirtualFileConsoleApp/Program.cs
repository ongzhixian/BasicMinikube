using System.Net;
using System.Net.Http.Headers;

//String proxyURL = "http://103.167.135.111:80";
//WebProxy webProxy = new WebProxy(proxyURL);
//// make the HttpClient instance use a proxy
//// in its requests
//HttpClientHandler httpClientHandler = new HttpClientHandler
//{
//    Proxy = webProxy
//};
//client = new HttpClient(httpClientHandler);

HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5266");

//await GetFileListAsync();

string uploadDirectory = "C:\\src\\temp\\test-pics";

await AddFile("pics/camera.png", "C:\\src\\temp\\test-pics\\camera.png");

//foreach (var filePath in System.IO.Directory.GetFiles(uploadDirectory))
//{
//    Console.WriteLine(filePath);
//}

//await AddFile("virtualFilePath", "physicalFilePath");

//await GetFileListAsync("after AddFile");

//await GetFile("virtualFilePath", "saveToPhysicalFilePath");

//await DeleteFile("virtualFilePath");

return;

async Task AddFile(string virtualPath, string physicalFilePath)
{
    //ByteArrayContent content = new ByteArrayContent(await File.ReadAllBytesAsync(physicalFilePath));
    
    using FileStream fs = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    using StreamContent content = new StreamContent(fs);
    //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
    content.Headers.ContentType = GetContentTypeByFilePath(physicalFilePath);
    var response = await client.PostAsync($"/file/{virtualPath}", content);
    Console.WriteLine(response);
    Console.WriteLine(response.StatusCode);
}

MediaTypeHeaderValue GetContentTypeByFilePath(string physicalFilePath)
{
    switch (Path.GetExtension(physicalFilePath).ToUpperInvariant() ?? string.Empty)
    {
        case ".PNG":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Image.Png);
        case ".JPG":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Image.Jpeg);
        case ".GIF":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Image.Gif);
        case ".TXT":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Text.Plain);
        case ".CSV":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Text.Csv);
        case ".PDF":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Pdf);
        case ".ZIP":
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Zip);
        default:
            return new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
    }
}

async Task GetFileListAsync(string? comment = null)
{
    PrintTitle("GetFileListAsync");
    if (string.IsNullOrEmpty(comment)) PrintComment(comment);
    var response = await client.GetAsync("/file/hshad");
    Console.WriteLine(response.StatusCode);
    Console.WriteLine(response);

}

void PrintComment(string? comment)
{
    Console.WriteLine($"{comment}");
}

static void PrintTitle(string title)
{
    var foregroundColorBackup = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{title}");
    Console.ForegroundColor = foregroundColorBackup;
}