using System;
using System.Runtime.Serialization;

namespace ReportingServices.RdlObjectModel
{
	[Serializable]
	internal class ArgumentTooLargeException : ArgumentConstraintException
	{
		public ArgumentTooLargeException(object component, string property, object value, object maximum)
			: base(component, property, value, maximum, SRErrors.InvalidParamLessThan(property, maximum))
		{
		}
	}
}
