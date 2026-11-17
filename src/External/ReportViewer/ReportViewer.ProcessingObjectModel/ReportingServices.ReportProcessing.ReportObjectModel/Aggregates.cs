using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Aggregates : MarshalByRefObject
	{
		public abstract object this[string key]
		{
			get;
		}
	}
}
