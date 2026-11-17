using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeGroupingObjDetailUserSort : RuntimeGroupingObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size;

		internal RuntimeGroupingObjDetailUserSort()
		{
		}

		internal RuntimeGroupingObjDetailUserSort(RuntimeHierarchyObj owner, ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void Cleanup()
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			Global.Tracer.Assert(condition: false, "This implementation of RuntimeGroupingObj does not support NextRow");
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = m_owner as RuntimeGroupRootObj;
			Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRootOwner)");
			runtimeGroupRootObj.GroupOrDetailSortTree.Traverse(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			Global.Tracer.Assert(condition: false, "Domain Scope should only be applied to Hash groups");
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
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjDetailUserSort;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjDetailUserSort, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
