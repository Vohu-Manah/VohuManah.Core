using System.Collections;

namespace ReportingServices.ReportIntermediateFormat.Persistence
{
	internal delegate T CreateDictionary<T>(int dictionaryLength) where T : IDictionary;
}
