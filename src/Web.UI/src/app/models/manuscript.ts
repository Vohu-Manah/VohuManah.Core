export type Manuscript = {
  id: number;
  name: string;
  author: string;
  writingPlaceId: number;
  writingPlaceName?: string;
  year: string;
  pageCount: number;
  size: string;
  gapId: number;
  gapTitle?: string;
  versionNo: string;
  languageId: number;
  languageName?: string;
  subjectId: number;
  subjectTitle?: string;
};

export type CreateManuscriptRequest = {
  name: string;
  author: string;
  writingPlaceId: number;
  year: string;
  pageCount: number;
  size: string;
  gapId: number;
  versionNo: string;
  languageId: number;
  subjectId: number;
};

export type UpdateManuscriptRequest = CreateManuscriptRequest & {
  id: number;
};

export type ManuscriptSearchRequest = {
  name?: string;
  author?: string;
  gapId?: number;
  writingPlaceId?: number;
  languageId?: number;
  subjectId?: number;
  year?: string;
};

export type ManuscriptSearchResponse = {
  id: number;
  name: string;
  author: string;
  gapTitle?: string;
  writingPlaceName?: string;
  year: string;
  languageName?: string;
  subjectTitle?: string;
  pageCount: number;
  size: string;
};


