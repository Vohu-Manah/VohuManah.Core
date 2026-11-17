using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class NavigationItem : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_reportItemReference;

		private BandNavigationCell m_bandNavigationCell;

		internal string ReportItemReference
		{
			get
			{
				return m_reportItemReference;
			}
			set
			{
				m_reportItemReference = value;
			}
		}

		internal BandNavigationCell BandNavigationCell
		{
			get
			{
				return m_bandNavigationCell;
			}
			set
			{
				m_bandNavigationCell = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context, string NavigationType)
		{
			if (ReportItemReference != null && !tablix.ValidateBandReportItemReference(ReportItemReference))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigationReference, Severity.Error, context.ObjectType, context.ObjectName, NavigationType, ReportItemReference);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportItemReference, Token.String));
			list.Add(new MemberInfo(MemberName.BandNavigationCell, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandNavigationCell));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportItemReference:
					writer.Write(m_reportItemReference);
					break;
				case MemberName.BandNavigationCell:
					writer.Write(m_bandNavigationCell);
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
				case MemberName.ReportItemReference:
					m_reportItemReference = reader.ReadString();
					break;
				case MemberName.BandNavigationCell:
					m_bandNavigationCell = (BandNavigationCell)reader.ReadRIFObject();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem;
		}
	}
}
