using ReportingServices.Library;
using ReportingServices.ReportProcessing;
using System;
using System.Security.Permissions;

namespace Reporting
{
	[Serializable]
	internal class StoredReport : IDisposable
	{
		public ControlSnapshot Snapshot
		{
			get;
			private set;
		}

		public PublishingResult PublishingResult
		{
			get;
			private set;
		}

		public bool GeneratedExpressionHostWithRefusedPermissions
		{
			get;
			private set;
		}

		public StoredReport(PublishingResult publishingResult, ControlSnapshot snapshot, bool generatedExpressionHostWithRefusedPermissions)
		{
			PublishingResult = publishingResult;
			Snapshot = snapshot;
			GeneratedExpressionHostWithRefusedPermissions = generatedExpressionHostWithRefusedPermissions;
		}

		public void Dispose()
		{
			if (Snapshot != null)
			{
				Snapshot.Dispose();
				Snapshot = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}
