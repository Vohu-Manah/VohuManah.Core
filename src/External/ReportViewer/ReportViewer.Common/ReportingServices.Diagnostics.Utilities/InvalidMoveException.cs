using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidMoveException : ReportCatalogException
	{
		public InvalidMoveException(string itemPath, string targetPath)
			: base(ErrorCode.rsInvalidMove, ErrorStrings.rsInvalidMove(itemPath, targetPath), null, null)
		{
		}
	}
}
