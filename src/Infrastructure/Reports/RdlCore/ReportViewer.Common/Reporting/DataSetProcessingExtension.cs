using ReportingServices.DataProcessing;
using ReportingServices.Interfaces;
using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace Reporting
{
	internal class DataSetProcessingExtension : ReportingServices.DataProcessing.IDbConnection, IDisposable, IExtension
	{
		private string m_dataSetName;

		private ReportingServices.DataProcessing.IDataReader m_dataReader;

		public string ConnectionString
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int ConnectionTimeout => 0;

		public string LocalizedName => ProcessingStrings.DataSetExtensionName;

		public DataSetProcessingExtension(IEnumerable dataSources, string dataSetName)
		{
			if (dataSources == null || dataSetName == null)
			{
				return;
			}
			m_dataSetName = dataSetName;
			foreach (IDataSource dataSource in dataSources)
			{
				if (dataSource.Name == dataSetName && dataSource.Value != null)
				{
					if (dataSource.Value is DataTable)
					{
						m_dataReader = new DataTableReader((DataTable)dataSource.Value);
					}
					else
					{
						CreateDataReaderFromObject(dataSource.Value);
					}
					break;
				}
			}
		}

		internal void CreateDataReaderFromObject(object dataSourceValue)
		{
			IEnumerable enumerable = dataSourceValue as IEnumerable;
			if (enumerable != null)
			{
				m_dataReader = new DataEnumerableReader(enumerable);
				return;
			}
			bool flag = false;
			if (dataSourceValue is Type)
			{
				try
				{
					dataSourceValue = Activator.CreateInstance((Type)dataSourceValue);
				}
				catch
				{
					flag = true;
				}
			}
			if (!flag)
			{
				ArrayList arrayList = new ArrayList(1);
				arrayList.Add(dataSourceValue);
				m_dataReader = new DataEnumerableReader(arrayList);
			}
		}

		public void Open()
		{
		}

		public void Close()
		{
		}

		public ReportingServices.DataProcessing.IDbCommand CreateCommand()
		{
			return new DataSetProcessingCommand(m_dataReader);
		}

		public ReportingServices.DataProcessing.IDbTransaction BeginTransaction()
		{
			return new DataSetProcessingTransaction();
		}

		public void Dispose()
		{
			if (m_dataReader != null)
			{
				m_dataReader.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public void SetConfiguration(string configuration)
		{
		}

		public ReportingServices.DataProcessing.IDataReader GetReader()
		{
			if (m_dataReader == null)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, ProcessingStrings.MissingDataReader, m_dataSetName));
			}
			return m_dataReader;
		}
	}
}
