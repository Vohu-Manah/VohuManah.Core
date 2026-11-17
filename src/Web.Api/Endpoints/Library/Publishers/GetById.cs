using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publishers.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Publishers;

[RequireRole(EndpointNames.Publishers.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publishers/{id:int}", async (
            int id,
            IQueryHandler<GetPublisherByIdQuery, PublisherResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublisherByIdQuery(id);

            Result<PublisherResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

