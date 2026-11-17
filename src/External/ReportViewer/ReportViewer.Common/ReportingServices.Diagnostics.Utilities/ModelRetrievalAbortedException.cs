using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelRetrievalAbortedException : ReportCatalogException
	{
		internal enum CancelationTrigger
		{
			None,
			ModelResolutionAfterConnectionOpen,
			ModelResolutionBeforeConnectionOpen,
			ModelResolutionDuringConnectionOpenException,
			ModelResolutionDuringConnectionOpenRSException,
			ModelResolutionDuringModelResolution,
			ModelResolutionDuringModelResolutionException,
			ModelResolutionDuringModelResolutionRSException,
			ServerDataSourceResolverAfterDataSourceResolution,
			ServerDataSourceResolverBeforeDataSourceResolution,
			ServerDataSourceResolverDuringModelResolution,
			ServerDataSourceResolverDuringModelResolutionException,
			ServerDataSourceResolverDuringModelResolutionRSException
		}

		private readonly CancelationTrigger m_cancelationTrigger;

		internal CancelationTrigger Trigger => m_cancelationTrigger;

		public ModelRetrievalAbortedException(CancelationTrigger cancelationTrigger)
			: base(ErrorCode.rsModelRetrievalCanceled, ErrorStrings.rsModelRetrievalCanceled, null, CreateAdditionalTraceMessage(cancelationTrigger))
		{
			m_cancelationTrigger = cancelationTrigger;
		}

		private static string CreateAdditionalTraceMessage(CancelationTrigger trigger)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", trigger.ToString());
		}
	}
}
