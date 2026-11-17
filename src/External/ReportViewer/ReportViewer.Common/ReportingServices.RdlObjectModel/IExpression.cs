using System.Collections.Generic;

namespace ReportingServices.RdlObjectModel
{
	internal interface IExpression
	{
		object Value
		{
			get;
			set;
		}

		string Expression
		{
			get;
			set;
		}

		bool IsExpression
		{
			get;
		}

		void GetDependencies(IList<ReportObject> dependencies, ReportObject parent);
	}
}
