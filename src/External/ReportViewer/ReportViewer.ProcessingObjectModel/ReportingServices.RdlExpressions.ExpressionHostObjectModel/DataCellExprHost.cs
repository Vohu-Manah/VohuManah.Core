using System;
using System.Collections.Generic;

namespace ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataCellExprHost : CellExprHost
	{
		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_dataValueHostsRemotable;

		internal IList<DataValueExprHost> DataValueHostsRemotable => m_dataValueHostsRemotable;
	}
}
