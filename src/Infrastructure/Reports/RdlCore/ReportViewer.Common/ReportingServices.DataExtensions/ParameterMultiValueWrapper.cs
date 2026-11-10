using ReportingServices.DataProcessing;
using System.Data;

namespace ReportingServices.DataExtensions
{
	internal class ParameterMultiValueWrapper : ParameterWrapper, IDataMultiValueParameter, ReportingServices.DataProcessing.IDataParameter
	{
		private object[] m_values;

		public virtual object[] Values
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values = value;
			}
		}

		public ParameterMultiValueWrapper(System.Data.IDataParameter param)
			: base(param)
		{
		}
	}
}
