using System;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	internal abstract class CalculatedFieldWrapper : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}
