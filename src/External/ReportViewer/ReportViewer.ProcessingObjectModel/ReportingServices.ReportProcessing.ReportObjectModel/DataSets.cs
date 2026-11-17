using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSets : MarshalByRefObject
	{
		public abstract DataSet this[string key]
		{
			get;
		}
	}
}
