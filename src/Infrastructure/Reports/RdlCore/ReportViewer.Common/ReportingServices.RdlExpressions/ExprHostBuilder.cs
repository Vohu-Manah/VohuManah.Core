using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace ReportingServices.RdlExpressions
{
	internal sealed class ExprHostBuilder
	{
		internal enum ErrorSource
		{
			Expression,
			CodeModuleClassInstanceDecl,
			CustomCode,
			Unknown
		}

		internal enum DataRegionMode
		{
			Tablix,
			Chart,
			GaugePanel,
			CustomReportItem,
			MapDataRegion,
			DataShape
		}

		private static class Constants
		{
			internal const string ReportObjectModelNS = "ReportingServices.ReportProcessing.ReportObjectModel";

			internal const string ExprHostObjectModelNS = "ReportingServices.RdlExpressions.ExpressionHostObjectModel";

			internal const string ReportExprHost = "ReportExprHost";

			internal const string IndexedExprHost = "IndexedExprHost";

			internal const string ReportParamExprHost = "ReportParamExprHost";

			internal const string CalcFieldExprHost = "CalcFieldExprHost";

			internal const string DataSourceExprHost = "DataSourceExprHost";

			internal const string DataSetExprHost = "DataSetExprHost";

			internal const string ReportItemExprHost = "ReportItemExprHost";

			internal const string ActionExprHost = "ActionExprHost";

			internal const string ActionInfoExprHost = "ActionInfoExprHost";

			internal const string TextBoxExprHost = "TextBoxExprHost";

			internal const string ImageExprHost = "ImageExprHost";

			internal const string ParamExprHost = "ParamExprHost";

			internal const string SubreportExprHost = "SubreportExprHost";

			internal const string SortExprHost = "SortExprHost";

			internal const string FilterExprHost = "FilterExprHost";

			internal const string GroupExprHost = "GroupExprHost";

			internal const string StyleExprHost = "StyleExprHost";

			internal const string AggregateParamExprHost = "AggregateParamExprHost";

			internal const string LookupExprHost = "LookupExprHost";

			internal const string LookupDestExprHost = "LookupDestExprHost";

			internal const string ReportSectionExprHost = "ReportSectionExprHost";

			internal const string JoinConditionExprHost = "JoinConditionExprHost";

			internal const string IncludeParametersParam = "includeParameters";

			internal const string ParametersOnlyParam = "parametersOnly";

			internal const string CustomCodeProxy = "CustomCodeProxy";

			internal const string CustomCodeProxyBase = "CustomCodeProxyBase";

			internal const string ReportObjectModelParam = "reportObjectModel";

			internal const string SetReportObjectModel = "SetReportObjectModel";

			internal const string Code = "Code";

			internal const string CodeProxyBase = "m_codeProxyBase";

			internal const string CodeParam = "code";

			internal const string Report = "Report";

			internal const string RemoteArrayWrapper = "RemoteArrayWrapper";

			internal const string RemoteMemberArrayWrapper = "RemoteMemberArrayWrapper";

			internal const string LabelExpr = "LabelExpr";

			internal const string ValueExpr = "ValueExpr";

			internal const string NoRowsExpr = "NoRowsExpr";

			internal const string ParameterHosts = "m_parameterHostsRemotable";

			internal const string IndexParam = "index";

			internal const string FilterHosts = "m_filterHostsRemotable";

			internal const string SortHost = "m_sortHost";

			internal const string GroupHost = "m_groupHost";

			internal const string VisibilityHiddenExpr = "VisibilityHiddenExpr";

			internal const string SortDirectionHosts = "SortDirectionHosts";

			internal const string DataValueHosts = "m_dataValueHostsRemotable";

			internal const string CustomPropertyHosts = "m_customPropertyHostsRemotable";

			internal const string VariableValueHosts = "VariableValueHosts";

			internal const string ReportLanguageExpr = "ReportLanguageExpr";

			internal const string AutoRefreshExpr = "AutoRefreshExpr";

			internal const string AggregateParamHosts = "m_aggregateParamHostsRemotable";

			internal const string ReportParameterHosts = "m_reportParameterHostsRemotable";

			internal const string DataSourceHosts = "m_dataSourceHostsRemotable";

			internal const string DataSetHosts = "m_dataSetHostsRemotable";

			internal const string PageSectionHosts = "m_pageSectionHostsRemotable";

			internal const string PageHosts = "m_pageHostsRemotable";

			internal const string ReportSectionHosts = "m_reportSectionHostsRemotable";

			internal const string LineHosts = "m_lineHostsRemotable";

			internal const string RectangleHosts = "m_rectangleHostsRemotable";

			internal const string TextBoxHosts = "m_textBoxHostsRemotable";

			internal const string ImageHosts = "m_imageHostsRemotable";

			internal const string SubreportHosts = "m_subreportHostsRemotable";

			internal const string TablixHosts = "m_tablixHostsRemotable";

			internal const string ChartHosts = "m_chartHostsRemotable";

			internal const string GaugePanelHosts = "m_gaugePanelHostsRemotable";

			internal const string CustomReportItemHosts = "m_customReportItemHostsRemotable";

			internal const string LookupExprHosts = "m_lookupExprHostsRemotable";

			internal const string LookupDestExprHosts = "m_lookupDestExprHostsRemotable";

			internal const string ReportInitialPageName = "InitialPageNameExpr";

			internal const string ConnectStringExpr = "ConnectStringExpr";

			internal const string FieldHosts = "m_fieldHostsRemotable";

			internal const string QueryParametersHost = "QueryParametersHost";

			internal const string QueryCommandTextExpr = "QueryCommandTextExpr";

			internal const string JoinConditionHosts = "m_joinConditionExprHostsRemotable";

			internal const string PromptExpr = "PromptExpr";

			internal const string ValidValuesHost = "ValidValuesHost";

			internal const string ValidValueLabelsHost = "ValidValueLabelsHost";

			internal const string ValidationExpressionExpr = "ValidationExpressionExpr";

			internal const string ActionInfoHost = "ActionInfoHost";

			internal const string ActionHost = "ActionHost";

			internal const string ActionItemHosts = "m_actionItemHostsRemotable";

			internal const string BookmarkExpr = "BookmarkExpr";

			internal const string ToolTipExpr = "ToolTipExpr";

			internal const string ToggleImageInitialStateExpr = "ToggleImageInitialStateExpr";

			internal const string UserSortExpressionsHost = "UserSortExpressionsHost";

			internal const string MIMETypeExpr = "MIMETypeExpr";

			internal const string TagExpr = "TagExpr";

			internal const string OmitExpr = "OmitExpr";

			internal const string HyperlinkExpr = "HyperlinkExpr";

			internal const string DrillThroughReportNameExpr = "DrillThroughReportNameExpr";

			internal const string DrillThroughParameterHosts = "m_drillThroughParameterHostsRemotable";

			internal const string DrillThroughBookmakLinkExpr = "DrillThroughBookmarkLinkExpr";

			internal const string BookmarkLinkExpr = "BookmarkLinkExpr";

			internal const string FilterExpressionExpr = "FilterExpressionExpr";

			internal const string ParentExpressionsHost = "ParentExpressionsHost";

			internal const string ReGroupExpressionsHost = "ReGroupExpressionsHost";

			internal const string DataValueExprHost = "DataValueExprHost";

			internal const string DataValueNameExpr = "DataValueNameExpr";

			internal const string DataValueValueExpr = "DataValueValueExpr";

			internal const string TablixExprHost = "TablixExprHost";

			internal const string DataShapeExprHost = "DataShapeExprHost";

			internal const string ChartExprHost = "ChartExprHost";

			internal const string GaugePanelExprHost = "GaugePanelExprHost";

			internal const string CustomReportItemExprHost = "CustomReportItemExprHost";

			internal const string MapDataRegionExprHost = "MapDataRegionExprHost";

			internal const string TablixMemberExprHost = "TablixMemberExprHost";

			internal const string DataShapeMemberExprHost = "DataShapeMemberExprHost";

			internal const string ChartMemberExprHost = "ChartMemberExprHost";

			internal const string GaugeMemberExprHost = "GaugeMemberExprHost";

			internal const string DataGroupExprHost = "DataGroupExprHost";

			internal const string TablixCellExprHost = "TablixCellExprHost";

			internal const string DataShapeIntersectionExprHost = "DataShapeIntersectionExprHost";

			internal const string ChartDataPointExprHost = "ChartDataPointExprHost";

			internal const string GaugeCellExprHost = "GaugeCellExprHost";

			internal const string DataCellExprHost = "DataCellExprHost";

			internal const string MemberTreeHosts = "m_memberTreeHostsRemotable";

			internal const string DataCellHosts = "m_cellHostsRemotable";

			internal const string MapMemberExprHost = "MapMemberExprHost";

			internal const string TablixCornerCellHosts = "m_cornerCellHostsRemotable";

			internal const string ChartTitleExprHost = "ChartTitleExprHost";

			internal const string ChartAxisTitleExprHost = "ChartAxisTitleExprHost";

			internal const string ChartLegendTitleExprHost = "ChartLegendTitleExprHost";

			internal const string ChartLegendExprHost = "ChartLegendExprHost";

			internal const string ChartTitleHost = "TitleHost";

			internal const string ChartNoDataMessageHost = "NoDataMessageHost";

			internal const string ChartLegendTitleHost = "TitleExprHost";

			internal const string PaletteExpr = "PaletteExpr";

			internal const string PaletteHatchBehaviorExpr = "PaletteHatchBehaviorExpr";

			internal const string ChartAreaExprHost = "ChartAreaExprHost";

			internal const string ChartNoMoveDirectionsExprHost = "ChartNoMoveDirectionsExprHost";

			internal const string ChartNoMoveDirectionsHost = "NoMoveDirectionsHost";

			internal const string UpExpr = "UpExpr";

			internal const string DownExpr = "DownExpr";

			internal const string LeftExpr = "LeftExpr";

			internal const string RightExpr = "RightExpr";

			internal const string UpLeftExpr = "UpLeftExpr";

			internal const string UpRightExpr = "UpRightExpr";

			internal const string DownLeftExpr = "DownLeftExpr";

			internal const string DownRightExpr = "DownRightExpr";

			internal const string ChartSmartLabelExprHost = "ChartSmartLabelExprHost";

			internal const string ChartSmartLabelHost = "SmartLabelHost";

			internal const string AllowOutSidePlotAreaExpr = "AllowOutSidePlotAreaExpr";

			internal const string CalloutBackColorExpr = "CalloutBackColorExpr";

			internal const string CalloutLineAnchorExpr = "CalloutLineAnchorExpr";

			internal const string CalloutLineColorExpr = "CalloutLineColorExpr";

			internal const string CalloutLineStyleExpr = "CalloutLineStyleExpr";

			internal const string CalloutLineWidthExpr = "CalloutLineWidthExpr";

			internal const string CalloutStyleExpr = "CalloutStyleExpr";

			internal const string HideOverlappedExpr = "HideOverlappedExpr";

			internal const string MarkerOverlappingExpr = "MarkerOverlappingExpr";

			internal const string MaxMovingDistanceExpr = "MaxMovingDistanceExpr";

			internal const string MinMovingDistanceExpr = "MinMovingDistanceExpr";

			internal const string DisabledExpr = "DisabledExpr";

			internal const string ChartAxisScaleBreakExprHost = "ChartAxisScaleBreakExprHost";

			internal const string ChartAxisScaleBreakHost = "AxisScaleBreakHost";

			internal const string ChartBorderSkinExprHost = "ChartBorderSkinExprHost";

			internal const string ChartBorderSkinHost = "BorderSkinHost";

			internal const string TitleSeparatorExpr = "TitleSeparatorExpr";

			internal const string ColumnTypeExpr = "ColumnTypeExpr";

			internal const string MinimumWidthExpr = "MinimumWidthExpr";

			internal const string MaximumWidthExpr = "MaximumWidthExpr";

			internal const string SeriesSymbolWidthExpr = "SeriesSymbolWidthExpr";

			internal const string SeriesSymbolHeightExpr = "SeriesSymbolHeightExpr";

			internal const string CellTypeExpr = "CellTypeExpr";

			internal const string TextExpr = "TextExpr";

			internal const string CellSpanExpr = "CellSpanExpr";

			internal const string ImageWidthExpr = "ImageWidthExpr";

			internal const string ImageHeightExpr = "ImageHeightExpr";

			internal const string SymbolHeightExpr = "SymbolHeightExpr";

			internal const string SymbolWidthExpr = "SymbolWidthExpr";

			internal const string AlignmentExpr = "AlignmentExpr";

			internal const string TopMarginExpr = "TopMarginExpr";

			internal const string BottomMarginExpr = "BottomMarginExpr";

			internal const string LeftMarginExpr = "LeftMarginExpr";

			internal const string RightMarginExpr = "RightMarginExpr";

			internal const string VisibleExpr = "VisibleExpr";

			internal const string MarginExpr = "MarginExpr";

			internal const string IntervalExpr = "IntervalExpr";

			internal const string IntervalTypeExpr = "IntervalTypeExpr";

			internal const string IntervalOffsetExpr = "IntervalOffsetExpr";

			internal const string IntervalOffsetTypeExpr = "IntervalOffsetTypeExpr";

			internal const string MarksAlwaysAtPlotEdgeExpr = "MarksAlwaysAtPlotEdgeExpr";

			internal const string ReverseExpr = "ReverseExpr";

			internal const string LocationExpr = "LocationExpr";

			internal const string InterlacedExpr = "InterlacedExpr";

			internal const string InterlacedColorExpr = "InterlacedColorExpr";

			internal const string LogScaleExpr = "LogScaleExpr";

			internal const string LogBaseExpr = "LogBaseExpr";

			internal const string HideLabelsExpr = "HideLabelsExpr";

			internal const string AngleExpr = "AngleExpr";

			internal const string ArrowsExpr = "ArrowsExpr";

			internal const string PreventFontShrinkExpr = "PreventFontShrinkExpr";

			internal const string PreventFontGrowExpr = "PreventFontGrowExpr";

			internal const string PreventLabelOffsetExpr = "PreventLabelOffsetExpr";

			internal const string PreventWordWrapExpr = "PreventWordWrapExpr";

			internal const string AllowLabelRotationExpr = "AllowLabelRotationExpr";

			internal const string IncludeZeroExpr = "IncludeZeroExpr";

			internal const string LabelsAutoFitDisabledExpr = "LabelsAutoFitDisabledExpr";

			internal const string MinFontSizeExpr = "MinFontSizeExpr";

			internal const string MaxFontSizeExpr = "MaxFontSizeExpr";

			internal const string OffsetLabelsExpr = "OffsetLabelsExpr";

			internal const string HideEndLabelsExpr = "HideEndLabelsExpr";

			internal const string ChartTickMarksExprHost = "ChartTickMarksExprHost";

			internal const string ChartTickMarksHost = "TickMarksHost";

			internal const string ChartGridLinesExprHost = "ChartGridLinesExprHost";

			internal const string ChartGridLinesHost = "GridLinesHost";

			internal const string ChartDataPointInLegendExprHost = "ChartDataPointInLegendExprHost";

			internal const string ChartDataPointInLegendHost = "DataPointInLegendHost";

			internal const string ChartEmptyPointsExprHost = "ChartEmptyPointsExprHost";

			internal const string ChartEmptyPointsHost = "EmptyPointsHost";

			internal const string AxisLabelExpr = "AxisLabelExpr";

			internal const string LegendTextExpr = "LegendTextExpr";

			internal const string ChartLegendColumnHeaderExprHost = "ChartLegendColumnHeaderExprHost";

			internal const string ChartLegendColumnHeaderHost = "ChartLegendColumnHeaderHost";

			internal const string ChartCustomPaletteColorExprHost = "ChartCustomPaletteColorExprHost";

			internal const string ChartCustomPaletteColorHosts = "m_customPaletteColorHostsRemotable";

			internal const string ChartLegendCustomItemCellExprHost = "ChartLegendCustomItemCellExprHost";

			internal const string ChartLegendCustomItemCellsHosts = "m_legendCustomItemCellHostsRemotable";

			internal const string ChartDerivedSeriesExprHost = "ChartDerivedSeriesExprHost";

			internal const string ChartDerivedSeriesCollectionHosts = "m_derivedSeriesCollectionHostsRemotable";

			internal const string SourceChartSeriesNameExpr = "SourceChartSeriesNameExpr";

			internal const string DerivedSeriesFormulaExpr = "DerivedSeriesFormulaExpr";

			internal const string SizeExpr = "SizeExpr";

			internal const string TypeExpr = "TypeExpr";

			internal const string SubtypeExpr = "SubtypeExpr";

			internal const string LegendNameExpr = "LegendNameExpr";

			internal const string ChartAreaNameExpr = "ChartAreaNameExpr";

			internal const string ValueAxisNameExpr = "ValueAxisNameExpr";

			internal const string CategoryAxisNameExpr = "CategoryAxisNameExpr";

			internal const string ChartStripLineExprHost = "ChartStripLineExprHost";

			internal const string ChartStripLinesHosts = "m_stripLinesHostsRemotable";

			internal const string ChartSeriesExprHost = "ChartSeriesExprHost";

			internal const string ChartSeriesHost = "ChartSeriesHost";

			internal const string TitleExpr = "TitleExpr";

			internal const string TitleAngleExpr = "TitleAngleExpr";

			internal const string StripWidthExpr = "StripWidthExpr";

			internal const string StripWidthTypeExpr = "StripWidthTypeExpr";

			internal const string HiddenExpr = "HiddenExpr";

			internal const string ChartFormulaParameterExprHost = "ChartFormulaParameterExprHost";

			internal const string ChartFormulaParametersHosts = "m_formulaParametersHostsRemotable";

			internal const string ChartLegendColumnExprHost = "ChartLegendColumnExprHost";

			internal const string ChartLegendColumnsHosts = "m_legendColumnsHostsRemotable";

			internal const string ChartLegendCustomItemExprHost = "ChartLegendCustomItemExprHost";

			internal const string ChartLegendCustomItemsHosts = "m_legendCustomItemsHostsRemotable";

			internal const string SeparatorExpr = "SeparatorExpr";

			internal const string SeparatorColorExpr = "SeparatorColorExpr";

			internal const string ChartValueAxesHosts = "m_valueAxesHostsRemotable";

			internal const string ChartCategoryAxesHosts = "m_categoryAxesHostsRemotable";

			internal const string ChartTitlesHosts = "m_titlesHostsRemotable";

			internal const string ChartLegendsHosts = "m_legendsHostsRemotable";

			internal const string ChartAreasHosts = "m_chartAreasHostsRemotable";

			internal const string ChartAxisExprHost = "ChartAxisExprHost";

			internal const string MemberLabelExpr = "MemberLabelExpr";

			internal const string MemberStyleHost = "MemberStyleHost";

			internal const string DataLabelStyleHost = "DataLabelStyleHost";

			internal const string StyleHost = "StyleHost";

			internal const string MarkerStyleHost = "MarkerStyleHost";

			internal const string CaptionExpr = "CaptionExpr";

			internal const string CategoryAxisHost = "CategoryAxisHost";

			internal const string PlotAreaHost = "PlotAreaHost";

			internal const string AxisMinExpr = "AxisMinExpr";

			internal const string AxisMaxExpr = "AxisMaxExpr";

			internal const string AxisCrossAtExpr = "AxisCrossAtExpr";

			internal const string AxisMajorIntervalExpr = "AxisMajorIntervalExpr";

			internal const string AxisMinorIntervalExpr = "AxisMinorIntervalExpr";

			internal const string ChartDataPointValueXExpr = "DataPointValuesXExpr";

			internal const string ChartDataPointValueYExpr = "DataPointValuesYExpr";

			internal const string ChartDataPointValueSizeExpr = "DataPointValuesSizeExpr";

			internal const string ChartDataPointValueHighExpr = "DataPointValuesHighExpr";

			internal const string ChartDataPointValueLowExpr = "DataPointValuesLowExpr";

			internal const string ChartDataPointValueStartExpr = "DataPointValuesStartExpr";

			internal const string ChartDataPointValueEndExpr = "DataPointValuesEndExpr";

			internal const string ChartDataPointValueMeanExpr = "DataPointValuesMeanExpr";

			internal const string ChartDataPointValueMedianExpr = "DataPointValuesMedianExpr";

			internal const string ChartDataPointValueHighlightXExpr = "DataPointValuesHighlightXExpr";

			internal const string ChartDataPointValueHighlightYExpr = "DataPointValuesHighlightYExpr";

			internal const string ChartDataPointValueHighlightSizeExpr = "DataPointValuesHighlightSizeExpr";

			internal const string ChartDataPointValueFormatXExpr = "ChartDataPointValueFormatXExpr";

			internal const string ChartDataPointValueFormatYExpr = "ChartDataPointValueFormatYExpr";

			internal const string ChartDataPointValueFormatSizeExpr = "ChartDataPointValueFormatSizeExpr";

			internal const string ChartDataPointValueCurrencyLanguageXExpr = "ChartDataPointValueCurrencyLanguageXExpr";

			internal const string ChartDataPointValueCurrencyLanguageYExpr = "ChartDataPointValueCurrencyLanguageYExpr";

			internal const string ChartDataPointValueCurrencyLanguageSizeExpr = "ChartDataPointValueCurrencyLanguageSizeExpr";

			internal const string ColorExpr = "ColorExpr";

			internal const string BorderSkinTypeExpr = "BorderSkinTypeExpr";

			internal const string LengthExpr = "LengthExpr";

			internal const string EnabledExpr = "EnabledExpr";

			internal const string BreakLineTypeExpr = "BreakLineTypeExpr";

			internal const string CollapsibleSpaceThresholdExpr = "CollapsibleSpaceThresholdExpr";

			internal const string MaxNumberOfBreaksExpr = "MaxNumberOfBreaksExpr";

			internal const string SpacingExpr = "SpacingExpr";

			internal const string AxesViewExpr = "AxesViewExpr";

			internal const string CursorExpr = "CursorExpr";

			internal const string InnerPlotPositionExpr = "InnerPlotPositionExpr";

			internal const string ChartAlignTypePositionExpr = "ChartAlignTypePositionExpr";

			internal const string EquallySizedAxesFontExpr = "EquallySizedAxesFontExpr";

			internal const string AlignOrientationExpr = "AlignOrientationExpr";

			internal const string Chart3DPropertiesExprHost = "Chart3DPropertiesExprHost";

			internal const string Chart3DPropertiesHost = "Chart3DPropertiesHost";

			internal const string LayoutExpr = "LayoutExpr";

			internal const string DockOutsideChartAreaExpr = "DockOutsideChartAreaExpr";

			internal const string TitleExprHost = "TitleExprHost";

			internal const string AutoFitTextDisabledExpr = "AutoFitTextDisabledExpr";

			internal const string HeaderSeparatorExpr = "HeaderSeparatorExpr";

			internal const string HeaderSeparatorColorExpr = "HeaderSeparatorColorExpr";

			internal const string ColumnSeparatorExpr = "ColumnSeparatorExpr";

			internal const string ColumnSeparatorColorExpr = "ColumnSeparatorColorExpr";

			internal const string ColumnSpacingExpr = "ColumnSpacingExpr";

			internal const string InterlacedRowsExpr = "InterlacedRowsExpr";

			internal const string InterlacedRowsColorExpr = "InterlacedRowsColorExpr";

			internal const string EquallySpacedItemsExpr = "EquallySpacedItemsExpr";

			internal const string ReversedExpr = "ReversedExpr";

			internal const string MaxAutoSizeExpr = "MaxAutoSizeExpr";

			internal const string TextWrapThresholdExpr = "TextWrapThresholdExpr";

			internal const string DockingExpr = "DockingExpr";

			internal const string ChartTitlePositionExpr = "ChartTitlePositionExpr";

			internal const string DockingOffsetExpr = "DockingOffsetExpr";

			internal const string ChartLegendPositionExpr = "ChartLegendPositionExpr";

			internal const string DockOutsideChartArea = "DockOutsideChartArea";

			internal const string AutoFitTextDisabled = "AutoFitTextDisabled";

			internal const string MinFontSize = "MinFontSize";

			internal const string HeaderSeparator = "HeaderSeparator";

			internal const string HeaderSeparatorColor = "HeaderSeparatorColor";

			internal const string ColumnSeparator = "ColumnSeparator";

			internal const string ColumnSeparatorColor = "ColumnSeparatorColor";

			internal const string ColumnSpacing = "ColumnSpacing";

			internal const string InterlacedRows = "InterlacedRows";

			internal const string InterlacedRowsColor = "InterlacedRowsColor";

			internal const string EquallySpacedItems = "EquallySpacedItems";

			internal const string HideInLegendExpr = "HideInLegendExpr";

			internal const string ShowOverlappedExpr = "ShowOverlappedExpr";

			internal const string MajorChartTickMarksHost = "MajorTickMarksHost";

			internal const string MinorChartTickMarksHost = "MinorTickMarksHost";

			internal const string MajorChartGridLinesHost = "MajorGridLinesHost";

			internal const string MinorChartGridLinesHost = "MinorGridLinesHost";

			internal const string RotationExpr = "RotationExpr";

			internal const string ProjectionModeExpr = "ProjectionModeExpr";

			internal const string InclinationExpr = "InclinationExpr";

			internal const string PerspectiveExpr = "PerspectiveExpr";

			internal const string DepthRatioExpr = "DepthRatioExpr";

			internal const string ShadingExpr = "ShadingExpr";

			internal const string GapDepthExpr = "GapDepthExpr";

			internal const string WallThicknessExpr = "WallThicknessExpr";

			internal const string ClusteredExpr = "ClusteredExpr";

			internal const string ChartDataLabelExprHost = "ChartDataLabelExprHost";

			internal const string ChartDataLabelPositionExpr = "ChartDataLabelPositionExpr";

			internal const string UseValueAsLabelExpr = "UseValueAsLabelExpr";

			internal const string ChartDataLabelHost = "DataLabelHost";

			internal const string ChartMarkerExprHost = "ChartMarkerExprHost";

			internal const string ChartMarkerHost = "ChartMarkerHost";

			internal const string VariableAutoIntervalExpr = "VariableAutoIntervalExpr";

			internal const string LabelIntervalExpr = "LabelIntervalExpr";

			internal const string LabelIntervalTypeExpr = "LabelIntervalTypeExpr";

			internal const string LabelIntervalOffsetExpr = "LabelIntervalOffsetExpr";

			internal const string LabelIntervalOffsetTypeExpr = "LabelIntervalOffsetTypeExpr";

			internal const string DynamicWidthExpr = "DynamicWidthExpr";

			internal const string DynamicHeightExpr = "DynamicHeightExpr";

			internal const string TextOrientationExpr = "TextOrientationExpr";

			internal const string ChartElementPositionExprHost = "ChartElementPositionExprHost";

			internal const string ChartElementPositionHost = "ChartElementPositionHost";

			internal const string ChartInnerPlotPositionHost = "ChartInnerPlotPositionHost";

			internal const string BaseGaugeImageExprHost = "BaseGaugeImageExprHost";

			internal const string BaseGaugeImageHost = "BaseGaugeImageHost";

			internal const string SourceExpr = "SourceExpr";

			internal const string TransparentColorExpr = "TransparentColorExpr";

			internal const string CapImageExprHost = "CapImageExprHost";

			internal const string CapImageHost = "CapImageHost";

			internal const string TopImageHost = "TopImageHost";

			internal const string TopImageExprHost = "TopImageExprHost";

			internal const string HueColorExpr = "HueColorExpr";

			internal const string OffsetXExpr = "OffsetXExpr";

			internal const string OffsetYExpr = "OffsetYExpr";

			internal const string FrameImageExprHost = "FrameImageExprHost";

			internal const string FrameImageHost = "FrameImageHost";

			internal const string IndicatorImageExprHost = "IndicatorImageExprHost";

			internal const string IndicatorImageHost = "IndicatorImageHost";

			internal const string TransparencyExpr = "TransparencyExpr";

			internal const string ClipImageExpr = "ClipImageExpr";

			internal const string PointerImageExprHost = "PointerImageExprHost";

			internal const string PointerImageHost = "PointerImageHost";

			internal const string BackFrameExprHost = "BackFrameExprHost";

			internal const string BackFrameHost = "BackFrameHost";

			internal const string FrameStyleExpr = "FrameStyleExpr";

			internal const string FrameShapeExpr = "FrameShapeExpr";

			internal const string FrameWidthExpr = "FrameWidthExpr";

			internal const string GlassEffectExpr = "GlassEffectExpr";

			internal const string FrameBackgroundExprHost = "FrameBackgroundExprHost";

			internal const string FrameBackgroundHost = "FrameBackgroundHost";

			internal const string CustomLabelExprHost = "CustomLabelExprHost";

			internal const string FontAngleExpr = "FontAngleExpr";

			internal const string UseFontPercentExpr = "UseFontPercentExpr";

			internal const string GaugeExprHost = "GaugeExprHost";

			internal const string ClipContentExpr = "ClipContentExpr";

			internal const string GaugeImageExprHost = "GaugeImageExprHost";

			internal const string AspectRatioExpr = "AspectRatioExpr";

			internal const string GaugeInputValueExprHost = "GaugeInputValueExprHost";

			internal const string FormulaExpr = "FormulaExpr";

			internal const string MinPercentExpr = "MinPercentExpr";

			internal const string MaxPercentExpr = "MaxPercentExpr";

			internal const string MultiplierExpr = "MultiplierExpr";

			internal const string AddConstantExpr = "AddConstantExpr";

			internal const string GaugeLabelExprHost = "GaugeLabelExprHost";

			internal const string AntiAliasingExpr = "AntiAliasingExpr";

			internal const string AutoLayoutExpr = "AutoLayoutExpr";

			internal const string ShadowIntensityExpr = "ShadowIntensityExpr";

			internal const string TileLanguageExpr = "TileLanguageExpr";

			internal const string TextAntiAliasingQualityExpr = "TextAntiAliasingQualityExpr";

			internal const string GaugePanelItemExprHost = "GaugePanelItemExprHost";

			internal const string TopExpr = "TopExpr";

			internal const string HeightExpr = "HeightExpr";

			internal const string GaugePointerExprHost = "GaugePointerExprHost";

			internal const string BarStartExpr = "BarStartExpr";

			internal const string MarkerLengthExpr = "MarkerLengthExpr";

			internal const string MarkerStyleExpr = "MarkerStyleExpr";

			internal const string SnappingEnabledExpr = "SnappingEnabledExpr";

			internal const string SnappingIntervalExpr = "SnappingIntervalExpr";

			internal const string GaugeScaleExprHost = "GaugeScaleExprHost";

			internal const string LogarithmicExpr = "LogarithmicExpr";

			internal const string LogarithmicBaseExpr = "LogarithmicBaseExpr";

			internal const string TickMarksOnTopExpr = "TickMarksOnTopExpr";

			internal const string GaugeTickMarksExprHost = "GaugeTickMarksExprHost";

			internal const string LinearGaugeExprHost = "LinearGaugeExprHost";

			internal const string OrientationExpr = "OrientationExpr";

			internal const string LinearPointerExprHost = "LinearPointerExprHost";

			internal const string LinearScaleExprHost = "LinearScaleExprHost";

			internal const string StartMarginExpr = "StartMarginExpr";

			internal const string EndMarginExpr = "EndMarginExpr";

			internal const string NumericIndicatorExprHost = "NumericIndicatorExprHost";

			internal const string PinLabelExprHost = "PinLabelExprHost";

			internal const string AllowUpsideDownExpr = "AllowUpsideDownExpr";

			internal const string RotateLabelExpr = "RotateLabelExpr";

			internal const string PointerCapExprHost = "PointerCapExprHost";

			internal const string OnTopExpr = "OnTopExpr";

			internal const string ReflectionExpr = "ReflectionExpr";

			internal const string CapStyleExpr = "CapStyleExpr";

			internal const string RadialGaugeExprHost = "RadialGaugeExprHost";

			internal const string PivotXExpr = "PivotXExpr";

			internal const string PivotYExpr = "PivotYExpr";

			internal const string RadialPointerExprHost = "RadialPointerExprHost";

			internal const string NeedleStyleExpr = "NeedleStyleExpr";

			internal const string RadialScaleExprHost = "RadialScaleExprHost";

			internal const string RadiusExpr = "RadiusExpr";

			internal const string StartAngleExpr = "StartAngleExpr";

			internal const string SweepAngleExpr = "SweepAngleExpr";

			internal const string ScaleLabelsExprHost = "ScaleLabelsExprHost";

			internal const string RotateLabelsExpr = "RotateLabelsExpr";

			internal const string ShowEndLabelsExpr = "ShowEndLabelsExpr";

			internal const string ScalePinExprHost = "ScalePinExprHost";

			internal const string EnableExpr = "EnableExpr";

			internal const string ScaleRangeExprHost = "ScaleRangeExprHost";

			internal const string DistanceFromScaleExpr = "DistanceFromScaleExpr";

			internal const string StartWidthExpr = "StartWidthExpr";

			internal const string EndWidthExpr = "EndWidthExpr";

			internal const string InRangeBarPointerColorExpr = "InRangeBarPointerColorExpr";

			internal const string InRangeLabelColorExpr = "InRangeLabelColorExpr";

			internal const string InRangeTickMarksColorExpr = "InRangeTickMarksColorExpr";

			internal const string BackgroundGradientTypeExpr = "BackgroundGradientTypeExpr";

			internal const string PlacementExpr = "PlacementExpr";

			internal const string StateIndicatorExprHost = "StateIndicatorExprHost";

			internal const string ThermometerExprHost = "ThermometerExprHost";

			internal const string BulbOffsetExpr = "BulbOffsetExpr";

			internal const string BulbSizeExpr = "BulbSizeExpr";

			internal const string ThermometerStyleExpr = "ThermometerStyleExpr";

			internal const string TickMarkStyleExprHost = "TickMarkStyleExprHost";

			internal const string EnableGradientExpr = "EnableGradientExpr";

			internal const string GradientDensityExpr = "GradientDensityExpr";

			internal const string GaugeMajorTickMarksHost = "GaugeMajorTickMarksHost";

			internal const string GaugeMinorTickMarksHost = "GaugeMinorTickMarksHost";

			internal const string GaugeMaximumPinHost = "MaximumPinHost";

			internal const string GaugeMinimumPinHost = "MinimumPinHost";

			internal const string GaugeInputValueHost = "GaugeInputValueHost";

			internal const string WidthExpr = "WidthExpr";

			internal const string ZIndexExpr = "ZIndexExpr";

			internal const string PositionExpr = "PositionExpr";

			internal const string ShapeExpr = "ShapeExpr";

			internal const string ScaleLabelsHost = "ScaleLabelsHost";

			internal const string ScalePinHost = "ScalePinHost";

			internal const string PinLabelHost = "PinLabelHost";

			internal const string PointerCapHost = "PointerCapHost";

			internal const string ThermometerHost = "ThermometerHost";

			internal const string TickMarkStyleHost = "TickMarkStyleHost";

			internal const string ResizeModeExpr = "ResizeModeExpr";

			internal const string TextShadowOffsetExpr = "TextShadowOffsetExpr";

			internal const string CustomLabelsHosts = "m_customLabelsHostsRemotable";

			internal const string GaugeImagesHosts = "m_gaugeImagesHostsRemotable";

			internal const string GaugeLabelsHosts = "m_gaugeLabelsHostsRemotable";

			internal const string LinearGaugesHosts = "m_linearGaugesHostsRemotable";

			internal const string RadialGaugesHosts = "m_radialGaugesHostsRemotable";

			internal const string LinearPointersHosts = "m_linearPointersHostsRemotable";

			internal const string RadialPointersHosts = "m_radialPointersHostsRemotable";

			internal const string LinearScalesHosts = "m_linearScalesHostsRemotable";

			internal const string RadialScalesHosts = "m_radialScalesHostsRemotable";

			internal const string ScaleRangesHosts = "m_scaleRangesHostsRemotable";

			internal const string NumericIndicatorsHosts = "m_numericIndicatorsHostsRemotable";

			internal const string StateIndicatorsHosts = "m_stateIndicatorsHostsRemotable";

			internal const string GaugeInputValuesHosts = "m_gaugeInputValueHostsRemotable";

			internal const string NumericIndicatorRangesHosts = "m_numericIndicatorRangesHostsRemotable";

			internal const string IndicatorStatesHosts = "m_indicatorStatesHostsRemotable";

			internal const string NumericIndicatorHost = "NumericIndicatorHost";

			internal const string DecimalDigitColorExpr = "DecimalDigitColorExpr";

			internal const string DigitColorExpr = "DigitColorExpr";

			internal const string DecimalDigitsExpr = "DecimalDigitsExpr";

			internal const string DigitsExpr = "DigitsExpr";

			internal const string NonNumericStringExpr = "NonNumericStringExpr";

			internal const string OutOfRangeStringExpr = "OutOfRangeStringExpr";

			internal const string ShowDecimalPointExpr = "ShowDecimalPointExpr";

			internal const string ShowLeadingZerosExpr = "ShowLeadingZerosExpr";

			internal const string IndicatorStyleExpr = "IndicatorStyleExpr";

			internal const string ShowSignExpr = "ShowSignExpr";

			internal const string LedDimColorExpr = "LedDimColorExpr";

			internal const string SeparatorWidthExpr = "SeparatorWidthExpr";

			internal const string NumericIndicatorRangeExprHost = "NumericIndicatorRangeExprHost";

			internal const string NumericIndicatorRangeHost = "NumericIndicatorRangeHost";

			internal const string StateIndicatorHost = "StateIndicatorHost";

			internal const string IndicatorStateExprHost = "IndicatorStateExprHost";

			internal const string IndicatorStateHost = "IndicatorStateHost";

			internal const string TransformationTypeExpr = "TransformationTypeExpr";

			internal const string MapViewExprHost = "MapViewExprHost";

			internal const string MapViewHost = "MapViewHost";

			internal const string ZoomExpr = "ZoomExpr";

			internal const string MapElementViewExprHost = "MapElementViewExprHost";

			internal const string MapElementViewHost = "MapElementViewHost";

			internal const string LayerNameExpr = "LayerNameExpr";

			internal const string MapDataBoundViewExprHost = "MapDataBoundViewExprHost";

			internal const string MapDataBoundViewHost = "MapDataBoundViewHost";

			internal const string MapCustomViewExprHost = "MapCustomViewExprHost";

			internal const string MapCustomViewHost = "MapCustomViewHost";

			internal const string CenterXExpr = "CenterXExpr";

			internal const string CenterYExpr = "CenterYExpr";

			internal const string MapBorderSkinExprHost = "MapBorderSkinExprHost";

			internal const string MapBorderSkinHost = "MapBorderSkinHost";

			internal const string MapBorderSkinTypeExpr = "MapBorderSkinTypeExpr";

			internal const string MapDataRegionNameExpr = "MapDataRegionNameExpr";

			internal const string MapTileLayerExprHost = "MapTileLayerExprHost";

			internal const string MapTileLayerHost = "MapTileLayerHost";

			internal const string ServiceUrlExpr = "ServiceUrlExpr";

			internal const string TileStyleExpr = "TileStyleExpr";

			internal const string MapTileExprHost = "MapTileExprHost";

			internal const string MapTileHost = "MapTileHost";

			internal const string UseSecureConnectionExpr = "UseSecureConnectionExpr";

			internal const string MapPolygonLayerExprHost = "MapPolygonLayerExprHost";

			internal const string MapPointLayerExprHost = "MapPointLayerExprHost";

			internal const string MapLineLayerExprHost = "MapLineLayerExprHost";

			internal const string MapSpatialDataSetExprHost = "MapSpatialDataSetExprHost";

			internal const string DataSetNameExpr = "DataSetNameExpr";

			internal const string SpatialFieldExpr = "SpatialFieldExpr";

			internal const string MapSpatialDataRegionExprHost = "MapSpatialDataRegionExprHost";

			internal const string VectorDataExpr = "VectorDataExpr";

			internal const string MapSpatialDataExprHost = "MapSpatialDataExprHost";

			internal const string MapSpatialDataHost = "MapSpatialDataHost";

			internal const string SimplificationResolutionExpr = "SimplificationResolutionExpr";

			internal const string MapShapefileExprHost = "MapShapefileExprHost";

			internal const string MapLayerExprHost = "MapLayerExprHost";

			internal const string MapLayerHost = "MapLayerHost";

			internal const string VisibilityModeExpr = "VisibilityModeExpr";

			internal const string MapFieldNameExprHost = "MapFieldNameExprHost";

			internal const string MapFieldNameHost = "MapFieldNameHost";

			internal const string NameExpr = "NameExpr";

			internal const string MapFieldDefinitionExprHost = "MapFieldDefinitionExprHost";

			internal const string MapFieldDefinitionHost = "MapFieldDefinitionHost";

			internal const string MapPointExprHost = "MapPointExprHost";

			internal const string MapPointHost = "MapPointHost";

			internal const string MapSpatialElementExprHost = "MapSpatialElementExprHost";

			internal const string MapSpatialElementHost = "MapSpatialElementHost";

			internal const string MapPolygonExprHost = "MapPolygonExprHost";

			internal const string MapPolygonHost = "MapPolygonHost";

			internal const string UseCustomPolygonTemplateExpr = "UseCustomPolygonTemplateExpr";

			internal const string UseCustomPointTemplateExpr = "UseCustomPointTemplateExpr";

			internal const string MapLineExprHost = "MapLineExprHost";

			internal const string MapLineHost = "MapLineHost";

			internal const string UseCustomLineTemplateExpr = "UseCustomLineTemplateExpr";

			internal const string MapFieldExprHost = "MapFieldExprHost";

			internal const string MapFieldHost = "MapFieldHost";

			internal const string MapPointTemplateExprHost = "MapPointTemplateExprHost";

			internal const string MapPointTemplateHost = "MapPointTemplateHost";

			internal const string MapMarkerTemplateExprHost = "MapMarkerTemplateExprHost";

			internal const string MapMarkerTemplateHost = "MapMarkerTemplateHost";

			internal const string MapPolygonTemplateExprHost = "MapPolygonTemplateExprHost";

			internal const string MapPolygonTemplateHost = "MapPolygonTemplateHost";

			internal const string ScaleFactorExpr = "ScaleFactorExpr";

			internal const string CenterPointOffsetXExpr = "CenterPointOffsetXExpr";

			internal const string CenterPointOffsetYExpr = "CenterPointOffsetYExpr";

			internal const string ShowLabelExpr = "ShowLabelExpr";

			internal const string MapLineTemplateExprHost = "MapLineTemplateExprHost";

			internal const string MapLineTemplateHost = "MapLineTemplateHost";

			internal const string MapCustomColorRuleExprHost = "MapCustomColorRuleExprHost";

			internal const string MapCustomColorExprHost = "MapCustomColorExprHost";

			internal const string MapCustomColorHost = "MapCustomColorHost";

			internal const string MapPointRulesExprHost = "MapPointRulesExprHost";

			internal const string MapPointRulesHost = "MapPointRulesHost";

			internal const string MapMarkerRuleExprHost = "MapMarkerRuleExprHost";

			internal const string MapMarkerRuleHost = "MapMarkerRuleHost";

			internal const string MapMarkerExprHost = "MapMarkerExprHost";

			internal const string MapMarkerHost = "MapMarkerHost";

			internal const string MapMarkerStyleExpr = "MapMarkerStyleExpr";

			internal const string MapMarkerImageExprHost = "MapMarkerImageExprHost";

			internal const string MapMarkerImageHost = "MapMarkerImageHost";

			internal const string MapSizeRuleExprHost = "MapSizeRuleExprHost";

			internal const string MapSizeRuleHost = "MapSizeRuleHost";

			internal const string StartSizeExpr = "StartSizeExpr";

			internal const string EndSizeExpr = "EndSizeExpr";

			internal const string MapPolygonRulesExprHost = "MapPolygonRulesExprHost";

			internal const string MapPolygonRulesHost = "MapPolygonRulesHost";

			internal const string MapLineRulesExprHost = "MapLineRulesExprHost";

			internal const string MapLineRulesHost = "MapLineRulesHost";

			internal const string MapColorRuleExprHost = "MapColorRuleExprHost";

			internal const string MapColorRuleHost = "MapColorRuleHost";

			internal const string ShowInColorScaleExpr = "ShowInColorScaleExpr";

			internal const string MapColorRangeRuleExprHost = "MapColorRangeRuleExprHost";

			internal const string StartColorExpr = "StartColorExpr";

			internal const string MiddleColorExpr = "MiddleColorExpr";

			internal const string EndColorExpr = "EndColorExpr";

			internal const string MapColorPaletteRuleExprHost = "MapColorPaletteRuleExprHost";

			internal const string MapBucketExprHost = "MapBucketExprHost";

			internal const string MapBucketHost = "MapBucketHost";

			internal const string MapAppearanceRuleExprHost = "MapAppearanceRuleExprHost";

			internal const string MapAppearanceRuleHost = "MapAppearanceRuleHost";

			internal const string DataValueExpr = "DataValueExpr";

			internal const string DistributionTypeExpr = "DistributionTypeExpr";

			internal const string BucketCountExpr = "BucketCountExpr";

			internal const string StartValueExpr = "StartValueExpr";

			internal const string EndValueExpr = "EndValueExpr";

			internal const string MapLegendTitleExprHost = "MapLegendTitleExprHost";

			internal const string MapLegendTitleHost = "MapLegendTitleHost";

			internal const string TitleSeparatorColorExpr = "TitleSeparatorColorExpr";

			internal const string MapLegendExprHost = "MapLegendExprHost";

			internal const string MapLegendHost = "MapLegendHost";

			internal const string MapLocationExprHost = "MapLocationExprHost";

			internal const string MapLocationHost = "MapLocationHost";

			internal const string MapSizeExprHost = "MapSizeExprHost";

			internal const string MapSizeHost = "MapSizeHost";

			internal const string UnitExpr = "UnitExpr";

			internal const string MapGridLinesExprHost = "MapGridLinesExprHost";

			internal const string MapMeridiansHost = "MapMeridiansHost";

			internal const string MapParallelsHost = "MapParallelsHost";

			internal const string ShowLabelsExpr = "ShowLabelsExpr";

			internal const string LabelPositionExpr = "LabelPositionExpr";

			internal const string MapHosts = "m_mapHostsRemotable";

			internal const string MapDataRegionHosts = "m_mapDataRegionHostsRemotable";

			internal const string MapDockableSubItemExprHost = "MapDockableSubItemExprHost";

			internal const string MapDockableSubItemHost = "MapDockableSubItemHost";

			internal const string DockOutsideViewportExpr = "DockOutsideViewportExpr";

			internal const string MapSubItemExprHost = "MapSubItemExprHost";

			internal const string MapSubItemHost = "MapSubItemHost";

			internal const string MapBindingFieldPairExprHost = "MapBindingFieldPairExprHost";

			internal const string MapBindingFieldPairHost = "MapBindingFieldPairHost";

			internal const string FieldNameExpr = "FieldNameExpr";

			internal const string BindingExpressionExpr = "BindingExpressionExpr";

			internal const string ZoomEnabledExpr = "ZoomEnabledExpr";

			internal const string MapViewportExprHost = "MapViewportExprHost";

			internal const string MapViewportHost = "MapViewportHost";

			internal const string MapCoordinateSystemExpr = "MapCoordinateSystemExpr";

			internal const string MapProjectionExpr = "MapProjectionExpr";

			internal const string ProjectionCenterXExpr = "ProjectionCenterXExpr";

			internal const string ProjectionCenterYExpr = "ProjectionCenterYExpr";

			internal const string MaximumZoomExpr = "MaximumZoomExpr";

			internal const string MinimumZoomExpr = "MinimumZoomExpr";

			internal const string ContentMarginExpr = "ContentMarginExpr";

			internal const string GridUnderContentExpr = "GridUnderContentExpr";

			internal const string MapBindingFieldPairsHosts = "m_mapBindingFieldPairsHostsRemotable";

			internal const string MapLimitsExprHost = "MapLimitsExprHost";

			internal const string MapLimitsHost = "MapLimitsHost";

			internal const string MinimumXExpr = "MinimumXExpr";

			internal const string MinimumYExpr = "MinimumYExpr";

			internal const string MaximumXExpr = "MaximumXExpr";

			internal const string MaximumYExpr = "MaximumYExpr";

			internal const string LimitToDataExpr = "LimitToDataExpr";

			internal const string MapColorScaleExprHost = "MapColorScaleExprHost";

			internal const string MapColorScaleHost = "MapColorScaleHost";

			internal const string TickMarkLengthExpr = "TickMarkLengthExpr";

			internal const string ColorBarBorderColorExpr = "ColorBarBorderColorExpr";

			internal const string LabelFormatExpr = "LabelFormatExpr";

			internal const string LabelPlacementExpr = "LabelPlacementExpr";

			internal const string LabelBehaviorExpr = "LabelBehaviorExpr";

			internal const string RangeGapColorExpr = "RangeGapColorExpr";

			internal const string NoDataTextExpr = "NoDataTextExpr";

			internal const string MapColorScaleTitleExprHost = "MapColorScaleTitleExprHost";

			internal const string MapColorScaleTitleHost = "MapColorScaleTitleHost";

			internal const string MapDistanceScaleExprHost = "MapDistanceScaleExprHost";

			internal const string MapDistanceScaleHost = "MapDistanceScaleHost";

			internal const string ScaleColorExpr = "ScaleColorExpr";

			internal const string ScaleBorderColorExpr = "ScaleBorderColorExpr";

			internal const string MapTitleExprHost = "MapTitleExprHost";

			internal const string MapTitleHost = "MapTitleHost";

			internal const string MapLegendsHosts = "m_mapLegendsHostsRemotable";

			internal const string MapTitlesHosts = "m_mapTitlesHostsRemotable";

			internal const string MapMarkersHosts = "m_mapMarkersHostsRemotable";

			internal const string MapBucketsHosts = "m_mapBucketsHostsRemotable";

			internal const string MapCustomColorsHosts = "m_mapCustomColorsHostsRemotable";

			internal const string MapPointsHosts = "m_mapPointsHostsRemotable";

			internal const string MapPolygonsHosts = "m_mapPolygonsHostsRemotable";

			internal const string MapLinesHosts = "m_mapLinesHostsRemotable";

			internal const string MapTileLayersHosts = "m_mapTileLayersHostsRemotable";

			internal const string MapTilesHosts = "m_mapTilesHostsRemotable";

			internal const string MapPointLayersHosts = "m_mapPointLayersHostsRemotable";

			internal const string MapPolygonLayersHosts = "m_mapPolygonLayersHostsRemotable";

			internal const string MapLineLayersHosts = "m_mapLineLayersHostsRemotable";

			internal const string MapFieldNamesHosts = "m_mapFieldNamesHostsRemotable";

			internal const string MapExprHost = "MapExprHost";

			internal const string DataElementLabelExpr = "DataElementLabelExpr";

			internal const string ParagraphExprHost = "ParagraphExprHost";

			internal const string ParagraphHosts = "m_paragraphHostsRemotable";

			internal const string LeftIndentExpr = "LeftIndentExpr";

			internal const string RightIndentExpr = "RightIndentExpr";

			internal const string HangingIndentExpr = "HangingIndentExpr";

			internal const string SpaceBeforeExpr = "SpaceBeforeExpr";

			internal const string SpaceAfterExpr = "SpaceAfterExpr";

			internal const string ListStyleExpr = "ListStyleExpr";

			internal const string ListLevelExpr = "ListLevelExpr";

			internal const string TextRunExprHost = "TextRunExprHost";

			internal const string TextRunHosts = "m_textRunHostsRemotable";

			internal const string MarkupTypeExpr = "MarkupTypeExpr";

			internal const string LookupSourceExpr = "SourceExpr";

			internal const string LookupDestExpr = "DestExpr";

			internal const string LookupResultExpr = "ResultExpr";

			internal const string PageBreakExprHost = "PageBreakExprHost";

			internal const string PageBreakDisabledExpr = "DisabledExpr";

			internal const string PageBreakPageNameExpr = "PageNameExpr";

			internal const string PageBreakResetPageNumberExpr = "ResetPageNumberExpr";

			internal const string JoinConditionForeignKeyExpr = "ForeignKeyExpr";

			internal const string JoinConditionPrimaryKeyExpr = "PrimaryKeyExpr";
		}

		private abstract class TypeDecl
		{
			internal CodeTypeDeclaration Type;

			internal string BaseTypeName;

			internal TypeDecl Parent;

			internal CodeConstructor Constructor;

			internal bool HasExpressions;

			internal CodeExpressionCollection DataValues;

			protected readonly bool m_setCode;

			internal void NestedTypeAdd(string name, CodeTypeDeclaration nestedType)
			{
				ConstructorCreate();
				Type.Members.Add(nestedType);
				Constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), CreateTypeCreateExpression(nestedType.Name)));
			}

			internal int NestedTypeColAdd(string name, string baseTypeName, ref CodeExpressionCollection initializers, CodeTypeDeclaration nestedType)
			{
				Type.Members.Add(nestedType);
				TypeColInit(name, baseTypeName, ref initializers);
				return initializers.Add(CreateTypeCreateExpression(nestedType.Name));
			}

			protected TypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
			{
				BaseTypeName = baseTypeName;
				Parent = parent;
				m_setCode = setCode;
				Type = CreateType(typeName, baseTypeName);
			}

			protected void ConstructorCreate()
			{
				if (Constructor == null)
				{
					Constructor = CreateConstructor();
					Type.Members.Add(Constructor);
				}
			}

			protected virtual CodeConstructor CreateConstructor()
			{
				return new CodeConstructor
				{
					Attributes = MemberAttributes.Public
				};
			}

			protected CodeAssignStatement CreateTypeColInitStatement(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				CodeObjectCreateExpression codeObjectCreateExpression = new CodeObjectCreateExpression();
				codeObjectCreateExpression.CreateType = new CodeTypeReference((name == "m_memberTreeHostsRemotable") ? "RemoteMemberArrayWrapper" : "RemoteArrayWrapper", new CodeTypeReference(baseTypeName));
				if (initializers != null)
				{
					codeObjectCreateExpression.Parameters.AddRange(initializers);
				}
				initializers = codeObjectCreateExpression.Parameters;
				return new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), codeObjectCreateExpression);
			}

			protected virtual CodeTypeDeclaration CreateType(string name, string baseType)
			{
				Global.Tracer.Assert(name != null, "(name != null)");
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
				if (baseType != null)
				{
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(baseType));
				}
				codeTypeDeclaration.Attributes = (MemberAttributes)24578;
				return codeTypeDeclaration;
			}

			private void TypeColInit(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				ConstructorCreate();
				if (initializers == null)
				{
					Constructor.Statements.Add(CreateTypeColInitStatement(name, baseTypeName, ref initializers));
				}
			}

			private CodeObjectCreateExpression CreateTypeCreateExpression(string typeName)
			{
				if (m_setCode)
				{
					return new CodeObjectCreateExpression(typeName, new CodeArgumentReferenceExpression("Code"));
				}
				return new CodeObjectCreateExpression(typeName);
			}
		}

		private sealed class RootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Aggregates;

			internal CodeExpressionCollection PageSections;

			internal CodeExpressionCollection ReportParameters;

			internal CodeExpressionCollection DataSources;

			internal CodeExpressionCollection DataSets;

			internal CodeExpressionCollection Lines;

			internal CodeExpressionCollection Rectangles;

			internal CodeExpressionCollection TextBoxes;

			internal CodeExpressionCollection Images;

			internal CodeExpressionCollection Subreports;

			internal CodeExpressionCollection Tablices;

			internal CodeExpressionCollection DataShapes;

			internal CodeExpressionCollection Charts;

			internal CodeExpressionCollection GaugePanels;

			internal CodeExpressionCollection CustomReportItems;

			internal CodeExpressionCollection Lookups;

			internal CodeExpressionCollection LookupDests;

			internal CodeExpressionCollection Pages;

			internal CodeExpressionCollection ReportSections;

			internal CodeExpressionCollection Maps;

			internal RootTypeDecl(bool setCode)
				: base("ReportExprHostImpl", "ReportExprHost", null, setCode)
			{
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "includeParameters"));
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "parametersOnly"));
				CodeParameterDeclarationExpression value = new CodeParameterDeclarationExpression(typeof(object), "reportObjectModel");
				codeConstructor.Parameters.Add(value);
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				ReportParameters = new CodeExpressionCollection();
				DataSources = new CodeExpressionCollection();
				DataSets = new CodeExpressionCollection();
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(name, baseType);
				if (m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}

			internal void CompleteConstructorCreation()
			{
				if (!HasExpressions)
				{
					return;
				}
				if (Constructor == null)
				{
					ConstructorCreate();
				}
				else
				{
					CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
					codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("parametersOnly"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
					codeConditionStatement.TrueStatements.Add(new CodeMethodReturnStatement());
					Constructor.Statements.Insert(0, codeConditionStatement);
					if (ReportParameters.Count > 0)
					{
						CodeConditionStatement codeConditionStatement2 = new CodeConditionStatement();
						codeConditionStatement2.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("includeParameters"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
						codeConditionStatement2.TrueStatements.Add(CreateTypeColInitStatement("m_reportParameterHostsRemotable", "ReportParamExprHost", ref ReportParameters));
						Constructor.Statements.Insert(0, codeConditionStatement2);
					}
					if (DataSources.Count > 0)
					{
						Constructor.Statements.Insert(0, CreateTypeColInitStatement("m_dataSourceHostsRemotable", "DataSourceExprHost", ref DataSources));
					}
					if (DataSets.Count > 0)
					{
						Constructor.Statements.Insert(0, CreateTypeColInitStatement("m_dataSetHostsRemotable", "DataSetExprHost", ref DataSets));
					}
				}
				Global.Tracer.Assert(Constructor != null, "Invalid EH constructor");
				CreateCustomCodeInitialization();
			}

			private void CreateCustomCodeInitialization()
			{
				if (m_setCode)
				{
					Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "m_codeProxyBase"), new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code")));
					Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeObjectCreateExpression("CustomCodeProxy", new CodeThisReferenceExpression())));
				}
			}
		}

		private sealed class NonRootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Parameters;

			internal CodeExpressionCollection Filters;

			internal CodeExpressionCollection Actions;

			internal CodeExpressionCollection Fields;

			internal CodeExpressionCollection ValueAxes;

			internal CodeExpressionCollection CategoryAxes;

			internal CodeExpressionCollection ChartTitles;

			internal CodeExpressionCollection ChartLegends;

			internal CodeExpressionCollection ChartAreas;

			internal CodeExpressionCollection TablixMembers;

			internal CodeExpressionCollection DataShapeMembers;

			internal CodeExpressionCollection ChartMembers;

			internal CodeExpressionCollection GaugeMembers;

			internal CodeExpressionCollection DataGroups;

			internal CodeExpressionCollection TablixCells;

			internal CodeExpressionCollection DataShapeIntersections;

			internal CodeExpressionCollection DataPoints;

			internal CodeExpressionCollection DataCells;

			internal CodeExpressionCollection ChartLegendCustomItemCells;

			internal CodeExpressionCollection ChartCustomPaletteColors;

			internal CodeExpressionCollection ChartStripLines;

			internal CodeExpressionCollection ChartDerivedSeriesCollection;

			internal CodeExpressionCollection ChartFormulaParameters;

			internal CodeExpressionCollection ChartLegendColumns;

			internal CodeExpressionCollection ChartLegendCustomItems;

			internal CodeExpressionCollection Paragraphs;

			internal CodeExpressionCollection TextRuns;

			internal CodeExpressionCollection GaugeCells;

			internal CodeExpressionCollection CustomLabels;

			internal CodeExpressionCollection GaugeImages;

			internal CodeExpressionCollection GaugeLabels;

			internal CodeExpressionCollection LinearGauges;

			internal CodeExpressionCollection RadialGauges;

			internal CodeExpressionCollection RadialPointers;

			internal CodeExpressionCollection LinearPointers;

			internal CodeExpressionCollection LinearScales;

			internal CodeExpressionCollection RadialScales;

			internal CodeExpressionCollection ScaleRanges;

			internal CodeExpressionCollection NumericIndicators;

			internal CodeExpressionCollection StateIndicators;

			internal CodeExpressionCollection GaugeInputValues;

			internal CodeExpressionCollection NumericIndicatorRanges;

			internal CodeExpressionCollection IndicatorStates;

			internal CodeExpressionCollection MapMembers;

			internal CodeExpressionCollection MapBindingFieldPairs;

			internal CodeExpressionCollection MapLegends;

			internal CodeExpressionCollection MapTitles;

			internal CodeExpressionCollection MapMarkers;

			internal CodeExpressionCollection MapBuckets;

			internal CodeExpressionCollection MapCustomColors;

			internal CodeExpressionCollection MapPoints;

			internal CodeExpressionCollection MapPolygons;

			internal CodeExpressionCollection MapLines;

			internal CodeExpressionCollection MapTileLayers;

			internal CodeExpressionCollection MapTiles;

			internal CodeExpressionCollection MapPointLayers;

			internal CodeExpressionCollection MapPolygonLayers;

			internal CodeExpressionCollection MapLineLayers;

			internal CodeExpressionCollection MapFieldNames;

			internal CodeExpressionCollection JoinConditions;

			internal ReturnStatementList IndexedExpressions;

			internal NonRootTypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
				: base(typeName, baseTypeName, parent, setCode)
			{
				if (setCode)
				{
					ConstructorCreate();
				}
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				if (m_setCode)
				{
					codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression("CustomCodeProxy", "code"));
					codeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeArgumentReferenceExpression("code")));
				}
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", name, baseType), baseType);
				if (m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}
		}

		private sealed class CustomCodeProxyDecl : TypeDecl
		{
			internal CustomCodeProxyDecl(TypeDecl parent)
				: base("CustomCodeProxy", "CustomCodeProxyBase", parent, setCode: false)
			{
				ConstructorCreate();
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IReportObjectModelProxyForCustomCode), "reportObjectModel"));
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				return codeConstructor;
			}

			internal void AddClassInstance(string className, string instanceName, int id)
			{
				string fileName = "CMCID" + id.ToString(CultureInfo.InvariantCulture) + "end";
				CodeMemberField codeMemberField = new CodeMemberField(className, "m_" + instanceName);
				codeMemberField.Attributes = (MemberAttributes)20482;
				codeMemberField.InitExpression = new CodeObjectCreateExpression(className);
				codeMemberField.LinePragma = new CodeLinePragma(fileName, 0);
				Type.Members.Add(codeMemberField);
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Type = new CodeTypeReference(className);
				codeMemberProperty.Name = instanceName;
				codeMemberProperty.Attributes = (MemberAttributes)24578;
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), codeMemberField.Name)));
				codeMemberProperty.LinePragma = new CodeLinePragma(fileName, 2);
				Type.Members.Add(codeMemberProperty);
			}

			internal void AddCode(string code)
			{
				CodeTypeMember codeTypeMember = new CodeSnippetTypeMember(code);
				codeTypeMember.LinePragma = new CodeLinePragma("CustomCode", 0);
				Type.Members.Add(codeTypeMember);
			}
		}

		private sealed class ReturnStatementList
		{
			private ArrayList m_list = new ArrayList();

			internal CodeMethodReturnStatement this[int index] => (CodeMethodReturnStatement)m_list[index];

			internal int Count => m_list.Count;

			internal int Add(CodeMethodReturnStatement retStatement)
			{
				return m_list.Add(retStatement);
			}
		}

		internal const string RootType = "ReportExprHostImpl";

		internal const int InvalidExprHostId = -1;

		private RootTypeDecl m_rootTypeDecl;

		private TypeDecl m_currentTypeDecl;

		private bool m_setCode;

		private const string EndSrcMarker = "end";

		private const string ExprSrcMarker = "Expr";

		private static readonly Regex m_findExprNumber = new Regex("^Expr([0-9]+)end", RegexOptions.Compiled);

		private const string CustomCodeSrcMarker = "CustomCode";

		private const string CodeModuleClassInstanceDeclSrcMarker = "CMCID";

		private static readonly Regex m_findCodeModuleClassInstanceDeclNumber = new Regex("^CMCID([0-9]+)end", RegexOptions.Compiled);

		internal bool HasExpressions
		{
			get
			{
				if (m_rootTypeDecl != null)
				{
					return m_rootTypeDecl.HasExpressions;
				}
				return false;
			}
		}

		internal bool CustomCode => m_setCode;

		internal ExprHostBuilder()
		{
		}

		internal void SetCustomCode()
		{
			m_setCode = true;
		}

		internal CodeCompileUnit GetExprHost(ProcessingIntermediateFormatVersion version, bool refusePermissions)
		{
			Global.Tracer.Assert(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null, "(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null)");
			CodeCompileUnit codeCompileUnit = null;
			if (HasExpressions)
			{
				codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Reflection.AssemblyVersion", new CodeAttributeArgument(new CodePrimitiveExpression(version.ToString()))));
				CodeNamespace codeNamespace = new CodeNamespace();
				codeCompileUnit.Namespaces.Add(codeNamespace);
				codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Convert"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Math"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.SqlServer.Types"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("ReportingServices.ReportProcessing.ReportObjectModel"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("ReportingServices.RdlExpressions.ExpressionHostObjectModel"));
				codeNamespace.Types.Add(m_rootTypeDecl.Type);
			}
			m_rootTypeDecl = null;
			return codeCompileUnit;
		}

		internal ErrorSource ParseErrorSource(CompilerError error, out int id)
		{
			Global.Tracer.Assert(error.FileName != null, "(error.FileName != null)");
			id = -1;
			if (error.FileName.StartsWith("CustomCode", StringComparison.Ordinal))
			{
				return ErrorSource.CustomCode;
			}
			try
			{
				Match match = m_findCodeModuleClassInstanceDeclNumber.Match(error.FileName);
				if (match.Success)
				{
					id = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					return ErrorSource.CodeModuleClassInstanceDecl;
				}
				match = m_findExprNumber.Match(error.FileName);
				if (match.Success)
				{
					id = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					return ErrorSource.Expression;
				}
			}
			catch (FormatException)
			{
			}
			return ErrorSource.Unknown;
		}

		internal void SharedDataSetStart()
		{
			m_currentTypeDecl = (m_rootTypeDecl = new RootTypeDecl(m_setCode));
		}

		internal void SharedDataSetEnd()
		{
			m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportStart()
		{
			m_currentTypeDecl = (m_rootTypeDecl = new RootTypeDecl(m_setCode));
		}

		internal void ReportEnd()
		{
			m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportLanguage(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ReportLanguageExpr", expression);
		}

		internal void ReportAutoRefresh(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AutoRefreshExpr", expression);
		}

		internal void ReportInitialPageName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InitialPageNameExpr", expression);
		}

		internal void GenericLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelExpr", expression);
		}

		internal void GenericValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void GenericNoRows(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("NoRowsExpr", expression);
		}

		internal void GenericVisibilityHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VisibilityHiddenExpr", expression);
		}

		internal void AggregateParamExprAdd(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			AggregateStart();
			GenericValue(expression);
			expression.ExprHostID = AggregateEnd();
		}

		internal void CustomCodeProxyStart()
		{
			Global.Tracer.Assert(m_setCode, "(m_setCode)");
			m_currentTypeDecl = new CustomCodeProxyDecl(m_currentTypeDecl);
			m_currentTypeDecl.HasExpressions = true;
		}

		internal void CustomCodeProxyEnd()
		{
			m_rootTypeDecl.Type.Members.Add(m_currentTypeDecl.Type);
			TypeEnd(m_rootTypeDecl);
		}

		internal void CustomCodeClassInstance(string className, string instanceName, int id)
		{
			((CustomCodeProxyDecl)m_currentTypeDecl).AddClassInstance(className, instanceName, id);
		}

		internal void ReportCode(string code)
		{
			((CustomCodeProxyDecl)m_currentTypeDecl).AddCode(code);
		}

		internal void ReportParameterStart(string name)
		{
			TypeStart(name, "ReportParamExprHost");
		}

		internal int ReportParameterEnd()
		{
			ExprIndexerCreate();
			return TypeEnd(m_rootTypeDecl, "m_reportParameterHostsRemotable", ref m_rootTypeDecl.ReportParameters);
		}

		internal void ReportParameterValidationExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValidationExpressionExpr", expression);
		}

		internal void ReportParameterPromptExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PromptExpr", expression);
		}

		internal void ReportParameterDefaultValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValuesStart()
		{
			TypeStart("ReportParameterValidValues", "IndexedExprHost");
		}

		internal void ReportParameterValidValuesEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ValidValuesHost");
		}

		internal void ReportParameterValidValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValueLabelsStart()
		{
			TypeStart("ReportParameterValidValueLabels", "IndexedExprHost");
		}

		internal void ReportParameterValidValueLabelsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ValidValueLabelsHost");
		}

		internal void ReportParameterValidValueLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void CalcFieldStart(string name)
		{
			TypeStart(name, "CalcFieldExprHost");
		}

		internal int CalcFieldEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_fieldHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Fields);
		}

		internal void QueryParametersStart()
		{
			TypeStart("QueryParameters", "IndexedExprHost");
		}

		internal void QueryParametersEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "QueryParametersHost");
		}

		internal void QueryParameterValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void DataSourceStart(string name)
		{
			TypeStart(name, "DataSourceExprHost");
		}

		internal int DataSourceEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_dataSourceHostsRemotable", ref m_rootTypeDecl.DataSources);
		}

		internal void DataSourceConnectString(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ConnectStringExpr", expression);
		}

		internal void DataSetStart(string name)
		{
			TypeStart(name, "DataSetExprHost");
		}

		internal int DataSetEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_dataSetHostsRemotable", ref m_rootTypeDecl.DataSets);
		}

		internal void DataSetQueryCommandText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("QueryCommandTextExpr", expression);
		}

		internal void PageSectionStart()
		{
			TypeStart(CreateTypeName("PageSection", m_rootTypeDecl.PageSections), "StyleExprHost");
		}

		internal int PageSectionEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_pageSectionHostsRemotable", ref m_rootTypeDecl.PageSections);
		}

		internal void PageStart()
		{
			TypeStart(CreateTypeName("Page", m_rootTypeDecl.Pages), "StyleExprHost");
		}

		internal int PageEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_pageHostsRemotable", ref m_rootTypeDecl.Pages);
		}

		internal void ReportSectionStart()
		{
			TypeStart(CreateTypeName("ReportSection", m_rootTypeDecl.ReportSections), "ReportSectionExprHost");
		}

		internal int ReportSectionEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_reportSectionHostsRemotable", ref m_rootTypeDecl.ReportSections);
		}

		internal void ParameterOmit(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OmitExpr", expression);
		}

		internal void StyleAttribute(string name, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd(name + "Expr", expression);
		}

		internal void ActionInfoStart()
		{
			TypeStart("ActionInfo", "ActionInfoExprHost");
		}

		internal void ActionInfoEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ActionInfoHost");
		}

		internal void ActionStart()
		{
			TypeStart(CreateTypeName("Action", ((NonRootTypeDecl)m_currentTypeDecl).Actions), "ActionExprHost");
		}

		internal int ActionEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_actionItemHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Actions);
		}

		internal void ActionHyperlink(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HyperlinkExpr", expression);
		}

		internal void ActionDrillThroughReportName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DrillThroughReportNameExpr", expression);
		}

		internal void ActionDrillThroughBookmarkLink(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DrillThroughBookmarkLinkExpr", expression);
		}

		internal void ActionBookmarkLink(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BookmarkLinkExpr", expression);
		}

		internal void ActionDrillThroughParameterStart()
		{
			ParameterStart();
		}

		internal int ActionDrillThroughParameterEnd()
		{
			return ParameterEnd("m_drillThroughParameterHostsRemotable");
		}

		internal void ReportItemBookmark(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BookmarkExpr", expression);
		}

		internal void ReportItemToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void LineStart(string name)
		{
			TypeStart(name, "ReportItemExprHost");
		}

		internal int LineEnd()
		{
			return ReportItemEnd("m_lineHostsRemotable", ref m_rootTypeDecl.Lines);
		}

		internal void RectangleStart(string name)
		{
			TypeStart(name, "ReportItemExprHost");
		}

		internal int RectangleEnd()
		{
			return ReportItemEnd("m_rectangleHostsRemotable", ref m_rootTypeDecl.Rectangles);
		}

		internal void TextBoxStart(string name)
		{
			TypeStart(name, "TextBoxExprHost");
		}

		internal int TextBoxEnd()
		{
			return ReportItemEnd("m_textBoxHostsRemotable", ref m_rootTypeDecl.TextBoxes);
		}

		internal void TextBoxToggleImageInitialState(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToggleImageInitialStateExpr", expression);
		}

		internal void UserSortExpressionsStart()
		{
			TypeStart("UserSort", "IndexedExprHost");
		}

		internal void UserSortExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "UserSortExpressionsHost");
		}

		internal void UserSortExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ImageStart(string name)
		{
			TypeStart(name, "ImageExprHost");
		}

		internal int ImageEnd()
		{
			return ReportItemEnd("m_imageHostsRemotable", ref m_rootTypeDecl.Images);
		}

		internal void ImageMIMEType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void ImageTag(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TagExpr", expression);
		}

		internal void SubreportStart(string name)
		{
			TypeStart(name, "SubreportExprHost");
		}

		internal int SubreportEnd()
		{
			return ReportItemEnd("m_subreportHostsRemotable", ref m_rootTypeDecl.Subreports);
		}

		internal void SubreportParameterStart()
		{
			ParameterStart();
		}

		internal int SubreportParameterEnd()
		{
			return ParameterEnd("m_parameterHostsRemotable");
		}

		internal void SortStart()
		{
			TypeStart("Sort", "SortExprHost");
		}

		internal void SortEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "m_sortHost");
		}

		internal void SortExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void SortDirectionsStart()
		{
			TypeStart("SortDirections", "IndexedExprHost");
		}

		internal void SortDirectionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "SortDirectionHosts");
		}

		internal void SortDirection(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void FilterStart()
		{
			TypeStart(CreateTypeName("Filter", ((NonRootTypeDecl)m_currentTypeDecl).Filters), "FilterExprHost");
		}

		internal int FilterEnd()
		{
			ExprIndexerCreate();
			return TypeEnd(m_currentTypeDecl.Parent, "m_filterHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Filters);
		}

		internal void FilterExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FilterExpressionExpr", expression);
		}

		internal void FilterValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void GroupStart(string typeName)
		{
			TypeStart(typeName, "GroupExprHost");
		}

		internal void GroupEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "m_groupHost");
		}

		internal void GroupExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void GroupParentExpressionsStart()
		{
			TypeStart("Parent", "IndexedExprHost");
		}

		internal void GroupParentExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ParentExpressionsHost");
		}

		internal void GroupParentExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ReGroupExpressionsStart()
		{
			TypeStart("ReGroup", "IndexedExprHost");
		}

		internal void ReGroupExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ReGroupExpressionsHost");
		}

		internal void ReGroupExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void VariableValuesStart()
		{
			TypeStart("Variables", "IndexedExprHost");
		}

		internal void VariableValuesEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "VariableValueHosts");
		}

		internal void VariableValueExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void DataRegionStart(DataRegionMode mode, string dataregionName)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				TypeStart(dataregionName, "TablixExprHost");
				break;
			case DataRegionMode.DataShape:
				TypeStart(dataregionName, "DataShapeExprHost");
				break;
			case DataRegionMode.Chart:
				TypeStart(dataregionName, "ChartExprHost");
				break;
			case DataRegionMode.GaugePanel:
				TypeStart(dataregionName, "GaugePanelExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				TypeStart(dataregionName, "CustomReportItemExprHost");
				break;
			case DataRegionMode.MapDataRegion:
				TypeStart(dataregionName, "MapDataRegionExprHost");
				break;
			}
		}

		internal int DataRegionEnd(DataRegionMode mode)
		{
			int result = -1;
			switch (mode)
			{
			case DataRegionMode.Tablix:
				result = ReportItemEnd("m_tablixHostsRemotable", ref m_rootTypeDecl.Tablices);
				break;
			case DataRegionMode.DataShape:
				result = ReportItemEnd("DataShapeExprHost", ref m_rootTypeDecl.DataShapes);
				break;
			case DataRegionMode.Chart:
				result = ReportItemEnd("m_chartHostsRemotable", ref m_rootTypeDecl.Charts);
				break;
			case DataRegionMode.GaugePanel:
				result = ReportItemEnd("m_gaugePanelHostsRemotable", ref m_rootTypeDecl.GaugePanels);
				break;
			case DataRegionMode.CustomReportItem:
				result = ReportItemEnd("m_customReportItemHostsRemotable", ref m_rootTypeDecl.CustomReportItems);
				break;
			case DataRegionMode.MapDataRegion:
				result = ReportItemEnd("m_mapDataRegionHostsRemotable", ref m_rootTypeDecl.CustomReportItems);
				break;
			}
			return result;
		}

		internal void DataGroupStart(DataRegionMode mode, bool column)
		{
			string str = column ? "Column" : "Row";
			switch (mode)
			{
			case DataRegionMode.Tablix:
				TypeStart(CreateTypeName("TablixMember" + str, ((NonRootTypeDecl)m_currentTypeDecl).TablixMembers), "TablixMemberExprHost");
				break;
			case DataRegionMode.DataShape:
				TypeStart(CreateTypeName("DataShapeMember" + str, ((NonRootTypeDecl)m_currentTypeDecl).DataShapeMembers), "DataShapeMemberExprHost");
				break;
			case DataRegionMode.Chart:
				TypeStart(CreateTypeName("ChartMember" + str, ((NonRootTypeDecl)m_currentTypeDecl).ChartMembers), "ChartMemberExprHost");
				break;
			case DataRegionMode.GaugePanel:
				TypeStart(CreateTypeName("GaugeMember" + str, ((NonRootTypeDecl)m_currentTypeDecl).GaugeMembers), "GaugeMemberExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				TypeStart(CreateTypeName("DataGroup" + str, ((NonRootTypeDecl)m_currentTypeDecl).DataGroups), "DataGroupExprHost");
				break;
			case DataRegionMode.MapDataRegion:
				TypeStart(CreateTypeName("MapMember" + str, ((NonRootTypeDecl)m_currentTypeDecl).MapMembers), "MapMemberExprHost");
				break;
			}
		}

		internal int DataGroupEnd(DataRegionMode mode, bool column)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).TablixMembers);
			case DataRegionMode.DataShape:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataShapeMembers);
			case DataRegionMode.Chart:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartMembers);
			case DataRegionMode.GaugePanel:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).GaugeMembers);
			case DataRegionMode.CustomReportItem:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataGroups);
			case DataRegionMode.MapDataRegion:
				return TypeEnd(m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapMembers);
			default:
				return -1;
			}
		}

		internal void DataCellStart(DataRegionMode mode)
		{
			switch (mode)
			{
			case DataRegionMode.MapDataRegion:
				break;
			case DataRegionMode.Tablix:
				TypeStart(CreateTypeName("TablixCell", ((NonRootTypeDecl)m_currentTypeDecl).TablixCells), "TablixCellExprHost");
				break;
			case DataRegionMode.DataShape:
				TypeStart(CreateTypeName("DataShapeIntersection", ((NonRootTypeDecl)m_currentTypeDecl).DataShapeIntersections), "DataShapeIntersectionExprHost");
				break;
			case DataRegionMode.Chart:
				TypeStart(CreateTypeName("ChartDataPoint", ((NonRootTypeDecl)m_currentTypeDecl).DataPoints), "ChartDataPointExprHost");
				break;
			case DataRegionMode.GaugePanel:
				TypeStart(CreateTypeName("GaugeCell", ((NonRootTypeDecl)m_currentTypeDecl).GaugeCells), "GaugeCellExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				TypeStart(CreateTypeName("DataCell", ((NonRootTypeDecl)m_currentTypeDecl).DataCells), "DataCellExprHost");
				break;
			}
		}

		internal int DataCellEnd(DataRegionMode mode)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				return TypeEnd(m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).TablixCells);
			case DataRegionMode.DataShape:
				return TypeEnd(m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataShapeIntersections);
			case DataRegionMode.Chart:
				return TypeEnd(m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataPoints);
			case DataRegionMode.GaugePanel:
				return TypeEnd(m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).GaugeCells);
			case DataRegionMode.CustomReportItem:
				return TypeEnd(m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataCells);
			default:
				return -1;
			}
		}

		internal void MarginExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string marginPosition)
		{
			ExpressionAdd(marginPosition + "Expr", expression);
		}

		internal void ChartTitleStart(string titleName)
		{
			TypeStart(CreateTypeName("ChartTitle" + titleName, ((NonRootTypeDecl)m_currentTypeDecl).ChartTitles), "ChartTitleExprHost");
		}

		internal void ChartTitlePosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartTitlePositionExpr", expression);
		}

		internal void ChartTitleHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartTitleDocking(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DockingExpr", expression);
		}

		internal void ChartTitleDockOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DockingOffsetExpr", expression);
		}

		internal void ChartTitleDockOutsideChartArea(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DockOutsideChartAreaExpr", expression);
		}

		internal void ChartTitleToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartTitleTextOrientation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextOrientationExpr", expression);
		}

		internal int ChartTitleEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_titlesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartTitles);
		}

		internal void ChartNoDataMessageStart()
		{
			TypeStart("ChartTitle", "ChartTitleExprHost");
		}

		internal void ChartNoDataMessageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "NoDataMessageHost");
		}

		internal void ChartCaption(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CaptionExpr", expression);
		}

		internal void ChartAxisTitleStart()
		{
			TypeStart("ChartAxisTitle", "ChartAxisTitleExprHost");
		}

		internal void ChartAxisTitleTextOrientation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextOrientationExpr", expression);
		}

		internal void ChartAxisTitleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "TitleHost");
		}

		internal void ChartLegendTitleStart()
		{
			TypeStart("ChartLegendTitle", "ChartLegendTitleExprHost");
		}

		internal void ChartLegendTitleSeparator(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TitleSeparatorExpr", expression);
		}

		internal void ChartLegendTitleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "TitleExprHost");
		}

		internal void AxisMin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisMinExpr", expression);
		}

		internal void AxisMax(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisMaxExpr", expression);
		}

		internal void AxisCrossAt(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisCrossAtExpr", expression);
		}

		internal void AxisMajorInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisMajorIntervalExpr", expression);
		}

		internal void AxisMinorInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisMinorIntervalExpr", expression);
		}

		internal void ChartPalette(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PaletteExpr", expression);
		}

		internal void ChartPaletteHatchBehavior(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PaletteHatchBehaviorExpr", expression);
		}

		internal void DynamicWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DynamicWidthExpr", expression);
		}

		internal void DynamicHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DynamicHeightExpr", expression);
		}

		internal void ChartAxisStart(string axisName, bool isValueAxis)
		{
			if (isValueAxis)
			{
				TypeStart(CreateTypeName("ValueAxis" + axisName, ((NonRootTypeDecl)m_currentTypeDecl).ValueAxes), "ChartAxisExprHost");
			}
			else
			{
				TypeStart(CreateTypeName("CategoryAxis" + axisName, ((NonRootTypeDecl)m_currentTypeDecl).CategoryAxes), "ChartAxisExprHost");
			}
		}

		internal void ChartAxisVisible(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VisibleExpr", expression);
		}

		internal void ChartAxisMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarginExpr", expression);
		}

		internal void ChartAxisInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartAxisIntervalType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartAxisIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartAxisIntervalOffsetType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartAxisMarksAlwaysAtPlotEdge(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarksAlwaysAtPlotEdgeExpr", expression);
		}

		internal void ChartAxisReverse(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ReverseExpr", expression);
		}

		internal void ChartAxisLocation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LocationExpr", expression);
		}

		internal void ChartAxisInterlaced(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedExpr", expression);
		}

		internal void ChartAxisInterlacedColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedColorExpr", expression);
		}

		internal void ChartAxisLogScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LogScaleExpr", expression);
		}

		internal void ChartAxisLogBase(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LogBaseExpr", expression);
		}

		internal void ChartAxisHideLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HideLabelsExpr", expression);
		}

		internal void ChartAxisAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AngleExpr", expression);
		}

		internal void ChartAxisArrows(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ArrowsExpr", expression);
		}

		internal void ChartAxisPreventFontShrink(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PreventFontShrinkExpr", expression);
		}

		internal void ChartAxisPreventFontGrow(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PreventFontGrowExpr", expression);
		}

		internal void ChartAxisPreventLabelOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PreventLabelOffsetExpr", expression);
		}

		internal void ChartAxisPreventWordWrap(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PreventWordWrapExpr", expression);
		}

		internal void ChartAxisAllowLabelRotation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AllowLabelRotationExpr", expression);
		}

		internal void ChartAxisIncludeZero(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IncludeZeroExpr", expression);
		}

		internal void ChartAxisLabelsAutoFitDisabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelsAutoFitDisabledExpr", expression);
		}

		internal void ChartAxisMinFontSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void ChartAxisMaxFontSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaxFontSizeExpr", expression);
		}

		internal void ChartAxisOffsetLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetLabelsExpr", expression);
		}

		internal void ChartAxisHideEndLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HideEndLabelsExpr", expression);
		}

		internal void ChartAxisVariableAutoInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VariableAutoIntervalExpr", expression);
		}

		internal void ChartAxisLabelInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelIntervalExpr", expression);
		}

		internal void ChartAxisLabelIntervalType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelIntervalTypeExpr", expression);
		}

		internal void ChartAxisLabelIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelIntervalOffsetExpr", expression);
		}

		internal void ChartAxisLabelIntervalOffsetType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelIntervalOffsetTypeExpr", expression);
		}

		internal int ChartAxisEnd(bool isValueAxis)
		{
			if (isValueAxis)
			{
				return TypeEnd(m_currentTypeDecl.Parent, "m_valueAxesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ValueAxes);
			}
			return TypeEnd(m_currentTypeDecl.Parent, "m_categoryAxesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).CategoryAxes);
		}

		internal void ChartGridLinesStart(bool isMajor)
		{
			TypeStart("ChartGridLines" + (isMajor ? "MajorGridLinesHost" : "MinorGridLinesHost"), "ChartGridLinesExprHost");
		}

		internal void ChartGridLinesEnd(bool isMajor)
		{
			TypeEnd(m_currentTypeDecl.Parent, isMajor ? "MajorGridLinesHost" : "MinorGridLinesHost");
		}

		internal void ChartGridLinesIntervalOffsetType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartGridLinesIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartGridLinesEnabledIntervalType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartGridLinesInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartGridLinesEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartLegendStart(string legendName)
		{
			TypeStart(CreateTypeName("ChartLegend" + legendName, ((NonRootTypeDecl)m_currentTypeDecl).ChartLegends), "ChartLegendExprHost");
		}

		internal void ChartLegendHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartLegendPosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartLegendPositionExpr", expression);
		}

		internal void ChartLegendLayout(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LayoutExpr", expression);
		}

		internal void ChartLegendDockOutsideChartArea(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DockOutsideChartAreaExpr", expression);
		}

		internal void ChartLegendAutoFitTextDisabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AutoFitTextDisabledExpr", expression);
		}

		internal void ChartLegendMinFontSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void ChartLegendHeaderSeparator(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HeaderSeparatorExpr", expression);
		}

		internal void ChartLegendHeaderSeparatorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HeaderSeparatorColorExpr", expression);
		}

		internal void ChartLegendColumnSeparator(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColumnSeparatorExpr", expression);
		}

		internal void ChartLegendColumnSeparatorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColumnSeparatorColorExpr", expression);
		}

		internal void ChartLegendColumnSpacing(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColumnSpacingExpr", expression);
		}

		internal void ChartLegendInterlacedRows(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedRowsExpr", expression);
		}

		internal void ChartLegendInterlacedRowsColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedRowsColorExpr", expression);
		}

		internal void ChartLegendEquallySpacedItems(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EquallySpacedItemsExpr", expression);
		}

		internal void ChartLegendReversed(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ReversedExpr", expression);
		}

		internal void ChartLegendMaxAutoSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaxAutoSizeExpr", expression);
		}

		internal void ChartLegendTextWrapThreshold(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextWrapThresholdExpr", expression);
		}

		internal int ChartLegendEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_legendsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartLegends);
		}

		internal void ChartSeriesStart()
		{
			TypeStart("ChartSeries", "ChartSeriesExprHost");
		}

		internal void ChartSeriesType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TypeExpr", expression);
		}

		internal void ChartSeriesSubtype(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SubtypeExpr", expression);
		}

		internal void ChartSeriesLegendName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LegendNameExpr", expression);
		}

		internal void ChartSeriesLegendText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LegendTextExpr", expression);
		}

		internal void ChartSeriesChartAreaName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartAreaNameExpr", expression);
		}

		internal void ChartSeriesValueAxisName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueAxisNameExpr", expression);
		}

		internal void ChartSeriesCategoryAxisName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CategoryAxisNameExpr", expression);
		}

		internal void ChartSeriesHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartSeriesHideInLegend(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HideInLegendExpr", expression);
		}

		internal void ChartSeriesToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartSeriesEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ChartSeriesHost");
		}

		internal void ChartNoMoveDirectionsStart()
		{
			TypeStart("ChartNoMoveDirections", "ChartNoMoveDirectionsExprHost");
		}

		internal void ChartNoMoveDirectionsUp(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UpExpr", expression);
		}

		internal void ChartNoMoveDirectionsDown(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DownExpr", expression);
		}

		internal void ChartNoMoveDirectionsLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsRight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RightExpr", expression);
		}

		internal void ChartNoMoveDirectionsUpLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UpLeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsUpRight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UpRightExpr", expression);
		}

		internal void ChartNoMoveDirectionsDownLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DownLeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsDownRight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DownRightExpr", expression);
		}

		internal void ChartNoMoveDirectionsEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "NoMoveDirectionsHost");
		}

		internal void ChartElementPositionStart(bool innerPlot)
		{
			TypeStart(innerPlot ? "ChartInnerPlotPosition" : "ChartElementPosition", "ChartElementPositionExprHost");
		}

		internal void ChartElementPositionEnd(bool innerPlot)
		{
			TypeEnd(m_currentTypeDecl.Parent, innerPlot ? "ChartInnerPlotPositionHost" : "ChartElementPositionHost");
		}

		internal void ChartElementPositionTop(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TopExpr", expression);
		}

		internal void ChartElementPositionLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftExpr", expression);
		}

		internal void ChartElementPositionHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HeightExpr", expression);
		}

		internal void ChartElementPositionWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void ChartSmartLabelStart()
		{
			TypeStart("ChartSmartLabel", "ChartSmartLabelExprHost");
		}

		internal void ChartSmartLabelAllowOutSidePlotArea(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AllowOutSidePlotAreaExpr", expression);
		}

		internal void ChartSmartLabelCalloutBackColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutBackColorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineAnchor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutLineAnchorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutLineColorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutLineStyleExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutLineWidthExpr", expression);
		}

		internal void ChartSmartLabelCalloutStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CalloutStyleExpr", expression);
		}

		internal void ChartSmartLabelShowOverlapped(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowOverlappedExpr", expression);
		}

		internal void ChartSmartLabelMarkerOverlapping(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarkerOverlappingExpr", expression);
		}

		internal void ChartSmartLabelDisabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DisabledExpr", expression);
		}

		internal void ChartSmartLabelMaxMovingDistance(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaxMovingDistanceExpr", expression);
		}

		internal void ChartSmartLabelMinMovingDistance(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinMovingDistanceExpr", expression);
		}

		internal void ChartSmartLabelEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "SmartLabelHost");
		}

		internal void ChartAxisScaleBreakStart()
		{
			TypeStart("ChartAxisScaleBreak", "ChartAxisScaleBreakExprHost");
		}

		internal void ChartAxisScaleBreakEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartAxisScaleBreakBreakLineType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BreakLineTypeExpr", expression);
		}

		internal void ChartAxisScaleBreakCollapsibleSpaceThreshold(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CollapsibleSpaceThresholdExpr", expression);
		}

		internal void ChartAxisScaleBreakMaxNumberOfBreaks(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaxNumberOfBreaksExpr", expression);
		}

		internal void ChartAxisScaleBreakSpacing(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SpacingExpr", expression);
		}

		internal void ChartAxisScaleBreakIncludeZero(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IncludeZeroExpr", expression);
		}

		internal void ChartAxisScaleBreakEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "AxisScaleBreakHost");
		}

		internal void ChartBorderSkinStart()
		{
			TypeStart("ChartBorderSkin", "ChartBorderSkinExprHost");
		}

		internal void ChartBorderSkinBorderSkinType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BorderSkinTypeExpr", expression);
		}

		internal void ChartBorderSkinEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "BorderSkinHost");
		}

		internal void ChartItemInLegendStart()
		{
			TypeStart("ChartItemInLegend", "ChartDataPointInLegendExprHost");
		}

		internal void ChartItemInLegendLegendText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LegendTextExpr", expression);
		}

		internal void ChartItemInLegendToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartItemInLegendHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartItemInLegendEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "DataPointInLegendHost");
		}

		internal void ChartTickMarksStart(bool isMajor)
		{
			TypeStart("ChartTickMarks" + (isMajor ? "MajorTickMarksHost" : "MinorTickMarksHost"), "ChartTickMarksExprHost");
		}

		internal void ChartTickMarksEnd(bool isMajor)
		{
			TypeEnd(m_currentTypeDecl.Parent, isMajor ? "MajorTickMarksHost" : "MinorTickMarksHost");
		}

		internal void ChartTickMarksEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartTickMarksType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TypeExpr", expression);
		}

		internal void ChartTickMarksLength(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LengthExpr", expression);
		}

		internal void ChartTickMarksInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartTickMarksIntervalType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartTickMarksIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartTickMarksIntervalOffsetType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartEmptyPointsStart()
		{
			TypeStart("ChartEmptyPoints", "ChartEmptyPointsExprHost");
		}

		internal void ChartEmptyPointsAxisLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisLabelExpr", expression);
		}

		internal void ChartEmptyPointsToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartEmptyPointsEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "EmptyPointsHost");
		}

		internal void ChartLegendColumnHeaderStart()
		{
			TypeStart("ChartLegendColumnHeader", "ChartLegendColumnHeaderExprHost");
		}

		internal void ChartLegendColumnHeaderValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void ChartLegendColumnHeaderEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ChartLegendColumnHeaderHost");
		}

		internal void ChartCustomPaletteColorStart(int index)
		{
			TypeStart(CreateTypeName("ChartCustomPaletteColor" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).ChartCustomPaletteColors), "ChartCustomPaletteColorExprHost");
		}

		internal int ChartCustomPaletteColorEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_customPaletteColorHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartCustomPaletteColors);
		}

		internal void ChartCustomPaletteColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColorExpr", expression);
		}

		internal void ChartLegendCustomItemCellStart(string name)
		{
			TypeStart(CreateTypeName("ChartLegendCustomItemCell" + name, ((NonRootTypeDecl)m_currentTypeDecl).ChartLegendCustomItemCells), "ChartLegendCustomItemCellExprHost");
		}

		internal void ChartLegendCustomItemCellCellType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CellTypeExpr", expression);
		}

		internal void ChartLegendCustomItemCellText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextExpr", expression);
		}

		internal void ChartLegendCustomItemCellCellSpan(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CellSpanExpr", expression);
		}

		internal void ChartLegendCustomItemCellToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartLegendCustomItemCellImageWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ImageWidthExpr", expression);
		}

		internal void ChartLegendCustomItemCellImageHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ImageHeightExpr", expression);
		}

		internal void ChartLegendCustomItemCellSymbolHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SymbolHeightExpr", expression);
		}

		internal void ChartLegendCustomItemCellSymbolWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SymbolWidthExpr", expression);
		}

		internal void ChartLegendCustomItemCellAlignment(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AlignmentExpr", expression);
		}

		internal void ChartLegendCustomItemCellTopMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TopMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellBottomMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BottomMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellLeftMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellRightMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RightMarginExpr", expression);
		}

		internal int ChartLegendCustomItemCellEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_legendCustomItemCellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartLegendCustomItemCells);
		}

		internal void ChartDerivedSeriesStart(int index)
		{
			TypeStart(CreateTypeName("ChartDerivedSeries" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).ChartDerivedSeriesCollection), "ChartDerivedSeriesExprHost");
		}

		internal void ChartDerivedSeriesSourceChartSeriesName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SourceChartSeriesNameExpr", expression);
		}

		internal void ChartDerivedSeriesDerivedSeriesFormula(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DerivedSeriesFormulaExpr", expression);
		}

		internal int ChartDerivedSeriesEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_derivedSeriesCollectionHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartDerivedSeriesCollection);
		}

		internal void ChartStripLineStart(int index)
		{
			TypeStart(CreateTypeName("ChartStripLine" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).ChartStripLines), "ChartStripLineExprHost");
		}

		internal void ChartStripLineTitle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TitleExpr", expression);
		}

		internal void ChartStripLineTitleAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TitleAngleExpr", expression);
		}

		internal void ChartStripLineTextOrientation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextOrientationExpr", expression);
		}

		internal void ChartStripLineToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartStripLineInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartStripLineIntervalType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartStripLineIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartStripLineIntervalOffsetType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartStripLineStripWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StripWidthExpr", expression);
		}

		internal void ChartStripLineStripWidthType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StripWidthTypeExpr", expression);
		}

		internal int ChartStripLineEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_stripLinesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartStripLines);
		}

		internal void ChartFormulaParameterStart(string name)
		{
			TypeStart(CreateTypeName("ChartFormulaParameter" + name, ((NonRootTypeDecl)m_currentTypeDecl).ChartFormulaParameters), "ChartFormulaParameterExprHost");
		}

		internal void ChartFormulaParameterValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal int ChartFormulaParameterEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_formulaParametersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartFormulaParameters);
		}

		internal void ChartLegendColumnStart(string name)
		{
			TypeStart(CreateTypeName("ChartLegendColumn" + name, ((NonRootTypeDecl)m_currentTypeDecl).ChartLegendColumns), "ChartLegendColumnExprHost");
		}

		internal void ChartLegendColumnColumnType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColumnTypeExpr", expression);
		}

		internal void ChartLegendColumnValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void ChartLegendColumnToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartLegendColumnMinimumWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinimumWidthExpr", expression);
		}

		internal void ChartLegendColumnMaximumWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaximumWidthExpr", expression);
		}

		internal void ChartLegendColumnSeriesSymbolWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeriesSymbolWidthExpr", expression);
		}

		internal void ChartLegendColumnSeriesSymbolHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeriesSymbolHeightExpr", expression);
		}

		internal int ChartLegendColumnEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_legendColumnsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartLegendColumns);
		}

		internal void ChartLegendCustomItemStart(string name)
		{
			TypeStart(CreateTypeName("ChartLegendCustomItem" + name, ((NonRootTypeDecl)m_currentTypeDecl).ChartLegendCustomItems), "ChartLegendCustomItemExprHost");
		}

		internal void ChartLegendCustomItemSeparator(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeparatorExpr", expression);
		}

		internal void ChartLegendCustomItemSeparatorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeparatorColorExpr", expression);
		}

		internal void ChartLegendCustomItemToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal int ChartLegendCustomItemEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_legendCustomItemsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartLegendCustomItems);
		}

		internal void ChartAreaStart(string chartAreaName)
		{
			TypeStart(CreateTypeName("ChartArea" + chartAreaName, ((NonRootTypeDecl)m_currentTypeDecl).ChartAreas), "ChartAreaExprHost");
		}

		internal void Chart3DPropertiesStart()
		{
			TypeStart("Chart3DProperties", "Chart3DPropertiesExprHost");
		}

		internal void Chart3DPropertiesEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnabledExpr", expression);
		}

		internal void Chart3DPropertiesRotation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RotationExpr", expression);
		}

		internal void Chart3DPropertiesProjectionMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ProjectionModeExpr", expression);
		}

		internal void Chart3DPropertiesInclination(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InclinationExpr", expression);
		}

		internal void Chart3DPropertiesPerspective(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PerspectiveExpr", expression);
		}

		internal void Chart3DPropertiesDepthRatio(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DepthRatioExpr", expression);
		}

		internal void Chart3DPropertiesShading(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShadingExpr", expression);
		}

		internal void Chart3DPropertiesGapDepth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("GapDepthExpr", expression);
		}

		internal void Chart3DPropertiesWallThickness(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WallThicknessExpr", expression);
		}

		internal void Chart3DPropertiesClustered(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ClusteredExpr", expression);
		}

		internal void Chart3DPropertiesEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "Chart3DPropertiesHost");
		}

		internal void ChartAreaHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartAreaAlignOrientation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AlignOrientationExpr", expression);
		}

		internal void ChartAreaEquallySizedAxesFont(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EquallySizedAxesFontExpr", expression);
		}

		internal void ChartAlignTypePosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartAlignTypePositionExpr", expression);
		}

		internal void ChartAlignTypeInnerPlotPosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InnerPlotPositionExpr", expression);
		}

		internal void ChartAlignTypCursor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CursorExpr", expression);
		}

		internal void ChartAlignTypeAxesView(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxesViewExpr", expression);
		}

		internal int ChartAreaEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_chartAreasHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ChartAreas);
		}

		internal void ChartDataPointValueX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesXExpr", expression);
		}

		internal void ChartDataPointValueY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesYExpr", expression);
		}

		internal void ChartDataPointValueSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesSizeExpr", expression);
		}

		internal void ChartDataPointValueHigh(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesHighExpr", expression);
		}

		internal void ChartDataPointValueLow(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesLowExpr", expression);
		}

		internal void ChartDataPointValueStart(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesStartExpr", expression);
		}

		internal void ChartDataPointValueEnd(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesEndExpr", expression);
		}

		internal void ChartDataPointValueMean(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesMeanExpr", expression);
		}

		internal void ChartDataPointValueMedian(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesMedianExpr", expression);
		}

		internal void ChartDataPointValueHighlightX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesHighlightXExpr", expression);
		}

		internal void ChartDataPointValueHighlightY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesHighlightYExpr", expression);
		}

		internal void ChartDataPointValueHighlightSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataPointValuesHighlightSizeExpr", expression);
		}

		internal void ChartDataPointValueFormatX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueFormatXExpr", expression);
		}

		internal void ChartDataPointValueFormatY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueFormatYExpr", expression);
		}

		internal void ChartDataPointValueFormatSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueFormatSizeExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueCurrencyLanguageXExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueCurrencyLanguageYExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataPointValueCurrencyLanguageSizeExpr", expression);
		}

		internal void ChartDataPointAxisLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AxisLabelExpr", expression);
		}

		internal void ChartDataPointToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void DataLabelStart()
		{
			TypeStart("DataLabel", "ChartDataLabelExprHost");
		}

		internal void DataLabelLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelExpr", expression);
		}

		internal void DataLabelVisible(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VisibleExpr", expression);
		}

		internal void DataLabelPosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ChartDataLabelPositionExpr", expression);
		}

		internal void DataLabelRotation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RotationExpr", expression);
		}

		internal void DataLabelUseValueAsLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseValueAsLabelExpr", expression);
		}

		internal void ChartDataLabelToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void DataLabelEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "DataLabelHost");
		}

		internal void DataPointStyleStart()
		{
			StyleStart("Style");
		}

		internal void DataPointStyleEnd()
		{
			StyleEnd("StyleHost");
		}

		internal void DataPointMarkerStart()
		{
			TypeStart("ChartMarker", "ChartMarkerExprHost");
		}

		internal void DataPointMarkerSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SizeExpr", expression);
		}

		internal void DataPointMarkerType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TypeExpr", expression);
		}

		internal void DataPointMarkerEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ChartMarkerHost");
		}

		internal void ChartMemberLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MemberLabelExpr", expression);
		}

		internal void ChartMemberStyleStart()
		{
			StyleStart("MemberStyle");
		}

		internal void ChartMemberStyleEnd()
		{
			StyleEnd("MemberStyleHost");
		}

		internal void DataValueStart()
		{
			TypeStart(CreateTypeName("DataValue", m_currentTypeDecl.DataValues), "DataValueExprHost");
		}

		internal int DataValueEnd(bool isCustomProperty)
		{
			return TypeEnd(m_currentTypeDecl.Parent, isCustomProperty ? "m_customPropertyHostsRemotable" : "m_dataValueHostsRemotable", ref m_currentTypeDecl.Parent.DataValues);
		}

		internal void DataValueName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataValueNameExpr", expression);
		}

		internal void DataValueValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataValueValueExpr", expression);
		}

		internal void BaseGaugeImageSource(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SourceExpr", expression);
		}

		internal void BaseGaugeImageValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void BaseGaugeImageMIMEType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void BaseGaugeImageTransparentColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparentColorExpr", expression);
		}

		internal void CapImageStart()
		{
			TypeStart("CapImage", "CapImageExprHost");
		}

		internal void CapImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "CapImageHost");
		}

		internal void CapImageHueColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HueColorExpr", expression);
		}

		internal void CapImageOffsetX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetXExpr", expression);
		}

		internal void CapImageOffsetY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetYExpr", expression);
		}

		internal void FrameImageStart()
		{
			TypeStart("FrameImage", "FrameImageExprHost");
		}

		internal void FrameImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "FrameImageHost");
		}

		internal void FrameImageHueColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HueColorExpr", expression);
		}

		internal void FrameImageTransparency(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparencyExpr", expression);
		}

		internal void FrameImageClipImage(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ClipImageExpr", expression);
		}

		internal void PointerImageStart()
		{
			TypeStart("PointerImage", "PointerImageExprHost");
		}

		internal void PointerImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "PointerImageHost");
		}

		internal void PointerImageHueColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HueColorExpr", expression);
		}

		internal void PointerImageTransparency(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparencyExpr", expression);
		}

		internal void PointerImageOffsetX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetXExpr", expression);
		}

		internal void PointerImageOffsetY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetYExpr", expression);
		}

		internal void TopImageStart()
		{
			TypeStart("TopImage", "TopImageExprHost");
		}

		internal void TopImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "TopImageHost");
		}

		internal void TopImageHueColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HueColorExpr", expression);
		}

		internal void BackFrameStart()
		{
			TypeStart("BackFrame", "BackFrameExprHost");
		}

		internal void BackFrameEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "BackFrameHost");
		}

		internal void BackFrameFrameStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FrameStyleExpr", expression);
		}

		internal void BackFrameFrameShape(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FrameShapeExpr", expression);
		}

		internal void BackFrameFrameWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FrameWidthExpr", expression);
		}

		internal void BackFrameGlassEffect(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("GlassEffectExpr", expression);
		}

		internal void FrameBackgroundStart()
		{
			TypeStart("FrameBackground", "FrameBackgroundExprHost");
		}

		internal void FrameBackgroundEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "FrameBackgroundHost");
		}

		internal void CustomLabelStart(string name)
		{
			TypeStart(CreateTypeName("CustomLabel" + name, ((NonRootTypeDecl)m_currentTypeDecl).CustomLabels), "CustomLabelExprHost");
		}

		internal int CustomLabelEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_customLabelsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).CustomLabels);
		}

		internal void CustomLabelText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextExpr", expression);
		}

		internal void CustomLabelAllowUpsideDown(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void CustomLabelDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void CustomLabelFontAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FontAngleExpr", expression);
		}

		internal void CustomLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void CustomLabelRotateLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RotateLabelExpr", expression);
		}

		internal void CustomLabelValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void CustomLabelHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void CustomLabelUseFontPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void GaugeClipContent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ClipContentExpr", expression);
		}

		internal void GaugeImageStart(string name)
		{
			TypeStart(CreateTypeName("GaugeImage" + name, ((NonRootTypeDecl)m_currentTypeDecl).GaugeImages), "GaugeImageExprHost");
		}

		internal int GaugeImageEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_gaugeImagesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).GaugeImages);
		}

		internal void GaugeAspectRatio(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AspectRatioExpr", expression);
		}

		internal void GaugeInputValueStart(int index)
		{
			TypeStart(CreateTypeName("GaugeInputValue" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).GaugeInputValues), "GaugeInputValueExprHost");
		}

		internal int GaugeInputValueEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_gaugeInputValueHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).GaugeInputValues);
		}

		internal void GaugeInputValueValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void GaugeInputValueFormula(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FormulaExpr", expression);
		}

		internal void GaugeInputValueMinPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinPercentExpr", expression);
		}

		internal void GaugeInputValueMaxPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaxPercentExpr", expression);
		}

		internal void GaugeInputValueMultiplier(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MultiplierExpr", expression);
		}

		internal void GaugeInputValueAddConstant(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AddConstantExpr", expression);
		}

		internal void GaugeLabelStart(string name)
		{
			TypeStart(CreateTypeName("GaugeLabel" + name, ((NonRootTypeDecl)m_currentTypeDecl).GaugeLabels), "GaugeLabelExprHost");
		}

		internal int GaugeLabelEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_gaugeLabelsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).GaugeLabels);
		}

		internal void GaugeLabelText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextExpr", expression);
		}

		internal void GaugeLabelAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AngleExpr", expression);
		}

		internal void GaugeLabelResizeMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void GaugeLabelTextShadowOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextShadowOffsetExpr", expression);
		}

		internal void GaugeLabelUseFontPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void GaugePanelAntiAliasing(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AntiAliasingExpr", expression);
		}

		internal void GaugePanelAutoLayout(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AutoLayoutExpr", expression);
		}

		internal void GaugePanelShadowIntensity(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShadowIntensityExpr", expression);
		}

		internal void GaugePanelTextAntiAliasingQuality(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextAntiAliasingQualityExpr", expression);
		}

		internal void GaugePanelItemTop(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TopExpr", expression);
		}

		internal void GaugePanelItemLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftExpr", expression);
		}

		internal void GaugePanelItemHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HeightExpr", expression);
		}

		internal void GaugePanelItemWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugePanelItemZIndex(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ZIndexExpr", expression);
		}

		internal void GaugePanelItemHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugePanelItemToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugePointerBarStart(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BarStartExpr", expression);
		}

		internal void GaugePointerDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void GaugePointerMarkerLength(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarkerLengthExpr", expression);
		}

		internal void GaugePointerMarkerStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarkerStyleExpr", expression);
		}

		internal void GaugePointerPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void GaugePointerSnappingEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SnappingEnabledExpr", expression);
		}

		internal void GaugePointerSnappingInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SnappingIntervalExpr", expression);
		}

		internal void GaugePointerToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugePointerHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugePointerWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugeScaleInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void GaugeScaleIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void GaugeScaleLogarithmic(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LogarithmicExpr", expression);
		}

		internal void GaugeScaleLogarithmicBase(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LogarithmicBaseExpr", expression);
		}

		internal void GaugeScaleMultiplier(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MultiplierExpr", expression);
		}

		internal void GaugeScaleReversed(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ReversedExpr", expression);
		}

		internal void GaugeScaleTickMarksOnTop(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TickMarksOnTopExpr", expression);
		}

		internal void GaugeScaleToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugeScaleHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugeScaleWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugeTickMarksStart(bool isMajor)
		{
			TypeStart("GaugeTickMarks" + (isMajor ? "GaugeMajorTickMarksHost" : "GaugeMinorTickMarksHost"), "GaugeTickMarksExprHost");
		}

		internal void GaugeTickMarksEnd(bool isMajor)
		{
			TypeEnd(m_currentTypeDecl.Parent, isMajor ? "GaugeMajorTickMarksHost" : "GaugeMinorTickMarksHost");
		}

		internal void TickMarkStyleStart()
		{
			TypeStart("TickMarkStyle", "TickMarkStyleExprHost");
		}

		internal void TickMarkStyleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "TickMarkStyleHost");
		}

		internal void GaugeTickMarksInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void GaugeTickMarksIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void LinearGaugeStart(string name)
		{
			TypeStart(CreateTypeName("LinearGauge" + name, ((NonRootTypeDecl)m_currentTypeDecl).LinearGauges), "LinearGaugeExprHost");
		}

		internal int LinearGaugeEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_linearGaugesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).LinearGauges);
		}

		internal void LinearGaugeOrientation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OrientationExpr", expression);
		}

		internal void LinearPointerStart(string name)
		{
			TypeStart(CreateTypeName("LinearPointer" + name, ((NonRootTypeDecl)m_currentTypeDecl).LinearPointers), "LinearPointerExprHost");
		}

		internal int LinearPointerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_linearPointersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).LinearPointers);
		}

		internal void LinearPointerType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TypeExpr", expression);
		}

		internal void LinearScaleStart(string name)
		{
			TypeStart(CreateTypeName("LinearScale" + name, ((NonRootTypeDecl)m_currentTypeDecl).LinearScales), "LinearScaleExprHost");
		}

		internal int LinearScaleEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_linearScalesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).LinearScales);
		}

		internal void LinearScaleStartMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartMarginExpr", expression);
		}

		internal void LinearScaleEndMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndMarginExpr", expression);
		}

		internal void LinearScalePosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PositionExpr", expression);
		}

		internal void NumericIndicatorStart(string name)
		{
			TypeStart(CreateTypeName("NumericIndicator" + name, ((NonRootTypeDecl)m_currentTypeDecl).NumericIndicators), "NumericIndicatorExprHost");
		}

		internal int NumericIndicatorEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_numericIndicatorsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).NumericIndicators);
		}

		internal void NumericIndicatorDecimalDigitColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DecimalDigitColorExpr", expression);
		}

		internal void NumericIndicatorDigitColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DigitColorExpr", expression);
		}

		internal void NumericIndicatorUseFontPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void NumericIndicatorDecimalDigits(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DecimalDigitsExpr", expression);
		}

		internal void NumericIndicatorDigits(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DigitsExpr", expression);
		}

		internal void NumericIndicatorMultiplier(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MultiplierExpr", expression);
		}

		internal void NumericIndicatorNonNumericString(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("NonNumericStringExpr", expression);
		}

		internal void NumericIndicatorOutOfRangeString(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OutOfRangeStringExpr", expression);
		}

		internal void NumericIndicatorResizeMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void NumericIndicatorShowDecimalPoint(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowDecimalPointExpr", expression);
		}

		internal void NumericIndicatorShowLeadingZeros(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowLeadingZerosExpr", expression);
		}

		internal void NumericIndicatorIndicatorStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void NumericIndicatorShowSign(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowSignExpr", expression);
		}

		internal void NumericIndicatorSnappingEnabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SnappingEnabledExpr", expression);
		}

		internal void NumericIndicatorSnappingInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SnappingIntervalExpr", expression);
		}

		internal void NumericIndicatorLedDimColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LedDimColorExpr", expression);
		}

		internal void NumericIndicatorSeparatorWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeparatorWidthExpr", expression);
		}

		internal void NumericIndicatorSeparatorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SeparatorColorExpr", expression);
		}

		internal void NumericIndicatorRangeStart(string name)
		{
			TypeStart(CreateTypeName("NumericIndicatorRange" + name, ((NonRootTypeDecl)m_currentTypeDecl).NumericIndicatorRanges), "NumericIndicatorRangeExprHost");
		}

		internal int NumericIndicatorRangeEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_numericIndicatorRangesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).NumericIndicatorRanges);
		}

		internal void NumericIndicatorRangeDecimalDigitColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DecimalDigitColorExpr", expression);
		}

		internal void NumericIndicatorRangeDigitColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DigitColorExpr", expression);
		}

		internal void PinLabelStart()
		{
			TypeStart("PinLabel", "PinLabelExprHost");
		}

		internal void PinLabelEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "PinLabelHost");
		}

		internal void PinLabelText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextExpr", expression);
		}

		internal void PinLabelAllowUpsideDown(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void PinLabelDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void PinLabelFontAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FontAngleExpr", expression);
		}

		internal void PinLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void PinLabelRotateLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RotateLabelExpr", expression);
		}

		internal void PinLabelUseFontPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void PointerCapStart()
		{
			TypeStart("PointerCap", "PointerCapExprHost");
		}

		internal void PointerCapEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "PointerCapHost");
		}

		internal void PointerCapOnTop(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OnTopExpr", expression);
		}

		internal void PointerCapReflection(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ReflectionExpr", expression);
		}

		internal void PointerCapCapStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CapStyleExpr", expression);
		}

		internal void PointerCapHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void PointerCapWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void RadialGaugeStart(string name)
		{
			TypeStart(CreateTypeName("RadialGauge" + name, ((NonRootTypeDecl)m_currentTypeDecl).RadialGauges), "RadialGaugeExprHost");
		}

		internal int RadialGaugeEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_radialGaugesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).RadialGauges);
		}

		internal void RadialGaugePivotX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PivotXExpr", expression);
		}

		internal void RadialGaugePivotY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PivotYExpr", expression);
		}

		internal void RadialPointerStart(string name)
		{
			TypeStart(CreateTypeName("RadialPointer" + name, ((NonRootTypeDecl)m_currentTypeDecl).RadialPointers), "RadialPointerExprHost");
		}

		internal int RadialPointerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_radialPointersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).RadialPointers);
		}

		internal void RadialPointerType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TypeExpr", expression);
		}

		internal void RadialPointerNeedleStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("NeedleStyleExpr", expression);
		}

		internal void RadialScaleStart(string name)
		{
			TypeStart(CreateTypeName("RadialScale" + name, ((NonRootTypeDecl)m_currentTypeDecl).RadialScales), "RadialScaleExprHost");
		}

		internal int RadialScaleEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_radialScalesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).RadialScales);
		}

		internal void RadialScaleRadius(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RadiusExpr", expression);
		}

		internal void RadialScaleStartAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartAngleExpr", expression);
		}

		internal void RadialScaleSweepAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SweepAngleExpr", expression);
		}

		internal void ScaleLabelsStart()
		{
			TypeStart("ScaleLabels", "ScaleLabelsExprHost");
		}

		internal void ScaleLabelsEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ScaleLabelsHost");
		}

		internal void ScaleLabelsInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void ScaleLabelsIntervalOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ScaleLabelsAllowUpsideDown(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void ScaleLabelsDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void ScaleLabelsFontAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FontAngleExpr", expression);
		}

		internal void ScaleLabelsPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void ScaleLabelsRotateLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RotateLabelsExpr", expression);
		}

		internal void ScaleLabelsShowEndLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowEndLabelsExpr", expression);
		}

		internal void ScaleLabelsHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void ScaleLabelsUseFontPercent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void ScalePinStart(bool isMaximum)
		{
			TypeStart("ScalePin" + (isMaximum ? "MaximumPinHost" : "MinimumPinHost"), "ScalePinExprHost");
		}

		internal void ScalePinEnd(bool isMaximum)
		{
			TypeEnd(m_currentTypeDecl.Parent, isMaximum ? "MaximumPinHost" : "MinimumPinHost");
		}

		internal void ScalePinLocation(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LocationExpr", expression);
		}

		internal void ScalePinEnable(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnableExpr", expression);
		}

		internal void ScaleRangeStart(string name)
		{
			TypeStart(CreateTypeName("ScaleRange" + name, ((NonRootTypeDecl)m_currentTypeDecl).ScaleRanges), "ScaleRangeExprHost");
		}

		internal int ScaleRangeEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_scaleRangesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).ScaleRanges);
		}

		internal void ScaleRangeDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void ScaleRangeStartWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartWidthExpr", expression);
		}

		internal void ScaleRangeEndWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndWidthExpr", expression);
		}

		internal void ScaleRangeInRangeBarPointerColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InRangeBarPointerColorExpr", expression);
		}

		internal void ScaleRangeInRangeLabelColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InRangeLabelColorExpr", expression);
		}

		internal void ScaleRangeInRangeTickMarksColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InRangeTickMarksColorExpr", expression);
		}

		internal void ScaleRangeBackgroundGradientType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BackgroundGradientTypeExpr", expression);
		}

		internal void ScaleRangePlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void ScaleRangeToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ScaleRangeHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void IndicatorImageStart()
		{
			TypeStart("IndicatorImage", "IndicatorImageExprHost");
		}

		internal void IndicatorImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "IndicatorImageHost");
		}

		internal void IndicatorImageHueColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HueColorExpr", expression);
		}

		internal void IndicatorImageTransparency(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparencyExpr", expression);
		}

		internal void StateIndicatorStart(string name)
		{
			TypeStart(CreateTypeName("StateIndicator" + name, ((NonRootTypeDecl)m_currentTypeDecl).StateIndicators), "StateIndicatorExprHost");
		}

		internal int StateIndicatorEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_stateIndicatorsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).StateIndicators);
		}

		internal void StateIndicatorIndicatorStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void StateIndicatorScaleFactor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void StateIndicatorResizeMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void StateIndicatorAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AngleExpr", expression);
		}

		internal void StateIndicatorTransformationType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransformationTypeExpr", expression);
		}

		internal void ThermometerStart()
		{
			TypeStart("Thermometer", "ThermometerExprHost");
		}

		internal void ThermometerEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ThermometerHost");
		}

		internal void ThermometerBulbOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BulbOffsetExpr", expression);
		}

		internal void ThermometerBulbSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BulbSizeExpr", expression);
		}

		internal void ThermometerThermometerStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ThermometerStyleExpr", expression);
		}

		internal void TickMarkStyleDistanceFromScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void TickMarkStylePlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PlacementExpr", expression);
		}

		internal void TickMarkStyleEnableGradient(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EnableGradientExpr", expression);
		}

		internal void TickMarkStyleGradientDensity(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("GradientDensityExpr", expression);
		}

		internal void TickMarkStyleLength(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LengthExpr", expression);
		}

		internal void TickMarkStyleWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void TickMarkStyleShape(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShapeExpr", expression);
		}

		internal void TickMarkStyleHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void IndicatorStateStart(string name)
		{
			TypeStart(CreateTypeName("IndicatorState" + name, ((NonRootTypeDecl)m_currentTypeDecl).IndicatorStates), "IndicatorStateExprHost");
		}

		internal int IndicatorStateEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_indicatorStatesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).IndicatorStates);
		}

		internal void IndicatorStateColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColorExpr", expression);
		}

		internal void IndicatorStateScaleFactor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void IndicatorStateIndicatorStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void MapViewZoom(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ZoomExpr", expression);
		}

		internal void MapElementViewStart()
		{
			TypeStart("MapElementView", "MapElementViewExprHost");
		}

		internal void MapElementViewEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapElementViewLayerName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LayerNameExpr", expression);
		}

		internal void MapCustomViewStart()
		{
			TypeStart("MapCustomView", "MapCustomViewExprHost");
		}

		internal void MapCustomViewEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapCustomViewCenterX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CenterXExpr", expression);
		}

		internal void MapCustomViewCenterY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CenterYExpr", expression);
		}

		internal void MapDataBoundViewStart()
		{
			TypeStart("MapDataBoundView", "MapDataBoundViewExprHost");
		}

		internal void MapDataBoundViewEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapBorderSkinStart()
		{
			TypeStart("MapBorderSkin", "MapBorderSkinExprHost");
		}

		internal void MapBorderSkinEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapBorderSkinHost");
		}

		internal void MapBorderSkinMapBorderSkinType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MapBorderSkinTypeExpr", expression);
		}

		internal void MapAntiAliasing(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AntiAliasingExpr", expression);
		}

		internal void MapTextAntiAliasingQuality(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextAntiAliasingQualityExpr", expression);
		}

		internal void MapShadowIntensity(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShadowIntensityExpr", expression);
		}

		internal void MapTileLanguage(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TileLanguageExpr", expression);
		}

		internal void MapVectorLayerMapDataRegionName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MapDataRegionNameExpr", expression);
		}

		internal void MapTileLayerStart(string name)
		{
			TypeStart(CreateTypeName("MapTileLayer" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapTileLayers), "MapTileLayerExprHost");
		}

		internal int MapTileLayerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapTileLayersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapTileLayers);
		}

		internal void MapTileLayerServiceUrl(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ServiceUrlExpr", expression);
		}

		internal void MapTileLayerTileStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TileStyleExpr", expression);
		}

		internal void MapTileLayerUseSecureConnection(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseSecureConnectionExpr", expression);
		}

		internal void MapTileStart(string name)
		{
			TypeStart(CreateTypeName("MapTile" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapTiles), "MapTileExprHost");
		}

		internal int MapTileEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapTilesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapTiles);
		}

		internal void MapPointLayerStart(string name)
		{
			TypeStart(CreateTypeName("MapPointLayer" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapPointLayers), "MapPointLayerExprHost");
		}

		internal int MapPointLayerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapPointLayersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapPointLayers);
		}

		internal void MapSpatialDataSetStart()
		{
			TypeStart("MapSpatialDataSet", "MapSpatialDataSetExprHost");
		}

		internal void MapSpatialDataSetEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapSpatialDataSetDataSetName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataSetNameExpr", expression);
		}

		internal void MapSpatialDataSetSpatialField(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SpatialFieldExpr", expression);
		}

		internal void MapSpatialDataRegionStart()
		{
			TypeStart("MapSpatialDataRegion", "MapSpatialDataRegionExprHost");
		}

		internal void MapSpatialDataRegionEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapSpatialDataRegionVectorData(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VectorDataExpr", expression);
		}

		internal void MapPolygonLayerStart(string name)
		{
			TypeStart(CreateTypeName("MapPolygonLayer" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapPolygonLayers), "MapPolygonLayerExprHost");
		}

		internal int MapPolygonLayerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapPolygonLayersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapPolygonLayers);
		}

		internal void MapShapefileStart()
		{
			TypeStart("MapShapefile", "MapShapefileExprHost");
		}

		internal void MapShapefileEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapShapefileSource(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SourceExpr", expression);
		}

		internal void MapLineLayerStart(string name)
		{
			TypeStart(CreateTypeName("MapLineLayer" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapLineLayers), "MapLineLayerExprHost");
		}

		internal int MapLineLayerEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapLineLayersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapLineLayers);
		}

		internal void MapLayerVisibilityMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("VisibilityModeExpr", expression);
		}

		internal void MapLayerMinimumZoom(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinimumZoomExpr", expression);
		}

		internal void MapLayerMaximumZoom(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaximumZoomExpr", expression);
		}

		internal void MapLayerTransparency(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparencyExpr", expression);
		}

		internal void MapFieldNameStart(string name)
		{
			TypeStart(CreateTypeName("MapFieldName" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapFieldNames), "MapFieldNameExprHost");
		}

		internal int MapFieldNameEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapFieldNamesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapFieldNames);
		}

		internal void MapFieldNameName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("NameExpr", expression);
		}

		internal void MapPointStart(string name)
		{
			TypeStart(CreateTypeName("MapPoint" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapPoints), "MapPointExprHost");
		}

		internal int MapPointEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapPointsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapPoints);
		}

		internal void MapPointUseCustomPointTemplate(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseCustomPointTemplateExpr", expression);
		}

		internal void MapPolygonStart(string name)
		{
			TypeStart(CreateTypeName("MapPolygon" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapPolygons), "MapPolygonExprHost");
		}

		internal int MapPolygonEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapPolygonsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapPolygons);
		}

		internal void MapPolygonUseCustomPolygonTemplate(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseCustomPolygonTemplateExpr", expression);
		}

		internal void MapPolygonUseCustomCenterPointTemplate(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseCustomPointTemplateExpr", expression);
		}

		internal void MapLineStart(string name)
		{
			TypeStart(CreateTypeName("MapLine" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapLines), "MapLineExprHost");
		}

		internal int MapLineEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapLinesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapLines);
		}

		internal void MapLineUseCustomLineTemplate(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UseCustomLineTemplateExpr", expression);
		}

		internal void MapSpatialElementTemplateHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapSpatialElementTemplateOffsetX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetXExpr", expression);
		}

		internal void MapSpatialElementTemplateOffsetY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("OffsetYExpr", expression);
		}

		internal void MapSpatialElementTemplateLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelExpr", expression);
		}

		internal void MapSpatialElementTemplateDataElementLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataElementLabelExpr", expression);
		}

		internal void MapSpatialElementTemplateToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void MapPointTemplateSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SizeExpr", expression);
		}

		internal void MapPointTemplateLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapMarkerTemplateStart()
		{
			TypeStart("MapMarkerTemplate", "MapMarkerTemplateExprHost");
		}

		internal void MapMarkerTemplateEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapPointTemplateHost");
		}

		internal void MapPolygonTemplateStart()
		{
			TypeStart("MapPolygonTemplate", "MapPolygonTemplateExprHost");
		}

		internal void MapPolygonTemplateEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapPolygonTemplateHost");
		}

		internal void MapPolygonTemplateScaleFactor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void MapPolygonTemplateCenterPointOffsetX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CenterPointOffsetXExpr", expression);
		}

		internal void MapPolygonTemplateCenterPointOffsetY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CenterPointOffsetYExpr", expression);
		}

		internal void MapPolygonTemplateShowLabel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowLabelExpr", expression);
		}

		internal void MapPolygonTemplateLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapLineTemplateStart()
		{
			TypeStart("MapLineTemplate", "MapLineTemplateExprHost");
		}

		internal void MapLineTemplateEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapLineTemplateHost");
		}

		internal void MapLineTemplateWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void MapLineTemplateLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapCustomColorRuleStart()
		{
			TypeStart("MapCustomColorRule", "MapCustomColorRuleExprHost");
		}

		internal void MapCustomColorRuleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapCustomColorStart(string name)
		{
			TypeStart(CreateTypeName("MapCustomColor" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapCustomColors), "MapCustomColorExprHost");
		}

		internal int MapCustomColorEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapCustomColorsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapCustomColors);
		}

		internal void MapCustomColorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColorExpr", expression);
		}

		internal void MapPointRulesStart()
		{
			TypeStart("MapPointRules", "MapPointRulesExprHost");
		}

		internal void MapPointRulesEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapPointRulesHost");
		}

		internal void MapMarkerRuleStart()
		{
			TypeStart("MapMarkerRule", "MapMarkerRuleExprHost");
		}

		internal void MapMarkerRuleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapMarkerRuleHost");
		}

		internal void MapMarkerStart()
		{
			TypeStart("MapMarker", "MapMarkerExprHost");
		}

		internal void MapMarkerEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapMarkerHost");
		}

		internal void MapMarkerInCollectionStart(string name)
		{
			TypeStart(CreateTypeName("MapMarker" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapMarkers), "MapMarkerExprHost");
		}

		internal int MapMarkerInCollectionEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapMarkersHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapMarkers);
		}

		internal void MapMarkerMapMarkerStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MapMarkerStyleExpr", expression);
		}

		internal void MapMarkerImageStart()
		{
			TypeStart("MapMarkerImage", "MapMarkerImageExprHost");
		}

		internal void MapMarkerImageEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapMarkerImageHost");
		}

		internal void MapMarkerImageSource(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SourceExpr", expression);
		}

		internal void MapMarkerImageValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void MapMarkerImageMIMEType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void MapMarkerImageTransparentColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TransparentColorExpr", expression);
		}

		internal void MapMarkerImageResizeMode(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void MapSizeRuleStart()
		{
			TypeStart("MapSizeRule", "MapSizeRuleExprHost");
		}

		internal void MapSizeRuleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapSizeRuleHost");
		}

		internal void MapSizeRuleStartSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartSizeExpr", expression);
		}

		internal void MapSizeRuleEndSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndSizeExpr", expression);
		}

		internal void MapPolygonRulesStart()
		{
			TypeStart("MapPolygonRules", "MapPolygonRulesExprHost");
		}

		internal void MapPolygonRulesEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapPolygonRulesHost");
		}

		internal void MapLineRulesStart()
		{
			TypeStart("MapLineRules", "MapLineRulesExprHost");
		}

		internal void MapLineRulesEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapLineRulesHost");
		}

		internal void MapColorRuleShowInColorScale(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowInColorScaleExpr", expression);
		}

		internal void MapColorRangeRuleStart()
		{
			TypeStart("MapColorRangeRule", "MapColorRangeRuleExprHost");
		}

		internal void MapColorRangeRuleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapColorRangeRuleStartColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartColorExpr", expression);
		}

		internal void MapColorRangeRuleMiddleColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MiddleColorExpr", expression);
		}

		internal void MapColorRangeRuleEndColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndColorExpr", expression);
		}

		internal void MapColorPaletteRuleStart()
		{
			TypeStart("MapColorPaletteRule", "MapColorPaletteRuleExprHost");
		}

		internal void MapColorPaletteRuleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapColorPaletteRulePalette(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PaletteExpr", expression);
		}

		internal void MapBucketStart(string name)
		{
			TypeStart(CreateTypeName("MapBucket" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapBuckets), "MapBucketExprHost");
		}

		internal int MapBucketEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapBucketsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapBuckets);
		}

		internal void MapBucketStartValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartValueExpr", expression);
		}

		internal void MapBucketEndValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndValueExpr", expression);
		}

		internal void MapAppearanceRuleDataValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DataValueExpr", expression);
		}

		internal void MapAppearanceRuleDistributionType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DistributionTypeExpr", expression);
		}

		internal void MapAppearanceRuleBucketCount(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BucketCountExpr", expression);
		}

		internal void MapAppearanceRuleStartValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("StartValueExpr", expression);
		}

		internal void MapAppearanceRuleEndValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EndValueExpr", expression);
		}

		internal void MapAppearanceRuleLegendText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LegendTextExpr", expression);
		}

		internal void MapLegendTitleStart()
		{
			TypeStart("MapLegendTitle", "MapLegendTitleExprHost");
		}

		internal void MapLegendTitleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapLegendTitleHost");
		}

		internal void MapLegendTitleCaption(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CaptionExpr", expression);
		}

		internal void MapLegendTitleTitleSeparator(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TitleSeparatorExpr", expression);
		}

		internal void MapLegendTitleTitleSeparatorColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TitleSeparatorColorExpr", expression);
		}

		internal void MapLegendStart(string name)
		{
			TypeStart(CreateTypeName("MapLegend" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapLegends), "MapLegendExprHost");
		}

		internal int MapLegendEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapLegendsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapLegends);
		}

		internal void MapLegendLayout(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LayoutExpr", expression);
		}

		internal void MapLegendAutoFitTextDisabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AutoFitTextDisabledExpr", expression);
		}

		internal void MapLegendMinFontSize(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void MapLegendInterlacedRows(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedRowsExpr", expression);
		}

		internal void MapLegendInterlacedRowsColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("InterlacedRowsColorExpr", expression);
		}

		internal void MapLegendEquallySpacedItems(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("EquallySpacedItemsExpr", expression);
		}

		internal void MapLegendTextWrapThreshold(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextWrapThresholdExpr", expression);
		}

		internal void MapTitleStart(string name)
		{
			TypeStart(CreateTypeName("MapTitle" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapTitles), "MapTitleExprHost");
		}

		internal int MapTitleEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapTitlesHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapTitles);
		}

		internal void MapTitleText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextExpr", expression);
		}

		internal void MapTitleAngle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("AngleExpr", expression);
		}

		internal void MapTitleTextShadowOffset(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TextShadowOffsetExpr", expression);
		}

		internal void MapDistanceScaleStart()
		{
			TypeStart("MapDistanceScale", "MapDistanceScaleExprHost");
		}

		internal void MapDistanceScaleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapDistanceScaleHost");
		}

		internal void MapDistanceScaleScaleColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ScaleColorExpr", expression);
		}

		internal void MapDistanceScaleScaleBorderColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ScaleBorderColorExpr", expression);
		}

		internal void MapColorScaleTitleStart()
		{
			TypeStart("MapColorScaleTitle", "MapColorScaleTitleExprHost");
		}

		internal void MapColorScaleTitleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapColorScaleTitleHost");
		}

		internal void MapColorScaleTitleCaption(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("CaptionExpr", expression);
		}

		internal void MapColorScaleStart()
		{
			TypeStart("MapColorScale", "MapColorScaleExprHost");
		}

		internal void MapColorScaleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapColorScaleHost");
		}

		internal void MapColorScaleTickMarkLength(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TickMarkLengthExpr", expression);
		}

		internal void MapColorScaleColorBarBorderColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ColorBarBorderColorExpr", expression);
		}

		internal void MapColorScaleLabelInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelIntervalExpr", expression);
		}

		internal void MapColorScaleLabelFormat(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelFormatExpr", expression);
		}

		internal void MapColorScaleLabelPlacement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapColorScaleLabelBehavior(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelBehaviorExpr", expression);
		}

		internal void MapColorScaleHideEndLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HideEndLabelsExpr", expression);
		}

		internal void MapColorScaleRangeGapColor(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RangeGapColorExpr", expression);
		}

		internal void MapColorScaleNoDataText(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("NoDataTextExpr", expression);
		}

		internal void MapStart(string name)
		{
			TypeStart(name, "MapExprHost");
		}

		internal int MapEnd()
		{
			return ReportItemEnd("m_mapHostsRemotable", ref m_rootTypeDecl.Maps);
		}

		internal void MapLocationStart()
		{
			TypeStart("MapLocation", "MapLocationExprHost");
		}

		internal void MapLocationEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapLocationHost");
		}

		internal void MapLocationLeft(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftExpr", expression);
		}

		internal void MapLocationTop(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TopExpr", expression);
		}

		internal void MapLocationUnit(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UnitExpr", expression);
		}

		internal void MapSizeStart()
		{
			TypeStart("MapSize", "MapSizeExprHost");
		}

		internal void MapSizeEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapSizeHost");
		}

		internal void MapSizeWidth(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("WidthExpr", expression);
		}

		internal void MapSizeHeight(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HeightExpr", expression);
		}

		internal void MapSizeUnit(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("UnitExpr", expression);
		}

		internal void MapGridLinesStart(bool isMeridian)
		{
			TypeStart("MapGridLines" + (isMeridian ? "MapMeridiansHost" : "MapParallelsHost"), "MapGridLinesExprHost");
		}

		internal void MapGridLinesEnd(bool isMeridian)
		{
			TypeEnd(m_currentTypeDecl.Parent, isMeridian ? "MapMeridiansHost" : "MapParallelsHost");
		}

		internal void MapGridLinesHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapGridLinesInterval(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("IntervalExpr", expression);
		}

		internal void MapGridLinesShowLabels(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ShowLabelsExpr", expression);
		}

		internal void MapGridLinesLabelPosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LabelPositionExpr", expression);
		}

		internal void MapDockableSubItemPosition(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PositionExpr", expression);
		}

		internal void MapDockableSubItemDockOutsideViewport(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DockOutsideViewportExpr", expression);
		}

		internal void MapDockableSubItemHidden(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapDockableSubItemToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void MapSubItemLeftMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftMarginExpr", expression);
		}

		internal void MapSubItemRightMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RightMarginExpr", expression);
		}

		internal void MapSubItemTopMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("TopMarginExpr", expression);
		}

		internal void MapSubItemBottomMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BottomMarginExpr", expression);
		}

		internal void MapSubItemZIndex(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ZIndexExpr", expression);
		}

		internal void MapBindingFieldPairStart(string name)
		{
			TypeStart(CreateTypeName("MapBindingFieldPair" + name, ((NonRootTypeDecl)m_currentTypeDecl).MapBindingFieldPairs), "MapBindingFieldPairExprHost");
		}

		internal int MapBindingFieldPairEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_mapBindingFieldPairsHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).MapBindingFieldPairs);
		}

		internal void MapBindingFieldPairFieldName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("FieldNameExpr", expression);
		}

		internal void MapBindingFieldPairBindingExpression(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("BindingExpressionExpr", expression);
		}

		internal void MapViewportStart()
		{
			TypeStart("MapViewport", "MapViewportExprHost");
		}

		internal void MapViewportEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapViewportHost");
		}

		internal void MapViewportSimplificationResolution(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SimplificationResolutionExpr", expression);
		}

		internal void MapViewportMapCoordinateSystem(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MapCoordinateSystemExpr", expression);
		}

		internal void MapViewportMapProjection(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MapProjectionExpr", expression);
		}

		internal void MapViewportProjectionCenterX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ProjectionCenterXExpr", expression);
		}

		internal void MapViewportProjectionCenterY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ProjectionCenterYExpr", expression);
		}

		internal void MapViewportMaximumZoom(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaximumZoomExpr", expression);
		}

		internal void MapViewportMinimumZoom(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinimumZoomExpr", expression);
		}

		internal void MapViewportContentMargin(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ContentMarginExpr", expression);
		}

		internal void MapViewportGridUnderContent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("GridUnderContentExpr", expression);
		}

		internal void MapLimitsStart()
		{
			TypeStart("MapLimits", "MapLimitsExprHost");
		}

		internal void MapLimitsEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MapLimitsHost");
		}

		internal void MapLimitsMinimumX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinimumXExpr", expression);
		}

		internal void MapLimitsMinimumY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MinimumYExpr", expression);
		}

		internal void MapLimitsMaximumX(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaximumXExpr", expression);
		}

		internal void MapLimitsMaximumY(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MaximumYExpr", expression);
		}

		internal void MapLimitsLimitToData(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LimitToDataExpr", expression);
		}

		internal void ParagraphStart(int index)
		{
			TypeStart(CreateTypeName("Paragraph" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).Paragraphs), "ParagraphExprHost");
		}

		internal int ParagraphEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_paragraphHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Paragraphs);
		}

		internal void ParagraphLeftIndent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("LeftIndentExpr", expression);
		}

		internal void ParagraphRightIndent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("RightIndentExpr", expression);
		}

		internal void ParagraphHangingIndent(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("HangingIndentExpr", expression);
		}

		internal void ParagraphSpaceBefore(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SpaceBeforeExpr", expression);
		}

		internal void ParagraphSpaceAfter(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SpaceAfterExpr", expression);
		}

		internal void ParagraphListStyle(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ListStyleExpr", expression);
		}

		internal void ParagraphListLevel(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ListLevelExpr", expression);
		}

		internal void TextRunStart(int index)
		{
			TypeStart(CreateTypeName("TextRun" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)m_currentTypeDecl).TextRuns), "TextRunExprHost");
		}

		internal int TextRunEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_textRunHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).TextRuns);
		}

		internal void TextRunToolTip(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void TextRunValue(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void TextRunMarkupType(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("MarkupTypeExpr", expression);
		}

		internal void LookupStart()
		{
			TypeStart(CreateTypeName("Lookup", m_rootTypeDecl.Lookups), "LookupExprHost");
		}

		internal void LookupSourceExpr(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("SourceExpr", expression);
		}

		internal void LookupResultExpr(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResultExpr", expression);
		}

		internal int LookupEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_lookupExprHostsRemotable", ref m_rootTypeDecl.Lookups);
		}

		internal void LookupDestStart()
		{
			TypeStart(CreateTypeName("LookupDest", m_rootTypeDecl.LookupDests), "LookupDestExprHost");
		}

		internal void LookupDestExpr(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DestExpr", expression);
		}

		internal int LookupDestEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_lookupDestExprHostsRemotable", ref m_rootTypeDecl.LookupDests);
		}

		internal void PageBreakStart()
		{
			TypeStart("PageBreak", "PageBreakExprHost");
		}

		internal bool PageBreakEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "PageBreakExprHost");
		}

		internal void Disabled(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("DisabledExpr", expression);
		}

		internal void PageName(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PageNameExpr", expression);
		}

		internal void ResetPageNumber(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ResetPageNumberExpr", expression);
		}

		internal void JoinConditionStart()
		{
			TypeStart(CreateTypeName("JoinCondition", ((NonRootTypeDecl)m_currentTypeDecl).JoinConditions), "JoinConditionExprHost");
		}

		internal void JoinConditionForeignKeyExpr(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("ForeignKeyExpr", expression);
		}

		internal void JoinConditionPrimaryKeyExpr(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			ExpressionAdd("PrimaryKeyExpr", expression);
		}

		internal int JoinConditionEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_joinConditionExprHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).JoinConditions);
		}

		private void TypeStart(string typeName, string baseType)
		{
			m_currentTypeDecl = new NonRootTypeDecl(typeName, baseType, m_currentTypeDecl, m_setCode);
		}

		private int TypeEnd(TypeDecl container, string name, ref CodeExpressionCollection initializers)
		{
			int result = -1;
			if (m_currentTypeDecl.HasExpressions)
			{
				result = container.NestedTypeColAdd(name, m_currentTypeDecl.BaseTypeName, ref initializers, m_currentTypeDecl.Type);
			}
			TypeEnd(container);
			return result;
		}

		private bool TypeEnd(TypeDecl container, string name)
		{
			bool hasExpressions = m_currentTypeDecl.HasExpressions;
			if (hasExpressions)
			{
				container.NestedTypeAdd(name, m_currentTypeDecl.Type);
			}
			TypeEnd(container);
			return hasExpressions;
		}

		private void TypeEnd(TypeDecl container)
		{
			Global.Tracer.Assert(m_currentTypeDecl.Parent != null && container != null, "(m_currentTypeDecl.Parent != null && container != null)");
			container.HasExpressions |= m_currentTypeDecl.HasExpressions;
			m_currentTypeDecl = m_currentTypeDecl.Parent;
		}

		private int ReportItemEnd(string name, ref CodeExpressionCollection initializers)
		{
			return TypeEnd(m_rootTypeDecl, name, ref initializers);
		}

		private void ParameterStart()
		{
			TypeStart(CreateTypeName("Parameter", ((NonRootTypeDecl)m_currentTypeDecl).Parameters), "ParamExprHost");
		}

		private int ParameterEnd(string propName)
		{
			return TypeEnd(m_currentTypeDecl.Parent, propName, ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Parameters);
		}

		private void StyleStart(string typeName)
		{
			TypeStart(typeName, "StyleExprHost");
		}

		private void StyleEnd(string propName)
		{
			TypeEnd(m_currentTypeDecl.Parent, propName);
		}

		private void AggregateStart()
		{
			TypeStart(CreateTypeName("Aggregate", m_rootTypeDecl.Aggregates), "AggregateParamExprHost");
		}

		private int AggregateEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_aggregateParamHostsRemotable", ref m_rootTypeDecl.Aggregates);
		}

		private string CreateTypeName(string template, CodeExpressionCollection initializers)
		{
			return template + ((initializers == null) ? "0" : initializers.Count.ToString(CultureInfo.InvariantCulture));
		}

		private void ExprIndexerCreate()
		{
			NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)m_currentTypeDecl;
			if (nonRootTypeDecl.IndexedExpressions != null)
			{
				Global.Tracer.Assert(nonRootTypeDecl.IndexedExpressions.Count > 0, "(currentTypeDecl.IndexedExpressions.Count > 0)");
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = "Item";
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				nonRootTypeDecl.Type.Members.Add(codeMemberProperty);
				int count = nonRootTypeDecl.IndexedExpressions.Count;
				if (count == 1)
				{
					codeMemberProperty.GetStatements.Add(nonRootTypeDecl.IndexedExpressions[0]);
				}
				else
				{
					codeMemberProperty.GetStatements.Add(ExprIndexerTree(nonRootTypeDecl.IndexedExpressions, 0, count - 1));
				}
			}
		}

		private CodeStatement ExprIndexerTree(ReturnStatementList indexedExpressions, int leftIndex, int rightIndex)
		{
			if (leftIndex == rightIndex)
			{
				return indexedExpressions[leftIndex];
			}
			int num = rightIndex - leftIndex >> 1;
			return new CodeConditionStatement
			{
				Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("index"), CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(leftIndex + num)),
				TrueStatements = 
				{
					ExprIndexerTree(indexedExpressions, leftIndex, leftIndex + num)
				},
				FalseStatements = 
				{
					ExprIndexerTree(indexedExpressions, leftIndex + num + 1, rightIndex)
				}
			};
		}

		private void IndexedExpressionAdd(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)m_currentTypeDecl;
				if (nonRootTypeDecl.IndexedExpressions == null)
				{
					nonRootTypeDecl.IndexedExpressions = new ReturnStatementList();
				}
				nonRootTypeDecl.HasExpressions = true;
				expression.ExprHostID = nonRootTypeDecl.IndexedExpressions.Add(CreateExprReturnStatement(expression));
			}
		}

		private void ExpressionAdd(string name, ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = name;
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.GetStatements.Add(CreateExprReturnStatement(expression));
				m_currentTypeDecl.Type.Members.Add(codeMemberProperty);
				m_currentTypeDecl.HasExpressions = true;
			}
		}

		private CodeMethodReturnStatement CreateExprReturnStatement(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			return new CodeMethodReturnStatement(new CodeSnippetExpression(expression.TransformedExpression))
			{
				LinePragma = new CodeLinePragma("Expr" + expression.CompileTimeID.ToString(CultureInfo.InvariantCulture) + "end", 0)
			};
		}
	}
}
