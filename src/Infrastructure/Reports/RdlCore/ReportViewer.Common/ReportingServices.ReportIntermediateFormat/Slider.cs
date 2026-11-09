using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Slider : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private bool m_hidden;

		private LabelData m_labelData;

		internal bool Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal LabelData LabelData
		{
			get
			{
				return m_labelData;
			}
			set
			{
				m_labelData = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context)
		{
			if (m_labelData != null)
			{
				m_labelData.Initialize(tablix, context);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, Token.Boolean));
			list.Add(new MemberInfo(MemberName.LabelData, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.LabelData:
					writer.Write(m_labelData);
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
				case MemberName.Hidden:
					m_hidden = reader.ReadBoolean();
					break;
				case MemberName.LabelData:
					m_labelData = reader.ReadRIFObject<LabelData>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider;
		}
	}
}
