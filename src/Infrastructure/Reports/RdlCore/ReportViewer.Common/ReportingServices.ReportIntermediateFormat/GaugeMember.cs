using ReportingServices.RdlExpressions;
using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugeMember : ReportHierarchyNode, IPersistable
	{
		private GaugeMemberList m_innerMembers;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override string RdlElementName => "GaugeMember";

		internal override HierarchyNodeList InnerHierarchy => m_innerMembers;

		internal GaugeMember ChildGaugeMember
		{
			get
			{
				if (m_innerMembers != null && m_innerMembers.Count > 0)
				{
					return m_innerMembers[0];
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					if (m_innerMembers == null)
					{
						m_innerMembers = new GaugeMemberList();
					}
					else
					{
						m_innerMembers.Clear();
					}
					m_innerMembers.Add(value);
				}
			}
		}

		internal GaugeMember()
		{
		}

		internal GaugeMember(int id, GaugePanel crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			m_isColumn = value;
			if (ChildGaugeMember != null)
			{
				ChildGaugeMember.SetIsCategoryMember(value);
			}
		}

		protected override void DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, m_isColumn);
		}

		protected override int DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, m_isColumn);
		}

		internal override bool Initialize(InitializationContext context)
		{
			if (!m_isColumn)
			{
				if (m_grouping != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowGaugeMemberCannotBeDynamic, Severity.Error, ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember", "Group", m_grouping.Name);
				}
				if (m_innerMembers != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowGaugeMemberCannotContainChildMember, Severity.Error, ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember");
				}
			}
			else if (m_innerMembers != null && m_innerMembers.OriginalNodeCount > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidColumnGaugeMemberCannotContainMultipleChildMember, Severity.Error, ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember");
			}
			return base.Initialize(context);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			GaugeMember gaugeMember = (GaugeMember)base.PublishClone(context, newContainingRegion);
			if (ChildGaugeMember != null)
			{
				gaugeMember.ChildGaugeMember = (GaugeMember)ChildGaugeMember.PublishClone(context, newContainingRegion);
			}
			return gaugeMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeMember, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.ColumnMembers, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ColumnMembers)
				{
					writer.Write(m_innerMembers);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeMember:
					ChildGaugeMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.ColumnMembers:
					m_innerMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
			MemberNodeSetExprHost(memberExprHost, reportObjectModel);
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
