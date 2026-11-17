using System;

namespace ReportingServices.ReportIntermediateFormat.Persistence
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	internal sealed class NonPersistentAttribute : Attribute
	{
	}
}
