using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UploadController : Controller
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService) => _uploadService = uploadService;

    [HttpPost]
    public async Task<ActionResult<UploadDataResultDto>> UploadData(IFormFile file) =>
         new(await _uploadService.UploadData(file));
}