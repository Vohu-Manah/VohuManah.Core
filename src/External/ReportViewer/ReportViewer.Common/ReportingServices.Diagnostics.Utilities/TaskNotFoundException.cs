using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class TaskNotFoundException : ReportCatalogException
	{
		public TaskNotFoundException(string taskID)
			: base(ErrorCode.rsTaskNotFound, ErrorStrings.rsTaskNotFound(taskID), null, null)
		{
		}
	}
}
