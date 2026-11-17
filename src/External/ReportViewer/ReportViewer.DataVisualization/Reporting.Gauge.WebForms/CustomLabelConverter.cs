namespace Reporting.Gauge.WebForms
{
	internal class CustomLabelConverter : CollectionItemTypeConverter
	{
		public CustomLabelConverter()
		{
			simpleType = typeof(CustomLabel);
		}
	}
}
