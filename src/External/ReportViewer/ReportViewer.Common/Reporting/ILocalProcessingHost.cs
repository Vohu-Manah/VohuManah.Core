using ReportingServices.DataExtensions;
using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using ReportingServices;

namespace Reporting
{
	internal interface ILocalProcessingHost
	{
		PreviewItemContext ItemContext
		{
			get;
			set;
		}

		bool SupportsQueries
		{
			get;
		}

		ILocalCatalog Catalog
		{
			get;
		}

		LocalExecutionInfo ExecutionInfo
		{
			get;
		}

		bool ShowDetailedSubreportMessages
		{
			get;
			set;
		}

		LocalProcessingHostMapTileServerConfiguration MapTileServerConfiguration
		{
			get;
		}

		bool CanSelfCancel
		{
			get;
		}

		IDocumentMap GetDocumentMap();

		IEnumerable<LocalRenderingExtensionInfo> ListRenderingExtensions();

		void CompileReport();

		void ResetExecution();

		void SetReportParameters(NameValueCollection parameterValues);

		string[] GetDataSetNames(PreviewItemContext itemContext);

		DataSourcePromptCollection GetReportDataSourcePrompts(out bool allCredentialsSatisfied);

		void SetReportDataSourceCredentials(DatasourceCredentials[] credentials);

		ProcessingMessageList Render(string format, string deviceInfo, string paginationMode, bool allowInternalRenderers, IEnumerable dataSets, CreateAndRegisterStream createStreamCallback);

		byte[] RenderStream(string format, string deviceInfo, string streamId, out string mimeType);

		void PerformToggle(string toggleId);

		int PerformBookmarkNavigation(string bookmarkId, out string uniqueName);

		int PerformDocumentMapNavigation(string documentMapId);

		int PerformSort(string paginationMode, string sortId, SortOptions sortOptions, bool clearSort, out string uniqueName);

		int PerformSearch(int startPage, int endPage, string searchText);

		string PerformDrillthrough(string drillthroughId, out NameValueCollection parametersForDrillthroughReport);

		void ExecuteReportInCurrentAppDomain(Evidence evidence);

		void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName);

		void ExecuteReportInSandboxAppDomain();

		void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName);

		void ReleaseSandboxAppDomain();

		void CopySecuritySettingsFrom(ILocalProcessingHost sourceProcessingHost);

		void SetCancelState(bool shouldCancelRequests);
	}
}
