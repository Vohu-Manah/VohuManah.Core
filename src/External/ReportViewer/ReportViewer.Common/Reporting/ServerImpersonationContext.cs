using System;
using System.Security.Principal;

namespace Reporting
{
	internal sealed class ServerImpersonationContext : IDisposable
	{

		public ServerImpersonationContext(object /* WindowsIdentity */ userToImpersonate)
		{
		}

		public void Dispose()
		{
		}
	}
}
