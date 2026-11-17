using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Attachments.GetByEntity;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Attachments;

[RequireRole(EndpointNames.Attachments.GetByEntity)]
internal sealed class GetByEntity : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/attachments/{entityType}/{entityId:long}", async (
            string entityType,
            long entityId,
            IQueryHandler<GetAttachmentsByEntityQuery, List<AttachmentResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttachmentsByEntityQuery(entityType, entityId);

            Result<List<AttachmentResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Attachments");
    }
}


