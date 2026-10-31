using SharedKernel;

namespace Application.Abstractions.HumanVerification;

public static class HumanVerifyErrors
{
    public static readonly Error InvalidOrExpired = Error.Failure(
        "HumanVerify.InvalidOrExpired",
        "چالش کهنه شده؛ با نگه داشتن دکمه، دوباره تلاش کنید");

    public static readonly Error TooFast = Error.Failure(
        "HumanVerify.TooFast",
        "بسیار زود رها کردید؛ نگه دارید و دوباره تلاش کنید");

    public static readonly Error RateLimited = Error.Failure(
        "HumanVerify.RateLimited",
        "بیش از اندازه درخواست فرستاده شد؛ کمی صبر کنید");

    public static readonly Error AutomationDetected = Error.Failure(
        "HumanVerify.AutomationDetected",
        "نشانههایی از فعالیت خودکار دیده شد؛ با نگه داشتن دکمه، دستی انجام دهید");
}
