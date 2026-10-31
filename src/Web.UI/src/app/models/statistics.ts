export type CountByItem = {
  text: string;
  value: number;
};

export type BookStatistics = {
  bookCount: number;
  volumeCount: number;
  countBySubject: CountByItem[];
  countByLanguage: CountByItem[];
  countByPublisher: CountByItem[];
  countByPublishPlace: CountByItem[];
  countByYear: CountByItem[];
};

export type ManuscriptStatistics = {
  manuscriptCount: number;
  countBySubject: CountByItem[];
  countByLanguage: CountByItem[];
  countByGap: CountByItem[];
  countByWritingPlace: CountByItem[];
  countByYear: CountByItem[];
};

export type PublicationStatistics = {
  publicationCount: number;
  countBySubject: CountByItem[];
  countByLanguage: CountByItem[];
  countByType: CountByItem[];
  countByPublishPlace: CountByItem[];
  countByPublishDate: CountByItem[];
};
