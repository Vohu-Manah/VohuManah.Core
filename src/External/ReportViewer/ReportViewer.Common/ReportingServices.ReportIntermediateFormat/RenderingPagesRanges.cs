using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal struct RenderingPagesRanges : IPersistable
	{
		private int m_startPage;

		private int m_endPage;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal int StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		internal int StartRow
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		internal int EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal int NumberOfDetails
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartPage, Token.Int32));
			list.Add(new MemberInfo(MemberName.EndPage, Token.Int32));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RenderingPagesRanges, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StartPage:
					writer.Write(m_startPage);
					break;
				case MemberName.EndPage:
					writer.Write(m_endPage);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StartPage:
					m_startPage = reader.ReadInt32();
					break;
				case MemberName.EndPage:
					m_endPage = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RenderingPagesRanges;
		}
	}
}
