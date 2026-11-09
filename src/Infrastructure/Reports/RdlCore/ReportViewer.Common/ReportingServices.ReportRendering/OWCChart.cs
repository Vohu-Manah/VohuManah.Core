using ReportingServices.ReportProcessing;
using System.Drawing.Imaging;

namespace ReportingServices.ReportRendering
{
	internal sealed class OWCChart : DataRegion
	{
		private OWCChartColumnCollection m_chartData;

		public OWCChartColumnCollection ChartData
		{
			get
			{
				OWCChartColumnCollection oWCChartColumnCollection = m_chartData;
				if (m_chartData == null)
				{
					oWCChartColumnCollection = new OWCChartColumnCollection((ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef, (OWCChartInstance)base.ReportItemInstance, this);
					if (base.RenderingContext.CacheState)
					{
						m_chartData = oWCChartColumnCollection;
					}
				}
				return oWCChartColumnCollection;
			}
		}

		public string ChartDefinition => ((ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef).ChartDefinition;

		public override bool NoRows
		{
			get
			{
				int count = ((ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef).ChartData.Count;
				bool result = true;
				if (count > 0)
				{
					OWCChartInstanceInfo oWCChartInstanceInfo = (OWCChartInstanceInfo)base.InstanceInfo;
					for (int i = 0; i < count; i++)
					{
						if (0 < oWCChartInstanceInfo[i].Count)
						{
							result = false;
							break;
						}
					}
				}
				return result;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((OWCChartInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal OWCChart(int intUniqueName, ReportingServices.ReportProcessing.OWCChart reportItemDef, OWCChartInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public void ChartDataXML(IChartStream chartStream)
		{
			((OWCChartInstanceInfo)base.InstanceInfo).ChartDataXML(chartStream);
		}

		internal bool ProcessChartXMLPivotList(ref string newDefinition, string chartDataUrl)
		{
			return false;
		}

		public Metafile GetImage()
		{
			return null;
		}

		public byte[] GetChart()
		{
			return null;
		}
	}
}
