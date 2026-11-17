using System;
using System.Collections.Generic;

namespace ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendCustomItemExprHost : StyleExprHost
	{
		public ChartMarkerExprHost ChartMarkerHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<ChartLegendCustomItemCellExprHost> m_legendCustomItemCellsHostsRemotable;

		public virtual object SeparatorExpr => null;

		public virtual object SeparatorColorExpr => null;

		public virtual object ToolTipExpr => null;

		internal IList<ChartLegendCustomItemCellExprHost> ChartLegendCustomItemCellsHostsRemotable => m_legendCustomItemCellsHostsRemotable;
	}
}
