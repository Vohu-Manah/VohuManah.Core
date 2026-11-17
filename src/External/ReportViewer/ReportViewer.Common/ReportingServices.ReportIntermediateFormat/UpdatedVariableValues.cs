using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class UpdatedVariableValues : IPersistable
	{
		private Dictionary<int, object> m_variableValues;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Dictionary<int, object> VariableValues
		{
			get
			{
				return m_variableValues;
			}
			set
			{
				m_variableValues = value;
			}
		}

		internal UpdatedVariableValues()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UpdatedVariableValues, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32SerializableDictionary, Token.Serializable));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.UpdatedVariableValues)
				{
					writer.Int32SerializableDictionary(m_variableValues);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.UpdatedVariableValues)
				{
					m_variableValues = reader.Int32SerializableDictionary();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues;
		}
	}
}
