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
	internal sealed class DataMember : ReportHierarchyNode
	{
		private DataMemberList m_dataMembers;

		[NonSerialized]
		private bool m_subtotal;

		[NonSerialized]
		private DataMember m_parentMember;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private DataGroupExprHost m_exprHost;

		internal override string RdlElementName => "DataMember";

		internal override HierarchyNodeList InnerHierarchy => m_dataMembers;

		internal DataMemberList SubMembers
		{
			get
			{
				return m_dataMembers;
			}
			set
			{
				m_dataMembers = value;
			}
		}

		internal DataGroupExprHost ExprHost => m_exprHost;

		internal DataMember ParentMember
		{
			get
			{
				return m_parentMember;
			}
			set
			{
				m_parentMember = value;
			}
		}

		internal bool Subtotal
		{
			get
			{
				return m_subtotal;
			}
			set
			{
				m_subtotal = value;
			}
		}

		internal DataMember()
		{
		}

		internal DataMember(int id, CustomReportItem crItem)
			: base(id, crItem)
		{
		}

		protected override void DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, m_isColumn);
		}

		protected override int DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, m_isColumn);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion, bool isSubtotal)
		{
			if (isSubtotal && m_grouping != null)
			{
				context.RegisterScopeName(m_grouping.Name);
			}
			DataMember dataMember = (DataMember)base.PublishClone(context, newContainingRegion, isSubtotal);
			if (m_dataMembers != null)
			{
				dataMember.m_dataMembers = new DataMemberList(m_dataMembers.Count);
				foreach (DataMember dataMember3 in m_dataMembers)
				{
					DataMember dataMember2 = (DataMember)dataMember3.PublishClone(context, newContainingRegion, isSubtotal);
					dataMember2.ParentMember = this;
					dataMember.m_dataMembers.Add(dataMember2);
				}
			}
			if (m_dataMembers == null && isSubtotal)
			{
				RowList rows = context.CurrentDataRegion.Rows;
				if (m_isColumn)
				{
					for (int i = 0; i < rows.Count; i++)
					{
						Cell value = (Cell)rows[i].Cells[context.CurrentIndex].PublishClone(context);
						context.CellLists[i].Add(value);
					}
				}
				else
				{
					context.Rows.Add((Row)rows[context.CurrentIndex].PublishClone(context));
				}
				context.CurrentIndex++;
			}
			return dataMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataMembers, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataMembers)
				{
					writer.Write(m_dataMembers);
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
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataMembers)
				{
					m_dataMembers = reader.ReadListOfRIFObjects<DataMemberList>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				m_exprHost = (DataGroupExprHost)memberExprHost;
				m_exprHost.SetReportObjectModel(reportObjectModel);
				MemberNodeSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
