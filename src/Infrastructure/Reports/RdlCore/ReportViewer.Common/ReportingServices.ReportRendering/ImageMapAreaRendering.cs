using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportRendering
{
	internal sealed class ImageMapAreaRendering : MemberBase
	{
		internal ImageMapAreaInstance m_mapAreaInstance;

		internal RenderingContext m_renderingContext;

		internal ImageMapAreaRendering()
			: base(isCustomControl: false)
		{
		}
	}
}
