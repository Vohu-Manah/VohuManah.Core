import { useMemo, useState } from 'react';
import { ActionIcon, Box, Group, ScrollArea, Table, TextInput } from '@mantine/core';
import { FaSearch, FaSyncAlt } from 'react-icons/fa';

export type ProTableColumn<T> = {
  key: keyof T | string;
  title: string;
  render?: (row: T) => React.ReactNode;
  width?: string | number;
};

type ProTableProps<T> = {
  data: T[];
  columns: ProTableColumn<T>[];
  onRefresh?: () => void;
  searchableKeys?: (keyof T)[];
};

export function ProTable<T extends Record<string, any>>({
  data,
  columns,
  onRefresh,
  searchableKeys,
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
        {onRefresh && (
          <ActionIcon variant="light" onClick={onRefresh}>
            <FaSyncAlt />
          </ActionIcon>
        )}
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
            {filtered.map((row, idx) => (
              <Table.Tr key={idx}>
                {columns.map((col) => (
                  <Table.Td key={String(col.key)}>
                    {col.render ? col.render(row) : String(row[col.key as keyof T] ?? '')}
                  </Table.Td>
                ))}
              </Table.Tr>
            ))}
          </Table.Tbody>
        </Table>
      </ScrollArea>
    </Box>
  );
}

