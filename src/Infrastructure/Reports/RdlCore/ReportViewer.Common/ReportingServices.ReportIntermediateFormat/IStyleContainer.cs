using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IStyleContainer
	{
		Style StyleClass
		{
			get;
		}

		IInstancePath InstancePath
		{
			get;
		}

		ObjectType ObjectType
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}
