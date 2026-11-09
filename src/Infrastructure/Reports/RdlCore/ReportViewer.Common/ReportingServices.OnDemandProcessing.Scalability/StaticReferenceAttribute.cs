using System;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class StaticReferenceAttribute : Attribute
	{
	}
}
