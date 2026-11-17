using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.IO;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class DocumentMapReader
	{
		private ReportingServices.OnDemandReportRendering.DocumentMapNode m_currentNode;

		private IntermediateFormatReader m_rifReader;

		private int m_level;

		private long m_startIndex;

		private Stream m_chunkStream;

		public ReportingServices.OnDemandReportRendering.DocumentMapNode Current
		{
			get
			{
				if (m_currentNode == null)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				return m_currentNode;
			}
		}

		public DocumentMapReader(Stream chunkStream)
		{
			m_chunkStream = chunkStream;
			ProcessingRIFObjectCreator rifObjectCreator = new ProcessingRIFObjectCreator(null, null);
			m_rifReader = new IntermediateFormatReader(m_chunkStream, rifObjectCreator);
			m_startIndex = m_rifReader.ObjectStartPosition;
			m_level = 1;
		}

		public bool MoveNext()
		{
			if (m_rifReader.EOS)
			{
				return false;
			}
			m_currentNode = null;
			IPersistable persistable = m_rifReader.ReadRIFObject();
			switch (persistable.GetObjectType())
			{
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapBeginContainer:
			{
				bool result = MoveNext();
				m_level++;
				return result;
			}
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer:
				m_level--;
				return MoveNext();
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapNode:
			{
				DocumentMapNode documentMapNode = (DocumentMapNode)persistable;
				m_currentNode = new ReportingServices.OnDemandReportRendering.DocumentMapNode(documentMapNode.Label, documentMapNode.Id, m_level);
				return true;
			}
			default:
				Global.Tracer.Assert(condition: false);
				return false;
			}
		}

		public void Reset()
		{
			m_chunkStream.Seek(m_startIndex, SeekOrigin.Begin);
			m_level = 1;
		}

		public void Close()
		{
			m_chunkStream.Close();
		}
	}
}
