namespace ReportingServices.Rendering.RichText
{
	internal struct SCRIPT_LOGATTR
	{
		private byte m_value;

		internal bool IsWhiteSpace => ((m_value >> 1) & 1) > 0;

		internal bool IsSoftBreak => (m_value & 1 & 1) > 0;

		/// <summary>
		/// Builds a SCRIPT_LOGATTR from the two bits this codebase actually reads
		/// (bit0 = fSoftBreak, bit1 = fWhiteSpace, matching Win32's SCRIPT_LOGATTR
		/// bitfield layout) - added for tasks/pdf-text-shaping-abstraction.md's P4
		/// line-break prototype (UnicodeLineBreakAnalyzer), since P/Invoke marshaling
		/// is normally what populates this struct's private field. fCharStop/fWordStop/
		/// fInvalid (also part of the real Win32 struct) are not modeled - nothing in
		/// this codebase reads them.
		/// </summary>
		internal static SCRIPT_LOGATTR FromFlags(bool isSoftBreak, bool isWhiteSpace)
		{
			byte value = 0;
			if (isSoftBreak)
			{
				value |= 1;
			}
			if (isWhiteSpace)
			{
				value |= 2;
			}
			return new SCRIPT_LOGATTR { m_value = value };
		}
	}
}
