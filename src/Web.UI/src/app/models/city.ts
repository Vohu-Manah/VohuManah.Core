export type City = {
  id: number;
  name: string;
};

export type CityName = {
  id: number;
  title: string;
};

export type CreateCityRequest = {
  name: string;
};

export type UpdateCityRequest = CreateCityRequest & {
  id: number;
};


