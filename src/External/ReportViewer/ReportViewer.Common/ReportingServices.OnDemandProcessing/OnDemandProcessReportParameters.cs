using Cloud.Platform.Utils;
using ReportingServices.DataExtensions;
using ReportingServices.Diagnostics;
using ReportingServices.Diagnostics.Utilities;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandProcessReportParameters : ProcessReportParameters
	{
		private ReportingServices.ReportIntermediateFormat.Report m_report;

		internal OnDemandProcessReportParameters(ReportingServices.ReportIntermediateFormat.Report aReport, OnDemandProcessingContext aContext)
			: base(aContext)
		{
			m_report = aReport;
			if (aContext.IsRdlSandboxingEnabled())
			{
				IRdlSandboxConfig rdlSandboxing = aContext.Configuration.RdlSandboxing;
				m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
			}
		}

		internal OnDemandProcessingContext GetOnDemandContext()
		{
			return (OnDemandProcessingContext)base.ProcessingContext;
		}

		internal override IParameterDef GetParameterDef(int aParamIndex)
		{
			Global.Tracer.Assert(aParamIndex < m_report.Parameters.Count, "Invalid Parameter Index.  Found: {0}.  Count: {1}", aParamIndex, m_report.Parameters.Count);
			return m_report.Parameters[aParamIndex];
		}

		internal override void InitParametersContext(ParameterInfoCollection parameters)
		{
		}

		internal override void Cleanup()
		{
		}

		internal override void AddToRuntime(ParameterInfo aParamInfo)
		{
			ParameterImpl parameter = new ParameterImpl(aParamInfo);
			GetOnDemandContext().ReportObjectModel.ParametersImpl.Add(aParamInfo.Name, parameter);
		}

		internal override void SetupExprHost(IParameterDef aParamDef)
		{
			OnDemandProcessingContext onDemandContext = GetOnDemandContext();
			if (onDemandContext.ReportRuntime.ReportExprHost != null)
			{
				((ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef).SetExprHost(onDemandContext.ReportRuntime.ReportExprHost, onDemandContext.ReportObjectModel);
			}
		}

		internal override object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex)
		{
			ReportingServices.RdlExpressions.VariantResult variantResult = GetOnDemandContext().ReportRuntime.EvaluateParamDefaultValue((ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex)
		{
			ReportingServices.RdlExpressions.VariantResult variantResult = GetOnDemandContext().ReportRuntime.EvaluateParamValidValue((ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex)
		{
			ReportingServices.RdlExpressions.VariantResult variantResult = GetOnDemandContext().ReportRuntime.EvaluateParamValidValueLabel((ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef)
		{
			return GetOnDemandContext().ReportRuntime.EvaluateParamPrompt((ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef);
		}

		internal override bool NeedPrompt(IParameterDataSource paramDS)
		{
			bool result = false;
			ReportingServices.ReportIntermediateFormat.DataSource dataSource = m_report.DataSources[paramDS.DataSourceIndex];
			if (GetOnDemandContext().DataSourceInfos != null)
			{
				DataSourceInfo byID = GetOnDemandContext().DataSourceInfos.GetByID(dataSource.ID);
				if (byID != null)
				{
					result = byID.NeedPrompt;
				}
			}
			return result;
		}

		internal override void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName)
		{
			ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_report.DataSources[aDataSourceIndex].DataSets[aDataSetIndex];
			throw new ReportProcessingException(ErrorCode.rsReportParameterQueryProcessingError, aParamName.MarkAsPrivate(), propertyName, dataSet.Fields[aFieldIndex].Name.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate(), ReportingServices.ReportProcessing.ReportRuntime.GetErrorName(aError.Status, aError.Message));
		}

		internal override ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues)
		{
			ReportParameterDataSetCache reportParameterDataSetCache = new OnDemandReportParameterDataSetCache(this, aParam, (ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aRetrieveValidValues, aRetrievalDefaultValues);
			new RetrievalManager(m_report, GetOnDemandContext()).FetchParameterData(reportParameterDataSetCache, paramDS.DataSourceIndex, paramDS.DataSetIndex);
			return reportParameterDataSetCache;
		}

		protected override string ApplySandboxStringRestriction(string value, string paramName, string propertyName)
		{
			return ProcessReportParameters.ApplySandboxRestriction(ref value, paramName, propertyName, GetOnDemandContext(), m_maxStringResultLength);
		}
	}
}
