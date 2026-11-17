using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeChartCriObj : RuntimeDataTablixObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeChartCriObj()
		{
		}

		internal RuntimeChartCriObj(IReference<IScope> outerScope, ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataRegionDef, ref dataAction, odpContext, onePassProcess, objectType)
		{
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(condition: false, "RuntimeChartCriObj cannot have nested data regions");
			throw new InvalidOperationException();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
