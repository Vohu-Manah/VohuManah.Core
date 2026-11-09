using ReportingServices.Diagnostics.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace ReportingServices.Diagnostics
{
	internal static class DataProtectionLocal
	{
		private sealed class DataProtectionLocalInstance : IDataProtection
		{
			public byte[] ProtectData(string unprotectedData, string tag)
			{
				if (unprotectedData == null)
				{
					return null;
				}
				return LocalProtectData(Encoding.Unicode.GetBytes(unprotectedData));
			}

			public string UnprotectDataToString(byte[] protectedData, string tag)
			{
				if (protectedData == null)
				{
					return null;
				}
				byte[] array = LocalUnprotectData(protectedData);
				if (array == null)
				{
					return null;
				}
				if (protectedData.Length == 0)
				{
					return string.Empty;
				}
				return Encoding.Unicode.GetString(array);
			}
		}

		private static IDataProtection m_dpInstance;

		// Vestigial from a real DPAPI-backed implementation (this local-only port's
		// LocalProtectData/LocalUnprotectData below are deliberate no-ops - "no need to
		// protect data for local reports"), so the requested mode has nothing left to
		// configure. No callers found in this repo; kept as a settable no-op rather than
		// removed outright since it's a public setter that external code could still call.
		public static ProtectionMode GlobalProtectionMode
		{
			set
			{
			}
		}

		public static IDataProtection Instance
		{
			[DebuggerStepThrough]
			get
			{
				if (m_dpInstance == null)
				{
					m_dpInstance = new DataProtectionLocalInstance();
				}
				return m_dpInstance;
			}
		}

		public static byte[] LocalProtectData(byte[] data)
		{
			// No need to protect data for local reports (no connection)
			return data;
		}

		public static byte[] LocalUnprotectData(byte[] data)
		{
			// No need to protect data for local reports (no connection)
			return data;
		}
	}
}
