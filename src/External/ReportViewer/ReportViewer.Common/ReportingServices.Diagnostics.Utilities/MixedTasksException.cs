using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MixedTasksException : ReportCatalogException
	{
		public MixedTasksException()
			: base(ErrorCode.rsMixedTasks, ErrorStrings.rsMixedTasks, null, null)
		{
		}
	}
}
