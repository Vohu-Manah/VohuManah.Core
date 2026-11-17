using ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.ObjectModel;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class LabelData
	{
		private readonly ReportingServices.ReportIntermediateFormat.LabelData m_labelData;

		private ReadOnlyCollection<string> m_keyFields;

		public string DataSetName => m_labelData.DataSetName;

		[Obsolete("Use KeyFields instead.")]
		public string Key => KeyFields[0];

		public ReadOnlyCollection<string> KeyFields
		{
			get
			{
				if (m_keyFields == null)
				{
					m_keyFields = new ReadOnlyCollection<string>(m_labelData.KeyFields);
				}
				return m_keyFields;
			}
		}

		public string Label => m_labelData.Label;

		internal LabelData(ReportingServices.ReportIntermediateFormat.LabelData labelData)
		{
			m_labelData = labelData;
		}
	}
}
