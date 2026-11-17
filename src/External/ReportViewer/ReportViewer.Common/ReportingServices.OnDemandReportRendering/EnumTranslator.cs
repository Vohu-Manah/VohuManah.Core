using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;

namespace ReportingServices.OnDemandReportRendering
{
	internal class EnumTranslator
	{
		internal static GaugeFrameShapes TranslateGaugeFrameShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameShapes.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return GaugeFrameShapes.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circular"))
			{
				return GaugeFrameShapes.Circular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return GaugeFrameShapes.Rectangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedRectangular"))
			{
				return GaugeFrameShapes.RoundedRectangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoShape"))
			{
				return GaugeFrameShapes.AutoShape;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular1"))
			{
				return GaugeFrameShapes.CustomCircular1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular2"))
			{
				return GaugeFrameShapes.CustomCircular2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular3"))
			{
				return GaugeFrameShapes.CustomCircular3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular4"))
			{
				return GaugeFrameShapes.CustomCircular4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular5"))
			{
				return GaugeFrameShapes.CustomCircular5;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular6"))
			{
				return GaugeFrameShapes.CustomCircular6;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular7"))
			{
				return GaugeFrameShapes.CustomCircular7;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular8"))
			{
				return GaugeFrameShapes.CustomCircular8;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular9"))
			{
				return GaugeFrameShapes.CustomCircular9;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular10"))
			{
				return GaugeFrameShapes.CustomCircular10;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular11"))
			{
				return GaugeFrameShapes.CustomCircular11;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular12"))
			{
				return GaugeFrameShapes.CustomCircular12;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular13"))
			{
				return GaugeFrameShapes.CustomCircular13;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular14"))
			{
				return GaugeFrameShapes.CustomCircular14;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular15"))
			{
				return GaugeFrameShapes.CustomCircular15;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN1"))
			{
				return GaugeFrameShapes.CustomSemiCircularN1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN2"))
			{
				return GaugeFrameShapes.CustomSemiCircularN2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN3"))
			{
				return GaugeFrameShapes.CustomSemiCircularN3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN4"))
			{
				return GaugeFrameShapes.CustomSemiCircularN4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS1"))
			{
				return GaugeFrameShapes.CustomSemiCircularS1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS2"))
			{
				return GaugeFrameShapes.CustomSemiCircularS2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS3"))
			{
				return GaugeFrameShapes.CustomSemiCircularS3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS4"))
			{
				return GaugeFrameShapes.CustomSemiCircularS4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE1"))
			{
				return GaugeFrameShapes.CustomSemiCircularE1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE2"))
			{
				return GaugeFrameShapes.CustomSemiCircularE2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE3"))
			{
				return GaugeFrameShapes.CustomSemiCircularE3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE4"))
			{
				return GaugeFrameShapes.CustomSemiCircularE4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW1"))
			{
				return GaugeFrameShapes.CustomSemiCircularW1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW2"))
			{
				return GaugeFrameShapes.CustomSemiCircularW2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW3"))
			{
				return GaugeFrameShapes.CustomSemiCircularW3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW4"))
			{
				return GaugeFrameShapes.CustomSemiCircularW4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW4;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeFrameShapes.Default;
		}

		internal static GaugeFrameStyles TranslateGaugeFrameStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameStyles.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeFrameStyles.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeFrameStyles.Simple;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Edged"))
			{
				return GaugeFrameStyles.Edged;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeFrameStyles.None;
		}

		internal static GaugeAntiAliasings TranslateGaugeAntiAliasings(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeAntiAliasings.All;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return GaugeAntiAliasings.All;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeAntiAliasings.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return GaugeAntiAliasings.Text;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return GaugeAntiAliasings.Graphics;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeAntiAliasings.All;
		}

		internal static GaugeGlassEffects TranslateGaugeGlassEffects(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeGlassEffects.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeGlassEffects.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeGlassEffects.Simple;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeGlassEffects.None;
		}

		internal static GaugeBarStarts TranslateGaugeBarStarts(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ScaleStart"))
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Zero"))
			{
				return GaugeBarStarts.Zero;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeBarStarts.ScaleStart;
		}

		internal static GaugeCapStyles TranslateGaugeCapStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedDark"))
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rounded"))
			{
				return GaugeCapStyles.Rounded;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedLight"))
			{
				return GaugeCapStyles.RoundedLight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithAdditionalTop"))
			{
				return GaugeCapStyles.RoundedWithAdditionalTop;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithWideIndentation"))
			{
				return GaugeCapStyles.RoundedWithWideIndentation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithIndentation"))
			{
				return GaugeCapStyles.FlattenedWithIndentation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithWideIndentation"))
			{
				return GaugeCapStyles.FlattenedWithWideIndentation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedGlossyWithIndentation"))
			{
				return GaugeCapStyles.RoundedGlossyWithIndentation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithIndentation"))
			{
				return GaugeCapStyles.RoundedWithIndentation;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeCapStyles.RoundedDark;
		}

		internal static GaugeInputValueFormulas TranslateGaugeInputValueFormulas(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeInputValueFormulas.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeInputValueFormulas.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Average"))
			{
				return GaugeInputValueFormulas.Average;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Linear"))
			{
				return GaugeInputValueFormulas.Linear;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Max"))
			{
				return GaugeInputValueFormulas.Max;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Min"))
			{
				return GaugeInputValueFormulas.Min;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return GaugeInputValueFormulas.Median;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OpenClose"))
			{
				return GaugeInputValueFormulas.OpenClose;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentile"))
			{
				return GaugeInputValueFormulas.Percentile;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Variance"))
			{
				return GaugeInputValueFormulas.Variance;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return GaugeInputValueFormulas.RateOfChange;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integral"))
			{
				return GaugeInputValueFormulas.Integral;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeInputValueFormulas.None;
		}

		internal static GaugeLabelPlacements TranslateGaugeLabelPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeLabelPlacements.Inside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugeLabelPlacements.Inside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugeLabelPlacements.Outside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugeLabelPlacements.Cross;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeLabelPlacements.Inside;
		}

		internal static GaugeMarkerStyles TranslateGaugeMarkerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeMarkerStyles.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeMarkerStyles.Rectangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeMarkerStyles.Circle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeMarkerStyles.Diamond;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeMarkerStyles.Trapezoid;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeMarkerStyles.Star;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeMarkerStyles.Wedge;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeMarkerStyles.Pentagon;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeMarkerStyles.Triangle;
		}

		internal static GaugeOrientations TranslateGaugeOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeOrientations.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return GaugeOrientations.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return GaugeOrientations.Horizontal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
			{
				return GaugeOrientations.Vertical;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeOrientations.Auto;
		}

		internal static GaugePointerPlacements TranslateGaugePointerPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugePointerPlacements.Cross;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugePointerPlacements.Cross;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugePointerPlacements.Outside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugePointerPlacements.Inside;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugePointerPlacements.Cross;
		}

		internal static GaugeThermometerStyles TranslateGaugeThermometerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeThermometerStyles.Standard;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Standard"))
			{
				return GaugeThermometerStyles.Standard;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flask"))
			{
				return GaugeThermometerStyles.Flask;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeThermometerStyles.Standard;
		}

		internal static GaugeTickMarkShapes TranslateGaugeTickMarkShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTickMarkShapes.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeTickMarkShapes.Triangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeTickMarkShapes.Circle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeTickMarkShapes.Diamond;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeTickMarkShapes.Trapezoid;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeTickMarkShapes.Star;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeTickMarkShapes.Wedge;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeTickMarkShapes.Pentagon;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeTickMarkShapes.Rectangle;
		}

		internal static LinearPointerTypes TranslateLinearPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return LinearPointerTypes.Marker;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return LinearPointerTypes.Marker;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return LinearPointerTypes.Bar;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Thermometer"))
			{
				return LinearPointerTypes.Thermometer;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return LinearPointerTypes.Marker;
		}

		internal static RadialPointerNeedleStyles TranslateRadialPointerNeedleStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangular"))
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return RadialPointerNeedleStyles.Rectangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithTail"))
			{
				return RadialPointerNeedleStyles.TaperedWithTail;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Tapered"))
			{
				return RadialPointerNeedleStyles.Tapered;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowWithTail"))
			{
				return RadialPointerNeedleStyles.ArrowWithTail;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
			{
				return RadialPointerNeedleStyles.Arrow;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithTail;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrow"))
			{
				return RadialPointerNeedleStyles.StealthArrow;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithStealthArrow"))
			{
				return RadialPointerNeedleStyles.TaperedWithStealthArrow;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithWideTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithWideTail;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithRoundedPoint"))
			{
				return RadialPointerNeedleStyles.TaperedWithRoundedPoint;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return RadialPointerNeedleStyles.Triangular;
		}

		internal static RadialPointerTypes TranslateRadialPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerTypes.Needle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Needle"))
			{
				return RadialPointerTypes.Needle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return RadialPointerTypes.Marker;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return RadialPointerTypes.Bar;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return RadialPointerTypes.Needle;
		}

		internal static ScaleRangePlacements TranslateScaleRangePlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ScaleRangePlacements.Inside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return ScaleRangePlacements.Inside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return ScaleRangePlacements.Outside;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return ScaleRangePlacements.Cross;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return ScaleRangePlacements.Inside;
		}

		internal static BackgroundGradientTypes TranslateBackgroundGradientTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StartToEnd"))
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return BackgroundGradientTypes.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftRight"))
			{
				return BackgroundGradientTypes.LeftRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopBottom"))
			{
				return BackgroundGradientTypes.TopBottom;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return BackgroundGradientTypes.Center;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalLeft"))
			{
				return BackgroundGradientTypes.DiagonalLeft;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalRight"))
			{
				return BackgroundGradientTypes.DiagonalRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HorizontalCenter"))
			{
				return BackgroundGradientTypes.HorizontalCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "VerticalCenter"))
			{
				return BackgroundGradientTypes.VerticalCenter;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return BackgroundGradientTypes.StartToEnd;
		}

		internal static TextAntiAliasingQualities TranslateTextAntiAliasingQualities(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextAntiAliasingQualities.High;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return TextAntiAliasingQualities.High;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return TextAntiAliasingQualities.Normal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return TextAntiAliasingQualities.SystemDefault;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return TextAntiAliasingQualities.High;
		}

		internal static GaugeResizeModes TranslateGaugeResizeModes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeResizeModes.AutoFit;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return GaugeResizeModes.AutoFit;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeResizeModes.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeResizeModes.AutoFit;
		}

		internal static GaugeIndicatorStyles TranslateGaugeIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mechanical"))
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital7Segment"))
			{
				return GaugeIndicatorStyles.Digital7Segment;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital14Segment"))
			{
				return GaugeIndicatorStyles.Digital14Segment;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeIndicatorStyles.Mechanical;
		}

		internal static GaugeShowSigns TranslateGaugeShowSigns(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "NegativeOnly"))
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Both"))
			{
				return GaugeShowSigns.Both;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeShowSigns.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeShowSigns.NegativeOnly;
		}

		internal static GaugeStateIndicatorStyles TranslateGaugeStateIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flag"))
			{
				return GaugeStateIndicatorStyles.Flag;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDown"))
			{
				return GaugeStateIndicatorStyles.ArrowDown;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowDownIncline;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowSide"))
			{
				return GaugeStateIndicatorStyles.ArrowSide;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUp"))
			{
				return GaugeStateIndicatorStyles.ArrowUp;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowUpIncline;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesAllFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesAllFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesNoneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesNoneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesOneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesOneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesTwoFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesTwoFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesThreeFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesThreeFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDown"))
			{
				return GaugeStateIndicatorStyles.LightArrowDown;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowDownIncline;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowSide"))
			{
				return GaugeStateIndicatorStyles.LightArrowSide;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUp"))
			{
				return GaugeStateIndicatorStyles.LightArrowUp;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowUpIncline;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersAllFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersNoneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersOneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersTwoFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersThreeFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterFourFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterFourFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterNoneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterNoneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterOneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterOneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterThreeFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterThreeFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterTwoFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterTwoFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersAllFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersNoneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersOneFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersTwoFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersThreeFilled;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsCircle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsCircle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsDiamond"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsDiamond;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsTriangle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsTriangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCheck;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCross;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolExclamation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCheck;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCross;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledExclamation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLight"))
			{
				return GaugeStateIndicatorStyles.TrafficLight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLightUnrimmed"))
			{
				return GaugeStateIndicatorStyles.TrafficLightUnrimmed;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDash"))
			{
				return GaugeStateIndicatorStyles.TriangleDash;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDown"))
			{
				return GaugeStateIndicatorStyles.TriangleDown;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleUp"))
			{
				return GaugeStateIndicatorStyles.TriangleUp;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonStop"))
			{
				return GaugeStateIndicatorStyles.ButtonStop;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPlay"))
			{
				return GaugeStateIndicatorStyles.ButtonPlay;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPause"))
			{
				return GaugeStateIndicatorStyles.ButtonPause;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceSmile"))
			{
				return GaugeStateIndicatorStyles.FaceSmile;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceNeutral"))
			{
				return GaugeStateIndicatorStyles.FaceNeutral;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceFrown"))
			{
				return GaugeStateIndicatorStyles.FaceFrown;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return GaugeStateIndicatorStyles.Image;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeStateIndicatorStyles.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeStateIndicatorStyles.Circle;
		}

		internal static GaugeTransformationType TranslateGaugeTransformationType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTransformationType.Percentage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return GaugeTransformationType.Percentage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTransformationType.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeTransformationType.Percentage;
		}

		internal static MapLegendTitleSeparator TranslateMapLegendTitleSeparator(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendTitleSeparator.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapLegendTitleSeparator.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
			{
				return MapLegendTitleSeparator.Line;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
			{
				return MapLegendTitleSeparator.ThickLine;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
			{
				return MapLegendTitleSeparator.DoubleLine;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
			{
				return MapLegendTitleSeparator.DashLine;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
			{
				return MapLegendTitleSeparator.DotLine;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
			{
				return MapLegendTitleSeparator.GradientLine;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				return MapLegendTitleSeparator.ThickGradientLine;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLegendTitleSeparator.None;
		}

		internal static MapLegendLayout TranslateMapLegendLayout(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendLayout.AutoTable;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
			{
				return MapLegendLayout.AutoTable;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
			{
				return MapLegendLayout.Column;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
			{
				return MapLegendLayout.Row;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
			{
				return MapLegendLayout.WideTable;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
			{
				return MapLegendLayout.TallTable;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLegendLayout.AutoTable;
		}

		internal static MapAutoBool TranslateMapAutoBool(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAutoBool.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapAutoBool.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
			{
				return MapAutoBool.True;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
			{
				return MapAutoBool.False;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapAutoBool.Auto;
		}

		internal static MapLabelPlacement TranslateLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPlacement.Alternate;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Alternate"))
			{
				return MapLabelPlacement.Alternate;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapLabelPlacement.Top;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapLabelPlacement.Bottom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelPlacement.Alternate;
		}

		internal static MapLabelBehavior TranslateLabelBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelBehavior.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapLabelBehavior.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowMiddleValue"))
			{
				return MapLabelBehavior.ShowMiddleValue;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowBorderValue"))
			{
				return MapLabelBehavior.ShowBorderValue;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelBehavior.Auto;
		}

		internal static Unit TranslateUnit(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Unit.Percentage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return Unit.Percentage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inch"))
			{
				return Unit.Inch;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Point"))
			{
				return Unit.Point;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Centimeter"))
			{
				return Unit.Centimeter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Millimeter"))
			{
				return Unit.Millimeter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pica"))
			{
				return Unit.Pica;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return Unit.Percentage;
		}

		internal static MapLabelPosition TranslateLabelPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPosition.Near;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
			{
				return MapLabelPosition.Near;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OneQuarter"))
			{
				return MapLabelPosition.OneQuarter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLabelPosition.Center;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeQuarters"))
			{
				return MapLabelPosition.ThreeQuarters;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
			{
				return MapLabelPosition.Far;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelPosition.Near;
		}

		internal static MapPosition TranslateMapPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPosition.TopCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPosition.TopCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPosition.TopLeft;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPosition.TopRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
			{
				return MapPosition.LeftTop;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
			{
				return MapPosition.LeftCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
			{
				return MapPosition.LeftBottom;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
			{
				return MapPosition.RightTop;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
			{
				return MapPosition.RightCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
			{
				return MapPosition.RightBottom;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPosition.BottomRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPosition.BottomCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPosition.BottomLeft;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPosition.TopCenter;
		}

		internal static MapCoordinateSystem TranslateMapCoordinateSystem(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapCoordinateSystem.Planar;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Planar"))
			{
				return MapCoordinateSystem.Planar;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Geographic"))
			{
				return MapCoordinateSystem.Geographic;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapCoordinateSystem.Planar;
		}

		internal static MapProjection TranslateMapProjection(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapProjection.Equirectangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Equirectangular"))
			{
				return MapProjection.Equirectangular;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mercator"))
			{
				return MapProjection.Mercator;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Robinson"))
			{
				return MapProjection.Robinson;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fahey"))
			{
				return MapProjection.Fahey;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert1"))
			{
				return MapProjection.Eckert1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert3"))
			{
				return MapProjection.Eckert3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HammerAitoff"))
			{
				return MapProjection.HammerAitoff;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wagner3"))
			{
				return MapProjection.Wagner3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bonne"))
			{
				return MapProjection.Bonne;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapProjection.Equirectangular;
		}

		internal static MapRuleDistributionType TranslateMapRuleDistributionType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapRuleDistributionType.Optimal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Optimal"))
			{
				return MapRuleDistributionType.Optimal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualInterval"))
			{
				return MapRuleDistributionType.EqualInterval;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualDistribution"))
			{
				return MapRuleDistributionType.EqualDistribution;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return MapRuleDistributionType.Custom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapRuleDistributionType.Optimal;
		}

		internal static MapMarkerStyle TranslateMapMarkerStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapMarkerStyle.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapMarkerStyle.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return MapMarkerStyle.Rectangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return MapMarkerStyle.Circle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return MapMarkerStyle.Diamond;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return MapMarkerStyle.Triangle;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return MapMarkerStyle.Trapezoid;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return MapMarkerStyle.Star;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return MapMarkerStyle.Wedge;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return MapMarkerStyle.Pentagon;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PushPin"))
			{
				return MapMarkerStyle.PushPin;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return MapMarkerStyle.Image;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapMarkerStyle.None;
		}

		internal static MapResizeMode TranslateMapResizeMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapResizeMode.AutoFit;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return MapResizeMode.AutoFit;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapResizeMode.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapResizeMode.AutoFit;
		}

		internal static MapPalette TranslateMapPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPalette.Random;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Random"))
			{
				return MapPalette.Random;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return MapPalette.Light;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return MapPalette.SemiTransparent;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return MapPalette.BrightPastel;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return MapPalette.Pacific;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPalette.Random;
		}

		internal static MapLineLabelPlacement TranslateMapLineLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLineLabelPlacement.Above;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Above"))
			{
				return MapLineLabelPlacement.Above;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLineLabelPlacement.Center;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Below"))
			{
				return MapLineLabelPlacement.Below;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLineLabelPlacement.Above;
		}

		internal static MapPolygonLabelPlacement TranslateMapPolygonLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleCenter"))
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleLeft"))
			{
				return MapPolygonLabelPlacement.MiddleLeft;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleRight"))
			{
				return MapPolygonLabelPlacement.MiddleRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPolygonLabelPlacement.TopCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPolygonLabelPlacement.TopLeft;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPolygonLabelPlacement.TopRight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPolygonLabelPlacement.BottomCenter;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPolygonLabelPlacement.BottomLeft;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPolygonLabelPlacement.BottomRight;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPolygonLabelPlacement.MiddleCenter;
		}

		internal static MapPointLabelPlacement TranslateMapPointLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapPointLabelPlacement.Top;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
			{
				return MapPointLabelPlacement.Left;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
			{
				return MapPointLabelPlacement.Right;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapPointLabelPlacement.Center;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPointLabelPlacement.Bottom;
		}

		internal static MapTileStyle TranslateMapTileStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTileStyle.Road;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Road"))
			{
				return MapTileStyle.Road;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Aerial"))
			{
				return MapTileStyle.Aerial;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hybrid"))
			{
				return MapTileStyle.Hybrid;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapTileStyle.Road;
		}

		internal static MapVisibilityMode TranslateMapVisibilityMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapVisibilityMode.Visible;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Visible"))
			{
				return MapVisibilityMode.Visible;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hidden"))
			{
				return MapVisibilityMode.Hidden;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ZoomBased"))
			{
				return MapVisibilityMode.ZoomBased;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapVisibilityMode.Visible;
		}

		internal static MapDataType TranslateDataType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapDataType.String;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Boolean"))
			{
				return MapDataType.Boolean;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DateTime"))
			{
				return MapDataType.DateTime;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integer"))
			{
				return MapDataType.Integer;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Float"))
			{
				return MapDataType.Float;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "String"))
			{
				return MapDataType.String;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Decimal"))
			{
				return MapDataType.Decimal;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapDataType.String;
		}

		internal static MapAntiAliasing TranslateMapAntiAliasing(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAntiAliasing.All;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return MapAntiAliasing.All;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapAntiAliasing.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return MapAntiAliasing.Text;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return MapAntiAliasing.Graphics;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapAntiAliasing.All;
		}

		internal static MapTextAntiAliasingQuality TranslateMapTextAntiAliasingQuality(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return MapTextAntiAliasingQuality.Normal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return MapTextAntiAliasingQuality.SystemDefault;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapTextAntiAliasingQuality.High;
		}

		internal static MapBorderSkinType TranslateMapBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapBorderSkinType.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapBorderSkinType.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
			{
				return MapBorderSkinType.Emboss;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
			{
				return MapBorderSkinType.Raised;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
			{
				return MapBorderSkinType.Sunken;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
			{
				return MapBorderSkinType.FrameThin1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
			{
				return MapBorderSkinType.FrameThin2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
			{
				return MapBorderSkinType.FrameThin3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
			{
				return MapBorderSkinType.FrameThin4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
			{
				return MapBorderSkinType.FrameThin5;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
			{
				return MapBorderSkinType.FrameThin6;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
			{
				return MapBorderSkinType.FrameTitle1;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
			{
				return MapBorderSkinType.FrameTitle2;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
			{
				return MapBorderSkinType.FrameTitle3;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
			{
				return MapBorderSkinType.FrameTitle4;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
			{
				return MapBorderSkinType.FrameTitle5;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
			{
				return MapBorderSkinType.FrameTitle6;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
			{
				return MapBorderSkinType.FrameTitle7;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				return MapBorderSkinType.FrameTitle8;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapBorderSkinType.None;
		}

		internal static ChartBreakLineType TranslateChartBreakLineType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Ragged"))
				{
					return ChartBreakLineType.Ragged;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBreakLineType.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Straight"))
				{
					return ChartBreakLineType.Straight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wave"))
				{
					return ChartBreakLineType.Wave;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartBreakLineType.Ragged;
		}

		internal static ChartIntervalType TranslateChartIntervalType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartIntervalType.Default;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartIntervalType.Auto;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Number"))
				{
					return ChartIntervalType.Number;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Years"))
				{
					return ChartIntervalType.Years;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Months"))
				{
					return ChartIntervalType.Months;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Weeks"))
				{
					return ChartIntervalType.Weeks;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Days"))
				{
					return ChartIntervalType.Days;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hours"))
				{
					return ChartIntervalType.Hours;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Minutes"))
				{
					return ChartIntervalType.Minutes;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Seconds"))
				{
					return ChartIntervalType.Seconds;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Milliseconds"))
				{
					return ChartIntervalType.Milliseconds;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartIntervalType.Default;
		}

		internal static ChartAutoBool TranslateChartAutoBool(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartAutoBool.Auto;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAutoBool.True;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAutoBool.False;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAutoBool.Auto;
		}

		internal static ChartAxisLabelRotation TranslateChartAxisLabelRotation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisLabelRotation.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate30"))
				{
					return ChartAxisLabelRotation.Rotate30;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate45"))
				{
					return ChartAxisLabelRotation.Rotate45;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate90"))
				{
					return ChartAxisLabelRotation.Rotate90;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisLabelRotation.None;
		}

		internal static ChartAxisLocation TranslateChartAxisLocation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartAxisLocation.Default;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Opposite"))
				{
					return ChartAxisLocation.Opposite;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisLocation.Default;
		}

		internal static ChartAxisArrow TranslateChartAxisArrow(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisArrow.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartAxisArrow.Triangle;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SharpTriangle"))
				{
					return ChartAxisArrow.SharpTriangle;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Lines"))
				{
					return ChartAxisArrow.Lines;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisArrow.None;
		}

		internal static ChartTickMarksType TranslateChartTickMarksType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartTickMarksType.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
				{
					return ChartTickMarksType.Inside;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartTickMarksType.Outside;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartTickMarksType.Cross;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTickMarksType.None;
		}

		internal static ChartColumnType TranslateChartColumnType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartColumnType.Text;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartColumnType.SeriesSymbol;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartColumnType.Text;
		}

		internal static ChartCellType TranslateChartCellType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartCellType.Text;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartCellType.SeriesSymbol;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
				{
					return ChartCellType.Image;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCellType.Text;
		}

		internal static ChartCellAlignment TranslateChartCellAlignment(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartCellAlignment.Center;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartCellAlignment.Top;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartCellAlignment.TopLeft;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartCellAlignment.TopRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartCellAlignment.Left;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartCellAlignment.Right;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartCellAlignment.BottomRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartCellAlignment.Bottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartCellAlignment.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCellAlignment.Center;
		}

		internal static ChartAllowOutsideChartArea TranslateChartAllowOutsideChartArea(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Partial"))
				{
					return ChartAllowOutsideChartArea.Partial;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAllowOutsideChartArea.True;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAllowOutsideChartArea.False;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAllowOutsideChartArea.Partial;
		}

		internal static ChartCalloutLineAnchor TranslateChartCalloutLineAnchor(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
				{
					return ChartCalloutLineAnchor.Arrow;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartCalloutLineAnchor.Diamond;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartCalloutLineAnchor.Square;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Round"))
				{
					return ChartCalloutLineAnchor.Round;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineAnchor.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutLineAnchor.Arrow;
		}

		internal static ChartCalloutLineStyle TranslateChartCalloutLineStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Solid"))
				{
					return ChartCalloutLineStyle.Solid;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dotted"))
				{
					return ChartCalloutLineStyle.Dotted;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dashed"))
				{
					return ChartCalloutLineStyle.Dashed;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Double"))
				{
					return ChartCalloutLineStyle.Double;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDot"))
				{
					return ChartCalloutLineStyle.DashDot;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDotDot"))
				{
					return ChartCalloutLineStyle.DashDotDot;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineStyle.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutLineStyle.Solid;
		}

		internal static ChartCalloutStyle TranslateChartCalloutStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Underline"))
				{
					return ChartCalloutStyle.Underline;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Box"))
				{
					return ChartCalloutStyle.Box;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutStyle.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutStyle.Underline;
		}

		internal static ChartSeriesFormula TranslateChartSeriesFormula(string val)
		{
			if (val == null)
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BollingerBands"))
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MovingAverage"))
			{
				return ChartSeriesFormula.MovingAverage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExponentialMovingAverage"))
			{
				return ChartSeriesFormula.ExponentialMovingAverage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangularMovingAverage"))
			{
				return ChartSeriesFormula.TriangularMovingAverage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WeightedMovingAverage"))
			{
				return ChartSeriesFormula.WeightedMovingAverage;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MACD"))
			{
				return ChartSeriesFormula.MACD;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DetrendedPriceOscillator"))
			{
				return ChartSeriesFormula.DetrendedPriceOscillator;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Envelopes"))
			{
				return ChartSeriesFormula.Envelopes;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Performance"))
			{
				return ChartSeriesFormula.Performance;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return ChartSeriesFormula.RateOfChange;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RelativeStrengthIndex"))
			{
				return ChartSeriesFormula.RelativeStrengthIndex;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StandardDeviation"))
			{
				return ChartSeriesFormula.StandardDeviation;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TRIX"))
			{
				return ChartSeriesFormula.TRIX;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mean"))
			{
				return ChartSeriesFormula.Mean;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return ChartSeriesFormula.Median;
			}
			return ChartSeriesFormula.BollingerBands;
		}

		internal static ChartTitlePositions TranslateChartTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartTitlePositions.TopCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartTitlePositions.TopLeft;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartTitlePositions.TopRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartTitlePositions.LeftTop;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartTitlePositions.LeftCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartTitlePositions.LeftBottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartTitlePositions.RightTop;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartTitlePositions.RightCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartTitlePositions.RightBottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartTitlePositions.BottomRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartTitlePositions.BottomCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartTitlePositions.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTitlePositions.TopCenter;
		}

		internal static ChartTitleDockings TranslateChartTitleDocking(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartTitleDockings.Top;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartTitleDockings.Left;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartTitleDockings.Right;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartTitleDockings.Bottom;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTitleDockings.Top;
		}

		internal static ChartAxisTitlePositions TranslateChartAxisTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartAxisTitlePositions.Center;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
				{
					return ChartAxisTitlePositions.Near;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
				{
					return ChartAxisTitlePositions.Far;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisTitlePositions.Center;
		}

		internal static ChartSeparators TranslateChartSeparator(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartSeparators.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeparators.Line;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
				{
					return ChartSeparators.ThickLine;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
				{
					return ChartSeparators.DoubleLine;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
				{
					return ChartSeparators.DashLine;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
				{
					return ChartSeparators.DotLine;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
				{
					return ChartSeparators.GradientLine;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
				{
					return ChartSeparators.ThickGradientLine;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeparators.None;
		}

		internal static ChartLegendLayouts TranslateChartLegendLayout(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartLegendLayouts.Column;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
				{
					return ChartLegendLayouts.Row;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
				{
					return ChartLegendLayouts.AutoTable;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
				{
					return ChartLegendLayouts.WideTable;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
				{
					return ChartLegendLayouts.TallTable;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartLegendLayouts.AutoTable;
		}

		internal static ChartLegendPositions TranslateChartLegendPositions(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartLegendPositions.TopCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartLegendPositions.TopLeft;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartLegendPositions.TopRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartLegendPositions.LeftTop;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartLegendPositions.LeftCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartLegendPositions.LeftBottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartLegendPositions.RightTop;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartLegendPositions.RightCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartLegendPositions.RightBottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartLegendPositions.BottomRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartLegendPositions.BottomCenter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartLegendPositions.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartLegendPositions.RightTop;
		}

		internal static ChartAreaAlignOrientations TranslateChartAreaAlignOrientation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAreaAlignOrientations.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
				{
					return ChartAreaAlignOrientations.Vertical;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
				{
					return ChartAreaAlignOrientations.Horizontal;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
				{
					return ChartAreaAlignOrientations.All;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAreaAlignOrientations.None;
		}

		internal static ChartThreeDProjectionModes TranslateChartThreeDProjectionMode(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Oblique"))
				{
					return ChartThreeDProjectionModes.Oblique;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Perspective"))
				{
					return ChartThreeDProjectionModes.Perspective;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartThreeDProjectionModes.Oblique;
		}

		internal static ChartThreeDShadingTypes TranslateChartThreeDShading(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartThreeDShadingTypes.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Real"))
				{
					return ChartThreeDShadingTypes.Real;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
				{
					return ChartThreeDShadingTypes.Simple;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartThreeDShadingTypes.Real;
		}

		internal static ChartBorderSkinType TranslateChartBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBorderSkinType.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
				{
					return ChartBorderSkinType.Emboss;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
				{
					return ChartBorderSkinType.Raised;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
				{
					return ChartBorderSkinType.Sunken;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
				{
					return ChartBorderSkinType.FrameThin1;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
				{
					return ChartBorderSkinType.FrameThin2;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
				{
					return ChartBorderSkinType.FrameThin3;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
				{
					return ChartBorderSkinType.FrameThin4;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
				{
					return ChartBorderSkinType.FrameThin5;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
				{
					return ChartBorderSkinType.FrameThin6;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
				{
					return ChartBorderSkinType.FrameTitle1;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
				{
					return ChartBorderSkinType.FrameTitle2;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
				{
					return ChartBorderSkinType.FrameTitle3;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
				{
					return ChartBorderSkinType.FrameTitle4;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
				{
					return ChartBorderSkinType.FrameTitle5;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
				{
					return ChartBorderSkinType.FrameTitle6;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
				{
					return ChartBorderSkinType.FrameTitle7;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
				{
					return ChartBorderSkinType.FrameTitle8;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartBorderSkinType.None;
		}

		internal static ChartSeriesType TranslateChartSeriesType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesType.Column;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesType.Bar;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeriesType.Line;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Shape"))
				{
					return ChartSeriesType.Shape;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Scatter"))
				{
					return ChartSeriesType.Scatter;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Area"))
				{
					return ChartSeriesType.Area;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Range"))
				{
					return ChartSeriesType.Range;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Polar"))
				{
					return ChartSeriesType.Polar;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeriesType.Column;
		}

		internal static ChartSeriesSubtype TranslateChartSeriesSubtype(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Plain"))
				{
					return ChartSeriesSubtype.Plain;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
				{
					return ChartSeriesSubtype.Stacked;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PercentStacked"))
				{
					return ChartSeriesSubtype.PercentStacked;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Smooth"))
				{
					return ChartSeriesSubtype.Smooth;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stepped"))
				{
					return ChartSeriesSubtype.Stepped;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pie"))
				{
					return ChartSeriesSubtype.Pie;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedPie"))
				{
					return ChartSeriesSubtype.ExplodedPie;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Doughnut"))
				{
					return ChartSeriesSubtype.Doughnut;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedDoughnut"))
				{
					return ChartSeriesSubtype.ExplodedDoughnut;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Funnel"))
				{
					return ChartSeriesSubtype.Funnel;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pyramid"))
				{
					return ChartSeriesSubtype.Pyramid;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TreeMap"))
				{
					return ChartSeriesSubtype.TreeMap;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunburst"))
				{
					return ChartSeriesSubtype.Sunburst;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bubble"))
				{
					return ChartSeriesSubtype.Bubble;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Candlestick"))
				{
					return ChartSeriesSubtype.Candlestick;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stock"))
				{
					return ChartSeriesSubtype.Stock;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesSubtype.Bar;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesSubtype.Column;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxPlot"))
				{
					return ChartSeriesSubtype.BoxPlot;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ErrorBar"))
				{
					return ChartSeriesSubtype.ErrorBar;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Radar"))
				{
					return ChartSeriesSubtype.Radar;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeriesSubtype.Plain;
		}

		internal static ChartDataLabelPositions TranslateChartDataLabelPosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartDataLabelPositions.Auto;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartDataLabelPositions.Top;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartDataLabelPositions.TopLeft;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartDataLabelPositions.TopRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartDataLabelPositions.Left;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartDataLabelPositions.Center;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartDataLabelPositions.Right;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartDataLabelPositions.BottomRight;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartDataLabelPositions.BottomLeft;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartDataLabelPositions.Bottom;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartDataLabelPositions.Outside;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartDataLabelPositions.Auto;
		}

		internal static ChartMarkerTypes TranslateChartMarkerType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartMarkerTypes.None;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartMarkerTypes.Square;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
				{
					return ChartMarkerTypes.Circle;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartMarkerTypes.Diamond;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartMarkerTypes.Triangle;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartMarkerTypes.Cross;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star4"))
				{
					return ChartMarkerTypes.Star4;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star5"))
				{
					return ChartMarkerTypes.Star5;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star6"))
				{
					return ChartMarkerTypes.Star6;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star10"))
				{
					return ChartMarkerTypes.Star10;
				}
				if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartMarkerTypes.Auto;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartMarkerTypes.None;
		}

		internal static ChartPalette TranslateChartPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ChartPalette.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return ChartPalette.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EarthTones"))
			{
				return ChartPalette.EarthTones;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Excel"))
			{
				return ChartPalette.Excel;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GrayScale"))
			{
				return ChartPalette.GrayScale;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return ChartPalette.Light;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pastel"))
			{
				return ChartPalette.Pastel;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return ChartPalette.SemiTransparent;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Berry"))
			{
				return ChartPalette.Berry;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Chocolate"))
			{
				return ChartPalette.Chocolate;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fire"))
			{
				return ChartPalette.Fire;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeaGreen"))
			{
				return ChartPalette.SeaGreen;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return ChartPalette.BrightPastel;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return ChartPalette.Pacific;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificLight"))
			{
				return ChartPalette.PacificLight;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificSemiTransparent"))
			{
				return ChartPalette.PacificSemiTransparent;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return ChartPalette.Custom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return ChartPalette.Default;
		}

		internal static PaletteHatchBehavior TranslatePaletteHatchBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return PaletteHatchBehavior.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return PaletteHatchBehavior.Default;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return PaletteHatchBehavior.None;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Always"))
			{
				return PaletteHatchBehavior.Always;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return PaletteHatchBehavior.Default;
		}

		internal static Image.SourceType TranslateImageSourceType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Image.SourceType.External;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "External"))
			{
				return Image.SourceType.External;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Embedded"))
			{
				return Image.SourceType.Embedded;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Database"))
			{
				return Image.SourceType.Database;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return Image.SourceType.External;
		}

		internal static TextOrientations TranslateTextOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextOrientations.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return TextOrientations.Auto;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return TextOrientations.Horizontal;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated90"))
			{
				return TextOrientations.Rotated90;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated270"))
			{
				return TextOrientations.Rotated270;
			}
			if (ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
			{
				return TextOrientations.Stacked;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return TextOrientations.Auto;
		}
	}
}
