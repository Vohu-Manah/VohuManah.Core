using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IVisibilityOwner : IReferenceable
	{
		Visibility Visibility
		{
			get;
		}

		IVisibilityOwner ContainingDynamicVisibility
		{
			get;
			set;
		}

		IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get;
			set;
		}

		IVisibilityOwner ContainingDynamicRowVisibility
		{
			get;
			set;
		}

		string SenderUniqueName
		{
			get;
		}

		bool ComputeHidden(RenderingContext renderingContext, ToggleCascadeDirection direction);

		bool ComputeDeepHidden(RenderingContext renderingContext, ToggleCascadeDirection direction);

		bool ComputeStartHidden(RenderingContext renderingContext);
	}
}
