using Infrastructure.Reports;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.View)]
internal sealed class ViewReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("reports/view/{reportName}", async (
            string reportName,
            [FromQuery] string datasourceName,
            [FromQuery] string? parameters,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var emptyDataSource = Array.Empty<object>();
                var paramDict = ReportParameterParser.Parse(parameters);

                var htmlContent = await reportService.GenerateHtmlAsync(
                    reportName,
                    datasourceName,
                    emptyDataSource,
                    paramDict,
                    cancellationToken);

                return Results.Content(htmlContent, "text/html; charset=utf-8");
            }
            catch (FileNotFoundException ex)
            {
                return CustomResults.Problem(
                    title: "Ú¯Ø²Ø§Ø±Ø´ ÛŒØ§ÙØª Ù†Ø´Ø¯",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return CustomResults.Problem(
                    title: "خطا در تولید گزارش",
                    detail: ex.Message);
            }
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Reports");
    }
}

