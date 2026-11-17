namespace ReportingServices.OnDemandReportRendering
{
	internal interface IShimDataRegionMember
	{
		bool SetNewContext(int index);

		void ResetContext();
	}
}
