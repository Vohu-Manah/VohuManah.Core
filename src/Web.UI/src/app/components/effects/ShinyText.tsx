import { useEffect, useRef } from 'react';

type ShinyTextProps = {
  text: string;
  color?: string;
  shineColor?: string;
  duration?: number;
  className?: string;
};

export default function ShinyText({
  text,
  color = 'var(--mantine-color-text)',
  shineColor = 'rgba(255, 255, 255, 0.85)',
  duration = 3.2,
  className,
}: ShinyTextProps) {
  const ref = useRef<HTMLSpanElement>(null);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    el.style.setProperty('--shiny-color', color);
    el.style.setProperty('--shiny-shine', shineColor);
    el.style.setProperty('--shiny-duration', `${duration}s`);
  }, [color, shineColor, duration]);

  return (
    <span ref={ref} className={`shiny-text ${className ?? ''}`}>
      {text}
    </span>
  );
}
