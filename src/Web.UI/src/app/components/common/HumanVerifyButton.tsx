import { useEffect, useRef, useState } from 'react';
import { Box, Text, useMantineColorScheme } from '@mantine/core';
import HoldButton from '@/app/components/common/HoldButton';
import {
  humanVerifyService,
  type HumanVerifyChallenge,
} from '@/app/services/library/humanVerifyService';
import { getErrorMessage } from '@/app/helpers/errorHandler';

const CheckMark = () => (
  <svg className="hv-check" viewBox="0 0 24 24" aria-hidden="true">
    <path d="M4 12.5l5 5L20 7" />
  </svg>
);

type HumanVerifyButtonProps = {
  onVerified?: (token: string) => void;
  onError?: (message: string) => void;
  disabled?: boolean;

  resetKey?: number | string;
};

export default function HumanVerifyButton({
  onVerified,
  onError,
  disabled,
  resetKey,
}: HumanVerifyButtonProps) {
  const { colorScheme } = useMantineColorScheme();
  const dark = colorScheme === 'dark';
  const [attempt, setAttempt] = useState(0);
  const [challenge, setChallenge] = useState<HumanVerifyChallenge | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [verified, setVerified] = useState(false);
  const verifiedRef = useRef(false);
  const [pointerSamples, setPointerSamples] = useState<{ x: number; y: number; t: number }[]>([]);
  const samplesRef = useRef<{ x: number; y: number; t: number }[]>([]);
  const honeypotRef = useRef<HTMLInputElement>(null);
  const modeRef = useRef<'pointer' | 'key'>('pointer');
  const keyRepeatsRef = useRef(0);

  useEffect(() => {
    if (verifiedRef.current) return;
    let alive = true;
    humanVerifyService
      .challenge()
      .then((c) => {
        if (alive) {
          setChallenge(c);
          setLoadError(null);
        }
      })
      .catch((error) => {
        if (alive) {
          setChallenge(null);
          setLoadError(getErrorMessage(error, 'چالش آماده نشد؛ نگه دارید و دوباره تلاش کنید'));
        }
      });
    return () => {
      alive = false;
    };
  }, [attempt]);

  useEffect(() => {
    if (resetKey === undefined) return;
    verifiedRef.current = false;
    setVerified(false);
    setErrorMsg(null);
    setChallenge(null);
    setAttempt((n) => n + 1);
  }, [resetKey]);

  const handleHold = async () => {
    if (!challenge || verifiedRef.current) return;
    const heldMs = Math.max(challenge.minHoldMs, 1500);
    const mode = modeRef.current;
    const samples = samplesRef.current.length > 0 ? samplesRef.current : pointerSamples;
    const keyRepeats = keyRepeatsRef.current;
    const honeypot = honeypotRef.current?.value ?? '';
    try {
      const token = await humanVerifyService.verify(
        challenge.challengeId,
        challenge.signature,
        heldMs,
        mode,
        samples,
        keyRepeats,
        honeypot,
      );
      verifiedRef.current = true;
      setVerified(true);
      setErrorMsg(null);
      onVerified?.(token.verifiedToken);
    } catch (error) {
      const message = getErrorMessage(
        error,
        'پذیرش انسانی پذیرفته نشد؛ نگه دارید و دوباره تلاش کنید',
      );
      setErrorMsg(message);
      onError?.(message);
      setAttempt((n) => n + 1);
    }
  };

  const fill = dark ? '#51cf66' : '#40c057';

  return (
    <Box mt="lg">
      {}
      <input
        ref={honeypotRef}
        type="text"
        name="website"
        tabIndex={-1}
        autoComplete="off"
        className="hidden-verify-input"
        aria-hidden="true"
      />

      {challenge ? (
        <HoldButton
          key={attempt}
          size="lg"
          holdTime={Math.max(challenge.minHoldMs, 1500)}
          resetAfter={0}
          disabled={disabled || verified}
          backgroundColor={dark ? '#26262b' : '#1f1f2d'}
          fillColor={fill}
          textColor={dark ? '#e8e8ee' : '#f6f6fb'}
          fillTextColor="#ffffff"
          glow
          wave
          onHold={handleHold}
          onPointerSample={(s) => {
            samplesRef.current.push(s);
            if (samplesRef.current.length > 200) samplesRef.current.shift();
            setPointerSamples([...samplesRef.current]);
          }}
          onModeChange={(m) => {
            modeRef.current = m;
          }}
          onKeyRepeat={() => {
            modeRef.current = 'key';
            keyRepeatsRef.current += 1;
          }}
          doneIcon={<CheckMark />}
          doneLabel="پذیرفته شد"
          className="w-100"
        >
          نشان دهید که ربات نیستید
        </HoldButton>
      ) : (
        <Text size="sm" c="dimmed" ta="center">
          {loadError ?? 'در حال آمادهسازی تأیید…'}
        </Text>
      )}

      {(errorMsg || verified) && (
        <Text
          role="status"
          aria-live="polite"
          size="xs"
          ta="center"
          mt={6}
          c={verified ? 'green' : 'red'}
        >
          {verified ? 'پذیرش انسانی پذیرفته شد' : errorMsg}
        </Text>
      )}
    </Box>
  );
}
