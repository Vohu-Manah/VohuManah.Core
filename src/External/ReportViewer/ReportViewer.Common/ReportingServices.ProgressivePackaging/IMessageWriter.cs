using System;
using System.IO;

namespace ReportingServices.ProgressivePackaging
{
	internal interface IMessageWriter : IDisposable
	{
		void WriteMessage(string name, object value);

		Stream CreateWritableStream(string name);
	}
}
