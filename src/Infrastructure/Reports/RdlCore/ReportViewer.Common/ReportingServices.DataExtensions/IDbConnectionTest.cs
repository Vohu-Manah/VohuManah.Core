using ReportingServices.DataProcessing;
using ReportingServices.Interfaces;
using System;

namespace ReportingServices.DataExtensions
{
	internal interface IDbConnectionTest : IDbConnection, IDisposable, IExtension
	{
		void TestConnection();
	}
}
