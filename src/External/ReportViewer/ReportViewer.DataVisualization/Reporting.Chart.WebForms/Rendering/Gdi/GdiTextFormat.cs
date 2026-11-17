using System.Drawing;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.StringFormat"/> behind <see cref="ITextFormat"/>.</summary>
	internal sealed class GdiTextFormat : ITextFormat
	{
		internal StringFormat NativeFormat { get; }

		internal GdiTextFormat(StringFormat format)
		{
			NativeFormat = format;
		}

		public StringAlignment Alignment
		{
			get => NativeFormat.Alignment;
			set => NativeFormat.Alignment = value;
		}

		public StringAlignment LineAlignment
		{
			get => NativeFormat.LineAlignment;
			set => NativeFormat.LineAlignment = value;
		}

		public StringFormatFlags FormatFlags
		{
			get => NativeFormat.FormatFlags;
			set => NativeFormat.FormatFlags = value;
		}

		public StringTrimming Trimming
		{
			get => NativeFormat.Trimming;
			set => NativeFormat.Trimming = value;
		}

		public ITextFormat Clone() => new GdiTextFormat((StringFormat)NativeFormat.Clone());

		public void Dispose() => NativeFormat.Dispose();
	}
}
