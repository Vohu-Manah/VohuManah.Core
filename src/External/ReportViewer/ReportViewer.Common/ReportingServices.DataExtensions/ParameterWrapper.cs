using ReportingServices.DataProcessing;
using System.Data;

namespace ReportingServices.DataExtensions
{
	internal class ParameterWrapper : BaseDataWrapper, ReportingServices.DataProcessing.IDataParameter
	{
		public virtual string ParameterName
		{
			get
			{
				return UnderlyingParameter.ParameterName;
			}
			set
			{
				UnderlyingParameter.ParameterName = value;
			}
		}

		public virtual object Value
		{
			get
			{
				return UnderlyingParameter.Value;
			}
			set
			{
				UnderlyingParameter.Value = value;
			}
		}

		protected internal System.Data.IDataParameter UnderlyingParameter => (System.Data.IDataParameter)base.UnderlyingObject;

		protected internal ParameterWrapper(System.Data.IDataParameter param)
			: base(param)
		{
		}
	}
}
