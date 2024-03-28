using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// add custom logging
ILogger logger = app.Services
                       .GetService<ILoggerFactory>()
                       .AddFile(builder.Configuration["Logging:LogFilePath"].ToString())
                       .CreateLogger<Program>();

app.UseStatusCodePages(async statusCodeContext 
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));

app.MapGet("/track", (string? trackingId) => string.IsNullOrEmpty(trackingId) ? Results.BadRequest() : Results.Redirect(GetTrackingURL(trackingId)));
app.MapGet("/notfound", (string? trackingId) => $"Unable to parse tracking number {trackingId} \n\nPlease enter a helpdesk ticket.");
string GetTrackingURL(string trackingId)
{
    string URL = string.Empty;
    trackingId = trackingId.Replace("-", "").Replace(" ", "").Trim();
    try
    {
        if (trackingId.ToUpper().StartsWith("1Z") || trackingId.Length == 9 || trackingId.ToUpper().StartsWith("K") || trackingId.ToUpper().StartsWith("H"))
        {
            URL = $"http://wwwapps.ups.com/WebTracking/processInputRequest?tracknum={trackingId}&AgreeToTermsAndConditions=yes";
        }
        else if (BigInteger.TryParse(trackingId, out _) && new List<int>(new[] { 10, 12, 15, 34, 20, 22 }).Contains(trackingId.Length))
        {
            URL = $"https://fedex.com/fedextrack/?tracknumbers={trackingId}";
        }
        else if (BigInteger.TryParse(trackingId, out _) && trackingId.Length == 22 && trackingId.StartsWith("94") || trackingId.ToUpper().EndsWith("US"))
        {
            URL = $"http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?origTrackNum={trackingId}";
        }
        else
        {
            URL = $"/notfound?trackingId={trackingId}";
        }

    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error when parsing trackingId {trackingId}");
    }
    logger.LogInformation($"{trackingId} -> {URL}");
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
