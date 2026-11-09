using ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	internal class XmlPart : RelPart
	{
		private OoxmlPart _hydratedPart;

		public virtual OoxmlPart HydratedPart
		{
			get
			{
				return _hydratedPart;
			}
			set
			{
				_hydratedPart = value;
			}
		}
	}
}
