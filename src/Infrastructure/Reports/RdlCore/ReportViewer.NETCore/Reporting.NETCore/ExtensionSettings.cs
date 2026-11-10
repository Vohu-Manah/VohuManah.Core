using Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Reporting.NETCore
{
	internal sealed class ExtensionSettings
	{
		private readonly string m_extensionName;

		private readonly NameValueCollection m_extensionParameters;

		public NameValueCollection Settings => m_extensionParameters;

		public ExtensionSettings(string name, NameValueCollection extensionParameters)
		{
			m_extensionName = name;
			m_extensionParameters = extensionParameters;
		}

		public Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution.ExtensionSettings ConvertSettings()
		{
			Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution.ExtensionSettings extensionSettings = new Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution.ExtensionSettings();
			extensionSettings.Extension = m_extensionName;
			List<ParameterValueOrFieldReference> list = new List<ParameterValueOrFieldReference>();
			foreach (string key in m_extensionParameters.Keys)
			{
				ParameterValue parameterValue = new ParameterValue();
				parameterValue.Name = key;
				parameterValue.Value = m_extensionParameters[key];
				list.Add(parameterValue);
			}
			extensionSettings.ParameterValues = list.ToArray();
			return extensionSettings;
		}
	}
}
