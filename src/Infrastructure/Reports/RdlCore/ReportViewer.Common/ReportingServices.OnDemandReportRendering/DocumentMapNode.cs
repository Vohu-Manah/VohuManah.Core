using ReportingServices.ReportProcessing;
using System;

namespace ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal sealed class DocumentMapNode : OnDemandDocumentMapNode
	{
		internal DocumentMapNode(string aLabel, string aId, int aLevel)
			: base(aLabel, aId, aLevel)
		{
		}
	}
}
