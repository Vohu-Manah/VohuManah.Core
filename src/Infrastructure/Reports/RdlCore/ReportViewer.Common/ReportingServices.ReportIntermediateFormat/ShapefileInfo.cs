using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ShapefileInfo : IPersistable
	{
		private string m_streamName;

		private bool m_errorOccurred;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string StreamName
		{
			get
			{
				return m_streamName;
			}
			set
			{
				m_streamName = value;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				return m_errorOccurred;
			}
			set
			{
				m_errorOccurred = value;
			}
		}

		internal ShapefileInfo()
		{
		}

		internal ShapefileInfo(string streamName)
		{
			m_streamName = streamName;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StreamName, Token.String));
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					writer.Write(m_streamName);
					break;
				case MemberName.ErrorOccurred:
					writer.Write(m_errorOccurred);
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
				case MemberName.StreamName:
					m_streamName = reader.ReadString();
					break;
				case MemberName.ErrorOccurred:
					m_errorOccurred = reader.ReadBoolean();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo;
		}
	}
}
