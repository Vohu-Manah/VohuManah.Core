import { useEffect, useState } from 'react';
import { Box, Text } from '@mantine/core';

type AnalogClockProps = {
  
  size?: number;
  
  label?: string;
};


export default function AnalogClock({ size = 120, label }: AnalogClockProps) {
  const [now, setNow] = useState(() => new Date());

  useEffect(() => {
    
    const id = window.setInterval(() => setNow(new Date()), 1000);
    return () => window.clearInterval(id);
  }, []);

  const seconds = now.getSeconds();
  const minutes = now.getMinutes() + seconds / 60;
  const hours = (now.getHours() % 12) + minutes / 60;

  const cx = size / 2;
  const cy = size / 2;
  const r = size / 2 - 4;

  const hand = (angle: number, length: number, width: number, color: string) => {
    const rad = (angle * Math.PI) / 180;
    return {
      x1: cx,
      y1: cy,
      x2: cx + Math.sin(rad) * length,
      y2: cy - Math.cos(rad) * length,
      stroke: color,
      strokeWidth: width,
      strokeLinecap: 'round' as const,
    };
  };

  const ticks = Array.from({ length: 12 }, (_, i) => i * 30);

  return (
    <Box style={{ textAlign: 'center' }}>
      <svg width={size} height={size} viewBox={`0 0 ${size} ${size}`} role="img" aria-label="ساعت">
        <circle cx={cx} cy={cy} r={r} fill="#f8f9fa" stroke="#adb5bd" strokeWidth={2} />

        {ticks.map((deg) => {
          const rad = (deg * Math.PI) / 180;
          const inner = r - 10;
          const outer = r - 4;
          return (
            <line
              key={deg}
              x1={cx + Math.sin(rad) * inner}
              y1={cy - Math.cos(rad) * inner}
              x2={cx + Math.sin(rad) * outer}
              y2={cy - Math.cos(rad) * outer}
              stroke="#495057"
              strokeWidth={deg % 90 === 0 ? 2.5 : 1}
              strokeLinecap="round"
            />
          );
        })}

        <line {...hand(hours * 30, r * 0.5, 4, '#212529')} />
        <line {...hand(minutes * 6, r * 0.72, 3, '#212529')} />
        <line {...hand(seconds * 6, r * 0.8, 1.5, '#e8590c')} />

        <circle cx={cx} cy={cy} r={4} fill="#212529" />
        <circle cx={cx} cy={cy} r={1.5} fill="#fff" />
      </svg>

      {label && (
        <Text size="xs" c="dimmed" mt={4}>
          {label}
        </Text>
      )}
    </Box>
  );
}
