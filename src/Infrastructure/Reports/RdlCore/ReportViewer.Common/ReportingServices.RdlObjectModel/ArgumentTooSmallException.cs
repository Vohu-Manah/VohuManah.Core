using System;
using System.Runtime.Serialization;

namespace ReportingServices.RdlObjectModel
{
	[Serializable]
	internal class ArgumentTooSmallException : ArgumentConstraintException
	{
		public ArgumentTooSmallException(object component, string property, object value, object minimum)
			: base(component, property, value, minimum, SRErrors.InvalidParamGreaterThan(property, minimum))
		{
		}
	}
}
