export type Publisher = {
  id: number;
  name: string;
  managerName: string;
  telephone: string;
  address: string;
  website?: string;
  email?: string;
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
  website?: string;
  email?: string;
  cityId: number;
};

export type UpdatePublisherRequest = CreatePublisherRequest & {
  id?: number;
};


