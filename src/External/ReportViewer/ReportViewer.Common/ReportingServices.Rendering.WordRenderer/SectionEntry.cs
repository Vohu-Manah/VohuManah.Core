using ReportingServices.Rendering.RPLProcessing;

namespace ReportingServices.Rendering.WordRenderer
{
	internal class SectionEntry
	{
		internal string SectionId;

		internal RPLItemMeasurement HeaderMeasurement;

		internal RPLItemMeasurement FooterMeasurement;

		public SectionEntry(RPLReportSection section)
		{
			SectionId = section.ID;
			HeaderMeasurement = section.Header;
			FooterMeasurement = section.Footer;
		}
	}
}
