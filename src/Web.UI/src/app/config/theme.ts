import type { MantineThemeOverride } from '@mantine/core';

export const theme: MantineThemeOverride = {
  primaryColor: 'blue',
  defaultRadius: 'md',
  fontFamily: "'Shabnam', IRANSans, Vazirmatn, 'Segoe UI', sans-serif",
  components: {
    Button: {
      defaultProps: { variant: 'filled' },
    },
  },
};

