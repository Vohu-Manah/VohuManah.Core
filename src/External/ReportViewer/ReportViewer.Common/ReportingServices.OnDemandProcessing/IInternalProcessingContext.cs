using ReportingServices.ReportProcessing;
using System;

namespace ReportingServices.OnDemandProcessing
{
	internal interface IInternalProcessingContext
	{
		ErrorContext ErrorContext
		{
			get;
		}

		bool SnapshotProcessing
		{
			get;
			set;
		}

		DateTime ExecutionTime
		{
			get;
		}

		bool EnableDataBackedParameters
		{
			get;
		}
	}
}
