using Cloud.Platform.Utils;
using ReportingServices.Diagnostics;
using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Diagnostics;
using System.Threading;

namespace ReportingServices.ReportProcessing.Execution
{
	internal abstract class ProcessReportOdp
	{
		private readonly IConfiguration m_configuration;

		private readonly ProcessingContext m_publicProcessingContext;

		private readonly ReportingServices.ReportIntermediateFormat.Report m_reportDefinition;

		private readonly ReportProcessing.StoreServerParameters m_storeServerParameters;

		private readonly GlobalIDOwnerCollection m_globalIDOwnerCollection;

		private readonly ExecutionLogContext m_executionLogContext;

		private readonly ErrorContext m_errorContext;

		protected abstract bool SnapshotProcessing
		{
			get;
		}

		protected abstract bool ReprocessSnapshot
		{
			get;
		}

		protected abstract bool ProcessWithCachedData
		{
			get;
		}

		protected abstract OnDemandProcessingContext.Mode OnDemandProcessingMode
		{
			get;
		}

		protected IConfiguration Configuration => m_configuration;

		protected ProcessingContext PublicProcessingContext => m_publicProcessingContext;

		protected ReportingServices.ReportIntermediateFormat.Report ReportDefinition => m_reportDefinition;

		protected GlobalIDOwnerCollection GlobalIDOwnerCollection => m_globalIDOwnerCollection;

		protected ErrorContext ErrorContext => m_errorContext;

		protected ReportProcessing.StoreServerParameters StoreServerParameters => m_storeServerParameters;

		protected ExecutionLogContext ExecutionLogContext => m_executionLogContext;

		public ProcessReportOdp(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext)
		{
			m_configuration = configuration;
			m_publicProcessingContext = pc;
			m_reportDefinition = report;
			m_errorContext = errorContext;
			m_storeServerParameters = storeServerParameters;
			m_globalIDOwnerCollection = globalIDOwnerCollection;
			m_executionLogContext = executionLogContext;
		}

		public ReportingServices.ReportIntermediateFormat.ReportSnapshot Execute(out OnDemandProcessingContext odpContext)
		{
			ReportProcessingCompatibilityVersion.TraceCompatibilityVersion(m_configuration);
			odpContext = null;
			OnDemandMetadata onDemandMetadata = PrepareMetadata();
			onDemandMetadata.GlobalIDOwnerCollection = m_globalIDOwnerCollection;
			ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = onDemandMetadata.ReportSnapshot;
			Global.Tracer.Assert(reportSnapshot != null, "ReportSnapshot object must exist");
			try
			{
				UserProfileState userProfileState = UserProfileState.None;
				if (PublicProcessingContext.Parameters != null)
				{
					userProfileState |= PublicProcessingContext.Parameters.UserProfileState;
				}
				odpContext = CreateOnDemandContext(onDemandMetadata, reportSnapshot, userProfileState);
				CompleteOdpContext(odpContext);
				Merge odpMerge;
				ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = CreateReportInstance(odpContext, onDemandMetadata, reportSnapshot, out odpMerge);
				PreProcessSnapshot(odpContext, odpMerge, reportInstance, reportSnapshot);
				odpContext.SnapshotProcessing = true;
				odpContext.IsUnrestrictedRenderFormatReferenceMode = true;
				ResetEnvironment(odpContext, reportInstance);
				if (odpContext.ThreadCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = odpContext.ThreadCulture;
				}
				UpdateUserProfileLocation(odpContext);
				return reportSnapshot;
			}
			finally
			{
				CleanupAbortHandler(odpContext);
				if (odpContext != null && odpContext.GlobalDataSourceInfo != null && odpContext.GlobalDataSourceInfo.Values != null)
				{
					foreach (ReportProcessing.DataSourceInfo value in odpContext.GlobalDataSourceInfo.Values)
					{
						if (value.TransactionInfo != null)
						{
							if (value.TransactionInfo.RollbackRequired)
							{
								if (Global.Tracer.TraceInfo)
								{
									Global.Tracer.Trace(TraceLevel.Info, "Data source '{0}': Rolling back transaction.", value.DataSourceName.MarkAsModelInfo());
								}
								try
								{
									value.TransactionInfo.Transaction.Rollback();
								}
								catch (Exception innerException)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, value.DataSourceName.MarkAsModelInfo());
								}
							}
							else
							{
								if (Global.Tracer.TraceVerbose)
								{
									Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", value.DataSourceName.MarkAsModelInfo());
								}
								try
								{
									value.TransactionInfo.Transaction.Commit();
								}
								catch (Exception innerException2)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException2, value.DataSourceName.MarkAsModelInfo());
								}
							}
						}
						if (value.Connection != null)
						{
							try
							{
								odpContext.CreateAndSetupDataExtensionFunction.CloseConnection(value.Connection, value.ProcDataSourceInfo, value.DataExtDataSourceInfo);
							}
							catch (Exception innerException3)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, value.DataSourceName.MarkAsModelInfo());
							}
						}
					}
				}
			}
		}

		protected virtual void UpdateUserProfileLocation(OnDemandProcessingContext odpContext)
		{
			odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
		}

		protected virtual void CleanupAbortHandler(OnDemandProcessingContext odpContext)
		{
			odpContext?.UnregisterAbortInfo();
		}

		protected virtual OnDemandProcessingContext CreateOnDemandContext(OnDemandMetadata odpMetadata, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, UserProfileState initialUserDependency)
		{
			return new OnDemandProcessingContext(PublicProcessingContext, ReportDefinition, odpMetadata, m_errorContext, reportSnapshot.ExecutionTime, SnapshotProcessing, ReprocessSnapshot, ProcessWithCachedData, m_storeServerParameters, initialUserDependency, m_executionLogContext, Configuration, OnDemandProcessingMode, GetAbortHelper());
		}

		protected virtual IAbortHelper GetAbortHelper()
		{
			if (PublicProcessingContext.JobContext == null)
			{
				return null;
			}
			return PublicProcessingContext.JobContext.GetAbortHelper();
		}

		protected virtual void ResetEnvironment(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpContext.SetupEnvironment(reportInstance);
			odpContext.ReportObjectModel.AggregatesImpl.ResetAll();
		}

		protected virtual ReportingServices.ReportIntermediateFormat.ReportInstance CreateReportInstance(OnDemandProcessingContext odpContext, OnDemandMetadata odpMetadata, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, out Merge odpMerge)
		{
			odpMerge = new Merge(ReportDefinition, odpContext);
			ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureGroupTreeStorageSetup(odpMetadata, odpContext.ChunkFactory, odpMetadata.GlobalIDOwnerCollection, openExisting: false, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
			ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance2 = odpContext.CurrentReportInstance = odpMerge.PrepareReportInstance(odpMetadata);
			ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance4 = reportSnapshot.ReportInstance = reportInstance2;
			ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance5 = reportInstance4;
			odpMerge.Init(PublicProcessingContext.Parameters);
			SetupReportLanguage(odpMerge, reportInstance5);
			odpMerge.SetupReport(reportInstance5);
			return reportInstance5;
		}

		protected abstract void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot);

		protected abstract OnDemandMetadata PrepareMetadata();

		protected virtual void CompleteOdpContext(OnDemandProcessingContext odpContext)
		{
		}

		protected abstract void SetupReportLanguage(Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance);

		protected void SetupInitialOdpState(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			reportSnapshot.HasUserSortFilter = ReportDefinition.ReportOrDescendentHasUserSortFilter;
			odpContext.SetupEnvironment(reportInstance);
		}
	}
}
