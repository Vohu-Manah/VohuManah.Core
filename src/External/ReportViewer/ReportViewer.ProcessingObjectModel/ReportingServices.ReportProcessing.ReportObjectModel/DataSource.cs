using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSource : MarshalByRefObject
	{
		public abstract string DataSourceReference
		{
			get;
		}

		public abstract string Type
		{
			get;
		}
	}
}
