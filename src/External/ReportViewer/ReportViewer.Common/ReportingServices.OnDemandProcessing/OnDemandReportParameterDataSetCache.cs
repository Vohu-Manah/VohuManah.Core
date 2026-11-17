using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace ReportingServices.OnDemandProcessing
{
	internal class OnDemandReportParameterDataSetCache : ReportParameterDataSetCache
	{
		internal OnDemandReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, ReportingServices.ReportIntermediateFormat.ParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
			: base(aParamProcessor, aParameter, aParamDef, aProcessValidValues, aProcessDefaultValues)
		{
		}

		internal override object GetFieldValue(object aRow, int col)
		{
			FieldImpl[] array = (FieldImpl[])aRow;
			if (array[col].IsMissing)
			{
				return null;
			}
			return array[col].Value;
		}
	}
}
