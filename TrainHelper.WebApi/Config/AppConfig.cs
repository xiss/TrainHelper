using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TrainHelper.WebApi.Config;

public class AppConfig
{
    public static readonly string SectionName = "App";
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int LifeTime { get; set; }
    public string NlReportTemplate { get; set; } = string.Empty;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}