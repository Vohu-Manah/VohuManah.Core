using ReportingServices.ReportProcessing;

namespace ReportingServices.RdlExpressions
{
	internal struct BinaryResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal byte[] Value;
	}
}
