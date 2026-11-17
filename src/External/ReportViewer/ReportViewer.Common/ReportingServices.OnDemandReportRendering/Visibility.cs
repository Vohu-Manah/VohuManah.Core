using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal abstract class Visibility
	{
		protected ReportBoolProperty m_startHidden;

		public abstract ReportBoolProperty Hidden
		{
			get;
		}

		public abstract string ToggleItem
		{
			get;
		}

		public abstract SharedHiddenState HiddenState
		{
			get;
		}

		public abstract bool RecursiveToggleReceiver
		{
			get;
		}

		internal static ReportBoolProperty GetStartHidden(ReportingServices.ReportProcessing.Visibility visibility)
		{
			if (visibility == null)
			{
				return new ReportBoolProperty();
			}
			return new ReportBoolProperty(visibility.Hidden);
		}

		internal static ReportBoolProperty GetStartHidden(ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			if (visibility == null)
			{
				return new ReportBoolProperty();
			}
			return new ReportBoolProperty(visibility.Hidden);
		}

		internal static SharedHiddenState GetHiddenState(ReportingServices.ReportProcessing.Visibility visibility)
		{
			return (SharedHiddenState)ReportingServices.ReportProcessing.Visibility.GetSharedHidden(visibility);
		}

		internal static SharedHiddenState GetHiddenState(ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			return ReportingServices.ReportIntermediateFormat.Visibility.GetSharedHidden(visibility);
		}
	}
}
