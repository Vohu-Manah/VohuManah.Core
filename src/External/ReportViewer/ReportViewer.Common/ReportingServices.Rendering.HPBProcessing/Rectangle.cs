using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.Rendering.RPLProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Rectangle : PageItemContainer
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override byte RPLFormatType => 10;

		internal Rectangle()
		{
		}

		internal Rectangle(ReportingServices.OnDemandReportRendering.Rectangle source, PageContext pageContext)
			: base(source)
		{
			ReportingServices.OnDemandReportRendering.Rectangle rectangle = (ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			RectangleInstance rectangleInstance = (RectangleInstance)m_source.Instance;
			m_itemPageSizes = new ItemSizes(source);
			m_pageBreakProperties = PageBreakProperties.Create(rectangle.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				m_pageName = rectangleInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && rectangle.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.KeepTogetherVertical = source.KeepTogether;
			base.UnresolvedKTV = (base.UnresolvedKTH = source.KeepTogether);
			base.UnresolvedPBE = (base.UnresolvedPBS = true);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Rectangle;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Rectangle, ObjectType.PageItemContainer, memberInfoList);
			}
			return m_declaration;
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			CreateChildren(((ReportingServices.OnDemandReportRendering.Rectangle)m_source).ReportItemCollection, pageContext);
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((ReportingServices.OnDemandReportRendering.Rectangle)m_source).OmitBorderOnPageBreak)
			{
				OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLRectangle();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			return new RPLRectangle(props as RPLItemProps);
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			ReportingServices.OnDemandReportRendering.Rectangle rectangle = (ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = reportItemCollection[rectangle.LinkToChild];
				spbifWriter.Write((byte)43);
				spbifWriter.Write(reportItem.ID);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			ReportingServices.OnDemandReportRendering.Rectangle rectangle = (ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = reportItemCollection[rectangle.LinkToChild];
				((RPLRectanglePropsDef)sharedProps).LinkToChildId = reportItem.ID;
			}
		}
	}
}
