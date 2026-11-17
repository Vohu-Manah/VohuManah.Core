using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class TextBoxList : ArrayList
	{
		internal new TextBox this[int index] => (TextBox)base[index];

		internal TextBoxList()
		{
		}

		internal TextBoxList(int capacity)
			: base(capacity)
		{
		}
	}
}
