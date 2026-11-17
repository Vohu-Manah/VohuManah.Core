using ReportingServices.OnDemandReportRendering;

namespace ReportingServices.Rendering.HtmlRenderer
{
	internal class ROMEnumStrings : EnumStrings
	{
		public static string GetValue(ReportingServices.OnDemandReportRendering.FontStyles val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.FontStyles.Normal:
				return "normal";
			case ReportingServices.OnDemandReportRendering.FontStyles.Italic:
				return "italic";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.FontWeights val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.FontWeights.Normal:
				return "400";
			case ReportingServices.OnDemandReportRendering.FontWeights.Thin:
				return "100";
			case ReportingServices.OnDemandReportRendering.FontWeights.ExtraLight:
				return "200";
			case ReportingServices.OnDemandReportRendering.FontWeights.Light:
				return "300";
			case ReportingServices.OnDemandReportRendering.FontWeights.Medium:
				return "500";
			case ReportingServices.OnDemandReportRendering.FontWeights.SemiBold:
				return "600";
			case ReportingServices.OnDemandReportRendering.FontWeights.Bold:
				return "700";
			case ReportingServices.OnDemandReportRendering.FontWeights.ExtraBold:
				return "800";
			case ReportingServices.OnDemandReportRendering.FontWeights.Heavy:
				return "900";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.TextDecorations val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.TextDecorations.None:
				return "none";
			case ReportingServices.OnDemandReportRendering.TextDecorations.Underline:
				return "underline";
			case ReportingServices.OnDemandReportRendering.TextDecorations.Overline:
				return "overline";
			case ReportingServices.OnDemandReportRendering.TextDecorations.LineThrough:
				return "line-through";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.TextAlignments val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.TextAlignments.Left:
				return "left";
			case ReportingServices.OnDemandReportRendering.TextAlignments.Center:
				return "center";
			case ReportingServices.OnDemandReportRendering.TextAlignments.Right:
				return "right";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.VerticalAlignments val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.VerticalAlignments.Top:
				return "top";
			case ReportingServices.OnDemandReportRendering.VerticalAlignments.Middle:
				return "middle";
			case ReportingServices.OnDemandReportRendering.VerticalAlignments.Bottom:
				return "bottom";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.Directions val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.Directions.LTR:
				return "ltr";
			case ReportingServices.OnDemandReportRendering.Directions.RTL:
				return "rtl";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.Normal:
				return "normal";
			case ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.Embed:
				return "embed";
			case ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.BiDiOverride:
				return "bidi-override";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.BorderStyles val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.BorderStyles.None:
				return "none";
			case ReportingServices.OnDemandReportRendering.BorderStyles.Dotted:
				return "dotted";
			case ReportingServices.OnDemandReportRendering.BorderStyles.Dashed:
				return "dashed";
			case ReportingServices.OnDemandReportRendering.BorderStyles.Solid:
				return "solid";
			case ReportingServices.OnDemandReportRendering.BorderStyles.Double:
				return "double";
			default:
				return null;
			}
		}

		public static string GetValue(ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes val)
		{
			switch (val)
			{
			case ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.Repeat:
				return "repeat";
			case ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.Clip:
				return "no-repeat";
			case ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.RepeatX:
				return "repeat-x";
			case ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.RepeatY:
				return "repeat-y";
			default:
				return null;
			}
		}
	}
}
