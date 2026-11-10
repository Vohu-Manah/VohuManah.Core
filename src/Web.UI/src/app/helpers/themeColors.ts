
import type { MantineColorsTuple, MantineThemeOverride } from '@mantine/core';

export type ThemeColors = {
  primary: string;
  secondary: string;
  accent: string;
};

export const DEFAULT_THEME: ThemeColors = {
  primary: '#228be6',
  secondary: '#868e96',
  accent: '#fd7e14',
};

export function applyThemeColors({ primary, secondary, accent }: ThemeColors): void {
  if (typeof window === 'undefined') return;
  
  window.dispatchEvent(new CustomEvent('theme-colors-changed', { detail: { primary } }));

  const root = document.documentElement;
  
  
  const textColor = isDarkScheme() ? invertLuminance(secondary) : secondary;
  const iconColor = isDarkScheme() ? invertLuminance(accent) : accent;
  root.style.setProperty('--mantine-color-primary', primary);
  root.style.setProperty('--mantine-color-text', textColor);
  root.style.setProperty('--mantine-color-icon', iconColor);

  
  

  let styleElement = document.getElementById('theme-colors-style');
  if (!styleElement) {
    styleElement = document.createElement('style');
    styleElement.id = 'theme-colors-style';
    document.head.appendChild(styleElement);
  }

  styleElement.textContent = `
    :root {
      --mantine-color-primary: ${primary};
      --mantine-color-text: ${textColor};
      --mantine-color-icon: ${iconColor};
    }

    /* ظاهر tooltip چارت: سفید، گوشه گرد، border نازک، متن تیره
       ponytail: جمع‌وجور — نسخهٔ قبلی آن‌قدر بزرگ بود که میله‌ها و
       برچسب‌های زیرش را می‌پوشاند. */
    .mantine-ChartTooltip-tooltip {
      background: var(--mantine-color-body);
      border: 1px solid var(--mantine-color-default-border);
      border-radius: 8px;
      box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
      padding: 4px 7px;
      font-size: 11px;
      line-height: 1.5;
    }

    .mantine-ChartTooltip-tooltipLabel {
      color: var(--mantine-color-text);
      font-weight: 600;
      font-size: 11px;
      margin-bottom: 2px;
    }

    .mantine-ChartTooltip-tooltipBody {
      gap: 2px;
    }

    .mantine-ChartTooltip-tooltipItemName,
    .mantine-ChartTooltip-tooltipItemData {
      color: var(--mantine-color-text);
      font-size: 11px;
    }

    .mantine-ChartTooltip-tooltipItemColor {
      width: 8px;
      height: 8px;
    }
  `;
}


function invertLuminance(hex: string): string {
  const [r, g, b] = hexToRgb(hex);
  const l = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  
  const t = l < 0.5 ? 0.82 : 0.18;
  return rgbToHex(r + (255 * t - r) * 0.85, g + (255 * t - g) * 0.85, b + (255 * t - b) * 0.85);
}

function isDarkScheme(): boolean {
  return document.documentElement.getAttribute('data-mantine-color-scheme') === 'dark';
}

function hexToRgb(hex: string): [number, number, number] {
  const h = hex.replace('#', '');
  const v = h.length === 3 ? h.split('').map((c) => c + c).join('') : h;
  return [parseInt(v.slice(0, 2), 16), parseInt(v.slice(2, 4), 16), parseInt(v.slice(4, 6), 16)];
}

function rgbToHex(r: number, g: number, b: number): string {
  const f = (n: number) => Math.max(0, Math.min(255, Math.round(n))).toString(16).padStart(2, '0');
  return `#${f(r)}${f(g)}${f(b)}`;
}


function hexToShades(hex: string): string[] {
  const [r, g, b] = hexToRgb(hex);
  const offsets = [0.85, 0.78, 0.70, 0.60, 0.50, 0.36, 0, -0.16, -0.30, -0.42];
  return offsets.map((o) =>
    o >= 0
      ? rgbToHex(r + (255 - r) * o, g + (255 - g) * o, b + (255 - b) * o)
      : rgbToHex(r * (1 + o), g * (1 + o), b * (1 + o)),
  );
}


export function createMantineTheme(baseTheme: MantineThemeOverride, primaryColor: string): MantineThemeOverride {

  
  const palette = hexToShades(primaryColor) as unknown as MantineColorsTuple;

  return {
    ...baseTheme,
    colors: {
      ...baseTheme.colors,
      blue: palette,
    },
  };
}
