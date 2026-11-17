using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportRendering
{
	internal sealed class ActionRendering : MemberBase
	{
		internal ActionItem m_actionDef;

		internal ReportUrl m_actionURL;

		internal ActionItemInstance m_actionInstance;

		internal RenderingContext m_renderingContext;

		internal string m_drillthroughId;

		internal ActionRendering()
			: base(isCustomControl: false)
		{
		}
	}
}
