using Microsoft.AspNetCore.Mvc;

using VirtualFileWebApi.DbContexts;

namespace VirtualFileWebApi.Controllers;

[ApiController]
[Route("")]
public class VirtualFileController : ControllerBase
{
    private readonly ILogger<VirtualFileController> logger;
    private readonly EimaContext dbContext;

    public VirtualFileController(ILogger<VirtualFileController> logger, EimaContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet("file", Name ="GetFileList")]
    //[Route("file")]
    public IResult GetFileList()
    {
        //dbContext.VirtualFiles.Add(new DbModels.VirtualFile
        //{
        //    VirtualPath = "somepath",
        //    MimeType = "UNKNOWN",
        //    FileSize = 0,
        //    FileContent = null,
        //    ModifiedDatetime = DateTime.UtcNow,
        //});
        //dbContext.SaveChanges();

        return TypedResults.Ok("TODO: GetFileList");
    }

    [HttpGet("file/{**filePath}", Name = "GetFile")]
    //[Route()]
    public IResult GetFile(string filePath)
    {
        return TypedResults.Ok("TODO: GetFile: " + filePath);
    }

    [HttpPost]
    [Route("/file/{**filePath}")]
    public async Task<IResult> PostAsync(string filePath)
    {
        using MemoryStream ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);

        dbContext.VirtualFiles.Add(new DbModels.VirtualFile
        {
            VirtualPath = filePath,
            MimeType = Request.ContentType ?? System.Net.Mime.MediaTypeNames.Application.Octet,
            FileSize = ms.Length,
            FileContent = ms.ToArray(),
            ModifiedDatetime = DateTime.UtcNow,
        });
        dbContext.SaveChanges();

        return TypedResults.Ok("TODO: PostFile");
    }

    [HttpPut]
    [Route("/file/{**filePath}")]
    public IResult Put(string filePath)
    {
        return TypedResults.Ok("TODO: PutFile");
    }

    [HttpDelete]
    [Route("/file/{**filePath}")]
    public IResult Delete(string filePath)
    {
        return TypedResults.Ok("TODO: DeleteFile");
    }
}
