namespace TrainHelper.WebApi.Config;

public class AppSettings
{
    public static readonly string SectionName = "AppSettings";

    public string NlReportTemplate { get; set; } = string.Empty;
}
