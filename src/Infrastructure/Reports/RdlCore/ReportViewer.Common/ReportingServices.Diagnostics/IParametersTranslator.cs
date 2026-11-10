using System.Collections.Specialized;

namespace ReportingServices.Diagnostics
{
	internal interface IParametersTranslator
	{
		void GetParamsInstance(string paramsInstanceId, out ExternalItemPath itemPath, out NameValueCollection parameters);
	}
}
