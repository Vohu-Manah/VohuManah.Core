using ReportingServices.DataProcessing;

namespace ReportingServices.DataExtensions
{
	internal interface ITokenDataExtension2 : ITokenDataExtension
	{
		bool UseTokenAuthentication
		{
			get;
		}
	}
}
