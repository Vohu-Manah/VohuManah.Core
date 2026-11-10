using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.IO;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal interface IInterleave : IStorable, IPersistable
	{
		int Index
		{
			get;
		}

		long Location
		{
			get;
		}

		void Write(TextWriter output);
	}
}
