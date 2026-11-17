using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class AggregatesImpl : Aggregates, IStaticReferenceable
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private Hashtable m_duplicateNames;

		private OnDemandProcessingContext m_odpContext;

		private int m_id = int.MinValue;

		public override object this[string key]
		{
			get
			{
				ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = GetAggregateObj(key);
				if (aggregateObj == null)
				{
					if (m_odpContext.IsTablixProcessingMode)
					{
						m_odpContext.ReportRuntime.UnfulfilledDependency = true;
					}
					if (!m_odpContext.CalculateAggregate(key))
					{
						return null;
					}
					aggregateObj = GetAggregateObj(key);
					Global.Tracer.Assert(aggregateObj != null, "(null != aggregateObj)");
				}
				object aggregateValue = GetAggregateValue(key, aggregateObj);
				if (aggregateValue == null && aggregateObj.AggregateDef.AggregateType == ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate && m_odpContext.StreamingMode && m_odpContext.StateManager.CheckForPrematureServerAggregate(key))
				{
					aggregateValue = GetAggregateValue(key, GetAggregateObj(key));
				}
				return aggregateValue;
			}
		}

		private IErrorContext ErrorContext => m_odpContext.ReportRuntime;

		internal ICollection Objects => m_collection.Values;

		public int ID => m_id;

		internal AggregatesImpl()
		{
		}

		internal AggregatesImpl(OnDemandProcessingContext odpContext)
			: this(lockAdd: false, odpContext)
		{
		}

		internal AggregatesImpl(bool lockAdd, OnDemandProcessingContext odpContext)
		{
			m_lockAdd = lockAdd;
			m_odpContext = odpContext;
			ClearAll();
		}

		private object GetAggregateValue(string key, ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj)
		{
			aggregateObj.UsedInExpression = true;
			ReportingServices.ReportIntermediateFormat.DataAggregateObjResult dataAggregateObjResult = aggregateObj.AggregateResult();
			if (dataAggregateObjResult == null)
			{
				Global.Tracer.Assert(m_odpContext.IsTablixProcessingMode, "Missing aggregate result outside of tablix processing");
				throw new ReportProcessingException_MissingAggregateDependency();
			}
			if (dataAggregateObjResult.HasCode)
			{
				if ((dataAggregateObjResult.FieldStatus == DataFieldStatus.None || dataAggregateObjResult.FieldStatus == DataFieldStatus.IsError) && dataAggregateObjResult.Code != 0)
				{
					ErrorContext.Register(dataAggregateObjResult.Code, dataAggregateObjResult.Severity, dataAggregateObjResult.Arguments);
				}
				else if (dataAggregateObjResult.FieldStatus == DataFieldStatus.UnSupportedDataType)
				{
					ErrorContext.Register(ProcessingErrorCode.rsAggregateOfInvalidExpressionDataType, Severity.Warning, dataAggregateObjResult.Arguments);
				}
				if (dataAggregateObjResult.ErrorOccurred)
				{
					throw new ReportProcessingException_InvalidOperationException();
				}
			}
			if (dataAggregateObjResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			return dataAggregateObjResult.Value;
		}

		internal void ClearAll()
		{
			if (m_collection != null)
			{
				m_collection.Clear();
			}
			else
			{
				m_collection = new Hashtable();
			}
			m_duplicateNames = null;
		}

		internal void ResetAll()
		{
			foreach (ReportingServices.ReportIntermediateFormat.DataAggregateObj value in m_collection.Values)
			{
				value.Init();
			}
		}

		internal void ResetAll<T>(IEnumerable<T> aggregateDefs) where T : ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregateDefs == null)
			{
				return;
			}
			foreach (T aggregateDef in aggregateDefs)
			{
				Reset(aggregateDef);
			}
		}

		internal void Reset(ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateDef)
		{
			if (m_collection != null)
			{
				GetAggregateObj(aggregateDef.Name)?.ResetForNoRows();
			}
		}

		internal void Add(ReportingServices.ReportIntermediateFormat.DataAggregateObj newObject)
		{
			Global.Tracer.Assert(!newObject.NonAggregateMode, "( !newObject.NonAggregateMode )");
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				m_collection.Add(newObject.Name, newObject);
				PopulateDuplicateNames(newObject.Name, newObject.DuplicateNames);
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}

		internal void Remove(ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggDef)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				if (m_collection == null)
				{
					return;
				}
				m_collection.Remove(aggDef.Name);
				List<string> duplicateNames = aggDef.DuplicateNames;
				if (m_duplicateNames != null && duplicateNames != null)
				{
					for (int i = 0; i < duplicateNames.Count; i++)
					{
						m_duplicateNames.Remove(duplicateNames[i]);
					}
				}
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}

		internal void Set(string name, ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateDef, List<string> duplicateNames, ReportingServices.ReportIntermediateFormat.DataAggregateObjResult aggregateResult)
		{
			ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = GetAggregateObj(name);
			if (aggregateObj == null)
			{
				try
				{
					if (m_lockAdd)
					{
						Monitor.Enter(m_collection);
					}
					aggregateObj = new ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggregateDef, aggregateResult);
					m_collection.Add(name, aggregateObj);
					PopulateDuplicateNames(name, duplicateNames);
				}
				finally
				{
					if (m_lockAdd)
					{
						Monitor.Exit(m_collection);
					}
				}
			}
			else
			{
				aggregateObj.Set(aggregateResult);
			}
		}

		internal ReportingServices.ReportIntermediateFormat.DataAggregateObj GetAggregateObj(string name)
		{
			ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = (ReportingServices.ReportIntermediateFormat.DataAggregateObj)m_collection[name];
			if (dataAggregateObj == null && m_duplicateNames != null)
			{
				string text = (string)m_duplicateNames[name];
				if (text != null)
				{
					dataAggregateObj = (ReportingServices.ReportIntermediateFormat.DataAggregateObj)m_collection[text];
				}
			}
			return dataAggregateObj;
		}

		private void PopulateDuplicateNames(string name, List<string> duplicateNames)
		{
			if (duplicateNames != null && 0 < duplicateNames.Count)
			{
				if (m_duplicateNames == null)
				{
					m_duplicateNames = new Hashtable();
				}
				for (int i = 0; i < duplicateNames.Count; i++)
				{
					m_duplicateNames[duplicateNames[i]] = name;
				}
			}
		}

		internal void ResetFieldsUsedInExpression()
		{
			foreach (ReportingServices.ReportIntermediateFormat.DataAggregateObj value in m_collection.Values)
			{
				value.UsedInExpression = false;
			}
		}

		internal void AddFieldsUsedInExpression(OnDemandProcessingContext odpContext, List<string> fieldsUsedInValueExpression)
		{
			Dictionary<string, List<string>> aggregateFieldReferences = odpContext.OdpMetadata.ReportSnapshot.AggregateFieldReferences;
			foreach (ReportingServices.ReportIntermediateFormat.DataAggregateObj value2 in m_collection.Values)
			{
				if (value2.UsedInExpression && value2.AggregateDef != null && aggregateFieldReferences.TryGetValue(value2.AggregateDef.Name, out List<string> value))
				{
					fieldsUsedInValueExpression.AddRange(value);
				}
			}
		}

		public void SetID(int id)
		{
			m_id = id;
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregatesImpl;
		}
	}
}
