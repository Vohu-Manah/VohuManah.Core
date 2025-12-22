export type Subject = {
  id: number;
  title: string;
};

export type SubjectName = {
  id: number;
  title: string;
};

export type CreateSubjectRequest = {
  title: string;
};

export type UpdateSubjectRequest = CreateSubjectRequest & {
  id: number;
};


