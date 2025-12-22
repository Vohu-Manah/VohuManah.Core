export type Publisher = {
  id: number;
  name: string;
  managerName: string;
  telephone: string;
  address: string;
  cityId?: number;
  cityName?: string;
};

export type PublisherName = {
  id: number;
  title: string;
};

export type CreatePublisherRequest = {
  name: string;
  managerName: string;
  telephone: string;
  address: string;
  cityId: number;
};

export type UpdatePublisherRequest = CreatePublisherRequest & {
  id: number;
};


