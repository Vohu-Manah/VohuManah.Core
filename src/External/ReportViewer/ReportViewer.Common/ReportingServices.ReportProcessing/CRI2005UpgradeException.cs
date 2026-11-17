using System;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CRI2005UpgradeException : Exception
	{
		public CRI2005UpgradeException(string msg)
			: base(msg)
		{
		}

		public CRI2005UpgradeException()
			: base("2005 CRI needs to be processed by Yukon Engine")
		{
		}

		public CRI2005UpgradeException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}
}
