using ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartNoDataMessage : ChartTitle
	{
		internal ChartNoDataMessage()
		{
		}

		internal ChartNoDataMessage(Chart chart)
			: base(chart)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartNoDataMessageStart();
			InitializeInternal(context);
			context.ExprHostBuilder.ChartNoDataMessageEnd();
		}

		public override ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoDataMessage;
		}
	}
}
