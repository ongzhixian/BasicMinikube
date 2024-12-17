using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VirtualFileWebApi.DbContexts;

namespace VirtualFileWebApi.Controllers;

[ApiController]
[Route("[controller]")]
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

    //[Route()]
    [HttpGet("file/{**filePath}", Name = "GetFile")]
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

        var virtualFile = await dbContext.VirtualFiles.FirstOrDefaultAsync(r => r.VirtualPath == filePath);

        if (virtualFile == null)
        {
            dbContext.VirtualFiles.Add(new DbModels.VirtualFile
            {
                VirtualPath = filePath,
                MimeType = Request.ContentType ?? System.Net.Mime.MediaTypeNames.Application.Octet,
                FileSize = ms.Length,
                FileContent = ms.ToArray(),
                ModifiedDatetime = DateTime.UtcNow,
            });
        }
        else
        {
            virtualFile.MimeType = Request.ContentType ?? System.Net.Mime.MediaTypeNames.Application.Octet;
            virtualFile.FileSize = ms.Length;
            virtualFile.FileContent = ms.ToArray();
            virtualFile.ModifiedDatetime = DateTime.UtcNow;
        }

        dbContext.SaveChanges();

        return TypedResults.Ok();
    }

    [HttpPut]
    [Route("/file/{**filePath}")]
    public async Task<IResult> PutAsync(string filePath)
    {
        using MemoryStream ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);

        var virtualFile = await dbContext.VirtualFiles.FirstOrDefaultAsync(r => r.VirtualPath == filePath);

        if (virtualFile == null)
        {
            dbContext.VirtualFiles.Add(new DbModels.VirtualFile
            {
                VirtualPath = filePath,
                MimeType = Request.ContentType ?? System.Net.Mime.MediaTypeNames.Application.Octet,
                FileSize = ms.Length,
                FileContent = ms.ToArray(),
                ModifiedDatetime = DateTime.UtcNow,
            });
        }
        else
        {
            virtualFile.MimeType = Request.ContentType ?? System.Net.Mime.MediaTypeNames.Application.Octet;
            virtualFile.FileSize = ms.Length;
            virtualFile.FileContent = ms.ToArray();
            virtualFile.ModifiedDatetime = DateTime.UtcNow;
        }

        dbContext.SaveChanges();

        return TypedResults.Ok("TODO: PutFile");
    }

    [HttpDelete]
    [Route("/file/{**filePath}")]
    public async Task<IResult> DeleteAsync(string filePath)
    {
        var virtualFile = await dbContext.VirtualFiles.FirstOrDefaultAsync(r => r.VirtualPath == filePath);

        if (virtualFile != null)
            dbContext.VirtualFiles.Remove(virtualFile);

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
