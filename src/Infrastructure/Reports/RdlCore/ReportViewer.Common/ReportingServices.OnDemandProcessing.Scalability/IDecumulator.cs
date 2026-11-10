using System;
using System.Collections;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IDecumulator<T> : IEnumerator<T>, IDisposable, IEnumerator
	{
		void RemoveCurrent();
	}
}
