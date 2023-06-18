using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UploadController : Controller
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService) => _uploadService = uploadService;
    /// <summary>
    /// Upload data from XML file to database
    /// </summary>
    /// <param name="file"></param>
    [HttpPost]
    public async Task<ActionResult> UploadData(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return new JsonResult(await _uploadService.UploadData(new UploadFileDto(file.FileName, stream)));
    }
}