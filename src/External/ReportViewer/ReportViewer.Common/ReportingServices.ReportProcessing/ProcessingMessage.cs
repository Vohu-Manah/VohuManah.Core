using ReportingServices.Diagnostics.Utilities;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingMessage : IPersistable
	{
		private ProcessingErrorCode m_code;

		private Severity m_severity;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private string m_message;

		private ProcessingMessageList m_processingMessages;

		private ErrorCode m_commonCode;

		[NonSerialized]
		private static readonly ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = GetNewDeclaration();

		public ProcessingErrorCode Code
		{
			get
			{
				return m_code;
			}
			set
			{
				m_code = value;
			}
		}

		public ErrorCode CommonCode
		{
			get
			{
				return m_commonCode;
			}
			set
			{
				m_commonCode = value;
			}
		}

		public Severity Severity
		{
			get
			{
				return m_severity;
			}
			set
			{
				m_severity = value;
			}
		}

		public ObjectType ObjectType
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

		public string ObjectName
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

		public string PropertyName
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

		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				m_message = value;
			}
		}

		public ProcessingMessageList ProcessingMessages
		{
			get
			{
				return m_processingMessages;
			}
			set
			{
				m_processingMessages = value;
			}
		}

		internal ProcessingMessage(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			m_code = code;
			m_commonCode = ErrorCode.rsProcessingError;
			m_severity = severity;
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			m_message = message;
			m_processingMessages = innerMessages;
		}

		internal ProcessingMessage(ErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			m_code = ProcessingErrorCode.rsNone;
			m_commonCode = code;
			m_severity = severity;
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			m_message = message;
			m_processingMessages = innerMessages;
		}

		internal ProcessingMessage()
		{
		}

		public string FormatMessage()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} ({1}.{2}) : {3} [{4}]", (m_severity == Severity.Warning) ? "Warning" : "Error", m_objectName, m_propertyName, m_message, m_code);
		}

		internal static ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.Code, ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.Severity, ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.ObjectType, ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.ObjectName, ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.PropertyName, ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.Message, ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.ProcessingMessages, ReportingServices.ReportProcessing.Persistence.ObjectType.ProcessingMessageList));
			memberInfoList.Add(new ReportingServices.ReportProcessing.Persistence.MemberInfo(ReportingServices.ReportProcessing.Persistence.MemberName.CommonCode, ReportingServices.ReportProcessing.Persistence.Token.Enum));
			return new ReportingServices.ReportProcessing.Persistence.Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code, ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity, ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType, ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName, ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName, ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message, ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage));
			list.Add(new ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode, ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			return new ReportingServices.ReportIntermediateFormat.Persistence.Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		void IPersistable.Serialize(ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					writer.WriteEnum((int)m_code);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					writer.WriteEnum((int)m_severity);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					writer.WriteEnum((int)m_objectType);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					writer.Write(m_objectName);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					writer.Write(m_propertyName);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					writer.Write(m_message);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					writer.Write(m_processingMessages);
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					writer.WriteEnum((int)m_commonCode);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					m_code = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					m_severity = (Severity)reader.ReadEnum();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					m_objectType = (ObjectType)reader.ReadEnum();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					m_objectName = reader.ReadString();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					m_propertyName = reader.ReadString();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					m_message = reader.ReadString();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					m_processingMessages = reader.ReadListOfRIFObjects<ProcessingMessageList>();
					break;
				case ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					m_commonCode = (ErrorCode)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage;
		}
	}
}
