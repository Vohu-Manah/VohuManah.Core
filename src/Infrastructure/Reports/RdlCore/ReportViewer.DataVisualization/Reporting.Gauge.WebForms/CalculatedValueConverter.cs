namespace Reporting.Gauge.WebForms
{
	internal class CalculatedValueConverter : CollectionItemTypeConverter
	{
		public CalculatedValueConverter()
		{
			simpleType = typeof(CalculatedValue);
		}
	}
}
