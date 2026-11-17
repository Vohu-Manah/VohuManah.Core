using System.ComponentModel;

namespace Reporting.Map.WebForms
{
	internal class LegendConverter : NoNameExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null && context.Instance is MapControl)
			{
				MapControl.controlCurrentContext = context;
			}
			return base.GetPropertiesSupported(context);
		}
	}
}
