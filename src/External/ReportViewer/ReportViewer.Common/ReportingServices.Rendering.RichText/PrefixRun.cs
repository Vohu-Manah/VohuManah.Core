using ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace ReportingServices.Rendering.RichText
{
	internal class PrefixRun : ITextRunProps
	{
		private string m_fontKey;

		private const float PrefixFontSize = 10f;

		public string FontFamily => FontName;

		public float FontSize => 10f;

		public Color Color => Color.Black;

		public bool Bold => false;

		public bool Italic => false;

		public RPLFormat.TextDecorations TextDecoration => RPLFormat.TextDecorations.None;

		public int IndexInParagraph => -1;

		public string FontKey
		{
			get
			{
				return m_fontKey;
			}
			set
			{
				m_fontKey = value;
			}
		}

		internal virtual string FontName => null;

		public void AddSplitIndex(int index)
		{
		}
	}
}
