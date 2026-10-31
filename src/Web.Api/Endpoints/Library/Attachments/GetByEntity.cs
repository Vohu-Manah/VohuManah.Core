using Application.Abstractions.Messaging;
using Application.Library.Attachments.GetByEntity;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Attachments;

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


