using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixColumn : IDOwner, IPersistable
	{
		private string m_width;

		private double m_widthValue;

		[NonSerialized]
		private bool m_forAutoSubtotal;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal double WidthValue
		{
			get
			{
				return m_widthValue;
			}
			set
			{
				m_widthValue = value;
			}
		}

		internal bool ForAutoSubtotal
		{
			get
			{
				return m_forAutoSubtotal;
			}
			set
			{
				m_forAutoSubtotal = value;
			}
		}

		internal TablixColumn()
		{
		}

		internal TablixColumn(int id)
			: base(id)
		{
		}

		internal void Initialize(InitializationContext context)
		{
			m_widthValue = context.ValidateSize(m_width, "Width");
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixColumn tablixColumn = (TablixColumn)base.PublishClone(context);
			if (m_width != null)
			{
				tablixColumn.m_width = (string)m_width.Clone();
			}
			return tablixColumn;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Width, Token.String));
			list.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.WidthValue:
					writer.Write(m_widthValue);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Width:
					m_width = reader.ReadString();
					break;
				case MemberName.WidthValue:
					m_widthValue = reader.ReadDouble();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn;
		}
	}
}
