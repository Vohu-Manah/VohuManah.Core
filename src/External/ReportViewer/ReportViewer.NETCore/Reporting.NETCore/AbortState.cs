using System.Threading;

namespace Reporting.NETCore
{
	internal sealed class AbortState
	{
		private object m_abortLock = new object();

		private bool m_pendingAbort;

		private CancellationTokenSource m_abortableRequest;

		public void AbortRequest()
		{
			lock (m_abortLock)
			{
				if (m_abortableRequest != null)
				{
					m_abortableRequest.Cancel();
				}
				m_pendingAbort = true;
			}
		}

		public bool RegisterAbortableRequest(CancellationTokenSource cancellationTokenSource)
		{
			lock (m_abortLock)
			{
				if (m_pendingAbort)
				{
					return false;
				}
				m_abortableRequest = cancellationTokenSource;
				return true;
			}
		}

		public void ClearPendingAbort()
		{
			lock (m_abortLock)
			{
				m_pendingAbort = false;
				m_abortableRequest = null;
			}
		}
	}
}
