using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeGaugePanelObj : RuntimeChartCriObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeGaugePanelObj()
		{
		}

		internal RuntimeGaugePanelObj(IReference<IScope> outerScope, GaugePanel gaugePanelDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess)
			: base(outerScope, gaugePanelDef, ref dataAction, odpContext, onePassProcess, ReportingServices.ReportProcessing.ObjectType.GaugePanel)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
