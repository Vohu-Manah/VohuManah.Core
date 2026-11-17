using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceNoPromptException : ReportCatalogException
	{
		public DataSourceNoPromptException(string dataSource)
			: base(ErrorCode.rsDataSourceNoPrompt, ErrorStrings.rsDataSourceNoPromptException(dataSource), null, null)
		{
		}
	}
}
