using System.Runtime.Serialization;

namespace Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Hint
	{
		[DataMember(Name = "hintType", EmitDefaultValue = false)]
		public string HintType
		{
			get;
			set;
		}

		[DataMember(Name = "text", EmitDefaultValue = false)]
		public string Text
		{
			get;
			set;
		}
	}
}
