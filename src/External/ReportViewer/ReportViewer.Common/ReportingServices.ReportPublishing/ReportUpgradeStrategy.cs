using System.IO;

namespace ReportingServices.ReportPublishing
{
	internal abstract class ReportUpgradeStrategy
	{
		internal abstract Stream Upgrade(Stream definitionStream);
	}
}
