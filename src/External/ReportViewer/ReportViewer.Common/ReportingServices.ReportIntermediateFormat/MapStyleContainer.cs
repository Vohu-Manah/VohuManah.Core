using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal abstract class MapStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected Map m_map;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath => m_map;

		ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType => ReportingServices.ReportProcessing.ObjectType.Map;

		string IStyleContainer.Name => m_map.Name;

		internal MapStyleContainer()
		{
		}

		internal MapStyleContainer(Map map)
		{
			m_map = map;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapStyleContainer mapStyleContainer = (MapStyleContainer)MemberwiseClone();
			mapStyleContainer.m_map = context.CurrentMapClone;
			if (m_styleClass != null)
			{
				mapStyleContainer.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			return mapStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Map, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
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
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Map)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer;
		}
	}
}
