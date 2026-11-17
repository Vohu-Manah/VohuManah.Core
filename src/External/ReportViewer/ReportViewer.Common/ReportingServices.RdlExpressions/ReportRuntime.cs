using ReportingServices.Common;
using ReportingServices.Diagnostics;
using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportProcessing.ReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace ReportingServices.RdlExpressions
{
	internal sealed class ReportRuntime : IErrorContext, IStaticReferenceable
	{
		private delegate object EvalulateDataPoint(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint);

		internal sealed class TextRunExprHostWrapper : TextRunExprHost
		{
			private TextBoxExprHost m_textBoxExprHost;

			public override object ValueExpr => m_textBoxExprHost.ValueExpr;

			internal TextRunExprHostWrapper(TextBoxExprHost textBoxExprHost)
			{
				m_textBoxExprHost = textBoxExprHost;
			}
		}

		private enum NormalizationCode
		{
			Success,
			InvalidType,
			StringLengthViolation,
			ArrayLengthViolation
		}

		private sealed class ExpressionHostLoader : MarshalByRefObject
		{
			private static readonly Hashtable ExpressionHosts = new Hashtable();

			private const string ExprHostRootType = "ReportExprHostImpl";

			private static readonly byte[] ReportExpressionsDefaultEvidencePK = new byte[160]
			{
				0,
				36,
				0,
				0,
				4,
				128,
				0,
				0,
				148,
				0,
				0,
				0,
				6,
				2,
				0,
				0,
				0,
				36,
				0,
				0,
				82,
				83,
				65,
				49,
				0,
				4,
				0,
				0,
				1,
				0,
				1,
				0,
				81,
				44,
				142,
				135,
				46,
				40,
				86,
				158,
				115,
				59,
				203,
				18,
				55,
				148,
				218,
				181,
				81,
				17,
				160,
				87,
				11,
				59,
				61,
				77,
				227,
				121,
				65,
				83,
				222,
				165,
				239,
				183,
				195,
				254,
				169,
				242,
				216,
				35,
				108,
				255,
				50,
				12,
				79,
				208,
				234,
				213,
				246,
				119,
				136,
				11,
				246,
				193,
				129,
				242,
				150,
				199,
				81,
				197,
				246,
				230,
				91,
				4,
				211,
				131,
				76,
				2,
				247,
				146,
				254,
				224,
				254,
				69,
				41,
				21,
				212,
				74,
				254,
				116,
				160,
				194,
				126,
				13,
				142,
				75,
				141,
				4,
				236,
				82,
				168,
				226,
				129,
				224,
				31,
				244,
				126,
				125,
				105,
				78,
				108,
				114,
				117,
				160,
				154,
				252,
				191,
				216,
				204,
				130,
				112,
				90,
				6,
				178,
				15,
				214,
				239,
				97,
				235,
				186,
				104,
				115,
				226,
				156,
				140,
				15,
				44,
				174,
				221,
				162
			};

			internal static ReportExprHost LoadExprHost(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				Type expressionHostLoaderType = typeof(ExpressionHostLoader);
				ExpressionHostLoader remoteEHL = new ExpressionHostLoader();
				return remoteEHL.LoadExprHostRemoteEntryPoint(exprHostBytes, exprHostAssemblyName, includeParameters, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}

			internal static ReportExprHost LoadExprHostIntoCurrentAppDomain(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				if (codeModules != null && 0 < codeModules.Count)
				{
					RevertImpersonationContext.RunFromRestrictedCasContext(delegate
					{
						for (int num = codeModules.Count - 1; num >= 0; num--)
						{
							Assembly.Load(codeModules[num]);
						}
					});
				}
				return (ReportExprHost)LoadExprHostAssembly(exprHostBytes, exprHostAssemblyName, evidence, assemblyLoadContext).GetType("ReportExprHostImpl").GetConstructors()[0].Invoke(new object[3]
				{
					includeParameters,
					parametersOnly,
					objectModel
				});
			}

			private static Assembly LoadExprHostAssembly(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, AssemblyLoadContext assemblyLoadContext)
			{
				lock (ExpressionHosts.SyncRoot)
				{
					Assembly assembly = (Assembly)ExpressionHosts[exprHostAssemblyName];
					if (assembly == null)
					{
						if (evidence == null)
						{
							evidence = CreateDefaultExpressionHostEvidence(exprHostAssemblyName);
						}
						if (assemblyLoadContext == null)
						{
							assembly = Assembly.Load(exprHostBytes);
							ExpressionHosts.Add(exprHostAssemblyName, assembly);
						}
						else
						{
							assembly = assemblyLoadContext.LoadFromStream(new MemoryStream(exprHostBytes));
						}
					}
					return assembly;
				}
			}

			private static Evidence CreateDefaultExpressionHostEvidence(string exprHostAssemblyName)
			{
				Evidence evidence = new Evidence();
				return evidence;
			}

			private ReportExprHost LoadExprHostRemoteEntryPoint(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				return LoadExprHostIntoCurrentAppDomain(exprHostBytes, exprHostAssemblyName, null, includeParameters, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}
		}

		private bool m_exprHostInSandboxAppDomain;

		private ReportExprHost m_reportExprHost;

		private ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl m_reportObjectModel;

		private ErrorContext m_errorContext;

		private IErrorContext m_delayedErrorContext;

		private bool m_contextUpdated;

		private IScope m_currentScope;

		private bool m_variableReferenceMode;

		private bool m_unfulfilledDependency;

		private ReportRuntime m_topLevelReportRuntime;

		private List<string> m_fieldsUsedInCurrentActionOwnerValue;

		private int m_id = int.MinValue;

		private int m_maxStringResultLength = -1;

		private int m_maxArrayResultLength = -1;

		private bool m_rdlSandboxingEnabled;

		private bool m_isSerializableValuesProhibited;

		private const int UnrestrictedStringResultLength = -1;

		private const int UnrestrictedArrayResultLength = -1;

		internal ReportExprHost ReportExprHost => m_reportExprHost;

		internal bool VariableReferenceMode
		{
			get
			{
				return m_variableReferenceMode;
			}
			set
			{
				m_variableReferenceMode = value;
			}
		}

		internal bool UnfulfilledDependency
		{
			get
			{
				return m_unfulfilledDependency;
			}
			set
			{
				m_unfulfilledDependency = value;
			}
		}

		internal bool ContextUpdated
		{
			get
			{
				return m_contextUpdated;
			}
			set
			{
				m_contextUpdated = value;
			}
		}

		internal IScope CurrentScope
		{
			get
			{
				return m_currentScope;
			}
			set
			{
				m_currentScope = value;
			}
		}

		internal ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return m_objectName;
			}
			set
			{
				m_objectName = value;
			}
		}

		internal string PropertyName
		{
			get
			{
				return m_propertyName;
			}
			set
			{
				m_propertyName = value;
			}
		}

		internal ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl ReportObjectModel => m_reportObjectModel;

		internal ErrorContext RuntimeErrorContext => m_errorContext;

		internal List<string> FieldsUsedInCurrentActionOwnerValue
		{
			get
			{
				return m_fieldsUsedInCurrentActionOwnerValue;
			}
			set
			{
				m_fieldsUsedInCurrentActionOwnerValue = value;
			}
		}

		public int ID => m_id;

		internal ReportRuntime(ReportingServices.ReportProcessing.ObjectType objectType, ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext)
		{
			m_objectType = objectType;
			m_reportObjectModel = reportObjectModel;
			m_errorContext = errorContext;
			if (reportObjectModel.OdpContext.IsRdlSandboxingEnabled())
			{
				m_rdlSandboxingEnabled = true;
				IRdlSandboxConfig rdlSandboxing = reportObjectModel.OdpContext.Configuration.RdlSandboxing;
				m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
				m_maxArrayResultLength = rdlSandboxing.MaxArrayResultLength;
			}
			if (reportObjectModel.OdpContext.Configuration != null)
			{
				m_isSerializableValuesProhibited = reportObjectModel.OdpContext.Configuration.ProhibitSerializableValues;
			}
		}

		internal ReportRuntime(ReportingServices.ReportProcessing.ObjectType objectType, ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext, ReportExprHost copyReportExprHost, ReportRuntime topLevelReportRuntime)
			: this(objectType, reportObjectModel, errorContext)
		{
			m_reportExprHost = copyReportExprHost;
			m_topLevelReportRuntime = topLevelReportRuntime;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (m_delayedErrorContext == null)
			{
				m_errorContext.Register(code, severity, m_objectType, m_objectName, m_propertyName, arguments);
			}
			else
			{
				m_delayedErrorContext.Register(code, severity, arguments);
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (m_delayedErrorContext == null)
			{
				m_errorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
			else
			{
				m_delayedErrorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
		}

		internal static string GetErrorName(DataFieldStatus status, string exceptionMessage)
		{
			if (exceptionMessage != null)
			{
				return exceptionMessage;
			}
			switch (status)
			{
			case DataFieldStatus.Overflow:
				return "OverflowException.";
			case DataFieldStatus.UnSupportedDataType:
				return "UnsupportedDatatypeException.";
			case DataFieldStatus.IsError:
				return "FieldValueException.";
			default:
				return null;
			}
		}

		internal string EvaluateReportLanguageExpression(ReportingServices.ReportIntermediateFormat.Report report, out CultureInfo language)
		{
			if (!EvaluateSimpleExpression(report.Language, report.ObjectType, report.Name, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.Language, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.ReportLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out language);
		}

		internal int EvaluateReportAutoRefreshExpression(ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (!EvaluateSimpleExpression(report.AutoRefreshExpression, report.ObjectType, report.Name, "AutoRefresh", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.AutoRefreshExpression, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.AutoRefreshExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateInitialPageNameExpression(ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (!EvaluateSimpleExpression(report.InitialPageName, report.ObjectType, report.Name, "InitialPageName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.InitialPageName, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.InitialPageNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result, autocast: true).Value;
		}

		internal string EvaluateParamPrompt(ReportingServices.ReportIntermediateFormat.ParameterDef paramDef)
		{
			if (!EvaluateSimpleExpression(paramDef.PromptExpression, ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Prompt", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramDef.PromptExpression, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.PromptExpression != null, "(paramDef.PromptExpression != null)");
						result.Value = paramDef.ExprHost.PromptExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessAutocastStringResult(result).Value;
		}

		internal VariantResult EvaluateParamDefaultValue(ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.DefaultExpressions != null, "(paramDef.DefaultExpressions != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.DefaultExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "DefaultValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && expressionInfo.ExprHostID >= 0, "(paramDef.ExprHost != null && defaultExpression.ExprHostID >= 0)");
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValue(ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesValueExpressions != null, "(paramDef.ValidValuesValueExpressions != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesValueExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "ValidValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValuesHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValuesHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValuesHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValueLabel(ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesLabelExpressions != null, "(paramDef.ValidValuesLabelExpressions != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesLabelExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValueLabelsHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValueLabelsHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValueLabelsHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			if (!result.ErrorOccurred && (result.Value is object[] || result.Value is IList))
			{
				object[] asObjectArray = GetAsObjectArray(ref result);
				try
				{
					VariantResult result2 = default(VariantResult);
					for (int i = 0; i < asObjectArray.Length; i++)
					{
						result2.Value = asObjectArray[i];
						ProcessLabelResult(ref result2);
						if (result2.ErrorOccurred)
						{
							result.ErrorOccurred = true;
							return result;
						}
						asObjectArray[i] = result2.Value;
					}
					result.Value = asObjectArray;
					return result;
				}
				catch (Exception)
				{
					RegisterInvalidExpressionDataTypeWarning();
					result.ErrorOccurred = true;
					return result;
				}
			}
			if (!result.ErrorOccurred)
			{
				ProcessLabelResult(ref result);
			}
			return result;
		}

		internal object EvaluateDataValueValueExpression(ReportingServices.ReportIntermediateFormat.DataValue value, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out TypeCode typeCode)
		{
			if (!EvaluateSimpleExpression(value.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Value, ref result, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
						result.Value = value.ExprHost.DataValueValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value.Value, ref result);
			typeCode = result.TypeCode;
			return result.Value;
		}

		internal string EvaluateDataValueNameExpression(ReportingServices.ReportIntermediateFormat.DataValue value, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(value.Name, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Name, ref result, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
						result.Value = value.ExprHost.DataValueNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateFilterVariantExpression(ReportingServices.ReportIntermediateFormat.Filter filter, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(filter.Expression, ref result, isArrayAllowed: false);
			return result;
		}

		internal StringResult EvaluateFilterStringExpression(ReportingServices.ReportIntermediateFormat.Filter filter, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal VariantResult EvaluateFilterVariantValue(ReportingServices.ReportIntermediateFormat.Filter filter, int index, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value, ref result, isArrayAllowed: true);
			return result;
		}

		internal FloatResult EvaluateFilterIntegerOrFloatValue(ReportingServices.ReportIntermediateFormat.Filter filter, int index, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateIntegerOrFloatExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result);
		}

		internal IntegerResult EvaluateFilterIntegerValue(ReportingServices.ReportIntermediateFormat.Filter filter, int index, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateIntegerExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result);
		}

		internal StringResult EvaluateFilterStringValue(ReportingServices.ReportIntermediateFormat.Filter filter, int index, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal object EvaluateQueryParamValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo paramValue, IndexedExprHost queryParamsExprHost, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(paramValue, objectType, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramValue, ref result, queryParamsExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(queryParamsExprHost != null && paramValue.ExprHostID >= 0, "(queryParamsExprHost != null && paramValue.ExprHostID >= 0)");
							result.Value = queryParamsExprHost[paramValue.ExprHostID];
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(paramValue, ref result, isArrayAllowed: true);
			return result.Value;
		}

		internal StringResult EvaluateConnectString(ReportingServices.ReportIntermediateFormat.DataSource dataSource)
		{
			if (!EvaluateSimpleExpression(dataSource.ConnectStringExpression, ReportingServices.ReportProcessing.ObjectType.DataSource, dataSource.Name, "ConnectString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSource.ConnectStringExpression, ref result, dataSource.ExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSource.ExprHost != null, "(dataSource.ExprHost != null)");
							result.Value = dataSource.ExprHost.ConnectStringExpr;
						}
					}
				}
				catch (Exception ex)
				{
					if (!(ex is ReportProcessingException_NonExistingParameterReference))
					{
						result = new VariantResult(errorOccurred: true, null);
					}
					else
					{
						RegisterRuntimeErrorInExpression(ref result, ex);
					}
				}
			}
			return ProcessStringResult(result);
		}

		internal StringResult EvaluateCommandText(ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			if (!EvaluateSimpleExpression(dataSet.Query.CommandText, ReportingServices.ReportProcessing.ObjectType.Query, dataSet.Name, "CommandText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSet.Query.CommandText, ref result, dataSet.ExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSet.ExprHost != null, "(dataSet.ExprHost != null)");
							result.Value = dataSet.ExprHost.QueryCommandTextExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal VariantResult EvaluateFieldValueExpression(ReportingServices.ReportIntermediateFormat.Field field)
		{
			if (!EvaluateSimpleExpression(field.Value, ReportingServices.ReportProcessing.ObjectType.Field, field.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(field.Value, ref result, field.ExprHost))
					{
						Global.Tracer.Assert(field.ExprHost != null, "(field.ExprHost != null)");
						field.EnsureExprHostReportObjectModelBinding(m_reportObjectModel);
						result.Value = field.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(field.Value, ref result);
			return result;
		}

		internal VariantResult EvaluateAggregateVariantOrBinaryParamExpr(ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateInfo, int index, IErrorContext errorContext)
		{
			IErrorContext delayedErrorContext = m_delayedErrorContext;
			try
			{
				m_delayedErrorContext = errorContext;
				Global.Tracer.Assert(aggregateInfo.Expressions != null, "(aggregateInfo.Expressions != null)");
				ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = aggregateInfo.Expressions[index];
				if (!EvaluateSimpleExpression(expressionInfo, out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expressionInfo, ref result, null))
						{
							Global.Tracer.Assert(aggregateInfo.ExpressionHosts != null && expressionInfo.ExprHostID >= 0, "(aggregateInfo.ExpressionHosts != null && aggParamExpression.ExprHostID >= 0)");
							if (m_exprHostInSandboxAppDomain)
							{
								aggregateInfo.ExpressionHosts[index].SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = aggregateInfo.ExpressionHosts[index].ValueExpr;
						}
					}
					catch (ReportProcessingException_MissingAggregateDependency)
					{
						throw;
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e, errorContext, isError: false);
					}
				}
				ProcessVariantOrBinaryResult(expressionInfo, ref result, isAggregate: true, allowArray: false);
				return result;
			}
			finally
			{
				m_delayedErrorContext = delayedErrorContext;
			}
		}

		internal VariantResult EvaluateLookupDestExpression(LookupDestinationInfo lookupDestInfo, IErrorContext errorContext)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo destinationExpr = lookupDestInfo.DestinationExpr;
			if (!EvaluateSimpleExpression(destinationExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(destinationExpr, ref result, lookupDestInfo.ExprHost))
					{
						Global.Tracer.Assert(lookupDestInfo.ExprHost != null, "(lookupDestInfo.ExprHost != null)");
						result.Value = lookupDestInfo.ExprHost.DestExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, errorContext, isError: false);
				}
			}
			ProcessLookupVariantResult(destinationExpr, errorContext, isArrayRequired: false, normalizeDBNullToNull: false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupSourceExpression(LookupInfo lookupInfo)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo sourceExpr = lookupInfo.SourceExpr;
			if (!EvaluateSimpleExpression(sourceExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(sourceExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(m_reportObjectModel);
						result.Value = lookupInfo.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessLookupVariantResult(sourceExpr, this, lookupInfo.LookupType == LookupType.MultiLookup, normalizeDBNullToNull: false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupResultExpression(LookupInfo lookupInfo)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo resultExpr = lookupInfo.ResultExpr;
			if (!EvaluateSimpleExpression(resultExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(resultExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(m_reportObjectModel);
						result.Value = exprHost.ResultExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessLookupVariantResult(resultExpr, this, isArrayRequired: false, normalizeDBNullToNull: true, ref result);
			return result;
		}

		internal bool EvaluateParamValueOmitExpression(ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateBooleanExpression(paramVal.Omit, canBeConstant: true, objectType, objectName, "ParameterOmit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Omit, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
						result.Value = paramVal.ExprHost.OmitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal ParameterValueResult EvaluateParameterValueExpression(ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Value, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessParameterValueResult(paramVal.Value, paramVal.Name, result);
		}

		internal bool EvaluateStartHiddenExpression(ReportingServices.ReportIntermediateFormat.Visibility visibility, IVisibilityHiddenExprHost hiddenExprHostRI, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(visibility.Hidden, ref result, (ReportObjectModelProxy)hiddenExprHostRI))
						{
							result.Value = hiddenExprHostRI.VisibilityHiddenExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpressionAndStop(ref result, e);
					}
				}
				return ProcessBooleanResult(result, stopOnError: true, objectType, objectName).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluateStartHiddenExpression(ReportingServices.ReportIntermediateFormat.Visibility visibility, IndexedExprHost hiddenExprHostIdx, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(visibility.Hidden, ref result, hiddenExprHostIdx))
						{
							Global.Tracer.Assert(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0, "(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0)");
							result.Value = hiddenExprHostIdx[visibility.Hidden.ExprHostID];
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpressionAndStop(ref result, e);
					}
				}
				return ProcessBooleanResult(result, stopOnError: true, objectType, objectName).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemDocumentMapLabelExpression(ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			UserProfileState newLocation = m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(reportItem.DocumentMapLabel, reportItem.ObjectType, reportItem.Name, "Label", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(reportItem.DocumentMapLabel, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
							result.Value = reportItem.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemBookmarkExpression(ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(reportItem.Bookmark, reportItem.ObjectType, reportItem.Name, "Bookmark", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(reportItem.Bookmark, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
							result.Value = reportItem.ExprHost.BookmarkExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemToolTipExpression(ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			if (!EvaluateSimpleExpression(reportItem.ToolTip, reportItem.ObjectType, reportItem.Name, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(reportItem.ToolTip, ref result, reportItem.ExprHost))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
						result.Value = reportItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateActionLabelExpression(ReportingServices.ReportIntermediateFormat.ActionItem actionItem, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemHyperlinkURLExpression(ReportingServices.ReportIntermediateFormat.ActionItem actionItem, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Hyperlink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.HyperlinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemDrillthroughReportName(ReportingServices.ReportIntermediateFormat.ActionItem actionItem, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "DrillthroughReportName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.DrillThroughReportNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemBookmarkLinkExpression(ReportingServices.ReportIntermediateFormat.ActionItem actionItem, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BookmarkLink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.BookmarkLinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateImageStringValueExpression(ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			errorOccurred = false;
			if (!EvaluateSimpleExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal VariantResult EvaluateImageTagExpression(ReportingServices.ReportIntermediateFormat.Image image, ReportingServices.ReportIntermediateFormat.ExpressionInfo tag)
		{
			if (!EvaluateSimpleExpression(tag, image.ObjectType, image.Name, "Tag", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tag, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						Global.Tracer.Assert(image.Tags.Count == 1, "Only a single Tag expression host is allowed from old snapshots");
						result.Value = image.ImageExprHost.TagExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(tag, ref result);
			return result;
		}

		internal byte[] EvaluateImageBinaryValueExpression(ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateImageMIMETypeExpression(ReportingServices.ReportIntermediateFormat.Image image)
		{
			if (!EvaluateSimpleExpression(image.MIMEType, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.MIMEType, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateTextBoxInitialToggleStateExpression(ReportingServices.ReportIntermediateFormat.TextBox textBox)
		{
			if (!EvaluateBooleanExpression(textBox.InitialToggleState, canBeConstant: true, textBox.ObjectType, textBox.Name, "InitialState", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textBox.InitialToggleState, ref result, textBox.ExprHost))
					{
						Global.Tracer.Assert(textBox.ExprHost != null, "(textBox.ExprHost != null)");
						result.Value = textBox.TextBoxExprHost.ToggleImageInitialStateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal object EvaluateUserSortExpression(IInScopeEventSource eventSource)
		{
			int sortExpressionIndex = eventSource.UserSort.SortExpressionIndex;
			ReportingServices.ReportIntermediateFormat.ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
			Global.Tracer.Assert(sortTarget.UserSortExpressions != null && 0 <= sortExpressionIndex && sortExpressionIndex < sortTarget.UserSortExpressions.Count);
			ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = sortTarget.UserSortExpressions[sortExpressionIndex];
			if (!EvaluateSimpleExpression(expressionInfo, eventSource.ObjectType, eventSource.Name, "SortExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, sortTarget.UserSortExpressionsHost))
					{
						Global.Tracer.Assert(sortTarget.UserSortExpressionsHost != null, "(sortTarget.UserSortExpressionsHost != null)");
						result.Value = sortTarget.UserSortExpressionsHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result);
			if (result.Value == null)
			{
				result.Value = DBNull.Value;
			}
			return result.Value;
		}

		internal string EvaluateGroupingLabelExpression(ReportingServices.ReportIntermediateFormat.Grouping grouping, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			UserProfileState newLocation = m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			try
			{
				if (!EvaluateSimpleExpression(grouping.GroupLabel, objectType, objectName, "Label", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(grouping.GroupLabel, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
			}
		}

		internal string EvaluateGroupingPageNameExpression(ReportingServices.ReportIntermediateFormat.Grouping grouping, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, ReportingServices.ReportProcessing.ObjectType.Grouping, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal object EvaluateRuntimeExpression(RuntimeExpressionInfo runtimeExpression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(runtimeExpression.Expression, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(runtimeExpression.Expression, ref result, runtimeExpression.ExpressionsHost))
					{
						Global.Tracer.Assert(runtimeExpression.ExpressionsHost != null && runtimeExpression.Expression.ExprHostID >= 0);
						result.Value = runtimeExpression.ExpressionsHost[runtimeExpression.Expression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(runtimeExpression.Expression, ref result);
			VerifyVariantResultAndStopOnError(ref result);
			return result.Value;
		}

		internal VariantResult EvaluateVariableValueExpression(ReportingServices.ReportIntermediateFormat.Variable variable, IndexedExprHost variableValuesHost, ReportingServices.ReportProcessing.ObjectType parentObjectType, string parentObjectName, bool isReportScope)
		{
			if (!EvaluateSimpleExpression(variable.Value, parentObjectType, parentObjectName, variable.GetPropertyName(), out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(variable.Value, ref result, variableValuesHost))
					{
						Global.Tracer.Assert(variableValuesHost != null && variable.Value.ExprHostID >= 0);
						result.Value = variableValuesHost[variable.Value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessSerializableResult(variable.Value, isReportScope, ref result);
			return result;
		}

		internal string EvaluateSubReportNoRowsExpression(ReportingServices.ReportIntermediateFormat.SubReport subReport, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(subReport.NoRowsMessage, ReportingServices.ReportProcessing.ObjectType.Subreport, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(subReport.NoRowsMessage, ref result, subReport.SubReportExprHost))
					{
						Global.Tracer.Assert(subReport.SubReportExprHost != null, "(subReport.SubReportExprHost != null)");
						result.Value = subReport.SubReportExprHost.NoRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionNoRowsExpression(ReportingServices.ReportIntermediateFormat.DataRegion region, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(region.NoRowsMessage, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(region.NoRowsMessage, ref result, region.ExprHost))
					{
						Global.Tracer.Assert(region.ExprHost != null, "(region.ExprHost != null)");
						result.Value = region.EvaluateNoRowsMessageExpression();
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionPageNameExpression(ReportingServices.ReportIntermediateFormat.DataRegion dataRegion, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, dataRegion.ExprHost))
						{
							Global.Tracer.Assert(dataRegion.ExprHost != null, "(dataRegion.ExprHost != null)");
							result.Value = dataRegion.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateTablixMarginExpression(ReportingServices.ReportIntermediateFormat.Tablix tablix, ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition marginPosition)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo expression = null;
			switch (marginPosition)
			{
			case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
				expression = tablix.TopMargin;
				break;
			case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
				expression = tablix.BottomMargin;
				break;
			case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
				expression = tablix.LeftMargin;
				break;
			case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
				expression = tablix.RightMargin;
				break;
			}
			if (!EvaluateSimpleExpression(expression, tablix.ObjectType, tablix.Name, marginPosition.ToString(), out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, tablix.ExprHost))
					{
						Global.Tracer.Assert(tablix.TablixExprHost != null, "(tablix.TablixExprHost != null)");
						switch (marginPosition)
						{
						case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
							result.Value = tablix.TablixExprHost.TopMarginExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
							result.Value = tablix.TablixExprHost.BottomMarginExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
							result.Value = tablix.TablixExprHost.LeftMarginExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
							result.Value = tablix.TablixExprHost.RightMarginExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartDynamicSizeExpression(ReportingServices.ReportIntermediateFormat.Chart chart, ReportingServices.ReportIntermediateFormat.ExpressionInfo expr, string propertyName, bool isDynamicWidth)
		{
			if (!EvaluateSimpleExpression(expr, ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expr, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						if (isDynamicWidth)
						{
							result.Value = chart.ChartExprHost.DynamicWidthExpr;
						}
						else
						{
							result.Value = chart.ChartExprHost.DynamicHeightExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateChartDynamicMemberLabelExpression(ReportingServices.ReportIntermediateFormat.ChartMember chartMember, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DocumentMapLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, chartMember.ExprHost))
					{
						Global.Tracer.Assert(chartMember.ExprHost != null, "(chartMember.ExprHost != null)");
						result.Value = chartMember.ExprHost.MemberLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result;
		}

		internal string EvaluateChartPaletteExpression(ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			if (!EvaluateSimpleExpression(chart.Palette, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Palette", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chart.Palette, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						result.Value = chart.ChartExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartPaletteHatchBehaviorExpression(ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			if (!EvaluateSimpleExpression(chart.PaletteHatchBehavior, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PaletteHatchBehavior", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chart.PaletteHatchBehavior, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "chart.ExprHost != null");
						result.Value = chart.ChartExprHost.PaletteHatchBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartTitleCaptionExpression(ChartTitleBase title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Caption, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = title.ExprHost.CaptionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(title.Caption, ref result);
			return result;
		}

		internal bool EvaluateEvaluateChartTitleHiddenExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Hidden, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartTitleDockingExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Position, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Docking, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartTitleDockOutsideChartAreaExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.DockOutsideChartArea, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateChartTitleDockOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.DockOffset, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartTitleToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.ToolTip, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitleTextOrientationExpression(ReportingServices.ReportIntermediateFormat.ChartTitle chartTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTitle.TextOrientation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTitle.TextOrientation, ref result, chartTitle.ExprHost))
					{
						Global.Tracer.Assert(chartTitle.ExprHost != null, "(chartTitle.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)chartTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitlePositionExpression(ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitleTextOrientationExpression(ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisTitle.TextOrientation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisTitle.TextOrientation, ref result, chartAxisTitle.ExprHost))
					{
						Global.Tracer.Assert(chartAxisTitle.ExprHost != null, "(chartAxisTitle.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)chartAxisTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendTitleTitleSeparatorExpression(ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendTitle.TitleSeparator, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartTitleSeparator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendTitle.TitleSeparator, ref result, chartLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(chartLegendTitle.ExprHost != null, "(chartLegendTitle.ExprHost != null)");
						result.Value = ((ChartLegendTitleExprHost)chartLegendTitle.ExprHost).TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelLabelExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Label, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Label, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartDataLabel.Label, ref result);
			return result;
		}

		internal string EvaluateChartDataLabePositionExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Position, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Position, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.ChartDataLabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartDataLabelRotationExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Rotation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Rotation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Rotation, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartDataLabelUseValueAsLabelExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.UseValueAsLabel, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UseValueAsLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.UseValueAsLabel, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.UseValueAsLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartDataLabelVisibleExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Visible, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Visible, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartDataLabel.ToolTip, ref result, chartDataLabel.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
					result.Value = chartDataLabel.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesXExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.X, objectName, "X", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesXExpr);
		}

		internal string EvaluateChartTickMarksEnabledExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Enabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Enabled, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTickMarksTypeExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Type, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Type, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksLengthExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Length, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Length", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Length, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Interval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Interval, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalTypeExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalOffset, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalOffsetTypeExpression(ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalOffsetType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalOffsetType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartItemInLegendLegendTextExpression(ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.LegendText, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartItemInLegend.LegendText, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						result.Value = chartItemInLegend.ExprHost.LegendTextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartItemInLegendToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartItemInLegend.ToolTip, ref result, chartItemInLegend.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
					result.Value = chartItemInLegend.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal bool EvaluateChartItemInLegendHiddenExpression(ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.Hidden, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartItemInLegend.Hidden, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						if (chartItemInLegend.ExprHost != null)
						{
							result.Value = chartItemInLegend.ExprHost.HiddenExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartEmptyPointsAxisLabelExpression(ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			if (!EvaluateSimpleExpression(chartEmptyPoints.AxisLabel, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartEmptyPoints.AxisLabel, ref result, chartEmptyPoints.ExprHost))
					{
						Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
						result.Value = chartEmptyPoints.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartEmptyPoints.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartEmptyPointsToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			if (!EvaluateSimpleExpression(chartEmptyPoints.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartEmptyPoints.ToolTip, ref result, chartEmptyPoints.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
					result.Value = chartEmptyPoints.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartFormulaParameterValueExpression(ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter, string objectName)
		{
			if (!EvaluateSimpleExpression(chartFormulaParameter.Value, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartFormulaParameter.Value, ref result, chartFormulaParameter.ExprHost))
					{
						Global.Tracer.Assert(chartFormulaParameter.ExprHost != null, "(chartFormulaParameter.ExprHost != null)");
						result.Value = chartFormulaParameter.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartFormulaParameter.Value, ref result);
			return result;
		}

		internal double EvaluateChartElementPositionExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string propertyName, ChartElementPositionExprHost exprHost, ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position position, string objectName)
		{
			if (!EvaluateSimpleExpression(expressionInfo, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (position)
						{
						case ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Top:
							result.Value = exprHost.TopExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Left:
							result.Value = exprHost.LeftExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Height:
							result.Value = exprHost.HeightExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Width:
							result.Value = exprHost.WidthExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartSmartLabelAllowOutSidePlotAreaExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.AllowOutSidePlotArea, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowOutSidePlotArea", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.AllowOutSidePlotArea, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.AllowOutSidePlotAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutBackColorExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutBackColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutBackColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutBackColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutBackColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartSmartLabelCalloutLineAnchorExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineAnchor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineAnchor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineAnchor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineAnchorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineColorExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartSmartLabelCalloutLineStyleExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineStyle, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineWidthExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineWidth, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelCalloutStyleExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutStyle, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelShowOverlappedExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.ShowOverlapped, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ShowOverlapped", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.ShowOverlapped, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.ShowOverlappedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelMarkerOverlappingExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MarkerOverlapping, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarkerOverlapping", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MarkerOverlapping, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MarkerOverlappingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelDisabledExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.Disabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Disabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.Disabled, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.DisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartSmartLabelMaxMovingDistanceExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MaxMovingDistance, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxMovingDistance", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MaxMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MaxMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelMinMovingDistanceExpression(ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MinMovingDistance, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinMovingDistance", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MinMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MinMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartLegendHiddenExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Hidden, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Hidden, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendPositionExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Position, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Position, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ChartLegendPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendLayoutExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Layout, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Layout, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartLegendDockOutsideChartAreaExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.DockOutsideChartArea, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.DockOutsideChartArea, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartLegendAutoFitTextDisabledExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.AutoFitTextDisabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.AutoFitTextDisabled, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendMinFontSizeExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.MinFontSize, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.MinFontSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendHeaderSeparatorExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.HeaderSeparator, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.HeaderSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendHeaderSeparatorColorExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.HeaderSeparatorColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.HeaderSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartLegendColumnSeparatorExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSeparator, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnSeparatorColorExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSeparatorColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal int EvaluateChartLegendColumnSpacingExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSpacing, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSpacing, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSpacingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartLegendInterlacedRowsExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.InterlacedRows, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.InterlacedRows, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendInterlacedRowsColorExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.InterlacedRowsColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.InterlacedRowsColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateChartLegendEquallySpacedItemsExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.EquallySpacedItems, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.EquallySpacedItems, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendReversedExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Reversed, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reversed", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Reversed, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendMaxAutoSizeExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.MaxAutoSize, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.MaxAutoSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MaxAutoSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendTextWrapThresholdExpression(ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.TextWrapThreshold, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.TextWrapThreshold, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendColumnHeaderValueExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeader, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumnHeader.Value, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumnHeader.Value, ref result, chartLegendColumnHeader.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumnHeader.ExprHost != null, "(chartLegendColumnHeader.ExprHost != null)");
						result.Value = chartLegendColumnHeader.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnColumnTypeExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.ColumnType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ColumnType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.ColumnType, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ColumnTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnValueExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.Value, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.Value, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.ToolTip, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnMinimumWidthExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.MinimumWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinimumWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.MinimumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MinimumWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendColumnMaximumWidthExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.MaximumWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaximumWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.MaximumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MaximumWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int EvaluateChartLegendColumnSeriesSymbolWidthExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.SeriesSymbolWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendColumnSeriesSymbolHeightExpression(ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolHeight, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.SeriesSymbolHeight, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellCellTypeExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.CellType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.CellType, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellTextExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.Text, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.Text, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellCellSpanExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.CellSpan, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellSpan", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.CellSpan, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellSpanExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ToolTip, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageWidthExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ImageWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ImageWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageHeightExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ImageHeight, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ImageHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolHeightExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolHeight, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.SymbolHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolWidthExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.SymbolWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellAlignmentExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.Alignment, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Alignment", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.Alignment, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.AlignmentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellTopMarginExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.TopMargin, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TopMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.TopMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellBottomMarginExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.BottomMargin, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BottomMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.BottomMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellLeftMarginExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.LeftMargin, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LeftMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.LeftMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellRightMarginExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.RightMargin, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "RightMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.RightMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Up, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Up", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Up, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Down, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Down", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Down, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsLeftExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Left, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Left, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsRightExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Right, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Right", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Right, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.RightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpLeftExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.UpLeft, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.UpLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpRightExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.UpRight, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.UpRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownLeftExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.DownLeft, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.DownLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownRightExpression(ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.DownRight, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.DownRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartStripLineTitleExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.Title, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Title", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.Title, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartStripLineTitleAngleExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.TitleAngle, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TitleAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.TitleAngle, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartStripLineTextOrientationExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.TextOrientation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.TextOrientation, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartStripLineToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.ToolTip, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.Interval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.Interval, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalTypeExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalOffset, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalOffsetTypeExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalOffsetType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalOffsetType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineStripWidthExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.StripWidth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.StripWidth, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineStripWidthTypeExpression(ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.StripWidthType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidthType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.StripWidthType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.Separator, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Separator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.Separator, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorColorExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.SeparatorColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.SeparatorColor, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartLegendCustomItemToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.ToolTip, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesTypeExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Type, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Type, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesSubtypeExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Subtype, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Subtype", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Subtype, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.SubtypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesLegendNameExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.LegendName, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.LegendName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.LegendNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesLegendTextExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.LegendText, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartSeries.LegendText, ref result, chartSeries.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
					result.Value = chartSeries.ExprHost.LegendTextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesChartAreaNameExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ChartAreaName, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartAreaName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.ChartAreaName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ChartAreaNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesValueAxisNameExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ValueAxisName, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ValueAxisName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.ValueAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ValueAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartSeries.ToolTip, ref result, chartSeries.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
					result.Value = chartSeries.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesCategoryAxisNameExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.CategoryAxisName, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CategoryAxisName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.CategoryAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.CategoryAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSeriesHiddenExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Hidden, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Hidden, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSeriesHideInLegendExpression(ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.HideInLegend, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideInLegend", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.HideInLegend, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HideInLegendExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartBorderSkinBorderSkinTypeExpression(ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkin, string objectName)
		{
			if (!EvaluateSimpleExpression(chartBorderSkin.BorderSkinType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartBorderSkinType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartBorderSkin.BorderSkinType, ref result, chartBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(chartBorderSkin.ExprHost != null, "(chartBorderSkin.ExprHost != null)");
						result.Value = chartBorderSkin.ExprHost.BorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisScaleBreakEnabledExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.Enabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.Enabled, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakBreakLineTypeExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.BreakLineType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BreakLineType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.BreakLineType, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.BreakLineTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result, autocast: true).Value;
		}

		internal int EvaluateChartAxisScaleBreakCollapsibleSpaceThresholdExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CollapsibleSpaceThreshold", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.CollapsibleSpaceThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartAxisScaleBreakMaxNumberOfBreaksExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.MaxNumberOfBreaks, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxNumberOfBreaks", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.MaxNumberOfBreaks, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.MaxNumberOfBreaksExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal double EvaluateChartAxisScaleBreakSpacingExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.Spacing, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Spacing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.Spacing, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.SpacingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakIncludeZeroExpression(ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.IncludeZero, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.IncludeZero, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartCustomPaletteColorExpression(ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor customPaletteColor, string objectName)
		{
			if (!EvaluateSimpleExpression(customPaletteColor.Color, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customPaletteColor.Color, ref result, customPaletteColor.ExprHost))
					{
						Global.Tracer.Assert(customPaletteColor.ExprHost != null, "(customPaletteColor.ExprHost != null)");
						result.Value = customPaletteColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateChartDataPointAxisLabelExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataPoint.AxisLabel, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataPoint.AxisLabel, ref result, chartDataPoint.ExprHost))
					{
						Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
						result.Value = chartDataPoint.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartDataPoint.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointToolTipExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataPoint.ToolTip, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartDataPoint.ToolTip, ref result, chartDataPoint.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
					result.Value = chartDataPoint.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesYExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Y, objectName, "Y", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesYExpr);
		}

		internal VariantResult EvaluateChartDataPointValueSizesExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Size, objectName, "Size", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesSizeExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(dataPoint.DataPointValues.High, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "High", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataPoint.DataPointValues.High, ref result, dataPoint.ExprHost))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
						result.Value = dataPoint.ExprHost.DataPointValuesHighExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(dataPoint.DataPointValues.High, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesLowExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Low, objectName, "Low", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesLowExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesStartExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Start, objectName, "Start", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesStartExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesEndExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.End, objectName, "End", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesEndExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMeanExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Mean, objectName, "Mean", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMeanExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMedianExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Median, objectName, "Median", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMedianExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightXExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightX, objectName, "HighlightX", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightXExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightYExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightY, objectName, "HighlightY", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightYExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightSizeExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightSize, objectName, "HighlightSize", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightSizeExpr);
		}

		internal string EvaluateChartDataPointValuesFormatXExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatX, objectName, "FormatX", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatXExpr);
		}

		internal string EvaluateChartDataPointValuesFormatYExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatY, objectName, "FormatY", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatYExpr);
		}

		internal string EvaluateChartDataPointValuesFormatSizeExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatSize, objectName, "FormatSize", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatSizeExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageXExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageX, objectName, "CurrencyLanguageX", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageXExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageYExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageY, objectName, "CurrencyLanguageY", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageYExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageSizeExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageSize, objectName, "CurrencyLanguageSize", (ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageSizeExpr);
		}

		private VariantResult EvaluateChartDataPointValuesExpressionAsVariant(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			ProcessVariantResult(expressionInfo, ref result);
			return result;
		}

		private string EvaluateChartDataPointValuesExpressionAsString(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			return ProcessStringResult(result).Value;
		}

		private VariantResult EvaluateChartDataPointValuesExpression(ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			if (!EvaluateSimpleExpression(expressionInfo, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(expressionInfo, ref result, dataPoint.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
					result.Value = expressionFunction(dataPoint);
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartMarkerSize(ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(chartMarker.Size, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Size", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartMarker.Size, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartMarkerType(ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(chartMarker.Type, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartMarker.Type, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisVisibleExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Visible, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Visible, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisMarginExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Margin, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Margin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Margin, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Interval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Interval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalTypeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalOffsetTypeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalOffsetType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisMarksAlwaysAtPlotEdgeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MarksAlwaysAtPlotEdge, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarksAlwaysAtPlotEdge", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MarksAlwaysAtPlotEdge, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarksAlwaysAtPlotEdgeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisReverseExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Reverse, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reverse", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Reverse, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ReverseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisLocationExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Location, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Location", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Location, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LocationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisInterlacedExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Interlaced, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interlaced", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Interlaced, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisInterlacedColorExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.InterlacedColor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "InterlacedColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.InterlacedColor, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateChartAxisLogScaleExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LogScale, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LogScale, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLogBaseExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LogBase, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogBase", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LogBase, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogBaseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateChartAxisHideLabelsExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.HideLabels, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.HideLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisAngleExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Angle, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Angle, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisArrowsExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Arrows, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Arrows", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Arrows, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ArrowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontShrinkExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventFontShrink, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontShrink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventFontShrink, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontShrinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontGrowExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventFontGrow, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontGrow", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventFontGrow, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontGrowExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventLabelOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventLabelOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventLabelOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventLabelOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventLabelOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventWordWrapExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventWordWrap, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventWordWrap", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventWordWrap, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventWordWrapExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisAllowLabelRotationExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.AllowLabelRotation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowLabelRotation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.AllowLabelRotation, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AllowLabelRotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisIncludeZeroExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IncludeZero, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IncludeZero, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisLabelsAutoFitDisabledExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelsAutoFitDisabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelsAutoFitDisabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelsAutoFitDisabled, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelsAutoFitDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisMinFontSizeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MinFontSize, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MinFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartAxisMaxFontSizeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MaxFontSize, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MaxFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MaxFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartAxisOffsetLabelsExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.OffsetLabels, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "OffsetLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.OffsetLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.OffsetLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisHideEndLabelsExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.HideEndLabels, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.HideEndLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisVariableAutoIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.VariableAutoInterval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "VariableAutoInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.VariableAutoInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VariableAutoIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelInterval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalTypeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalOffsetsExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalOffsetTypeExpression(ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalOffsetType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal object EvaluateChartAxisValueExpression(ChartAxisExprHost exprHost, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName, string propertyName, ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType type)
		{
			if (!EvaluateSimpleExpression(expression, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (type)
						{
						case ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Min:
							result.Value = exprHost.AxisMinExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Max:
							result.Value = exprHost.AxisMaxExpr;
							break;
						case ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.CrossAt:
							result.Value = exprHost.AxisCrossAtExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result.Value;
		}

		internal bool EvaluateChartAreaEquallySizedAxesFontExpression(ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.EquallySizedAxesFont, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.EquallySizedAxesFont, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.EquallySizedAxesFontExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAreaAlignOrientationExpression(ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.AlignOrientation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.AlignOrientation, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.AlignOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAreaHiddenExpression(ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.Hidden, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.Hidden, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeAxesViewExpression(ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.AxesView, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.AxesView, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.AxesViewExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeCursorExpression(ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.Cursor, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.Cursor, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.CursorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypePositionExpression(ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.Position, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.Position, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.ChartAlignTypePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeInnerPlotPositionExpression(ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.InnerPlotPosition, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.InnerPlotPosition, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.InnerPlotPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartGridLinesEnabledExpression(ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.Enabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.Enabled, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalExpression(ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.Interval, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.Interval, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalTypeExpression(ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalOffset, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalOffsetTypeExpression(ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalOffsetType, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalOffsetType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesEnabledExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Enabled, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Enabled, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesProjectionModeExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.ProjectionMode, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.ProjectionMode, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ProjectionModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesRotationExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Rotation, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Rotation, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesInclinationExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Inclination, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Inclination, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.InclinationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesPerspectiveExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Perspective, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Perspective, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.PerspectiveExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesDepthRatioExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.DepthRatio, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.DepthRatio, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.DepthRatioExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesShadingExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Shading, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Shading, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ShadingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesGapDepthExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.GapDepth, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.GapDepth, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.GapDepthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesWallThicknessExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.WallThickness, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.WallThickness, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.WallThicknessExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesClusteredExpression(ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Clustered, ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Clustered, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ClusteredExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateRectanglePageNameExpression(ReportingServices.ReportIntermediateFormat.Rectangle rectangle, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, ReportingServices.ReportProcessing.ObjectType.Rectangle, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, rectangle.ExprHost))
						{
							Global.Tracer.Assert(rectangle.ExprHost != null, "(rectangle.ExprHost != null)");
							result.Value = rectangle.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateStyleBorderColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorLeft(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorRight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorTop(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorBottom(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BorderStyles EvaluateStyleBorderStyle(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, (ReportingServices.ReportProcessing.ObjectType.Line != objectType) ? BorderStyles.None : BorderStyles.Solid, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleLeft(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleRight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleTop(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleBottom(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidth(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthLeft(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthRight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthTop(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthBottom(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBackgroundGradientEndColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientEndColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientEndColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BackgroundGradients EvaluateStyleBackgroundGradientType(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundGradientType(ProcessStringResult(result).Value, this);
		}

		internal BackgroundRepeatTypes EvaluateStyleBackgroundRepeat(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundRepeat", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundRepeatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundRepeat(ProcessStringResult(result).Value, this, objectType == ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal FontStyles EvaluateStyleFontStyle(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontFamily(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontFamily", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontFamilyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleFontSize(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateFontSize(ProcessStringResult(result).Value, this);
		}

		internal FontWeights EvaluateStyleFontWeight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontWeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontWeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontWeight(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFormat(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Format", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FormatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal TextDecorations EvaluateStyleTextDecoration(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextDecoration", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextDecorationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextDecoration(ProcessStringResult(result).Value, this);
		}

		internal TextAlignments EvaluateStyleTextAlign(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextAlign(ProcessStringResult(result).Value, this);
		}

		internal VerticalAlignments EvaluateStyleVerticalAlign(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "VerticalAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.VerticalAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateVerticalAlign(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStylePaddingLeft(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingRight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingTop(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingBottom(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingBottom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLineHeight(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "LineHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LineHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateLineHeight(ProcessStringResult(result).Value, this);
		}

		internal Directions EvaluateStyleDirection(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Direction", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.DirectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateDirection(ProcessStringResult(result).Value, this);
		}

		internal WritingModes EvaluateStyleWritingMode(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "WritingMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.WritingModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateWritingMode(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLanguage(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal UnicodeBiDiTypes EvaluateStyleUnicodeBiDi(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "UnicodeBiDi", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.UnicodeBiDiExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateUnicodeBiDi(ProcessStringResult(result).Value, this);
		}

		internal Calendars EvaluateStyleCalendar(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Calendar", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.CalendarExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateCalendar(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleCurrencyLanguage(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "CurrencyLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(condition: false, "(style.ExprHost should not be invoked for CurrencyLanguage.)");
						result.Value = null;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleNumeralLanguage(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "NumeralLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal object EvaluateStyleNumeralVariant(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, objectType, objectName, "NumeralVariant", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralVariantExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			if (result.Value == null)
			{
				return null;
			}
			IntegerResult integerResult = ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateNumeralVariant(integerResult.Value, this);
		}

		internal object EvaluateTransparentColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal object EvaluatePosition(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, objectType, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslatePosition(ProcessStringResult(result).Value, this, objectType == ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal string EvaluateStyleBackgroundUrlImageValue(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleBackgroundEmbeddedImageValue(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Dictionary<string, ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateEmbeddedImageName(ProcessStringResult(result).Value, embeddedImages, this);
		}

		internal byte[] EvaluateStyleBackgroundDatabaseImageValue(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBinaryResult(result).Value;
		}

		internal string EvaluateStyleBackgroundImageMIMEType(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageMIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageMIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateMimeType(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleTextEffect(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextEffect", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextEffectExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextEffect(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleShadowColor(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "ShadowColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleShadowOffset(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "ShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundHatchType(ReportingServices.ReportIntermediateFormat.Style style, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundHatchType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundHatchTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateBackgroundHatchType(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphLeftIndentExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.LeftIndent, paragraph.ObjectType, paragraph.Name, "LeftIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.LeftIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.LeftIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphRightIndentExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.RightIndent, paragraph.ObjectType, paragraph.Name, "RightIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.RightIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.RightIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphHangingIndentExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.HangingIndent, paragraph.ObjectType, paragraph.Name, "HangingIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.HangingIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.HangingIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, allowNegative: true, this);
		}

		internal string EvaluateParagraphSpaceBeforeExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.SpaceBefore, paragraph.ObjectType, paragraph.Name, "SpaceBefore", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.SpaceBefore, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceBeforeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphSpaceAfterExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.SpaceAfter, paragraph.ObjectType, paragraph.Name, "SpaceAfter", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.SpaceAfter, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceAfterExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int? EvaluateParagraphListLevelExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.ListLevel, paragraph.ObjectType, paragraph.Name, "ListLevel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.ListLevel, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListLevelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			IntegerResult integerResult = ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListLevel(integerResult.Value, this);
		}

		internal string EvaluateParagraphListStyleExpression(ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.ListStyle, paragraph.ObjectType, paragraph.Name, "ListStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.ListStyle, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateTextRunToolTipExpression(ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			if (!EvaluateSimpleExpression(textRun.ToolTip, textRun.ObjectType, textRun.Name, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textRun.ToolTip, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateTextRunMarkupTypeExpression(ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			if (!EvaluateSimpleExpression(textRun.MarkupType, textRun.ObjectType, textRun.Name, "MarkupType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textRun.MarkupType, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.MarkupTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextRunMarkupType(ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateTextRunValueExpression(ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo value = textRun.Value;
			if (!EvaluateSimpleExpression(value, textRun.ObjectType, textRun.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value, ref result);
			return result;
		}

		internal bool EvaluatePageBreakDisabledExpression(ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "Disabled", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.DisabledExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessBooleanResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluatePageBreakResetPageNumberExpression(ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "ResetPageNumber", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.ResetPageNumberExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessBooleanResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal VariantResult EvaluateJoinConditionForeignKeyExpression(Relationship.JoinCondition joinCondition)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo foreignKeyExpression = joinCondition.ForeignKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			if (!EvaluateSimpleExpression(foreignKeyExpression, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(foreignKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.ForeignKeyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(foreignKeyExpression, ref result);
			VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		internal VariantResult EvaluateJoinConditionPrimaryKeyExpression(Relationship.JoinCondition joinCondition)
		{
			ReportingServices.ReportIntermediateFormat.ExpressionInfo primaryKeyExpression = joinCondition.PrimaryKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			if (!EvaluateSimpleExpression(primaryKeyExpression, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(primaryKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.PrimaryKeyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(primaryKeyExpression, ref result);
			VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		private bool EvaluateSimpleExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			if (m_topLevelReportRuntime != null)
			{
				m_topLevelReportRuntime.ObjectType = objectType;
				m_topLevelReportRuntime.ObjectName = objectName;
				m_topLevelReportRuntime.PropertyName = propertyName;
			}
			return EvaluateSimpleExpression(expression, out result);
		}

		private bool EvaluateSimpleExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, out VariantResult result)
		{
			result = default(VariantResult);
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant:
					result.Value = expression.Value;
					result.TypeCode = expression.ConstantTypeCode;
					return true;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field:
					EvaluateSimpleFieldReference(expression.IntValue, ref result);
					return true;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token:
				{
					ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = m_reportObjectModel.DataSetsImpl[expression.StringValue];
					result.Value = dataSet.RewrittenCommandText;
					return true;
				}
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					return false;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					return false;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
					return false;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					return false;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.RdlFunction:
					EvaluateRdlFunction(expression, ref result);
					return true;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Literal:
					result.Value = expression.LiteralInfo.Value;
					return true;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		internal void EvaluateSimpleFieldReference(int fieldIndex, ref VariantResult result)
		{
			try
			{
				ReportingServices.ReportProcessing.OnDemandReportObjectModel.FieldImpl fieldImpl = m_reportObjectModel.FieldsImpl[fieldIndex];
				if (fieldImpl.IsMissing)
				{
					result.Value = null;
				}
				else if (fieldImpl.FieldStatus != 0)
				{
					result.ErrorOccurred = true;
					result.FieldStatus = fieldImpl.FieldStatus;
					result.ExceptionMessage = fieldImpl.ExceptionMessage;
					result.Value = null;
				}
				else
				{
					result.Value = fieldImpl.Value;
				}
			}
			catch (ReportProcessingException_NoRowsFieldAccess e)
			{
				RegisterRuntimeWarning(e, this);
				result.Value = null;
			}
			catch (ReportProcessingException_InvalidOperationException)
			{
				result.Value = null;
				result.ErrorOccurred = true;
			}
		}

		private bool EvaluateComplexExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, ReportObjectModelProxy exprHost)
		{
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					result.Value = m_reportObjectModel.AggregatesImpl[expression.StringValue];
					return true;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				{
					Lookup lookup2 = m_reportObjectModel.LookupsImpl[expression.StringValue];
					result.Value = lookup2.Value;
					return true;
				}
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
				{
					Lookup lookup = m_reportObjectModel.LookupsImpl[expression.StringValue];
					result.Value = lookup.Values;
					return true;
				}
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					try
					{
						m_reportObjectModel.OdpContext.StateManager.EvaluateScopedFieldReference(expression.StringValue, expression.ScopedFieldInfo.FieldIndex, ref result);
					}
					catch (ReportProcessingException_NonExistingScopeReference reportProcessingException_NonExistingScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_NonExistingScopeReference.Message;
					}
					catch (ReportProcessingException_InvalidScopeReference reportProcessingException_InvalidScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_InvalidScopeReference.Message;
					}
					return true;
				case ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					if (m_exprHostInSandboxAppDomain)
					{
						exprHost?.SetReportObjectModel(m_reportObjectModel);
					}
					return false;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		private void EvaluateRdlFunction(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			List<ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions = expression.RdlFunctionInfo.Expressions;
			object[] array = new object[expressions.Count];
			for (int i = 0; i < expressions.Count; i++)
			{
				if (!EvaluateSimpleExpression(expressions[i], out VariantResult result2) && !EvaluateComplexExpression(expressions[i], ref result2, null))
				{
					Global.Tracer.Assert(condition: false, "Rdl function argument is complex.");
				}
				array[i] = result2.Value;
				if (result2.ErrorOccurred)
				{
					result = result2;
					return;
				}
			}
			switch (expression.RdlFunctionInfo.FunctionType)
			{
			case RdlFunctionInfo.RdlFunctionType.MinValue:
				result.Value = MinValue(array);
				break;
			case RdlFunctionInfo.RdlFunctionType.MaxValue:
				result.Value = MaxValue(array);
				break;
			default:
				Global.Tracer.Assert(condition: false, "No case for: " + expression.RdlFunctionInfo.FunctionType);
				break;
			}
		}

		private void RegisterRuntimeWarning(Exception e, IErrorContext iErrorContext)
		{
			if (e is ReportProcessingException_NoRowsFieldAccess)
			{
				iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, Severity.Warning, e.Message);
				return;
			}
			if (Global.Tracer.TraceError)
			{
				Global.Tracer.Trace("Caught unexpected exception inside of RegisterRuntimeWarning.");
			}
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		private void RegisterRuntimeErrorInExpressionAndStop(ref VariantResult result, Exception e)
		{
			RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
			if (result.ErrorOccurred)
			{
				ProcessingMessageList messages = m_errorContext.Messages;
				if (messages != null && messages.Count > 0)
				{
					throw new ReportProcessingException(messages[0].Message, ErrorCode.rsProcessingError);
				}
				throw new ReportProcessingException(messages);
			}
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e)
		{
			RegisterRuntimeErrorInExpression(ref result, e, this, isError: false);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e, IErrorContext iErrorContext, bool isError)
		{
			if (e is RSException || AsynchronousExceptionDetection.IsStoppingException(e))
			{
				throw new ReportProcessingException(e.GetType().FullName + ": " + e.Message, ErrorCode.rsProcessingError);
			}
			if (e is ReportProcessingException_FieldError)
			{
				result.FieldStatus = ((ReportProcessingException_FieldError)e).Status;
				if (DataFieldStatus.IsMissing == result.FieldStatus)
				{
					result = new VariantResult(errorOccurred: false, null);
				}
				else
				{
					result = new VariantResult(errorOccurred: true, null);
				}
				return;
			}
			if (e is ReportProcessingException_InvalidOperationException)
			{
				result = new VariantResult(errorOccurred: true, null);
				return;
			}
			if (e is ReportProcessingException_UserProfilesDependencies)
			{
				iErrorContext.Register(ProcessingErrorCode.rsRuntimeUserProfileDependency, Severity.Error, (string[])null);
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			string text;
			if (e != null)
			{
				try
				{
					text = ((e.InnerException == null) ? e.Message : e.InnerException.Message);
				}
				catch
				{
					text = RPRes.NonClsCompliantException;
				}
			}
			else
			{
				text = RPRes.NonClsCompliantException;
			}
			iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, (!isError) ? Severity.Warning : Severity.Error, text);
			if (e is ReportProcessingException_NoRowsFieldAccess)
			{
				result = new VariantResult(errorOccurred: false, null);
				return;
			}
			if (isError)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			result = new VariantResult(errorOccurred: true, null);
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				if (ex.Message != null)
				{
					stringBuilder.Append(ex.Message);
					stringBuilder.Append(";");
				}
				if (ex.Source != null)
				{
					stringBuilder.Append(" Source: ");
					stringBuilder.Append(ex.Source);
					stringBuilder.Append(";");
				}
			}
			result.ExceptionMessage = stringBuilder.ToString();
		}

		private bool EvaluateBooleanExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool canBeConstant, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.BoolValue;
					result.TypeCode = TypeCode.Boolean;
				}
				else
				{
					result.ErrorOccurred = true;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result)
		{
			return ProcessBooleanResult(result, stopOnError: false, m_objectType, null);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result, bool stopOnError, ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			BooleanResult result2 = default(BooleanResult);
			bool processedValue;
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				if (stopOnError && result.FieldStatus != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, objectType, objectName, "Hidden", GetErrorName(result.FieldStatus, result.ExceptionMessage));
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			else if (TryProcessObjectToBoolean(result.Value, out processedValue))
			{
				result2.Value = processedValue;
			}
			else
			{
				result2.ErrorOccurred = true;
				if (stopOnError)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, objectType, objectName, "Hidden");
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		internal static bool TryProcessObjectToBoolean(object value, out bool processedValue)
		{
			if (value is bool)
			{
				processedValue = (bool)value;
				return true;
			}
			if (value == null || DBNull.Value == value)
			{
				processedValue = false;
				return true;
			}
			processedValue = false;
			return false;
		}

		private bool EvaluateBinaryExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BinaryResult ProcessBinaryResult(VariantResult result)
		{
			BinaryResult result2 = default(BinaryResult);
			object value = result.Value;
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (value is byte[])
			{
				byte[] array = (byte[])value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					result2.ErrorOccurred = true;
					result2.Value = null;
					RegisterSandboxMaxArrayLengthWarning();
				}
				else
				{
					result2.Value = array;
				}
			}
			else if (value == null || DBNull.Value == value)
			{
				result2.Value = null;
			}
			else
			{
				if (value is string)
				{
					try
					{
						string text = (string)value;
						if (ViolatesMaxStringResultLength(text))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							RegisterSandboxMaxStringLengthWarning();
							return result2;
						}
						byte[] array2 = Convert.FromBase64String(text);
						if (array2 != null && ViolatesMaxArrayResultLength(array2.Length))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							RegisterSandboxMaxArrayLengthWarning();
							return result2;
						}
						result2.Value = array2;
						return result2;
					}
					catch (FormatException)
					{
						result2.ErrorOccurred = true;
						RegisterInvalidExpressionDataTypeWarning();
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private StringResult ProcessAutocastStringResult(VariantResult result)
		{
			return ProcessStringResult(result, autocast: true);
		}

		private StringResult ProcessStringResult(VariantResult result)
		{
			return ProcessStringResult(result, autocast: false);
		}

		private StringResult ProcessStringResult(VariantResult result, bool autocast)
		{
			StringResult result2 = default(StringResult);
			result2.Value = ProcessVariantResultToString(result, autocast, Severity.Warning, out bool errorOccured);
			result2.ErrorOccurred = errorOccured;
			result2.FieldStatus = result.FieldStatus;
			return result2;
		}

		private void ProcessLabelResult(ref VariantResult result)
		{
			result.Value = ProcessVariantResultToString(result, autocast: true, Severity.Error, out bool errorOccured);
			result.ErrorOccurred = errorOccured;
			if (errorOccured)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
		}

		private string ProcessVariantResultToString(VariantResult result, bool autocast, Severity severity, out bool errorOccured)
		{
			string output = null;
			if (result.ErrorOccurred)
			{
				errorOccured = true;
			}
			else
			{
				errorOccured = !ProcessObjectToString(result.Value, autocast, out output);
				if (errorOccured)
				{
					RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, severity);
				}
				else if (ViolatesMaxStringResultLength(output))
				{
					RegisterSandboxMaxStringLengthWarning();
				}
			}
			return output;
		}

		internal static bool ProcessObjectToString(object value, bool autocast, out string output)
		{
			output = null;
			bool flag = false;
			if (value == null || DBNull.Value == value)
			{
				output = null;
			}
			else if (value is string)
			{
				output = (string)value;
			}
			else if (value is char)
			{
				output = Convert.ToString((char)value, CultureInfo.CurrentCulture);
			}
			else if (value is Guid)
			{
				output = ((Guid)value).ToString();
			}
			else if (autocast)
			{
				output = value.ToString();
			}
			else
			{
				flag = true;
			}
			return !flag;
		}

		private bool EvaluateIntegerExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private IntegerResult ProcessIntegerResult(VariantResult result)
		{
			IntegerResult result2 = default(IntegerResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
				return result2;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				return result2;
			}
			if (!SetVariantType(ref result))
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				return result2;
			}
			if (result.TypeCode == TypeCode.Object)
			{
				ConvertFromSqlTypes(ref result);
			}
			switch (result.TypeCode)
			{
			case TypeCode.Int32:
				result2.Value = (int)result.Value;
				break;
			case TypeCode.Byte:
				result2.Value = Convert.ToInt32((byte)result.Value);
				break;
			case TypeCode.SByte:
				result2.Value = Convert.ToInt32((sbyte)result.Value);
				break;
			case TypeCode.Int16:
				result2.Value = Convert.ToInt32((short)result.Value);
				break;
			case TypeCode.UInt16:
				result2.Value = Convert.ToInt32((ushort)result.Value);
				break;
			case TypeCode.UInt32:
				try
				{
					result2.Value = Convert.ToInt32((uint)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Int64:
				try
				{
					result2.Value = Convert.ToInt32((long)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.UInt64:
				try
				{
					result2.Value = Convert.ToInt32((ulong)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Double:
				try
				{
					result2.Value = Convert.ToInt32((double)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Single:
				try
				{
					result2.Value = Convert.ToInt32((float)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Decimal:
				try
				{
					result2.Value = Convert.ToInt32((decimal)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			default:
				if (result.Value is TimeSpan)
				{
					try
					{
						result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				break;
			}
			return result2;
		}

		private bool EvaluateIntegerOrFloatExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else if (expression.ConstantType == DataType.Float)
				{
					result.Value = expression.FloatValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result = default(VariantResult);
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private FloatResult ProcessIntegerOrFloatResult(VariantResult result)
		{
			FloatResult result2 = default(FloatResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
				return result2;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				return result2;
			}
			if (!SetVariantType(ref result))
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				return result2;
			}
			if (result.TypeCode == TypeCode.Object)
			{
				ConvertFromSqlTypes(ref result);
			}
			switch (result.TypeCode)
			{
			case TypeCode.Int32:
				result2.Value = (int)result.Value;
				break;
			case TypeCode.Byte:
				result2.Value = Convert.ToInt32((byte)result.Value);
				break;
			case TypeCode.SByte:
				result2.Value = Convert.ToInt32((sbyte)result.Value);
				break;
			case TypeCode.Int16:
				result2.Value = Convert.ToInt32((short)result.Value);
				break;
			case TypeCode.UInt16:
				result2.Value = Convert.ToInt32((ushort)result.Value);
				break;
			case TypeCode.Double:
				result2.Value = (double)result.Value;
				break;
			case TypeCode.Single:
				result2.Value = Convert.ToDouble((float)result.Value);
				break;
			case TypeCode.UInt32:
				try
				{
					result2.Value = Convert.ToInt32((uint)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Int64:
				try
				{
					result2.Value = Convert.ToInt32((long)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.UInt64:
				try
				{
					result2.Value = Convert.ToInt32((ulong)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Decimal:
				try
				{
					result2.Value = Convert.ToDouble((decimal)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			default:
				if (result.Value is TimeSpan)
				{
					try
					{
						result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				break;
			}
			return result2;
		}

		private void ProcessLookupVariantResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, IErrorContext errorContext, bool isArrayRequired, bool normalizeDBNullToNull, ref VariantResult result)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayRequired, isArrayRequired, isByteArrayAllowed: false, normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					errorContext.Register(ProcessingErrorCode.rsLookupOfInvalidExpressionDataType, Severity.Warning);
					break;
				case NormalizationCode.StringLengthViolation:
					errorContext.Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, Convert.ToString(m_maxStringResultLength, CultureInfo.InvariantCulture));
					break;
				case NormalizationCode.ArrayLengthViolation:
					errorContext.Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, Convert.ToString(m_maxArrayResultLength, CultureInfo.InvariantCulture));
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private bool NormalizeVariantValue(VariantResult result, bool isArrayAllowed, bool isArrayRequired, bool isByteArrayAllowed, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			if (!isArrayRequired && GetVariantTypeCode(result.Value, out typeCode))
			{
				if (typeCode == TypeCode.String && ViolatesMaxStringResultLength((string)result.Value))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.StringLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
			}
			else if (!isArrayRequired && (result.Value == null || result.Value == DBNull.Value))
			{
				if (normalizeDBNullToNull)
				{
					normalizedValue = null;
				}
				else
				{
					normalizedValue = DBNull.Value;
				}
				typeCode = TypeCode.Empty;
			}
			else if (isArrayAllowed && result.Value is object[])
			{
				object[] array = (object[])result.Value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				if (!NormalizeVariantArray(array.GetEnumerator(), array, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
			}
			else if (!isArrayRequired && isByteArrayAllowed && result.Value is byte[])
			{
				byte[] array2 = (byte[])result.Value;
				if (ViolatesMaxArrayResultLength(array2.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
				typeCode = TypeCode.Object;
			}
			else if (isArrayAllowed && result.Value is IList)
			{
				IList list = (IList)result.Value;
				if (ViolatesMaxArrayResultLength(list.Count))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				object[] destArr = new object[list.Count];
				if (!NormalizeVariantArray(list.GetEnumerator(), destArr, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
			}
			else if (!isArrayRequired && result.Value is Guid)
			{
				normalizedValue = ((Guid)result.Value).ToString();
				typeCode = TypeCode.String;
			}
			else
			{
				if (result.Value == null || !ConvertFromSqlTypes(ref result))
				{
					typeCode = TypeCode.Empty;
					normalizedValue = null;
					normalCode = NormalizationCode.InvalidType;
					return false;
				}
				typeCode = TypeCode.Object;
				normalizedValue = result.Value;
			}
			normalCode = NormalizationCode.Success;
			return true;
		}

		private bool NormalizeVariantArray(IEnumerator source, object[] destArr, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			int num = 0;
			while (source.MoveNext())
			{
				VariantResult result = default(VariantResult);
				result.Value = source.Current;
				if (!NormalizeVariantValue(result, isArrayAllowed: false, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			typeCode = TypeCode.Object;
			normalCode = NormalizationCode.Success;
			return true;
		}

		private void ProcessReportParameterVariantResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayAllowed: true, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error);
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private bool ViolatesMaxStringResultLength(string value)
		{
			if (m_maxStringResultLength != -1 && value != null)
			{
				return value.Length > m_maxStringResultLength;
			}
			return false;
		}

		private bool ViolatesMaxArrayResultLength(int count)
		{
			if (m_maxArrayResultLength != -1)
			{
				return count > m_maxArrayResultLength;
			}
			return false;
		}

		private void ProcessSerializableResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool isReportScope, ref VariantResult result)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				ProcessSerializableResult(isReportScope, ref result);
			}
		}

		internal bool ProcessSerializableResult(bool isReportScope, ref VariantResult result)
		{
			bool result2 = false;
			if (!NormalizeVariantValue(result, isArrayAllowed: true, isArrayRequired: false, !m_rdlSandboxingEnabled, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					if (isReportScope)
					{
						try
						{
#pragma warning disable SYSLIB0050 // No non-obsolete replacement exists for checking the [Serializable] attribute bit via reflection; this only inspects the flag, it never invokes formatter-based serialization.
							if (result.Value is ISerializable || (result.Value.GetType().Attributes & TypeAttributes.Serializable) != 0)
#pragma warning restore SYSLIB0050
							{
								result.TypeCode = TypeCode.Object;
								if (m_isSerializableValuesProhibited)
								{
									((IErrorContext)this).Register(ProcessingErrorCode.rsSerializableTypeNotSupported, Severity.Error, new string[2]
									{
										m_objectType.ToString(),
										m_objectName
									});
									result.Value = null;
									return false;
								}
								if (!m_rdlSandboxingEnabled)
								{
									result.ErrorOccurred = false;
									return false;
								}
							}
							else
							{
								result2 = true;
								result.Value = null;
							}
						}
						catch (Exception ex)
						{
							((IErrorContext)this).Register(ProcessingErrorCode.rsUnexpectedSerializationError, Severity.Warning, new string[1]
							{
								ex.Message
							});
							result.Value = null;
						}
					}
					else
					{
						result.Value = null;
					}
					RegisterInvalidExpressionDataTypeWarning();
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			else
			{
				result.Value = normalizedValue;
				result.TypeCode = typeCode;
			}
			return result2;
		}

		private void ProcessVariantResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			ProcessVariantResult(expression, ref result, isArrayAllowed: false);
		}

		private void ProcessVariantResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isArrayAllowed)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayAllowed, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					RegisterInvalidExpressionDataTypeWarning();
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private void VerifyVariantResultAndStopOnError(ref VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				if (result.FieldStatus != 0)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, new string[1]
					{
						GetErrorName(result.FieldStatus, result.ExceptionMessage)
					});
				}
				else
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, Array.Empty<string>());
				}
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			if (result.Value == null)
			{
				result.Value = DBNull.Value;
			}
		}

		private void ProcessVariantOrBinaryResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isAggregate, bool allowArray)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, allowArray, isArrayRequired: false, isByteArrayAllowed: true, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				if (isAggregate)
				{
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
				}
				else
				{
					switch (normalCode)
					{
					case NormalizationCode.InvalidType:
						RegisterInvalidExpressionDataTypeWarning();
						break;
					case NormalizationCode.StringLengthViolation:
						RegisterSandboxMaxStringLengthWarning();
						break;
					case NormalizationCode.ArrayLengthViolation:
						RegisterSandboxMaxArrayLengthWarning();
						break;
					}
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private ParameterValueResult ProcessParameterValueResult(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string paramName, VariantResult result)
		{
			ParameterValueResult result2 = default(ParameterValueResult);
			if (expression != null)
			{
				if (expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					result2.Value = result.Value;
					result2.Type = expression.ConstantType;
				}
				else if (result.ErrorOccurred)
				{
					result2.ErrorOccurred = true;
				}
				else
				{
					if (!NormalizeParameterVariantValue(result.Value, paramName, out object normalizedValue, out DataType dataType) && Type.GetTypeCode(result.Value.GetType()) == TypeCode.Object && !ConvertFromSqlTypes(ref result))
					{
						result2.ErrorOccurred = true;
					}
					result2.Value = normalizedValue;
					result2.Type = dataType;
				}
			}
			return result2;
		}

		private bool NormalizeParameterVariantValue(object value, string paramName, out object normalizedValue, out DataType dataType)
		{
			if (value == null || value == DBNull.Value)
			{
				normalizedValue = null;
				dataType = DataType.String;
			}
			else if (value is bool)
			{
				normalizedValue = value;
				dataType = DataType.Boolean;
			}
			else if (value is DateTime)
			{
				normalizedValue = value;
				dataType = DataType.DateTime;
			}
			else if (value is double || value is float || value is decimal)
			{
				normalizedValue = Convert.ToDouble(value, CultureInfo.CurrentCulture);
				dataType = DataType.Float;
			}
			else if (value is string)
			{
				string text = (string)value;
				if (ViolatesMaxStringResultLength(text))
				{
					dataType = DataType.String;
					normalizedValue = null;
					RegisterSandboxMaxStringLengthWarning();
					return false;
				}
				normalizedValue = text;
				dataType = DataType.String;
			}
			else if (value is char)
			{
				normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
				dataType = DataType.String;
			}
			else if (value is int || value is short || value is byte || value is sbyte || value is ushort)
			{
				normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
				dataType = DataType.Integer;
			}
			else if (value is uint || value is long || value is ulong)
			{
				try
				{
					normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
					dataType = DataType.Integer;
				}
				catch (OverflowException)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsParameterValueCastFailure, Severity.Warning, m_objectType, m_objectName, m_propertyName, new string[1]
					{
						paramName
					});
					normalizedValue = value;
					dataType = DataType.Integer;
					return false;
				}
			}
			else if (value is TimeSpan)
			{
				try
				{
					normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
					dataType = DataType.String;
				}
				catch (FormatException)
				{
					RegisterInvalidExpressionDataTypeWarning();
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
			}
			else if (value is DateTimeOffset)
			{
				normalizedValue = value;
				dataType = DataType.DateTime;
			}
			else if (value is Guid)
			{
				normalizedValue = ((Guid)value).ToString();
				dataType = DataType.String;
			}
			else if (value is object[])
			{
				object[] array = (object[])value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				if (!NormalizeParameterVariantArray(array.GetEnumerator(), array, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
			}
			else
			{
				if (!(value is IList))
				{
					RegisterInvalidExpressionDataTypeWarning();
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				IList list = (IList)value;
				if (ViolatesMaxArrayResultLength(list.Count))
				{
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				object[] destArr = new object[list.Count];
				if (!NormalizeParameterVariantArray(list.GetEnumerator(), destArr, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
			}
			return true;
		}

		private bool NormalizeParameterVariantArray(IEnumerator source, object[] destArr, string paramName, out object normalizedValue, out DataType dataType)
		{
			dataType = DataType.String;
			int num = 0;
			while (source.MoveNext())
			{
				if (!NormalizeParameterVariantValue(source.Current, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			return true;
		}

		private static object[] GetAsObjectArray(ref VariantResult result)
		{
			object[] array = result.Value as object[];
			if (array == null)
			{
				IList list = result.Value as IList;
				if (list != null)
				{
					array = new object[list.Count];
					list.CopyTo(array, 0);
				}
			}
			return array;
		}

		private DataType GetDataType(object obj)
		{
			TypeCode typeCode = TypeCode.Empty;
			if (obj != null)
			{
				typeCode = Type.GetTypeCode(obj.GetType());
			}
			switch (typeCode)
			{
			case TypeCode.Boolean:
				return DataType.Boolean;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return DataType.Float;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return DataType.Integer;
			case TypeCode.DateTime:
				return DataType.DateTime;
			case TypeCode.Empty:
			case TypeCode.DBNull:
			case TypeCode.Char:
			case TypeCode.String:
				return DataType.String;
			default:
				if (obj is TimeSpan)
				{
					return DataType.Integer;
				}
				if (obj is DateTimeOffset)
				{
					return DataType.DateTime;
				}
				return DataType.String;
			}
		}

		private void SetNullResult(ref VariantResult result)
		{
			result.Value = null;
			result.TypeCode = TypeCode.Empty;
		}

		private void SetGuidResult(ref VariantResult result)
		{
			result.Value = ((Guid)result.Value).ToString();
			result.TypeCode = TypeCode.String;
		}

		private bool ConvertFromSqlTypes(ref VariantResult result)
		{
			if (result.Value is SqlInt32)
			{
				if (((SqlInt32)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int32;
					result.Value = ((SqlInt32)result.Value).Value;
				}
			}
			else if (result.Value is SqlInt16)
			{
				if (((SqlInt16)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int16;
					result.Value = ((SqlInt16)result.Value).Value;
				}
			}
			else if (result.Value is SqlInt64)
			{
				if (((SqlInt64)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int64;
					result.Value = ((SqlInt64)result.Value).Value;
				}
			}
			else if (result.Value is SqlBoolean)
			{
				if (((SqlBoolean)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Boolean;
					result.Value = ((SqlBoolean)result.Value).Value;
				}
			}
			else if (result.Value is SqlDecimal)
			{
				if (((SqlDecimal)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlDecimal)result.Value).Value;
				}
			}
			else if (result.Value is SqlDouble)
			{
				if (((SqlDouble)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Double;
					result.Value = ((SqlDouble)result.Value).Value;
				}
			}
			else if (result.Value is SqlDateTime)
			{
				if (((SqlDateTime)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.DateTime;
					result.Value = ((SqlDateTime)result.Value).Value;
				}
			}
			else if (result.Value is SqlMoney)
			{
				if (((SqlMoney)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlMoney)result.Value).Value;
				}
			}
			else if (result.Value is SqlSingle)
			{
				if (((SqlSingle)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Single;
					result.Value = ((SqlSingle)result.Value).Value;
				}
			}
			else if (result.Value is SqlByte)
			{
				if (((SqlByte)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Byte;
					result.Value = ((SqlByte)result.Value).Value;
				}
			}
			else if (result.Value is SqlString)
			{
				if (((SqlString)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.String;
					string value = ((SqlString)result.Value).Value;
					if (ViolatesMaxStringResultLength(value))
					{
						value = null;
						RegisterSandboxMaxStringLengthWarning();
					}
					result.Value = value;
				}
			}
			else
			{
				if (!(result.Value is SqlGuid))
				{
					return false;
				}
				if (((SqlGuid)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					SetGuidResult(ref result);
				}
			}
			return true;
		}

		private bool EvaluateNoConstantExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = new VariantResult(errorOccurred: true, null);
				RegisterInvalidExpressionDataTypeWarning();
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		internal static bool GetVariantTypeCode(object o, out TypeCode typeCode)
		{
			if (o == null)
			{
				typeCode = TypeCode.Empty;
			}
			else
			{
				Type type = o.GetType();
				typeCode = Type.GetTypeCode(type);
				switch (typeCode)
				{
				case TypeCode.Empty:
				case TypeCode.DBNull:
					return false;
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				}
				if (o is TimeSpan || o is DateTimeOffset)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool SetVariantType(ref VariantResult result)
		{
			if (GetVariantTypeCode(result.Value, out TypeCode typeCode))
			{
				result.TypeCode = typeCode;
				return true;
			}
			return false;
		}

		internal static string ConvertToStringFallBack(object value)
		{
			try
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}", value);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void RegisterInvalidExpressionDataTypeWarning()
		{
			RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning);
		}

		private void RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode errorCode, Severity severity)
		{
			((IErrorContext)this).Register(errorCode, severity, Array.Empty<string>());
		}

		private void RegisterSandboxMaxStringLengthWarning()
		{
			ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(m_maxStringResultLength, CultureInfo.InvariantCulture)
			});
		}

		private void RegisterSandboxMaxArrayLengthWarning()
		{
			ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(m_maxArrayResultLength, CultureInfo.InvariantCulture)
			});
		}

		internal object MinValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) > 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		internal object MaxValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) < 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		private int CompareWithExtendedTypesAndStopOnError(object x, object y)
		{
			return m_reportObjectModel.OdpContext.CompareAndStopOnError(x, y, m_objectType, m_objectName, m_propertyName, extendedTypeComparisons: true);
		}

		internal int RecursiveLevel(string scope)
		{
			if (m_currentScope == null)
			{
				return 0;
			}
			int num = m_currentScope.RecursiveLevel(scope);
			if (-1 == num)
			{
				return 0;
			}
			return num;
		}

		internal void LoadCompiledCode(IExpressionHostAssemblyHolder expressionHostAssemblyHolder, bool includeParameters, bool parametersOnly, ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ReportRuntimeSetup runtimeSetup)
		{
			if (expressionHostAssemblyHolder.CompiledCode == null || expressionHostAssemblyHolder.CompiledCode.Length == 0)
			{
				return;
			}
			try
			{
				if (runtimeSetup.RequireExpressionHostWithRefusedPermissions && !expressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace("Expression host generated with refused permissions is required.");
					}
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (runtimeSetup.AssemblyLoadContext == null)
				{
					m_exprHostInSandboxAppDomain = false;
					if (expressionHostAssemblyHolder.CodeModules != null)
					{
						for (int i = 0; i < expressionHostAssemblyHolder.CodeModules.Count; i++)
						{
							if (!runtimeSetup.CheckCodeModuleIsTrustedInCurrentAppDomain(expressionHostAssemblyHolder.CodeModules[i]))
							{
								m_errorContext.Register(ProcessingErrorCode.rsUntrustedCodeModule, Severity.Error, expressionHostAssemblyHolder.ObjectType, null, null, expressionHostAssemblyHolder.CodeModules[i]);
								throw new ReportProcessingException(m_errorContext.Messages);
							}
						}
					}
					m_reportExprHost = ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(expressionHostAssemblyHolder.CompiledCode, expressionHostAssemblyHolder.ExprHostAssemblyName, runtimeSetup.ExprHostEvidence, includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
				else
				{
					m_exprHostInSandboxAppDomain = true;
					m_reportExprHost = ExpressionHostLoader.LoadExprHost(expressionHostAssemblyHolder.CompiledCode, expressionHostAssemblyHolder.ExprHostAssemblyName, includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
			}
			catch (ReportProcessingException)
			{
				throw;
			}
			catch (Exception e)
			{
				ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorLoadingExprHostAssembly);
			}
		}

		internal void CustomCodeOnInit(IExpressionHostAssemblyHolder expressionHostAssemblyHolder)
		{
			if (expressionHostAssemblyHolder.CompiledCode.Length != 0)
			{
				try
				{
					m_reportExprHost.CustomCodeOnInit();
				}
				catch (ReportProcessingException)
				{
					throw;
				}
				catch (Exception e)
				{
					ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorInOnInit);
				}
			}
		}

		private void ProcessLoadingExprHostException(ReportingServices.ReportProcessing.ObjectType assemblyHolderObjectType, Exception e, ProcessingErrorCode errorCode)
		{
			if (e != null && e is TargetInvocationException && e.InnerException != null)
			{
				e = e.InnerException;
			}
			string str = null;
			string text;
			if (e != null)
			{
				try
				{
					text = e.Message;
					str = e.ToString();
				}
				catch
				{
					text = RPRes.NonClsCompliantException;
				}
			}
			else
			{
				text = RPRes.NonClsCompliantException;
			}
			ProcessingMessage processingMessage = m_errorContext.Register(errorCode, Severity.Error, assemblyHolderObjectType, null, null, text);
			if (Global.Tracer.TraceError && processingMessage != null)
			{
				Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + str);
			}
			throw new ReportProcessingException(m_errorContext.Messages);
		}

		internal void Close()
		{
			m_reportExprHost = null;
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportRuntime;
		}

		public void SetID(int id)
		{
			m_id = id;
		}

		internal string EvaluateBaseGaugeImageSourceExpression(ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.Source, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Source, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageStringValueExpression(ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.Value, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal byte[] EvaluateBaseGaugeImageBinaryValueExpression(ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(baseGaugeImage.Value, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateBaseGaugeImageMIMETypeExpression(ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.MIMEType, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.MIMEType, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageTransparentColorExpression(ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.TransparentColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.TransparentColor, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateGaugeImageSourceExpression(ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.Source, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.Source, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateGaugeImageValueExpression(ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.Value, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.Value, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeImage.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeImageTransparentColorExpression(ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.TransparentColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.TransparentColor, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateCapImageHueColorExpression(ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.HueColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.HueColor, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateCapImageOffsetXExpression(ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.OffsetX, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.OffsetX, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateCapImageOffsetYExpression(ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.OffsetY, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.OffsetY, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateFrameImageHueColorExpression(ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.HueColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.HueColor, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateFrameImageTransparencyExpression(ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.Transparency, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.Transparency, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateFrameImageClipImageExpression(ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.ClipImage, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipImage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.ClipImage, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).ClipImageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateTopImageHueColorExpression(ReportingServices.ReportIntermediateFormat.TopImage topImage, string objectName)
		{
			if (!EvaluateSimpleExpression(topImage.HueColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(topImage.HueColor, ref result, topImage.ExprHost))
					{
						Global.Tracer.Assert(topImage.ExprHost != null, "(topImage.ExprHost != null)");
						result.Value = ((TopImageExprHost)topImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateBackFrameFrameStyleExpression(ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameStyle, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBackFrameFrameShapeExpression(ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameShape, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameShape", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameShape, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameShapeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateBackFrameFrameWidthExpression(ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameWidth, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameWidth, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateBackFrameGlassEffectExpression(ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.GlassEffect, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GlassEffect", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.GlassEffect, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.GlassEffectExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePanelAntiAliasingExpression(ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.AntiAliasing, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AntiAliasing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.AntiAliasing, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePanelAutoLayoutExpression(ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.AutoLayout, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AutoLayout", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.AutoLayout, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AutoLayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePanelShadowIntensityExpression(ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.ShadowIntensity, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShadowIntensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.ShadowIntensity, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePanelTextAntiAliasingQualityExpression(ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.TextAntiAliasingQuality, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextAntiAliasingQuality", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.TextAntiAliasingQuality, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePanelItemTopExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Top, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Top", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Top, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemLeftExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Left, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Left, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemHeightExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Height, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Height", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Height, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemWidthExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Width, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Width, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal int EvaluateGaugePanelItemZIndexExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.ZIndex, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ZIndex", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.ZIndex, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateGaugePanelItemHiddenExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Hidden, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugePanelItemToolTipExpression(ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.ToolTip, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.ToolTip, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerBarStartExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.BarStart, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BarStart", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.BarStart, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.BarStartExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePointerDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.DistanceFromScale, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePointerMarkerLengthExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.MarkerLength, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerLength", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.MarkerLength, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerLengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerMarkerStyleExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.MarkerStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.MarkerStyle, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerPlacementExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Placement, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerSnappingEnabledExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.SnappingEnabled, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.SnappingEnabled, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerSnappingIntervalExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.SnappingInterval, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.SnappingInterval, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerToolTipExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.ToolTip, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.ToolTip, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerHiddenExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Hidden, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerWidthExpression(ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Width, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Width, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Interval, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Interval, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.IntervalOffset, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleLogarithmicExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Logarithmic, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Logarithmic", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Logarithmic, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleLogarithmicBaseExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.LogarithmicBase, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LogarithmicBase", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.LogarithmicBase, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicBaseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleMultiplierExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Multiplier, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Multiplier, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleReversedExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Reversed, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reversed", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Reversed, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeScaleTickMarksOnTopExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.TickMarksOnTop, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TickMarksOnTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.TickMarksOnTop, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.TickMarksOnTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugeScaleToolTipExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.ToolTip, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.ToolTip, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugeScaleHiddenExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Hidden, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleWidthExpression(ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Width, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Width, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalExpression(ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeTickMarks.Interval, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeTickMarks.Interval, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeTickMarks.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeTickMarks.IntervalOffset, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateLinearGaugeOrientationExpression(ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(linearGauge.Orientation, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Orientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearGauge.Orientation, ref result, linearGauge.ExprHost))
					{
						Global.Tracer.Assert(linearGauge.ExprHost != null, "(linearGauge.ExprHost != null)");
						result.Value = ((LinearGaugeExprHost)linearGauge.ExprHost).OrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateLinearPointerTypeExpression(ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(linearPointer.Type, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearPointer.Type, ref result, linearPointer.ExprHost))
					{
						Global.Tracer.Assert(linearPointer.ExprHost != null, "(linearPointer.ExprHost != null)");
						result.Value = ((LinearPointerExprHost)linearPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateLinearScaleStartMarginExpression(ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.StartMargin, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.StartMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).StartMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScaleEndMarginExpression(ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.EndMargin, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.EndMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).EndMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScalePositionExpression(ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.Position, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.Position, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).PositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelTextExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.Text, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.Text, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelAllowUpsideDownExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.AllowUpsideDown, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.AllowUpsideDown, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePinLabelDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.DistanceFromScale, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluatePinLabelFontAngleExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.FontAngle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.FontAngle, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.Placement, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelRotateLabelExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.RotateLabel, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.RotateLabel, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePinLabelUseFontPercentExpression(ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.UseFontPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.UseFontPercent, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapOnTopExpression(ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.OnTop, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OnTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.OnTop, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.OnTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapReflectionExpression(ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Reflection, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reflection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Reflection, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.ReflectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluatePointerCapCapStyleExpression(ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.CapStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "CapStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.CapStyle, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.CapStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePointerCapHiddenExpression(ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Hidden, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePointerCapWidthExpression(ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Width, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Width, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageHueColorExpression(ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.HueColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.HueColor, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluatePointerImageTransparencyExpression(ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.Transparency, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.Transparency, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageOffsetXExpression(ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.OffsetX, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.OffsetX, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluatePointerImageOffsetYExpression(ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.OffsetY, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.OffsetY, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal double EvaluateRadialGaugePivotXExpression(ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(radialGauge.PivotX, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialGauge.PivotX, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialGaugePivotYExpression(ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(radialGauge.PivotY, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialGauge.PivotY, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateRadialPointerTypeExpression(ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(radialPointer.Type, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialPointer.Type, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateRadialPointerNeedleStyleExpression(ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(radialPointer.NeedleStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NeedleStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialPointer.NeedleStyle, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).NeedleStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateRadialScaleRadiusExpression(ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.Radius, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Radius", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.Radius, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).RadiusExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleStartAngleExpression(ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.StartAngle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.StartAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).StartAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleSweepAngleExpression(ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.SweepAngle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SweepAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.SweepAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).SweepAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Interval, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Interval, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalOffsetExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.IntervalOffset, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.IntervalOffset, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScaleLabelsAllowUpsideDownExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.AllowUpsideDown, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.AllowUpsideDown, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleLabelsDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.DistanceFromScale, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsFontAngleExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.FontAngle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.FontAngle, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleLabelsPlacementExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Placement, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleLabelsRotateLabelsExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.RotateLabels, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.RotateLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.RotateLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsShowEndLabelsExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.ShowEndLabels, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.ShowEndLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.ShowEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsHiddenExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Hidden, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsUseFontPercentExpression(ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.UseFontPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.UseFontPercent, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScalePinLocationExpression(ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			if (!EvaluateSimpleExpression(scalePin.Location, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Location", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scalePin.Location, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).LocationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScalePinEnableExpression(ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			if (!EvaluateSimpleExpression(scalePin.Enable, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Enable", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scalePin.Enable, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).EnableExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleRangeDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.DistanceFromScale, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeStartWidthExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.StartWidth, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.StartWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.StartWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeEndWidthExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.EndWidth, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.EndWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.EndWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleRangeInRangeBarPointerColorExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeBarPointerColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeBarPointerColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeBarPointerColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeBarPointerColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangeInRangeLabelColorExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeLabelColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeLabelColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeLabelColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeLabelColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangeInRangeTickMarksColorExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeTickMarksColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeTickMarksColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeTickMarksColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeTickMarksColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangePlacementExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.Placement, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateScaleRangeToolTipExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.ToolTip, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.ToolTip, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleRangeHiddenExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.Hidden, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateScaleRangeBackgroundGradientTypeExpression(ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.BackgroundGradientType, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BackgroundGradientType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.BackgroundGradientType, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateThermometerBulbOffsetExpression(ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.BulbOffset, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.BulbOffset, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateThermometerBulbSizeExpression(ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.BulbSize, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.BulbSize, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateThermometerThermometerStyleExpression(ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.ThermometerStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ThermometerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.ThermometerStyle, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.ThermometerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateTickMarkStyleDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.DistanceFromScale, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStylePlacementExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Placement, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleEnableGradientExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.EnableGradient, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EnableGradient", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.EnableGradient, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.EnableGradientExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateTickMarkStyleGradientDensityExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.GradientDensity, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GradientDensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.GradientDensity, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.GradientDensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleLengthExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Length, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Length", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Length, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleWidthExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Width, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Width, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStyleShapeExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Shape, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Shape", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Shape, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.ShapeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleHiddenExpression(ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Hidden, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateCustomLabelTextExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Text, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Text, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(customLabel.Text, ref result);
			return result;
		}

		internal bool EvaluateCustomLabelAllowUpsideDownExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.AllowUpsideDown, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.AllowUpsideDown, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelDistanceFromScaleExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.DistanceFromScale, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.DistanceFromScale, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateCustomLabelFontAngleExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.FontAngle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.FontAngle, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateCustomLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Placement, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Placement, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateCustomLabelRotateLabelExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.RotateLabel, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.RotateLabel, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelValueExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Value, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Value, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateCustomLabelHiddenExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Hidden, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Hidden, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateCustomLabelUseFontPercentExpression(ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.UseFontPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.UseFontPercent, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeClipContentExpression(ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			if (!EvaluateSimpleExpression(gauge.ClipContent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipContent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gauge.ClipContent, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).ClipContentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeAspectRatioExpression(ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			if (!EvaluateSimpleExpression(gauge.AspectRatio, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AspectRatio", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gauge.AspectRatio, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).AspectRatioExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeInputValueValueExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Value, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Value, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeInputValue.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeInputValueFormulaExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Formula, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Formula", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Formula, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.FormulaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMinPercentExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.MinPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MinPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.MinPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MinPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMaxPercentExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.MaxPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MaxPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.MaxPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MaxPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMultiplierExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Multiplier, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Multiplier, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueAddConstantExpression(ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.AddConstant, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AddConstant", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.AddConstant, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.AddConstantExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeLabelTextExpression(ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.Text, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.Text, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeLabel.Text, ref result);
			return result;
		}

		internal double EvaluateGaugeLabelAngleExpression(ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.Angle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.Angle, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugeLabelResizeModeExpression(ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.ResizeMode, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.ResizeMode, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugeLabelTextShadowOffsetExpression(ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.TextShadowOffset, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.TextShadowOffset, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateGaugeLabelUseFontPercentExpression(ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.UseFontPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.UseFontPercent, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorDecimalDigitColorExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DecimalDigitColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DecimalDigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorDigitColorExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DigitColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateNumericIndicatorUseFontPercentExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.UseFontPercent, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.UseFontPercent, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDecimalDigitsExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DecimalDigits, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigits", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DecimalDigits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDigitsExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.Digits, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Digits", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.Digits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal double EvaluateNumericIndicatorMultiplierExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.Multiplier, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.Multiplier, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorNonNumericStringExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.NonNumericString, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NonNumericString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.NonNumericString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.NonNumericStringExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorOutOfRangeStringExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.OutOfRangeString, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OutOfRangeString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.OutOfRangeString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.OutOfRangeStringExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorResizeModeExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ResizeMode, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ResizeMode, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowDecimalPointExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowDecimalPoint, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowDecimalPoint", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowDecimalPoint, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowDecimalPointExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowLeadingZerosExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowLeadingZeros, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowLeadingZeros", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowLeadingZeros, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowLeadingZerosExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorIndicatorStyleExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.IndicatorStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.IndicatorStyle, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorShowSignExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowSign, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowSign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowSign, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowSignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorSnappingEnabledExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SnappingEnabled, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SnappingEnabled, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateNumericIndicatorSnappingIntervalExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SnappingInterval, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SnappingInterval, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorLedDimColorExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.LedDimColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LedDimColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.LedDimColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.LedDimColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateNumericIndicatorSeparatorWidthExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SeparatorWidth, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SeparatorWidth, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorSeparatorColorExpression(ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SeparatorColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SeparatorColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorRangeDecimalDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicatorRange.DecimalDigitColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicatorRange.DecimalDigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorRangeDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicatorRange.DigitColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicatorRange.DigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateIndicatorImageHueColorExpression(ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorImage.HueColor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorImage.HueColor, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateIndicatorImageTransparencyExpression(ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorImage.Transparency, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorImage.Transparency, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorTransformationTypeExpression(ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.TransformationType, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransformationType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.TransformationType, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.TransformationTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStateIndicatorIndicatorStyleExpression(ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.IndicatorStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.IndicatorStyle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorScaleFactorExpression(ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.ScaleFactor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.ScaleFactor, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorResizeModeExpression(ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.ResizeMode, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.ResizeMode, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorAngleExpression(ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.Angle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.Angle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateColorExpression(ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.Color, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.Color, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateIndicatorStateScaleFactorExpression(ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.ScaleFactor, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.ScaleFactor, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateIndicatorStyleExpression(ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.IndicatorStyle, ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.IndicatorStyle, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLocationLeftExpression(ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Left, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Left, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLocationTopExpression(ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Top, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Top", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Top, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapLocationUnitExpression(ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Unit, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Unit, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapSizeWidthExpression(ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Width, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Width, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSizeHeightExpression(ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Height, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Height", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Height, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapSizeUnitExpression(ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Unit, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Unit, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapGridLinesHiddenExpression(ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.Hidden, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.Hidden, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapGridLinesIntervalExpression(ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.Interval, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.Interval, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapGridLinesShowLabelsExpression(ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.ShowLabels, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.ShowLabels, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.ShowLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapGridLinesLabelPositionExpression(ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.LabelPosition, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPosition", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.LabelPosition, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.LabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapDockableSubItemPositionExpression(ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.Position, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.Position, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.MapPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemDockOutsideViewportExpression(ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.DockOutsideViewport, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DockOutsideViewport", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.DockOutsideViewport, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.DockOutsideViewportExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemHiddenExpression(ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.Hidden, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.Hidden, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateMapDockableSubItemToolTipExpression(ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.ToolTip, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapDockableSubItem.ToolTip, ref result, mapDockableSubItem.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
					result.Value = mapDockableSubItem.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapSubItemLeftMarginExpression(ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.LeftMargin, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LeftMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.LeftMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemRightMarginExpression(ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.RightMargin, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RightMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.RightMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemTopMarginExpression(ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.TopMargin, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TopMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.TopMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemBottomMarginExpression(ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.BottomMargin, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BottomMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.BottomMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int EvaluateMapSubItemZIndexExpression(ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.ZIndex, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ZIndex", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.ZIndex, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapBindingFieldPairFieldNameExpression(ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBindingFieldPair.FieldName, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "FieldName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBindingFieldPair.FieldName, ref result, mapBindingFieldPair.ExprHost))
					{
						Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
						result.Value = mapBindingFieldPair.ExprHost.FieldNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapBindingFieldPairBindingExpressionExpression(ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBindingFieldPair.BindingExpression, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BindingExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBindingFieldPair.BindingExpression, ref result, null))
					{
						if (mapBindingFieldPair.InElementView)
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
							if (m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHost.SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHost.BindingExpressionExpr;
						}
						else
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHostMapMember != null, "(mapBindingFieldPair.ExprHostMapMember != null)");
							if (m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHostMapMember.SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHostMapMember.BindingExpressionExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBindingFieldPair.BindingExpression, ref result);
			return result;
		}

		internal string EvaluateMapViewportMapCoordinateSystemExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MapCoordinateSystem, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapCoordinateSystem", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MapCoordinateSystem, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapCoordinateSystemExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapViewportMapProjectionExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MapProjection, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapProjection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MapProjection, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapProjectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterXExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ProjectionCenterX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ProjectionCenterX, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterYExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ProjectionCenterY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ProjectionCenterY, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMaximumZoomExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MaximumZoom, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MaximumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMinimumZoomExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MinimumZoom, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MinimumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportSimplificationResolutionExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.SimplificationResolution, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SimplificationResolution", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.SimplificationResolution, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.SimplificationResolutionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapViewportContentMarginExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ContentMargin, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ContentMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ContentMargin, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ContentMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapViewportGridUnderContentExpression(ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.GridUnderContent, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "GridUnderContent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.GridUnderContent, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.GridUnderContentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumXExpression(ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MinimumX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MinimumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumYExpression(ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MinimumY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MinimumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumXExpression(ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MaximumX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MaximumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumYExpression(ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MaximumY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MaximumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapLimitsLimitToDataExpression(ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.LimitToData, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LimitToData", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.LimitToData, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.LimitToDataExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleTickMarkLengthExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.TickMarkLength, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TickMarkLength", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.TickMarkLength, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.TickMarkLengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapColorScaleColorBarBorderColorExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.ColorBarBorderColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ColorBarBorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.ColorBarBorderColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.ColorBarBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal int EvaluateMapColorScaleLabelIntervalExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelInterval, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelInterval, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelFormatExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelFormat, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelFormat", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelFormat, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelFormatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelPlacement, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelPlacement, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelBehaviorExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelBehavior, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelBehavior", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelBehavior, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapColorScaleHideEndLabelsExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.HideEndLabels, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "HideEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.HideEndLabels, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleRangeGapColorExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.RangeGapColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RangeGapColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.RangeGapColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.RangeGapColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorScaleNoDataTextExpression(ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.NoDataText, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "NoDataText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.NoDataText, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.NoDataTextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapColorScaleTitleCaptionExpression(ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScaleTitle.Caption, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapColorScaleTitle.Caption, ref result, mapColorScaleTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapColorScaleTitle.ExprHost != null, "(mapColorScaleTitle.ExprHost != null)");
					result.Value = mapColorScaleTitle.ExprHost.CaptionExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapDistanceScaleScaleColorExpression(ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDistanceScale.ScaleColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDistanceScale.ScaleColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapDistanceScaleScaleBorderColorExpression(ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDistanceScale.ScaleBorderColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleBorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDistanceScale.ScaleBorderColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateMapTitleTextExpression(ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.Text, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapTitle.Text, ref result, mapTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
					result.Value = mapTitle.ExprHost.TextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal double EvaluateMapTitleAngleExpression(ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.Angle, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTitle.Angle, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTitleTextShadowOffsetExpression(ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.TextShadowOffset, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTitle.TextShadowOffset, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLegendLayoutExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.Layout, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Layout", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.Layout, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLegendAutoFitTextDisabledExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.AutoFitTextDisabled, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AutoFitTextDisabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.AutoFitTextDisabled, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendMinFontSizeExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.MinFontSize, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.MinFontSize, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapLegendInterlacedRowsExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.InterlacedRows, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRows", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.InterlacedRows, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendInterlacedRowsColorExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.InterlacedRowsColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRowsColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.InterlacedRowsColor, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateMapLegendEquallySpacedItemsExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.EquallySpacedItems, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EquallySpacedItems", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.EquallySpacedItems, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateMapLegendTextWrapThresholdExpression(ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.TextWrapThreshold, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextWrapThreshold", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.TextWrapThreshold, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapLegendTitleCaptionExpression(ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.Caption, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapLegendTitle.Caption, ref result, mapLegendTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
					result.Value = mapLegendTitle.ExprHost.CaptionExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorExpression(ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.TitleSeparator, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegendTitle.TitleSeparator, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorColorExpression(ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.TitleSeparatorColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegendTitle.TitleSeparatorColor, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateMapAppearanceRuleDataValueExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.DataValue, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.DataValue, ref result, mapAppearanceRule.ExprHostMapMember))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHostMapMember != null || mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHostMapMember != null) || (mapAppearanceRule.ExprHost != null)");
						if (mapAppearanceRule.ExprHostMapMember != null)
						{
							result.Value = mapAppearanceRule.ExprHostMapMember.DataValueExpr;
						}
						else
						{
							result.Value = mapAppearanceRule.ExprHost.DataValueExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.DataValue, ref result);
			return result;
		}

		internal string EvaluateMapAppearanceRuleDistributionTypeExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.DistributionType, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DistributionType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.DistributionType, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.DistributionTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateMapAppearanceRuleBucketCountExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.BucketCount, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BucketCount", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.BucketCount, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.BucketCountExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapAppearanceRuleStartValueExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.StartValue, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.StartValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleEndValueExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.EndValue, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.EndValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.EndValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleLegendTextExpression(ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.LegendText, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapAppearanceRule.LegendText, ref result, mapAppearanceRule.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
					result.Value = mapAppearanceRule.ExprHost.LegendTextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapBucketStartValueExpression(ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBucket.StartValue, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBucket.StartValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBucket.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapBucketEndValueExpression(ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBucket.EndValue, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBucket.EndValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBucket.EndValue, ref result);
			return result;
		}

		internal string EvaluateMapColorPaletteRulePaletteExpression(ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorPaletteRule.Palette, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Palette", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorPaletteRule.Palette, ref result, mapColorPaletteRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorPaletteRule.ExprHost != null, "(mapColorPaletteRule.ExprHost != null)");
						result.Value = mapColorPaletteRule.ExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorRangeRuleStartColorExpression(ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.StartColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.StartColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.StartColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorRangeRuleMiddleColorExpression(ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.MiddleColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MiddleColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.MiddleColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.MiddleColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorRangeRuleEndColorExpression(ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.EndColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.EndColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.EndColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateMapColorRuleShowInColorScaleExpression(ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRule.ShowInColorScale, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowInColorScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRule.ShowInColorScale, ref result, mapColorRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRule.ExprHost != null, "(mapColorRule.ExprHost != null)");
						result.Value = mapColorRule.ExprHost.ShowInColorScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapSizeRuleStartSizeExpression(ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSizeRule.StartSize, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSizeRule.StartSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.StartSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSizeRuleEndSizeExpression(ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSizeRule.EndSize, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSizeRule.EndSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.EndSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapMarkerImageSourceExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Source, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Source, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapMarkerImageValueExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Value, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapMarkerImage.Value, ref result);
			return result;
		}

		internal string EvaluateMapMarkerImageStringValueExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Value, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal byte[] EvaluateMapMarkerImageBinaryValueExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(mapMarkerImage.Value, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateMapMarkerImageMIMETypeExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.MIMEType, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.MIMEType, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerImageTransparentColorExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.TransparentColor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.TransparentColor, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapMarkerImageResizeModeExpression(ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.ResizeMode, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.ResizeMode, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerMapMarkerStyleExpression(ReportingServices.ReportIntermediateFormat.MapMarker mapMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarker.MapMarkerStyle, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapMarkerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarker.MapMarkerStyle, ref result, mapMarker.ExprHost))
					{
						Global.Tracer.Assert(mapMarker.ExprHost != null, "(mapMarker.ExprHost != null)");
						result.Value = mapMarker.ExprHost.MapMarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapCustomColorColorExpression(ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomColor.Color, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomColor.Color, ref result, mapCustomColor.ExprHost))
					{
						Global.Tracer.Assert(mapCustomColor.ExprHost != null, "(mapCustomColor.ExprHost != null)");
						result.Value = mapCustomColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapLineTemplateWidthExpression(ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLineTemplate.Width, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLineTemplate.Width, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLineTemplateLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLineTemplate.LabelPlacement, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLineTemplate.LabelPlacement, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateScaleFactorExpression(ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.ScaleFactor, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.ScaleFactor, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetXExpression(ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetX, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetYExpression(ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetY, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateShowLabelExpression(ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.ShowLabel, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.ShowLabel, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ShowLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.LabelPlacement, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.LabelPlacement, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapLabelTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapSpatialElementTemplateHiddenExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.Hidden, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.Hidden, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetXExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.OffsetX, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetYExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.OffsetY, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateLabelExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.Label, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.Label, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.LabelExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateDataElementLabelExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.DataElementLabel, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataElementLabel", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.DataElementLabel, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.DataElementLabelExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateToolTipExpression(ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.ToolTip, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.ToolTip, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapPointTemplateSizeExpression(ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPointTemplate.Size, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Size", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPointTemplate.Size, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapPointTemplateLabelPlacementExpression(ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPointTemplate.LabelPlacement, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPointTemplate.LabelPlacement, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLineUseCustomLineTemplateExpression(ReportingServices.ReportIntermediateFormat.MapLine mapLine, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLine.UseCustomLineTemplate, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomLineTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLine.UseCustomLineTemplate, ref result, mapLine.ExprHost))
					{
						Global.Tracer.Assert(mapLine.ExprHost != null, "(mapLine.ExprHost != null)");
						result.Value = mapLine.ExprHost.UseCustomLineTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPolygonTemplateExpression(ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygon.UseCustomPolygonTemplate, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPolygonTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygon.UseCustomPolygonTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPolygonTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPointTemplateExpression(ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygon.UseCustomCenterPointTemplate, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomCenterPointTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygon.UseCustomCenterPointTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPointUseCustomPointTemplateExpression(ReportingServices.ReportIntermediateFormat.MapPoint mapPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPoint.UseCustomPointTemplate, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPointTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPoint.UseCustomPointTemplate, ref result, mapPoint.ExprHost))
					{
						Global.Tracer.Assert(mapPoint.ExprHost != null, "(mapPoint.ExprHost != null)");
						result.Value = mapPoint.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapFieldNameNameExpression(ReportingServices.ReportIntermediateFormat.MapFieldName mapFieldName, string objectName)
		{
			if (!EvaluateSimpleExpression(mapFieldName.Name, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Name", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapFieldName.Name, ref result, mapFieldName.ExprHost))
					{
						Global.Tracer.Assert(mapFieldName.ExprHost != null, "(mapFieldName.ExprHost != null)");
						result.Value = mapFieldName.ExprHost.NameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLayerVisibilityModeExpression(ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.VisibilityMode, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VisibilityMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.VisibilityMode, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.VisibilityModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLayerMinimumZoomExpression(ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.MinimumZoom, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.MinimumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerMaximumZoomExpression(ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.MaximumZoom, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.MaximumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerTransparencyExpression(ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.Transparency, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.Transparency, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapShapefileSourceExpression(ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile, string objectName)
		{
			if (!EvaluateSimpleExpression(mapShapefile.Source, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapShapefile.Source, ref result, mapShapefile.ExprHost))
					{
						Global.Tracer.Assert(mapShapefile.ExprHost != null, "(mapShapefile.ExprHost != null)");
						result.Value = mapShapefile.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialDataRegionVectorDataExpression(ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataRegion.VectorData, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VectorData", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataRegion.VectorData, ref result, mapSpatialDataRegion.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataRegion.ExprHost != null, "(mapSpatialDataRegion.ExprHost != null)");
						result.Value = mapSpatialDataRegion.ExprHost.VectorDataExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapSpatialDataRegion.VectorData, ref result);
			return result;
		}

		internal string EvaluateMapSpatialDataSetDataSetNameExpression(ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataSet.DataSetName, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataSetName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataSet.DataSetName, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.DataSetNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapSpatialDataSetSpatialFieldExpression(ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataSet.SpatialField, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SpatialField", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataSet.SpatialField, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.SpatialFieldExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerServiceUrlExpression(ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.ServiceUrl, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ServiceUrl", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.ServiceUrl, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.ServiceUrlExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerTileStyleExpression(ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.TileStyle, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.TileStyle, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.TileStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapTileLayerUseSecureConnectionExpression(ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.UseSecureConnection, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseSecureConnection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.UseSecureConnection, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.UseSecureConnectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapAntiAliasingExpression(ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.AntiAliasing, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AntiAliasing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.AntiAliasing, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTextAntiAliasingQualityExpression(ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.TextAntiAliasingQuality, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextAntiAliasingQuality", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.TextAntiAliasingQuality, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapShadowIntensityExpression(ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.ShadowIntensity, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShadowIntensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.ShadowIntensity, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTileLanguageExpression(ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.TileLanguage, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.TileLanguage, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TileLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapBorderSkinMapBorderSkinTypeExpression(ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBorderSkin.MapBorderSkinType, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapBorderSkinType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBorderSkin.MapBorderSkinType, ref result, mapBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(mapBorderSkin.ExprHost != null, "(mapBorderSkin.ExprHost != null)");
						result.Value = mapBorderSkin.ExprHost.MapBorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterXExpression(ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomView.CenterX, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomView.CenterX, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterYExpression(ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomView.CenterY, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomView.CenterY, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapElementViewLayerNameExpression(ReportingServices.ReportIntermediateFormat.MapElementView mapElementView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapElementView.LayerName, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LayerName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapElementView.LayerName, ref result, mapElementView.ExprHost))
					{
						Global.Tracer.Assert(mapElementView.ExprHost != null, "(mapElementView.ExprHost != null)");
						result.Value = mapElementView.ExprHost.LayerNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewZoomExpression(ReportingServices.ReportIntermediateFormat.MapView mapView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapView.Zoom, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Zoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapView.Zoom, ref result, mapView.ExprHost))
					{
						Global.Tracer.Assert(mapView.ExprHost != null, "(mapView.ExprHost != null)");
						result.Value = mapView.ExprHost.ZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPageNameExpression(ReportingServices.ReportIntermediateFormat.Map map, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, ReportingServices.ReportProcessing.ObjectType.Map, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, map.ExprHost))
						{
							Global.Tracer.Assert(map.ExprHost != null, "(map.ExprHost != null)");
							result.Value = map.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}
	}
}
