export type PublicationType = {
  id: number;
  title: string;
};

export type PublicationTypeName = {
  id: number;
  title: string;
};

export type CreatePublicationTypeRequest = {
  title: string;
};

export type UpdatePublicationTypeRequest = CreatePublicationTypeRequest & {
  id: number;
};


