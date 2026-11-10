using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IPageBreakOwner
	{
		PageBreak PageBreak
		{
			get;
			set;
		}

		ObjectType ObjectType
		{
			get;
		}

		string ObjectName
		{
			get;
		}

		IInstancePath InstancePath
		{
			get;
		}
	}
}
