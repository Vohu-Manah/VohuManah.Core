using ReportingServices.Common;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IDynamicInstance
	{
		void ResetContext();

		bool MoveNext();

		int GetInstanceIndex();

		bool SetInstanceIndex(int index);

		ScopeID GetScopeID();

		void SetScopeID(ScopeID scopeID);
	}
}
