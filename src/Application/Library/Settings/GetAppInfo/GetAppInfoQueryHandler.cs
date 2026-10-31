using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetAppInfo;

internal sealed class GetAppInfoQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAppInfoQuery, AppInfoResponse>
{
    public async Task<Result<AppInfoResponse>> Handle(GetAppInfoQuery query, CancellationToken cancellationToken)
    {
        var appInfos = unitOfWork.GetDbSet<AppInfo>();

        var appInfo = await appInfos.FirstOrDefaultAsync(cancellationToken);

        if (appInfo == null)
        {
            return Result.Failure<AppInfoResponse>(Error.NotFound(
                "AppInfo.NotFound",
                "اطلاعات برنامه یافت نشد"));
        }

        return Result.Success(new AppInfoResponse
        {
            Id = appInfo.Id,
            AppName = appInfo.AppName,
            Version = appInfo.Version,
            Author = appInfo.Author,
            Description = appInfo.Description,
            LastUpdateDate = appInfo.LastUpdateDate,
            Copyright = appInfo.Copyright,
            License = appInfo.License,
            Website = appInfo.Website,
            SupportEmail = appInfo.SupportEmail,
        });
    }
}


