export type Language = {
  id: number;
  name: string;
  abbreviation: string;
};

export type LanguageName = {
  id: number;
  title: string;
};

export type CreateLanguageRequest = {
  name: string;
  abbreviation: string;
};

export type UpdateLanguageRequest = CreateLanguageRequest & {
  id: number;
};


