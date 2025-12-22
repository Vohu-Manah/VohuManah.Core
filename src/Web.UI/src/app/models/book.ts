export type Book = {
  id: number;
  name: string;
  author: string;
  translator: string;
  editor: string;
  corrector: string;
  publisherId: number;
  publisherName?: string;
  publishPlaceId: number;
  publishPlaceName?: string;
  publishYear: string;
  publishOrder: string;
  isbn: string;
  no: string;
  volumeCount: number;
  languageId: number;
  languageName?: string;
  subjectId: number;
  subjectTitle?: string;
  bookShelfRow: string;
};

export type BookList = {
  id: number;
  name: string;
  author: string;
  publisherName?: string;
  translator: string;
  corrector: string;
  no: string;
  isbn: string;
  volumeCount: number;
  bookShelfRow: string;
};

export type CreateBookRequest = {
  name: string;
  author: string;
  translator: string;
  editor: string;
  corrector: string;
  publisherId: number;
  publishPlaceId: number;
  publishYear: string;
  publishOrder: string;
  isbn: string;
  no: string;
  volumeCount: number;
  languageId: number;
  subjectId: number;
  bookShelfRow: string;
};

export type UpdateBookRequest = CreateBookRequest & {
  id: number;
};

export type BookSearchRequest = {
  name?: string;
  author?: string;
  publisherId?: number;
  publishPlaceId?: number;
  languageId?: number;
  subjectId?: number;
  publishYear?: string;
};

export type BookSearchResponse = {
  id: number;
  name: string;
  author: string;
  publisherName?: string;
  publishPlaceName?: string;
  publishYear: string;
  languageName?: string;
  subjectTitle?: string;
  volumeCount: number;
  bookShelfRow: string;
};


