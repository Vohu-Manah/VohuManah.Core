using System;

namespace Reporting.Gauge.WebForms
{
	internal class RealTimeDataEventArgs : EventArgs
	{
		private RealTimeValueCollection realTimeValues;

		public RealTimeValueCollection RealTimeValues => realTimeValues;

		public RealTimeDataEventArgs()
		{
			realTimeValues = new RealTimeValueCollection();
		}
	}
}
