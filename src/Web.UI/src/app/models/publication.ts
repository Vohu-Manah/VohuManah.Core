export type Publication = {
  id: number;
  name: string;
  typeId: number;
  typeName?: string;
  concessionaire: string;
  responsibleDirector: string;
  editor: string;
  year: string;
  journalNo: string;
  publishDate: string;
  publishPlaceId: number;
  publishPlaceName?: string;
  no: string;
  period: string;
  languageId: number;
  languageName?: string;
  subjectId: number;
  subjectTitle?: string;
};

export type CreatePublicationRequest = {
  name: string;
  typeId: number;
  concessionaire: string;
  responsibleDirector: string;
  editor: string;
  year: string;
  journalNo: string;
  publishDate: string;
  publishPlaceId: number;
  no: string;
  period: string;
  languageId: number;
  subjectId: number;
};

export type UpdatePublicationRequest = CreatePublicationRequest & {
  id: number;
};

export type PublicationSearchRequest = {
  name?: string;
  typeId?: number;
  publishPlaceId?: number;
  languageId?: number;
  subjectId?: number;
  publishDate?: string;
};

export type PublicationSearchResponse = {
  id: number;
  name: string;
  typeName?: string;
  publishPlaceName?: string;
  publishDate: string;
  languageName?: string;
  subjectTitle?: string;
  journalNo: string;
  no: string;
};


