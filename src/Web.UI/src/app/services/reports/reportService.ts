import { apiClient } from '@/app/services/api/client';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { getTokens } from '@/app/services/api/tokenStore';
import { appConfig } from '@/app/config/app-config';
import { appMessage } from '@/app/messages/appMessages';

export interface ReportViewRequest {
  reportName: string;
  datasourceName: string;
  parameters?: Record<string, string>;
}

export interface ReportExportRequest {
  reportName: string;
  datasourceName: string;
  format: 'pdf' | 'excel';
  parameters?: Record<string, string>;
}

export interface SearchListReportRequest {
  reportType: 'book' | 'manuscript' | 'publication';
  searchCriteria: Record<string, string | number | boolean | null>;
}

export function buildReportUrl(request: ReportViewRequest): string {
  const { reportName, datasourceName, parameters = {} } = request;
  const apiBaseUrl = appConfig.apiBaseUrl;

  const isSpecialReport =
    reportName === 'BookStatistic' ||
    reportName === 'ManuscriptStatistic' ||
    reportName === 'PublicationStatistic';

  let baseUrl: string;
  if (isSpecialReport) {
    const reportEndpoint = reportName.toLowerCase().replace('statistic', '-statistics');
    baseUrl = `${apiBaseUrl}/reports/${reportEndpoint}`;
  } else {
    baseUrl = `${apiBaseUrl}/reports/view/${reportName}`;
  }

  const queryParams = new URLSearchParams({
    datasourceName,
    ...Object.fromEntries(Object.entries(parameters).map(([key, value]) => [key, String(value)])),
  });

  const tokens = getTokens();
  if (tokens?.accessToken) {
    queryParams.append('access_token', tokens.accessToken);
  }

  return `${baseUrl}?${queryParams.toString()}`;
}

export function buildSearchListReportUrl(request: SearchListReportRequest): string {
  const { reportType } = request;
  const apiBaseUrl = appConfig.apiBaseUrl;

  const endpointMap: Record<string, string> = {
    book: 'book-search-list',
    manuscript: 'manuscript-search-list',
    publication: 'publication-search-list',
  };

  const baseUrl = `${apiBaseUrl}/reports/${endpointMap[reportType]}`;

  const queryParams = new URLSearchParams();

  const tokens = getTokens();
  if (tokens?.accessToken) {
    queryParams.append('access_token', tokens.accessToken);
  }

  return `${baseUrl}?${queryParams.toString()}`;
}

export async function getSearchListReportHtml(request: SearchListReportRequest): Promise<string> {
  const { reportType, searchCriteria } = request;

  const endpointMap: Record<string, string> = {
    book: 'reports/book-search-list',
    manuscript: 'reports/manuscript-search-list',
    publication: 'reports/publication-search-list',
  };

  const endpoint = endpointMap[reportType];

  const response = await apiClient.post<string>(endpoint, searchCriteria, {
    headers: {
      'Content-Type': 'application/json',
    },
  });

  return response.data;
}

export async function exportReport(request: ReportExportRequest): Promise<void> {
  const { reportName, datasourceName, format, parameters = {} } = request;

  const queryParams = new URLSearchParams({
    format,
    datasourceName,
  });

  if (parameters.filterType) {
    queryParams.append('FilterType', String(parameters.filterType));
  }

  Object.entries(parameters).forEach(([key, value]) => {
    if (key !== 'filterType' && value != null) {
      queryParams.append(key, String(value));
    }
  });

  const url = `/reports/export/${reportName}?${queryParams.toString()}`;

  try {
    const response = await apiClient.get<Blob>(url, {
      responseType: 'blob',
      headers: {
        Accept:
          format === 'pdf'
            ? 'application/pdf'
            : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      },
    });

    const blob = new Blob([response.data], {
      type:
        format === 'pdf'
          ? 'application/pdf'
          : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });

    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = `${reportName}.${format === 'pdf' ? 'pdf' : 'xlsx'}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);

    appMessage.success('دانلود گزارش', 'گزارش با موفقیت دانلود شد');
  } catch (error) {
    appMessage.error('خطا در دانلود گزارش', getErrorMessage(error));
    throw error;
  }
}

export async function exportSearchListReport(
  request: SearchListReportRequest & { format: 'pdf' | 'excel' },
): Promise<void> {
  const { reportType, searchCriteria, format } = request;

  const endpointMap: Record<string, string> = {
    book: 'reports/export/book-search-list',
    manuscript: 'reports/export/manuscript-search-list',
    publication: 'reports/export/publication-search-list',
  };

  const endpoint = `${endpointMap[reportType]}?format=${format}`;

  try {
    const response = await apiClient.post<Blob>(endpoint, searchCriteria, {
      responseType: 'blob',
      headers: {
        'Content-Type': 'application/json',
        Accept:
          format === 'pdf'
            ? 'application/pdf'
            : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      },
    });

    const blob = new Blob([response.data], {
      type:
        format === 'pdf'
          ? 'application/pdf'
          : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });

    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;

    const fileNameMap: Record<string, string> = {
      book: 'BookSearchList',
      manuscript: 'ManuscriptSearchList',
      publication: 'PublicationSearchList',
    };

    link.download = `${fileNameMap[reportType]}.${format === 'pdf' ? 'pdf' : 'xlsx'}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);

    appMessage.success('دانلود گزارش', 'گزارش با موفقیت دانلود شد');
  } catch (error) {
    appMessage.error('خطا در دانلود گزارش', getErrorMessage(error));
    throw error;
  }
}

export const reportService = {
  buildReportUrl,
  buildSearchListReportUrl,
  getSearchListReportHtml,
  exportReport,
  exportSearchListReport,
};
