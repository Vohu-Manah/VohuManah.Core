using ReportingServices.ReportProcessing.ReportObjectModel;



namespace ReportingServices.ReportProcessing.ExprHostObjectModel

{

	public abstract class TextBoxExprHost : ReportItemExprHost

	{

		public const string ValueName = "Value";



		private TextBoxImpl m_textBox;



		public object Value => m_textBox.Value;



		internal TextBoxImpl ReportObjectModelTextBox => m_textBox;



		public virtual object ValueExpr => null;



		public virtual object ToggleImageInitialStateExpr => null;



		internal void SetTextBox(TextBoxImpl textBox)

		{

			m_textBox = textBox;

		}

	}

}

