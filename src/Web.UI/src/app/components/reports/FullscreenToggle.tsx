import { createPortal } from 'react-dom';
import { ActionIcon } from '@mantine/core';
import { FaExpand, FaCompress } from 'react-icons/fa';

interface FullscreenToggleProps {
  fullscreen: boolean;
  onToggle: () => void;
}

export function FullscreenToggle({ fullscreen, onToggle }: FullscreenToggleProps) {
  const button = (
    <ActionIcon
      variant="subtle"
      color="gray"
      title={fullscreen ? 'خروج از تمام‌صفحه' : 'نمایش تمام‌صفحه'}
      onClick={onToggle}
      className={fullscreen ? 'report-fullscreen-btn report-fullscreen-active' : 'report-fullscreen-btn'}
    >
      {fullscreen ? <FaCompress size={16} /> : <FaExpand size={16} />}
    </ActionIcon>
  );
  return fullscreen ? createPortal(button, document.body) : button;
}
