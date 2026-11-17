using System;

namespace ReportingServices.ReportProcessing
{
	internal sealed class ExpressionInfoExtended : ExpressionInfo
	{
		[NonSerialized]
		private bool m_isExtendedSimpleFieldReference;

		internal bool IsExtendedSimpleFieldReference
		{
			get
			{
				return m_isExtendedSimpleFieldReference;
			}
			set
			{
				m_isExtendedSimpleFieldReference = value;
			}
		}
	}
}
