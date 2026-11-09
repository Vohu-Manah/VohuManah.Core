using System;
using System.Collections.Generic;

namespace ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearGaugeExprHost : GaugeExprHost
	{
		[CLSCompliant(false)]
		protected IList<LinearScaleExprHost> m_linearScalesHostsRemotable;

		internal IList<LinearScaleExprHost> LinearScalesHostsRemotable => m_linearScalesHostsRemotable;

		public virtual object OrientationExpr => null;
	}
}
