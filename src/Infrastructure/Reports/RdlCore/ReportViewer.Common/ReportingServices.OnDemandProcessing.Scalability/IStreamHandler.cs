using System.IO;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStreamHandler
	{
		Stream OpenStream();
	}
}
