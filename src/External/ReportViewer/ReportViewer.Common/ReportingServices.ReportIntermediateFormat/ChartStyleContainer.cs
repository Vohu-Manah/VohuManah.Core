using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal abstract class ChartStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected Chart m_chart;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		public virtual IInstancePath InstancePath => m_chart;

		public ReportingServices.ReportProcessing.ObjectType ObjectType => ReportingServices.ReportProcessing.ObjectType.Chart;

		public string Name => m_chart.Name;

		internal ChartStyleContainer()
		{
		}

		internal ChartStyleContainer(Chart chart)
		{
			m_chart = chart;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			ChartStyleContainer chartStyleContainer = (ChartStyleContainer)MemberwiseClone();
			chartStyleContainer.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_styleClass != null)
			{
				chartStyleContainer.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			return chartStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(null != exprHost && null != reportObjectModel)");
			exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Chart, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Chart)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer;
		}
	}
}
