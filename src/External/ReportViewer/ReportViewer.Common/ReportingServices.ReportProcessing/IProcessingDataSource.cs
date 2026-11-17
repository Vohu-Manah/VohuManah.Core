using System;

namespace ReportingServices.ReportProcessing
{
	internal interface IProcessingDataSource
	{
		Guid ID
		{
			get;
		}

		string Name
		{
			get;
		}

		string Type
		{
			get;
			set;
		}

		string SharedDataSourceReferencePath
		{
			set;
		}

		string DataSourceReference
		{
			get;
		}

		bool IntegratedSecurity
		{
			get;
		}
	}
}
