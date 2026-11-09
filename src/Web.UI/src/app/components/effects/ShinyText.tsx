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
  return (
    <span
      className={className}
      style={{
        display: 'inline-block',
        backgroundImage: `linear-gradient(120deg, ${color} 40%, ${shineColor} 50%, ${color} 60%)`,
        backgroundSize: '220% 100%',
        backgroundClip: 'text',
        WebkitBackgroundClip: 'text',
        color: 'transparent',
        animation: `shiny-sweep ${duration}s linear infinite`,
        fontWeight: 700,
      }}
    >
      {text}
    </span>
  );
}
