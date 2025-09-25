using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _db;

    public MediaController(IWebHostEnvironment env, ApplicationDbContext db)
    {
        _env = env;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var files = await _db.MediaFiles.ToListAsync();
        return Ok(files);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        string folder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string ext = Path.GetExtension(file.FileName);
        string type = ext == ".mp4" ? "video" : "image";
        string fileName = Guid.NewGuid() + ext;
        string filePath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

        var media = new MediaFile
        {
            FileName = file.FileName,
            FilePath = url,
            FileType = type
        };

        _db.MediaFiles.Add(media);
        await _db.SaveChangesAsync();

        return Ok(media);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        var file = await _db.MediaFiles.FindAsync(id);
        if (file == null) return NotFound();

        var localPath = Path.Combine(_env.WebRootPath, "uploads", Path.GetFileName(file.FilePath));
        if (System.IO.File.Exists(localPath))
        {
            System.IO.File.Delete(localPath);
        }

        _db.MediaFiles.Remove(file);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully!" });
    }
}
