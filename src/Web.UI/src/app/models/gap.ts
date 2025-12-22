export type Gap = {
  id: number;
  title: string;
  note: string;
  state: boolean;
};

export type GapName = {
  id: number;
  title: string;
};

export type CreateGapRequest = {
  title: string;
  note: string;
  state: boolean;
};

export type UpdateGapRequest = CreateGapRequest & {
  id: number;
};


