using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class ReportController : Controller
{
    private const string ContentTypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string FileName = "data";
    private readonly IReportGeneratorService _reportGeneratorService;

    public ReportController(IReportGeneratorService reportGeneratorService) => _reportGeneratorService = reportGeneratorService;

    /// <summary>
    /// Get NL report by train number
    /// </summary>
    /// <param name="trainNumber"></param>
    [HttpGet]
    public async Task<ActionResult> GetNlDetail(int trainNumber)
    {
        var result = await _reportGeneratorService.GetNlDetailsReport(trainNumber);
        return result == null ? new NotFoundResult() : new JsonResult(result);
    }

    /// <summary>
    /// Get NL report by train number in .xlsx format
    /// </summary>
    /// <param name="trainNumber"></param>
    [HttpGet]
    public async Task<ActionResult> GetNlDetailXlsx(int trainNumber)
    {
        var result = await _reportGeneratorService.GetNlDetailsReportXlsx(trainNumber);
        return result.Length == 0 ? new NotFoundResult() : File(result, ContentTypeXlsx, FileName);
    }
}