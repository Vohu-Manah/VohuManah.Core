using System.Security.Permissions;

namespace ReportingServices.Interfaces
{
	public interface IDeliveryExtension : IExtension
	{
		Setting[] ExtensionSettings
		{
			get;
		}

		IDeliveryReportServerInformation ReportServerInformation
		{
			set;
		}

		bool IsPrivilegedUser
		{
			set;
		}

		bool Deliver(Notification notification);

		Setting[] ValidateUserData(Setting[] settings);
	}
}
