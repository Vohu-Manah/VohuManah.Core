using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal class LookupMatches : IStorable, IPersistable, ITransferable
	{
		private long m_firstRowOffset = DataFieldRow.UnInitializedStreamOffset;

		private ScalableList<long> m_rowOffsets;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool HasRow => m_firstRowOffset != DataFieldRow.UnInitializedStreamOffset;

		internal int MatchCount
		{
			get
			{
				int num = 0;
				if (m_rowOffsets != null)
				{
					num = m_rowOffsets.Count;
				}
				if (HasRow)
				{
					num++;
				}
				return num;
			}
		}

		public virtual int Size => 8 + ItemSizes.SizeOf(m_rowOffsets);

		internal LookupMatches()
		{
		}

		internal virtual void AddRow(long rowOffset, int rowIndex, IScalabilityCache scaleCache)
		{
			if (HasRow)
			{
				if (m_rowOffsets == null)
				{
					m_rowOffsets = new ScalableList<long>(0, scaleCache, 500, 10);
				}
				m_rowOffsets.Add(rowOffset);
			}
			else
			{
				m_firstRowOffset = rowOffset;
			}
		}

		internal virtual void SetupRow(int matchIndex, OnDemandProcessingContext odpContext)
		{
			long unInitializedStreamOffset = DataFieldRow.UnInitializedStreamOffset;
			unInitializedStreamOffset = ((matchIndex != 0) ? m_rowOffsets[matchIndex - 1] : m_firstRowOffset);
			odpContext.ReportObjectModel.UpdateFieldValues(unInitializedStreamOffset);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRowOffset:
					writer.Write(m_firstRowOffset);
					break;
				case MemberName.RowOffsets:
					writer.Write(m_rowOffsets);
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
				case MemberName.FirstRowOffset:
					m_firstRowOffset = reader.ReadInt64();
					break;
				case MemberName.RowOffsets:
					m_rowOffsets = reader.ReadRIFObject<ScalableList<long>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRowOffset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RowOffsets, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Token.Int64));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}

		public virtual void TransferTo(IScalabilityCache scaleCache)
		{
			if (m_rowOffsets != null)
			{
				m_rowOffsets.TransferTo(scaleCache);
			}
		}
	}
}
