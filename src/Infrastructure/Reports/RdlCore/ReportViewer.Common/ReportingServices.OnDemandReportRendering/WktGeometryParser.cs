using System;
using System.Globalization;

namespace ReportingServices.OnDemandReportRendering
{
	/// <summary>
	/// Minimal WKT (well-known text) geometry parser. Added to fix the empty/dead-code spatial-element
	/// population stubs documented in tasks/map-spatial-data-population-gap.md -- the Map engine already
	/// speaks WKT in the export direction (Reporting.Map.WebForms.Path.SaveWKT), but nothing
	/// ever parsed it back in. Scoped to POINT only for now; LINESTRING/POLYGON/MULTI* variants are a
	/// follow-up (see that task file).
	/// </summary>
	internal static class WktGeometryParser
	{
		internal static bool TryParsePoint(string wkt, out Reporting.Map.WebForms.MapPoint point)
		{
			point = default(Reporting.Map.WebForms.MapPoint);
			if (string.IsNullOrWhiteSpace(wkt))
			{
				return false;
			}
			string text = wkt.Trim();
			if (!text.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			int openParen = text.IndexOf('(');
			int closeParen = text.LastIndexOf(')');
			if (openParen < 0 || closeParen < 0 || closeParen <= openParen)
			{
				return false;
			}
			string coordinates = text.Substring(openParen + 1, closeParen - openParen - 1).Trim();
			string[] parts = coordinates.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
			{
				return false;
			}
			if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
			{
				return false;
			}
			if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
			{
				return false;
			}
			point = new Reporting.Map.WebForms.MapPoint(x, y);
			return true;
		}
	}
}
