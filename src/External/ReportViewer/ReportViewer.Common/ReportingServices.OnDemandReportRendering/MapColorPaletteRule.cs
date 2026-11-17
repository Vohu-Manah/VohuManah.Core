using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorPaletteRule : MapColorRule
	{
		private ReportEnumProperty<MapPalette> m_palette;

		public ReportEnumProperty<MapPalette> Palette
		{
			get
			{
				if (m_palette == null && MapColorPaletteRuleDef.Palette != null)
				{
					m_palette = new ReportEnumProperty<MapPalette>(MapColorPaletteRuleDef.Palette.IsExpression, MapColorPaletteRuleDef.Palette.OriginalText, EnumTranslator.TranslateMapPalette(MapColorPaletteRuleDef.Palette.StringValue, null));
				}
				return m_palette;
			}
		}

		internal ReportingServices.ReportIntermediateFormat.MapColorPaletteRule MapColorPaletteRuleDef => (ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)base.MapAppearanceRuleDef;

		public new MapColorPaletteRuleInstance Instance => (MapColorPaletteRuleInstance)GetInstance();

		internal MapColorPaletteRule(ReportingServices.ReportIntermediateFormat.MapColorPaletteRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapColorPaletteRuleInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
