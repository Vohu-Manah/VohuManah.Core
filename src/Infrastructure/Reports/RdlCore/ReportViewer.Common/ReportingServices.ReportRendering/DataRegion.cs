using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportRendering
{
	internal abstract class DataRegion : ReportItem
	{
		public virtual bool PageBreakAtEnd => ((ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtEnd;

		public virtual bool PageBreakAtStart => ((ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtStart;

		public virtual bool KeepTogether => ((ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).KeepTogether;

		public virtual bool NoRows => false;

		public string NoRowMessage
		{
			get
			{
				ExpressionInfo noRows = ((ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).NoRows;
				if (noRows != null)
				{
					if (ExpressionInfo.Types.Constant == noRows.Type)
					{
						return noRows.Value;
					}
					return InstanceInfoNoRowMessage;
				}
				return null;
			}
		}

		internal abstract string InstanceInfoNoRowMessage
		{
			get;
		}

		public string DataSetName => ((ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).DataSetName;

		internal DataRegion(int intUniqueName, ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext)
			: base(null, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public int[] GetRepeatSiblings()
		{
			ReportingServices.ReportProcessing.DataRegion dataRegion = (ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef;
			if (dataRegion.RepeatSiblings == null)
			{
				return new int[0];
			}
			int[] array = new int[dataRegion.RepeatSiblings.Count];
			dataRegion.RepeatSiblings.CopyTo(array);
			return array;
		}
	}
}
