using System.Security.Permissions;

namespace ReportingServices.Interfaces
{
	public interface IWindowsAuthenticationExtension2 : IAuthenticationExtension2, IExtension
	{
		byte[] PrincipalNameToSid(string name);

		string SidToPrincipalName(byte[] sid);
	}
}
