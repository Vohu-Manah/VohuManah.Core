using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInfoWithDynamicImageMap : ActionInfo, IPersistable
	{
		private ImageMapAreaInstanceCollection m_imageMapAreas;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public ImageMapAreaInstanceCollection ImageMapAreaInstances
		{
			get
			{
				if (m_imageMapAreas == null)
				{
					m_imageMapAreas = new ImageMapAreaInstanceCollection();
				}
				return m_imageMapAreas;
			}
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportItem owner, IROMActionOwner romActionOwner)
			: this(renderingContext, new ReportingServices.ReportIntermediateFormat.Action(), owner, romActionOwner)
		{
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportItem owner, IReportScope reportScope, IInstancePath instancePath, IROMActionOwner romActionOwner, bool chartConstructor)
			: this(renderingContext, reportScope, new ReportingServices.ReportIntermediateFormat.Action(), instancePath, owner, romActionOwner)
		{
			m_chartConstruction = chartConstructor;
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportingServices.ReportIntermediateFormat.Action actionDef, ReportItem owner, IROMActionOwner romActionOwner)
			: this(renderingContext, owner.ReportScope, actionDef, owner.ReportItemDef, owner, romActionOwner)
		{
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, IReportScope reportScope, ReportingServices.ReportIntermediateFormat.Action actionDef, IInstancePath instancePath, ReportItem owner, IROMActionOwner romActionOwner)
			: base(renderingContext, reportScope, actionDef, instancePath, owner, owner.ReportItemDef.ObjectType, owner.ReportItemDef.Name, romActionOwner)
		{
			base.IsDynamic = true;
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportingServices.ReportRendering.ActionInfo renderAction, ImageMapAreasCollection renderImageMap)
			: base(renderingContext, renderAction)
		{
			base.IsDynamic = true;
			m_imageMapAreas = new ImageMapAreaInstanceCollection(renderImageMap);
		}

		public ImageMapAreaInstance CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates)
		{
			return CreateImageMapAreaInstance(shape, coordinates, null);
		}

		public ImageMapAreaInstance CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates, string toolTip)
		{
			if (!m_chartConstruction && base.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
			if (coordinates == null || coordinates.Length < 1)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "coordinates");
			}
			if (m_imageMapAreas == null)
			{
				m_imageMapAreas = new ImageMapAreaInstanceCollection();
			}
			return m_imageMapAreas.Add(shape, coordinates, toolTip);
		}

		[Obsolete("ActionInfoWithDynamicImageMap objects are completely volatile, so there is no reason to reuse the same instance of this class. Hence there is no need to support Update and SetNewContext methods.")]
		internal new void Update(ReportingServices.ReportRendering.ActionInfo newActionInfo)
		{
			Global.Tracer.Assert(condition: false, "Update(...) should not be called on ActionInfoWithDynamicImageMap");
		}

		// CS0809: base ActionInfo.SetNewContext() is deliberately not obsolete - other
		// subclasses legitimately support it. Only this always-volatile subclass disables
		// it (matching the Obsolete Update() override above, same rationale message),
		// which is intentional, not an oversight - suppressed rather than removed.
#pragma warning disable CS0809
		[Obsolete("ActionInfoWithDynamicImageMap objects are completely volatile, so there is no reason to reuse the same instance of this class. Hence there is no need to support Update and SetNewContext methods.")]
		internal override void SetNewContext()
		{
			Global.Tracer.Assert(condition: false, "SetNewContext() should not be called on ActionInfoWithDynamicImageMap");
		}
#pragma warning restore CS0809

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ActionDefinition:
					writer.Write(base.ActionDef);
					break;
				case MemberName.Actions:
				{
					ActionInstance[] array = new ActionInstance[base.Actions.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = base.Actions[i].Instance;
					}
					writer.Write(array);
					break;
				}
				case MemberName.ImageMapAreas:
					writer.WriteRIFList(ImageMapAreaInstances.InternalList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ActionDefinition:
					base.ActionDef = (ReportingServices.ReportIntermediateFormat.Action)reader.ReadRIFObject();
					break;
				case MemberName.Actions:
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartActionInfoInstancesDeserialization(this);
					reader.ReadArrayOfRIFObjects<ActionInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteActionInfoInstancesDeserialization();
					break;
				case MemberName.ImageMapAreas:
					m_imageMapAreas = new ImageMapAreaInstanceCollection();
					reader.ReadListOfRIFObjects(m_imageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap;
		}

		[SkipMemberStaticValidation(MemberName.ActionDefinition)]
		[SkipMemberStaticValidation(MemberName.Actions)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ActionDefinition, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Actions, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance));
			list.Add(new MemberInfo(MemberName.ImageMapAreas, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
