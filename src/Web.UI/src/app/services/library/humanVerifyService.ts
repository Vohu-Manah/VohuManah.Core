import { rawClient } from '@/app/services/api/client';
import { toApiError } from '@/app/services/api/errors';

export type HumanVerifyChallenge = {
  challengeId: string;
  issuedAtUnixSeconds: number;
  signature: string;
  expiresInSeconds: number;
  minHoldMs: number;
};

export type HumanVerifyToken = {
  verifiedToken: string;
  expiresInSeconds: number;
};

export const humanVerifyService = {
  async challenge(): Promise<HumanVerifyChallenge> {
    try {
      const res = await rawClient.post<HumanVerifyChallenge>('/library/human-verify/challenge');
      return res.data;
    } catch (error) {
      throw toApiError(error);
    }
  },

  async verify(
    challengeId: string,
    signature: string,
    heldMs: number,
    mode: string,
    pointerSamples: { x: number; y: number; t: number }[],
    keyRepeats: number,
    honeypot: string,
  ): Promise<HumanVerifyToken> {
    try {
      const res = await rawClient.post<HumanVerifyToken>('/library/human-verify/verify', {
        challengeId,
        signature,
        heldMs,
        mode,
        pointerSamples,
        keyRepeats,
        honeypot,
      });
      return res.data;
    } catch (error) {
      throw toApiError(error);
    }
  },
};
