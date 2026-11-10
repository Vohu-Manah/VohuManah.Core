using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFDataScope
	{
		string Name
		{
			get;
		}

		DataScopeInfo DataScopeInfo
		{
			get;
		}

		ObjectType DataScopeObjectType
		{
			get;
		}
	}
}
