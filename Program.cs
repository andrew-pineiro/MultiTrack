using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// add custom logging
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/track", (string trackingId) => Results.Redirect(GetTrackingURL(trackingId)));
app.MapGet("/", () => "Tracking Number Not Found.");
string GetTrackingURL(string trackingId)
{
    string URL = string.Empty;
    try
    {
        if (trackingId.StartsWith("1Z") || trackingId.Length == 9 || trackingId.StartsWith("K") || trackingId.StartsWith("H"))
        {
            URL = $"http://wwwapps.ups.com/WebTracking/processInputRequest?tracknum={trackingId}&AgreeToTermsAndConditions=yes";
        }
        else if (BigInteger.TryParse(trackingId, out _) && new List<int>(new[] { 10, 12, 15, 34, 20, 22 }).Contains(trackingId.Length) && new List<int>(new[] { 27, 96, 39, 64, 56, 78 }).Contains(Int32.Parse(trackingId.Substring(0, 2))))
        {
            URL = $"https://fedex.com/fedextrack/?tracknumbers={trackingId}";
        }
        else if (BigInteger.TryParse(trackingId, out _) && trackingId.Length == 22 && trackingId.StartsWith("94") || trackingId.EndsWith("US"))
        {
            URL = $"http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?origTrackNum={trackingId}";
        }
        else
        {
            URL = "/";
        }

    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.Message);
    }
    return URL;
}

try
{

    app.Run();

} catch (Exception ex)
{

    app.Logger.LogCritical(ex.Message);

} finally
{

    await app.StopAsync();

}
