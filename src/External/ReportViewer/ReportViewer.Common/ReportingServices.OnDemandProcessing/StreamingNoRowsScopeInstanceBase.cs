using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal abstract class StreamingNoRowsScopeInstanceBase : IOnDemandScopeInstance, IStorable, IPersistable
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly IRIFReportDataScope m_dataScope;

		public bool IsNoRows => true;

		public bool IsMostRecentlyCreatedScopeInstance => false;

		public bool HasUnProcessedServerAggregate => false;

		public int Size
		{
			get
			{
				Global.Tracer.Assert(condition: false, "Size may not be used on a no rows scope instance.");
				throw new InvalidOperationException();
			}
		}

		public StreamingNoRowsScopeInstanceBase(OnDemandProcessingContext odpContext, IRIFReportDataScope dataScope)
		{
			m_odpContext = odpContext;
			m_dataScope = dataScope;
		}

		public void SetupEnvironment()
		{
			m_odpContext.EnsureRuntimeEnvironmentForDataSet(m_dataScope.DataScopeInfo.DataSet, noRows: true);
			m_odpContext.ReportObjectModel.ResetFieldValues();
			m_dataScope.ResetAggregates(m_odpContext.ReportObjectModel.AggregatesImpl);
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			return null;
		}

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			return null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(condition: false, "Serialize may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(condition: false, "Deserialize may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "ResolveReferences may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public abstract ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
