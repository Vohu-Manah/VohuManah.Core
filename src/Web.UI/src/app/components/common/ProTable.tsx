import { useMemo, useState } from 'react';
import { ActionIcon, Box, Group, ScrollArea, Table, TextInput, Tooltip } from '@mantine/core';
import { FaSearch, FaSyncAlt, FaFileExcel, FaFilePdf } from 'react-icons/fa';

export type ProTableColumn<T> = {
  key: keyof T | string;
  title: string;
  render?: (row: T) => React.ReactNode;
  width?: string | number;
  exportValue?: (row: T) => string | number;
};

type ProTableProps<T> = {
  data: T[];
  columns: ProTableColumn<T>[];
  onRefresh?: () => void;
  searchableKeys?: (keyof T)[];
  exportFileName?: string;
};

export function ProTable<T extends Record<string, any>>({
  data,
  columns,
  onRefresh,
  searchableKeys,
  exportFileName = 'export',
}: ProTableProps<T>) {
  const [query, setQuery] = useState('');

  const filtered = useMemo(() => {
    if (!query.trim() || !searchableKeys?.length) {
      return data;
    }
    const q = query.toLowerCase();
    return data.filter((row) =>
      searchableKeys.some((k) => String(row[k] ?? '').toLowerCase().includes(q)),
    );
  }, [data, query, searchableKeys]);

  const exportToExcel = () => {
    // Simple CSV export (can be enhanced with xlsx library)
    const headers = columns.map((col) => col.title).join(',');
    const rows = filtered.map((row) =>
      columns
        .map((col) => {
          const value = col.exportValue
            ? col.exportValue(row)
            : col.render
              ? String(col.render(row)).replace(/,/g, ';')
              : String(row[col.key as keyof T] ?? '');
          return `"${value}"`;
        })
        .join(','),
    );
    const csv = [headers, ...rows].join('\n');
    const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${exportFileName}.csv`;
    link.click();
  };

  const exportToPDF = () => {
    // Simple text-based PDF export
    const content = [
      exportFileName,
      '',
      ...filtered.map((row, idx) => {
        return columns
          .map((col) => {
            const value = col.exportValue
              ? col.exportValue(row)
              : col.render
                ? String(col.render(row))
                : String(row[col.key as keyof T] ?? '');
            return `${col.title}: ${value}`;
          })
          .join(' | ');
      }),
    ].join('\n');

    const blob = new Blob([content], { type: 'text/plain' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${exportFileName}.txt`;
    link.click();
  };

  return (
    <Box>
      <Group mb="sm" justify="space-between">
        <TextInput
          placeholder="جستجو..."
          leftSection={<FaSearch size={14} />}
          value={query}
          onChange={(e) => setQuery(e.currentTarget.value)}
          w="240px"
        />
        <Group gap="xs">
          <Tooltip label="خروجی Excel">
            <ActionIcon variant="light" color="green" onClick={exportToExcel}>
              <FaFileExcel />
            </ActionIcon>
          </Tooltip>
          <Tooltip label="خروجی PDF">
            <ActionIcon variant="light" color="red" onClick={exportToPDF}>
              <FaFilePdf />
            </ActionIcon>
          </Tooltip>
          {onRefresh && (
            <Tooltip label="بروزرسانی">
              <ActionIcon variant="light" onClick={onRefresh}>
                <FaSyncAlt />
              </ActionIcon>
            </Tooltip>
          )}
        </Group>
      </Group>
      <ScrollArea>
        <Table striped highlightOnHover>
          <Table.Thead>
            <Table.Tr>
              {columns.map((col) => (
                <Table.Th key={String(col.key)} style={{ width: col.width }}>
                  {col.title}
                </Table.Th>
              ))}
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {filtered.length === 0 ? (
              <Table.Tr>
                <Table.Td colSpan={columns.length} style={{ textAlign: 'center' }}>
                  داده‌ای یافت نشد
                </Table.Td>
              </Table.Tr>
            ) : (
              filtered.map((row, idx) => (
                <Table.Tr key={idx}>
                  {columns.map((col) => (
                    <Table.Td key={String(col.key)}>
                      {col.render ? col.render(row) : String(row[col.key as keyof T] ?? '')}
                    </Table.Td>
                  ))}
                </Table.Tr>
              ))
            )}
          </Table.Tbody>
        </Table>
      </ScrollArea>
    </Box>
  );
}
