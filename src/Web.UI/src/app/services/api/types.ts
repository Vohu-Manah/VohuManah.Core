export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errorMessage?: string;
  errors?: Record<string, string[]>;
};

export type ApiError = {
  status: number;
  message: string;
  detail?: string;
  errors?: Record<string, string[]>;
  raw?: unknown;
};

export type ApiResponse<T> = {
  data: T;
};

