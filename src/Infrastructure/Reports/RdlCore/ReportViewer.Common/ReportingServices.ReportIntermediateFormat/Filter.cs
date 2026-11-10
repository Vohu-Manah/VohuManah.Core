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
	internal sealed class Filter : IPersistable
	{
		internal enum Operators
		{
			Equal,
			Like,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			TopN,
			BottomN,
			TopPercent,
			BottomPercent,
			In,
			Between,
			NotEqual
		}

		private ExpressionInfo m_expression;

		private Operators m_operator;

		private List<ExpressionInfoTypeValuePair> m_values;

		private int m_exprHostID = -1;

		[NonSerialized]
		private FilterExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo Expression
		{
			get
			{
				return m_expression;
			}
			set
			{
				m_expression = value;
			}
		}

		internal Operators Operator
		{
			get
			{
				return m_operator;
			}
			set
			{
				m_operator = value;
			}
		}

		internal List<ExpressionInfoTypeValuePair> Values
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal FilterExprHost ExprHost => m_exprHost;

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FilterStart();
			if (m_expression != null)
			{
				m_expression.Initialize("FilterExpression", context);
				context.ExprHostBuilder.FilterExpression(m_expression);
			}
			if (m_values != null)
			{
				for (int i = 0; i < m_values.Count; i++)
				{
					ExpressionInfo value = m_values[i].Value;
					Global.Tracer.Assert(value != null, "(expression != null)");
					value.Initialize("FilterValue", context);
					context.ExprHostBuilder.FilterValue(value);
				}
			}
			m_exprHostID = context.ExprHostBuilder.FilterEnd();
		}

		internal void SetExprHost(IList<FilterExprHost> filterHosts, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(filterHosts != null && reportObjectModel != null, "(filterHosts != null && reportObjectModel != null)");
				m_exprHost = filterHosts[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Filter filter = (Filter)MemberwiseClone();
			if (m_expression != null)
			{
				filter.m_expression = (ExpressionInfo)m_expression.PublishClone(context);
			}
			if (m_values != null)
			{
				filter.m_values = new List<ExpressionInfoTypeValuePair>(m_values.Count);
				{
					foreach (ExpressionInfoTypeValuePair value in m_values)
					{
						filter.m_values.Add(new ExpressionInfoTypeValuePair(value.DataType, value.HadExplicitDataType, (ExpressionInfo)value.Value.PublishClone(context)));
					}
					return filter;
				}
			}
			return filter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Expression, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Operator, Token.Enum));
			list.Add(new MemberInfo(MemberName.Values, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Expression:
					writer.Write(m_expression);
					break;
				case MemberName.Operator:
					writer.WriteEnum((int)m_operator);
					break;
				case MemberName.Values:
					writer.Write(m_values);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.Expression:
					m_expression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Operator:
					m_operator = (Operators)reader.ReadEnum();
					break;
				case MemberName.Values:
					m_values = reader.ReadGenericListOfRIFObjects<ExpressionInfoTypeValuePair>();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter;
		}
	}
}
