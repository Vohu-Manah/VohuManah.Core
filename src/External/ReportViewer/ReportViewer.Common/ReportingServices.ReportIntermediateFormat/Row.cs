using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal abstract class Row : IDOwner, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal abstract CellList Cells
		{
			get;
		}

		internal Row()
		{
		}

		internal Row(int id)
			: base(id)
		{
		}

		internal virtual void Initialize(InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row;
		}
	}
}
