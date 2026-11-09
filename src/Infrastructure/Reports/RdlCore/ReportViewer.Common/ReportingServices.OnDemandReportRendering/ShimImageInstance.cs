using ReportingServices.ReportProcessing;
using ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimImageInstance : ImageInstance
	{
		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		public override byte[] ImageData
		{
			get
			{
				return ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).ImageData;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string StreamName
		{
			get
			{
				return ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).StreamName;
			}
			internal set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string MIMEType
		{
			get
			{
				return ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).MIMEType;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal override string ImageDataId => StreamName;

		public override TypeCode TagDataType => TypeCode.Empty;

		public override object Tag => null;

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (m_actionInfoImageMapAreas == null && ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).ImageMap != null && 0 < ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).ImageMap.Count)
				{
					m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection(m_reportElementDef.RenderingContext, ((ReportingServices.ReportRendering.Image)m_reportElementDef.RenderReportItem).ImageMap);
				}
				return m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage => false;

		internal ShimImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return null;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_actionInfoImageMapAreas = null;
		}
	}
}
