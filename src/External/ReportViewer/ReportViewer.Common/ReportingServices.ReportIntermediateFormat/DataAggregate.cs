using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal abstract class DataAggregate : IStorable, IPersistable
	{
		internal abstract DataAggregateInfo.AggregateTypes AggregateType
		{
			get;
		}

		public abstract int Size
		{
			get;
		}

		internal abstract void Init();

		internal abstract void Update(object[] expressions, IErrorContext iErrorContext);

		internal abstract object Result();

		internal abstract DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef);

		internal static ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o)
		{
			bool valid;
			return GetTypeCode(o, throwException: true, out valid);
		}

		internal static ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o, bool throwException, out bool valid)
		{
			valid = true;
			if (o is string)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.String;
			}
			if (o is int)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int32;
			}
			if (o is double)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double;
			}
			if (o == null || DBNull.Value == o)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			}
			if (o is ushort)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt16;
			}
			if (o is short)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int16;
			}
			if (o is long)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int64;
			}
			if (o is decimal)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal;
			}
			if (o is uint)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt32;
			}
			if (o is ulong)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt64;
			}
			if (o is byte)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Byte;
			}
			if (o is sbyte)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.SByte;
			}
			if (o is DateTime)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.DateTime;
			}
			if (o is char)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Char;
			}
			if (o is bool)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Boolean;
			}
			if (o is TimeSpan)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.TimeSpan;
			}
			if (o is DateTimeOffset)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.DateTimeOffset;
			}
			if (o is float)
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Single;
			}
			if (o is byte[])
			{
				return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.ByteArray;
			}
			valid = false;
			if (throwException)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
		}

		protected static bool IsNull(ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return typeCode == ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
		}

		protected static bool IsVariant(ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.ByteArray != typeCode;
		}

		protected static void ConvertToDoubleOrDecimal(ReportingServices.ReportProcessing.DataAggregate.DataTypeCode numericType, object numericData, out ReportingServices.ReportProcessing.DataAggregate.DataTypeCode doubleOrDecimalType, out object doubleOrDecimalData)
		{
			if (ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == numericType)
			{
				doubleOrDecimalType = ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal;
				doubleOrDecimalData = numericData;
			}
			else
			{
				doubleOrDecimalType = ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double;
				doubleOrDecimalData = DataTypeUtility.ConvertToDouble(numericType, numericData);
			}
		}

		protected static object Add(ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x, ReportingServices.ReportProcessing.DataAggregate.DataTypeCode yType, object y)
		{
			if (ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType && ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == yType)
			{
				return (double)x + (double)y;
			}
			if (ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType && ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == yType)
			{
				return (decimal)x + (decimal)y;
			}
			Global.Tracer.Assert(condition: false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		protected static object Square(ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x)
		{
			if (ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType)
			{
				return (double)x * (double)x;
			}
			if (ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType)
			{
				return (decimal)x * (decimal)x;
			}
			Global.Tracer.Assert(condition: false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		public abstract void Serialize(IntermediateFormatWriter writer);

		public abstract void Deserialize(IntermediateFormatReader reader);

		public abstract void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems);

		public abstract ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
