using AutoMapper;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Config;
using TrainHelper.WebApi.Dto;
using static TrainHelper.WebApi.Dto.NlDetailsReportDto;

namespace TrainHelper.WebApi.Services;

public class ReportGeneratorService : IReportGeneratorService
{
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;
    private readonly ITrainDataProvider _trainDataProvider;

    public ReportGeneratorService(ITrainDataProvider trainDataProvider, IMapper mapper, IOptions<AppSettings> appConfig)
    {
        _trainDataProvider = trainDataProvider;
        _mapper = mapper;
        _appSettings = appConfig.Value;
    }

    /// <summary>
    /// Get NL details for report by train number
    /// </summary>
    /// <param name="trainNumber"></param>
    public async Task<NlDetailsReportDto?> GetNlDetailsReport(int trainNumber)
    {
        NlDetailsReportDto? result = null;
        var train = await _trainDataProvider.GetTrainDetail(trainNumber);
        if (train != null)
        {
            result = new NlDetailsReportDto
            {
                TrainNumber = train.Number,
                TailNumber = train.TrainIndexCombined.Split('-')[1],
                LastStation = train.Cars.FirstOrDefault()?.WayPoints[0].Station.StationName ?? string.Empty,
                WhenLastOperation = train.Cars.FirstOrDefault()?.WayPoints[0].OperationDate ?? DateTimeOffset.MinValue,
                Cars = train.Cars.Select(c => _mapper.Map<CarDto>(c)).OrderBy(c => c.PositionInTrain).ToList(),
                Subtotals = train.Cars.GroupBy(c => c.Freight.FreightEtsngName).Select(g =>
                    new NlDetailsReportSubtotal(g.Count(), g.Key, g.Sum(g => (double)g.FreightTotalWeightKg / 1000))).ToList()
            };
        }

        return result;
    }

    /// <summary>
    /// Get NL report as .xlsx file as byte array
    /// </summary>
    /// <param name="trainNumber"></param>
    public async Task<byte[]> GetNlDetailsReportXlsx(int trainNumber)
    {
        var reportData = await GetNlDetailsReport(trainNumber);
        Stream stream;
        if (reportData == null) return Array.Empty<byte>();
        try
        {
            stream = File.OpenRead(_appSettings.NlReportTemplate);
        }
        catch
        {
            return Array.Empty<byte>();
        }

        using var package = new ExcelPackage(stream);
        var sheet = package.Workbook.Worksheets[0];

        // Short details
        sheet.Cells["C3"].Value = reportData.TrainNumber;
        sheet.Cells["C4"].Value = reportData.TailNumber;
        sheet.Cells["E3"].Value = reportData.LastStation;
        sheet.Cells["E4"].Value = reportData.WhenLastOperation.Date.ToShortDateString();

        // Table
        var row = 7;
        foreach (var car in reportData.Cars)
        {
            sheet.Cells[$"A{row}"].Value = car.PositionInTrain;
            sheet.Cells[$"B{row}"].Value = car.CarNumber;
            sheet.Cells[$"C{row}"].Value = car.InvoiceNumber;
            sheet.Cells[$"D{row}"].Value = car.WhenLastOperation.Date.ToShortDateString();
            sheet.Cells[$"E{row}"].Value = car.FreightEtsngName;
            sheet.Cells[$"F{row}"].Value = car.FreightTotalWeightTn;
            sheet.Cells[$"G{row}"].Value = car.LastOperationName;
            row++;
        }

        // Subtotals
        foreach (var subtotal in reportData.Subtotals)
        {
            sheet.Cells[$"B{row}"].Value = subtotal.CarsCount;
            sheet.Cells[$"E{row}"].Value = subtotal.FreightEtsngName;
            sheet.Cells[$"F{row}"].Value = subtotal.FreightTotalWeightTn;
            row++;
        }

        // Totals
        sheet.Cells[$"A{row}"].Value = "Всего: " + reportData.Cars.Count;
        sheet.Cells[$"E{row}"].Value = reportData.Subtotals.Count;
        sheet.Cells[$"F{row}"].Value = reportData.Cars.Sum(c => c.FreightTotalWeightTn);

        // Table style
        sheet.Cells[$"A{row - reportData.Subtotals.Count}:G{row}" + row].Style.Font.Bold = true;
        sheet.Cells[$"A6:G{row}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        sheet.Cells[$"A6:G{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        sheet.Cells[$"A6:G{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        sheet.Cells[$"A6:G{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        sheet.Cells[$"A6:G{row}"].AutoFitColumns();

        return package.GetAsByteArray();
    }
}