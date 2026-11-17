using System.Security.Permissions;

namespace ReportingServices.Interfaces
{
	public interface IExtension
	{
		string LocalizedName
		{
			get;
		}

		void SetConfiguration(string configuration);
	}
}
