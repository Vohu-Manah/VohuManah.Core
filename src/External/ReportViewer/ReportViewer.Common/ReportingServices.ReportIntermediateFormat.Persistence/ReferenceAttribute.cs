using System;

namespace ReportingServices.ReportIntermediateFormat.Persistence
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class ReferenceAttribute : Attribute
	{
	}
}
