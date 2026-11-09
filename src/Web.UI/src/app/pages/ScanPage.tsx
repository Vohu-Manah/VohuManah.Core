import { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Stack,
  Group,
  Title,
  Text,
  SegmentedControl,
  Button,
  TextInput,
  Alert,
  Center,
  Loader,
  Card,
} from '@mantine/core';
import { FaCamera, FaImage, FaKeyboard } from 'react-icons/fa';
import { Html5Qrcode } from 'html5-qrcode';
import { decodeQr } from '@/app/helpers/qrPayload';
import { appMessage } from '@/app/messages/appMessages';

const CAMERA_READER_ID = 'qr-scanner-camera';
const IMAGE_READER_ID = 'qr-scanner-image';

export default function ScanPage() {
  const navigate = useNavigate();
  const [mode, setMode] = useState<'camera' | 'image'>('camera');
  const [cameraSupported] = useState(
    () => typeof navigator !== 'undefined' && !!navigator.mediaDevices?.getUserMedia,
  );
  const [cameraError, setCameraError] = useState(false);
  const [imageBusy, setImageBusy] = useState(false);
  const [manualValue, setManualValue] = useState('');
  const handledRef = useRef(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const go = (raw: string) => {
    const path = decodeQr(raw);
    if (!path) {
      appMessage.error('بارکد شناسایی نشد');
      return;
    }
    if (handledRef.current) return;
    handledRef.current = true;
    navigate(path);
  };

  
  
  const stateRef = useRef<{ ins: Html5Qrcode; pending?: Promise<void> } | null>(null);
  const tokenRef = useRef(0);

  useEffect(() => {
    if (mode !== 'camera' || !cameraSupported) return;
    const token = ++tokenRef.current;
    let disposed = false;

    if (!stateRef.current) stateRef.current = { ins: new Html5Qrcode(CAMERA_READER_ID) };
    const st = stateRef.current;

    if (!st.pending) {
      st.pending = (async () => {
        try {
          await st.ins.start(
            { facingMode: 'environment' } as MediaTrackConstraints,
            { fps: 10, qrbox: { width: 260, height: 260 } },
            (decodedText) => {
              if (!disposed && token === tokenRef.current) go(decodedText);
            },
            () => {},
          );
          st.pending = undefined;
          
          if (disposed || token !== tokenRef.current) {
            try { await st.ins.stop(); } catch {
    
  }
          }
        } catch {
          st.pending = undefined;
          if (!disposed && token === tokenRef.current) setCameraError(true);
        }
      })();
    }

    return () => {
      disposed = true;
      try { void st.ins.stop().catch(() => {}); } catch {
    
  }
    };

  }, [mode, cameraSupported]);

  
  
  const normalizeImage = async (file: File): Promise<File> => {
    try {
      const bitmap = await createImageBitmap(file, { imageOrientation: 'from-image' });
      const MAX_DIM = 1000;
      const scale = Math.min(1, MAX_DIM / Math.max(bitmap.width, bitmap.height));
      const w = Math.max(1, Math.round(bitmap.width * scale));
      const h = Math.max(1, Math.round(bitmap.height * scale));
      const canvas = document.createElement('canvas');
      canvas.width = w;
      canvas.height = h;
      const ctx = canvas.getContext('2d');
      if (!ctx) {
        bitmap.close();
        return file;
      }
      ctx.drawImage(bitmap, 0, 0, w, h);
      bitmap.close();
      const blob = await new Promise<Blob | null>((resolve) => canvas.toBlob(resolve, 'image/png'));
      return blob ? new File([blob], file.name, { type: 'image/png' }) : file;
    } catch {
      return file;
    }
  };

  const handleFile = async (file: File) => {
    setImageBusy(true);
    const scanner = new Html5Qrcode(IMAGE_READER_ID);
    try {
      const normalized = await normalizeImage(file);
      const text = await scanner.scanFile(normalized, false);
      go(text);
    } catch {
      appMessage.error('بارکد در تصویر یافت نشد');
    } finally {
      setImageBusy(false);
      try {
        scanner.clear();
      } catch {
  
      }
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleManualSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (manualValue.trim()) go(manualValue.trim());
  };

  return (
    <Stack p="md" gap="lg">
      <Title order={2}>اسکن بارکد</Title>

      <SegmentedControl
        value={mode}
        onChange={(v) => {
          setMode(v as 'camera' | 'image');
          setCameraError(false);
        }}
        data={[
          { label: 'دوربین', value: 'camera' },
          { label: 'انتخاب عکس', value: 'image' },
        ]}
      />

      {mode === 'camera' ? (
        <>
          {!cameraSupported ? (
            <Alert color="yellow" icon={<FaCamera />}>
              دوربین در این مرورگر در دسترس نیست. از حالت «انتخاب عکس» یا «ورود دستی» استفاده کنید.
            </Alert>
          ) : (
            <Card withBorder p="md" radius="md" maw={420} mx="auto" w="100%">
              {cameraError ? (
                <Alert color="red" icon={<FaCamera />}>
                  دسترسی به دوربین رد شد یا یافت نشد. از «انتخاب عکس» یا «ورود دستی» استفاده کنید.
                </Alert>
              ) : (
                <>
                  <div id={CAMERA_READER_ID} />
                  {!imageBusy && (
                    <Center>
                      <Loader size="sm" />
                      <Text size="sm" c="dimmed" ml="xs">
                        در جستجوی بارکد…
                      </Text>
                    </Center>
                  )}
                </>
              )}
            </Card>
          )}
        </>
      ) : (
        <Card withBorder p="md" radius="md" maw={420} mx="auto" w="100%">
          <Stack gap="md">
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              capture="environment"
              className="d-none"
              onChange={(e) => {
                const file = e.target.files?.[0];
                if (file) void handleFile(file);
              }}
            />
            <Button
              variant="light"
              leftSection={<FaImage />}
              loading={imageBusy}
              onClick={() => fileInputRef.current?.click()}
            >
              {imageBusy ? 'در حال پردازش تصویر…' : 'انتخاب تصویر بارکد'}
            </Button>
            <div id={IMAGE_READER_ID} className="d-none" />
            <Text size="xs" c="dimmed">
              تصویری که بارکد در آن واضح باشد انتخاب کنید (قالب‌های JPG/PNG).
            </Text>
          </Stack>
        </Card>
      )}

      <Card withBorder p="md" radius="md" maw={420} mx="auto" w="100%">
        <form onSubmit={handleManualSubmit}>
          <Stack gap="xs">
            <Text fw={600} size="sm">
              <FaKeyboard className="m-i-end-6" />
              ورود دستی
            </Text>
            <Text size="xs" c="dimmed">
              کد کتاب، آدرس، یا خروجی اسکنر دستی را وارد کنید.
            </Text>
            <Group gap="xs">
              <TextInput
                className="flex-1"
                placeholder="مثلاً: 20015000 یا app://books/profile?id=20015000"
                value={manualValue}
                onChange={(e) => setManualValue(e.currentTarget.value)}
                autoFocus
              />
              <Button type="submit" variant="filled">
                برو
              </Button>
            </Group>
          </Stack>
        </form>
      </Card>
    </Stack>
  );
}