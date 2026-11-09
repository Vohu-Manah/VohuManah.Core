import { useMemo, useState, useEffect, isValidElement, cloneElement } from 'react';
import {
  ActionIcon, Box, Group, Table, TextInput, Tooltip, Loader, Center, Pagination, Checkbox, Radio,
  Stack, Text, Paper,
} from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { FaSearch, FaSyncAlt, FaFileExcel, FaFilePdf, FaSortUp, FaTable, FaListUl } from 'react-icons/fa';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';
import vazirFontUrl from '@/assets/fonts/vazir/Vazirmatn-Regular.ttf?url';

let vazirFontLoaded = false;
let vazirFontData: string | null = null;
let vazirFontPromise: Promise<void> | null = null;

export type ProTableColumn<T> = {
  key: keyof T | string;
  title: string;
  render?: (row: T) => React.ReactNode;
  width?: string | number;
  exportValue?: (row: T) => string | number;
  
  exportable?: boolean;
};

export type SelectionMode = 'single' | 'multi' | 'none';

type ProTableProps<T> = {
  data: T[];
  columns: ProTableColumn<T>[];
  onRefresh?: () => void;
  searchableKeys?: (keyof T)[];
  exportFileName?: string;
  loading?: boolean;
  itemsPerPage?: number;
  selectionMode?: SelectionMode;
  selectedRows?: T[];
  onSelectionChange?: (selectedRows: T[]) => void;
  getRowId?: (row: T) => string | number;
  enableExcelExport?: boolean;
  enablePdfExport?: boolean;
  
  serverTotal?: number;
  
  page?: number;
  
  mobileCards?: boolean;
  
  pageSize?: number;
  
  onPageChange?: (page: number) => void;
  
  filters?: React.ReactNode;
  
  sortBy?: string;
  
  sortDirection?: 'asc' | 'desc';
  
  onSortChange?: (columnKey: string, direction: 'asc' | 'desc') => void;
  
  onSearchChange?: (query: string) => void;
  
  exportLimit?: number;
  
  onExportAll?: (onProgress?: (done: number, total: number) => void) => Promise<T[]>;
};

export function ProTable<T extends Record<string, unknown>>({
  data,
  columns,
  onRefresh,
  searchableKeys,
  exportFileName = 'export',
  loading = false,
  itemsPerPage = 10,
  selectionMode = 'none',
  mobileCards = true,
  selectedRows: controlledSelectedRows,
  onSelectionChange,
  getRowId = (row: T) => String(row.id ?? Math.random()),
  enableExcelExport = true,
  enablePdfExport = true,
  serverTotal,
  page: controlledPage,
  pageSize,
  onPageChange,
  filters,
  sortBy,
  sortDirection,
  onSortChange,
  onSearchChange,
  exportLimit,
  onExportAll,
}: ProTableProps<T>) {
  const [query, setQuery] = useState('');
  const [activePage, setActivePage] = useState(1);
  
  const isMobile = useMediaQuery('(max-width: 48em)');
  const [showTable, setShowTable] = useState(false);
  const [exportState, setExportState] = useState<{ kind: 'excel' | 'pdf'; done: number; total: number } | null>(null);

  const useCards = mobileCards && isMobile;
  
  
  const [localSort, setLocalSort] = useState<{ key: string; dir: 'asc' | 'desc' } | null>(null);
  const [internalSelectedRows, setInternalSelectedRows] = useState<T[]>([]);
  
  const selectedRows = controlledSelectedRows !== undefined ? controlledSelectedRows : internalSelectedRows;
  
  const setSelectedRows = (rows: T[]) => {
    if (controlledSelectedRows === undefined) {
      setInternalSelectedRows(rows);
    }
    onSelectionChange?.(rows);
  };

  const serverMode = onPageChange !== undefined;

  const filtered = useMemo(() => {
    let rows = data;

    if (query.trim() && searchableKeys?.length) {
      const q = query.toLowerCase();
      rows = rows.filter((row) =>
        searchableKeys.some((k) => String(row[k] ?? '').toLowerCase().includes(q)),
      );
    }

    
    
    if (localSort && !onSortChange) {
      const { key, dir } = localSort;
      const factor = dir === 'asc' ? 1 : -1;
      rows = [...rows].sort((a, b) => {
        const av = a[key];
        const bv = b[key];
        if (typeof av === 'number' && typeof bv === 'number') {
          return (av - bv) * factor;
        }
        return String(av ?? '').localeCompare(String(bv ?? ''), 'fa') * factor;
      });
    }

    return rows;
  }, [data, query, searchableKeys, localSort, onSortChange]);

  
  const paginatedData = useMemo(() => {
    if (serverMode) {
      return filtered;
    }
    const start = (activePage - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    return filtered.slice(start, end);
  }, [filtered, activePage, itemsPerPage, serverMode]);

  
  
  

  const currentPage = serverMode ? (controlledPage ?? 1) : activePage;

  
  const effectivePageSize = serverMode ? (pageSize ?? itemsPerPage) : itemsPerPage;
  const totalPages = serverMode
    ? Math.ceil((serverTotal ?? 0) / effectivePageSize)
    : Math.ceil(filtered.length / effectivePageSize);

  useEffect(() => {
    if (!serverMode && activePage > totalPages && totalPages > 0) {
      setActivePage(1);
    }
  }, [totalPages, activePage, serverMode]);

  const handlePageChange = (next: number) => {
    if (serverMode) {
      onPageChange?.(next);
    } else {
      setActivePage(next);
    }
  };

  const resolveExportRows = async (kind: 'excel' | 'pdf'): Promise<T[]> => {
    if (!onExportAll) return filtered;
    setExportState({ kind, done: 0, total: 1 });
    try {
      return await onExportAll((done, total) => setExportState({ kind, done, total }));
    } finally {
      setExportState(null);
    }
  };

  const exportToExcel = async () => {
    const exportableColumns = columns.filter((col) => col.exportable !== false && col.key !== 'actions');
    
    const headers = exportableColumns.map((col) => col.title).join(',');
    const rows = (await resolveExportRows('excel')).map((row) =>
      exportableColumns
        .map((col) => {
          const value = col.exportValue
            ? col.exportValue(row)
            : col.render
              ? String(col.render(row)).replace(/,/g, ';').replace(/\[object Object\]/g, '')
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

  const exportToPDF = async () => {
    const exportableColumns = columns.filter((col) => col.exportable !== false && col.key !== 'actions');
    
    if (!vazirFontLoaded) {
      if (vazirFontPromise) {
        await vazirFontPromise;
      } else {
        vazirFontPromise = (async () => {
          try {
            const fontResponse = await fetch(vazirFontUrl);
            
            if (!fontResponse.ok) {
              throw new Error(`Font fetch failed: ${fontResponse.status} ${fontResponse.statusText}`);
            }
            
            const fontArrayBuffer = await fontResponse.arrayBuffer();
            const bytes = new Uint8Array(fontArrayBuffer);
            let binary = '';
            for (let i = 0; i < bytes.length; i++) {
              binary += String.fromCharCode(bytes[i]);
            }
            const fontBase64 = btoa(binary);
            
            vazirFontData = fontBase64;
            vazirFontLoaded = true;
          } catch (error) {
            vazirFontLoaded = false;
            vazirFontData = null;
            throw error;
          }
        })();
        
        await vazirFontPromise;
      }
    }
    const headers = exportableColumns.map((col) => col.title || '');
    const rows = (await resolveExportRows('pdf')).map((row) =>
      exportableColumns.map((col) => {
        try {
          if (col.exportValue) {
            const value = col.exportValue(row);
            return value !== null && value !== undefined ? String(value) : '';
          }
          if (col.render) {
            const rendered = col.render(row);
            
            const extractText = (element: React.ReactNode): string => {
              if (element === null || element === undefined) {
                return '';
              }

              if (typeof element === 'string' || typeof element === 'number') {
                return String(element);
              }

              if (typeof element === 'boolean') {
                return element ? 'بله' : 'خیر';
              }

              if (Array.isArray(element)) {
                return element.map(extractText).filter(Boolean).join(' ');
              }

              if (isValidElement(element)) {
                const props = element.props as {
                  children?: React.ReactNode;
                  label?: React.ReactNode;
                  value?: unknown;
                  text?: string;
                };

                if (props.children !== undefined && props.children !== null) {
                  return extractText(props.children);
                }

                if (props.label) {
                  return extractText(props.label);
                }

                if (props.value !== undefined) {
                  return extractText(props.value as React.ReactNode);
                }

                if (props.text) {
                  return extractText(props.text);
                }
              }

              return '';
            };
            
            return extractText(rendered).replace(/<[^>]*>/g, '');
          }
          const value = row[col.key as keyof T];
          return value !== null && value !== undefined ? String(value) : '';
        } catch {
          return '';
        }
      })
    );

    
    const sampleCap = Math.min(rows.length, 60);
    const widths = exportableColumns.map((_, ci) => {
      let longest = headers[ci].length;
      for (let ri = 0; ri < sampleCap; ri++) {
        const v = String(rows[ri][ci] ?? '');
        if (v.length > longest) longest = v.length;
      }
      return Math.min(Math.max(6 + longest * 1.7, 14), 75);
    });

    
    const totalWidth = widths.reduce((a, b) => a + b, 0);
    const landscape = totalWidth > 170; 
    const usable = landscape ? 273 : 170; 
    let finalWidths = widths;
    if (totalWidth > usable) {
      const k = usable / totalWidth;
      finalWidths = widths.map((w) => Math.max(w * k, 8));
    }

    const doc = new jsPDF({
      orientation: landscape ? 'landscape' : 'portrait',
      unit: 'mm',
      format: 'a4',
    });

    let useVazirFont = false;
    if (vazirFontLoaded && vazirFontData) {
      try {
        doc.addFileToVFS('Vazirmatn-Regular.ttf', vazirFontData);
        doc.addFont('Vazirmatn-Regular.ttf', 'Vazir', 'normal');
        doc.addFont('Vazirmatn-Regular.ttf', 'Vazir', 'bold');
        useVazirFont = true;
      } catch {
        useVazirFont = false;
      }
    }

    doc.setFontSize(16);
    if (useVazirFont) {
      doc.setFont('Vazir', 'normal');
    }
    doc.text(exportFileName, doc.internal.pageSize.getWidth() - 20, 20, {
      align: 'right',
    });

    const tableData = rows.map((row) => 
      row.map((cell) => cell || '')
    );

    autoTable(doc, {
      head: [headers],
      body: tableData,
      startY: 30,
      styles: {
        font: useVazirFont ? 'Vazir' : 'helvetica',
        fontSize: 9,
        halign: 'right',
        valign: 'middle',
        cellPadding: 1.6,
      },
      headStyles: {
        fillColor: [66, 139, 202],
        textColor: 255,
        fontStyle: 'bold',
        halign: 'right',
      },
      alternateRowStyles: {
        fillColor: [245, 247, 250],
      },
      columnStyles: Object.fromEntries(finalWidths.map((w, i) => [i, { cellWidth: w }])),
      margin: landscape
        ? { top: 30, right: 12, bottom: 12, left: 12 }
        : { top: 30, right: 20, bottom: 20, left: 20 },
      didParseCell: (data) => {
        data.cell.styles.halign = 'right';
      },
    });

    doc.save(`${exportFileName}.pdf`);
  };

  const isRowSelected = (row: T) => {
    const rowId = getRowId(row);
    return selectedRows.some((r) => getRowId(r) === rowId);
  };

  const handleRowSelection = (row: T) => {
    if (selectionMode === 'none') return;
    
    const rowId = getRowId(row);
    const isSelected = isRowSelected(row);
    
    if (selectionMode === 'single') {
      setSelectedRows(isSelected ? [] : [row]);
    } else if (selectionMode === 'multi') {
      if (isSelected) {
        setSelectedRows(selectedRows.filter((r) => getRowId(r) !== rowId));
      } else {
        setSelectedRows([...selectedRows, row]);
      }
    }
  };

  const handleSelectAll = () => {
    if (selectionMode !== 'multi') return;
    
    const allSelected = paginatedData.every((row) => isRowSelected(row));
    if (allSelected) {
      const rowIds = paginatedData.map((r) => getRowId(r));
      setSelectedRows(selectedRows.filter((r) => !rowIds.includes(getRowId(r))));
    } else {
      const newSelections = paginatedData.filter((row) => !isRowSelected(row));
      setSelectedRows([...selectedRows, ...newSelections]);
    }
  };

  const allSelected = selectionMode === 'multi' && paginatedData.length > 0 && paginatedData.every((row) => isRowSelected(row));
  const someSelected = selectionMode === 'multi' && paginatedData.some((row) => isRowSelected(row));


  
  const mapButtonVariant =
    (targetVariant: 'filled' | 'light') =>
    (node: React.ReactNode): React.ReactNode => {
      if (!isValidElement(node)) return node;

      const props = node.props as Record<string, unknown>;
      const children = props.children as React.ReactNode;

      
      const isButton =
        typeof props.variant === 'string' || typeof props.color === 'string';
      const hasChildren = Array.isArray(children);

      if (isButton && !hasChildren) {
        return cloneElement(node as React.ReactElement<Record<string, unknown>>, {
          variant: targetVariant,
        });
      }

      if (hasChildren) {
        
        
        return cloneElement(node as React.ReactElement<Record<string, unknown>>, {
          wrap: 'nowrap',
          children: (children as React.ReactNode[]).map(mapButtonVariant(targetVariant)),
        });
      }

      return node;
    };

  
  const cellContent = (col: ProTableColumn<T>, row: T): React.ReactNode => {
    if (String(col.key) === 'actions' && col.render) {
      const content = col.render(row);
      return (
        <Box className="pro-table-actions">
          {mapButtonVariant('filled')(content)}
        </Box>
      );
    }
    
    if (col.render) return mapButtonVariant('light')(col.render(row));
    const value = row[col.key as keyof T];
    return value === null || value === undefined || value === '' ? '—' : String(value);
  };

  
  const mobileCardsView = (
    <Stack gap="xs">
      {paginatedData.map((row) => {
        const rowId = getRowId(row);
        const isSelected = selectedRows.some((r) => getRowId(r) === rowId);
        return (
          <Paper key={rowId} withBorder p="sm" radius="md">
            <Group justify="space-between" align="flex-start" wrap="nowrap" gap="xs">
              {selectionMode !== 'none' && (
                <Checkbox
                  checked={isSelected}
                  onChange={() => handleRowSelection(row)}
                  aria-label="انتخاب ردیف"
                />
              )}
              <Stack gap="sm" style={{ flex: 1, minWidth: 0 }}>
                {columns.map((col) => {
                  const key = String(col.key);
                  const isAction = key === 'actions';
                  
                  
                  if (key === 'image' || key === 'avatar') return null;
                  return (
                    
                    
                    <Group key={key} gap="md" justify="space-between" wrap="nowrap">
                      <Text size="xs" c="dimmed" style={{ flexShrink: 0 }}>
                        {col.title}
                      </Text>
                      <Text
                        size="sm"
                        truncate
                        style={{
                          flex: 1,
                          minWidth: 0,
                          textAlign: 'left',
                          fontWeight: isAction ? 400 : 500,
                        }}
                      >
                        {cellContent(col, row)}
                      </Text>
                    </Group>
                  );
                })}
              </Stack>
            </Group>
          </Paper>
        );
      })}
      {paginatedData.length === 0 && (
        <Text size="sm" c="dimmed" ta="center" py="md">
          رکوردی یافت نشد
        </Text>
      )}
    </Stack>
  );

  
  const tableView = (
      <Box style={{ overflowX: 'auto', overflowY: 'hidden', maxWidth: '100%' }} hidden={useCards && !showTable}>
        {loading ? (
          <Center style={{ height: 200 }}>
            <Loader size="lg" />
          </Center>
        ) : (
          <Table
            striped
            highlightOnHover
            
            
            
            
            
            style={{
              
              
              
              
              
              
              
              
              
              
              width: '100%',
              tableLayout: 'auto',
            }}
          >
            <Table.Thead>
              <Table.Tr>
                {selectionMode !== 'none' && (
                  <Table.Th style={{ width: 44, textAlign: 'center', padding: '8px 6px' }}>
                    {selectionMode === 'multi' && (
                      <Checkbox
                        checked={allSelected}
                        indeterminate={someSelected && !allSelected}
                        onChange={handleSelectAll}
                      />
                  )}
                  </Table.Th>
              )}
                {columns.map((col) => {
                  const key = String(col.key);
                  const effectiveSort = onSortChange
                    ? { key: sortBy, dir: sortDirection }
                    : localSort;
                  const isSorted = effectiveSort?.key === key;
                  const currentDir = isSorted ? effectiveSort.dir : undefined;
                  const nextDir = isSorted && currentDir === 'asc' ? 'desc' : 'asc';
                  return (
                    <Table.Th
                      key={key}
                      className="pro-table-th"
                      style={{
                        textAlign: 'center',
                        padding: '8px 7px',
                        cursor: 'pointer',
                        userSelect: 'none',
                        whiteSpace: 'nowrap',
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                        maxWidth: 0,
                      }}
                      onClick={() => {
                        if (onSortChange) {
                          onSortChange(key, nextDir);
                        } else {
                          setLocalSort((prev) =>
                            prev?.key === key
                              ? { key, dir: prev.dir === 'asc' ? 'desc' : 'asc' }
                              : { key, dir: 'asc' },
                          );
                        }
                      }}
                    >
                      <Group gap={4} justify="center" wrap="nowrap">
                        <span>{col.title}</span>
                        {isSorted && (
                          <FaSortUp
                            size={12}
                            style={{
                              transform: currentDir === 'desc' ? 'rotate(180deg)' : 'none',
                              transition: 'transform 150ms ease',
                            }}
                          />
                      )}
                      </Group>
                    </Table.Th>
                  );
                })}
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {paginatedData.length === 0 ? (
                <Table.Tr>
                  <Table.Td colSpan={columns.length + (selectionMode !== 'none' ? 1 : 0)} style={{ textAlign: 'center' }}>
                    داده‌ای یافت نشد
                  </Table.Td>
                </Table.Tr>
              ) : (
                paginatedData.map((row, idx) => {
                  const rowSelected = isRowSelected(row);
                  return (
                    <Table.Tr
                      key={idx}
                      style={rowSelected ? { backgroundColor: 'var(--mantine-color-blue-0)' } : undefined}
                    >
                      {selectionMode !== 'none' && (
                        <Table.Td style={{ textAlign: 'center', padding: '8px 6px' }}>
                          {selectionMode === 'single' ? (
                            <Radio checked={rowSelected} onChange={() => handleRowSelection(row)} />
                          ) : (
                            <Checkbox checked={rowSelected} onChange={() => handleRowSelection(row)} />
                        )}
                        </Table.Td>
                    )}
                      {columns.map((col) => (
                        <Table.Td
                          key={String(col.key)}
                          style={{
                            textAlign: 'right',
                            padding: '8px 7px',
                            overflow: 'hidden',
                          }}
                        >
                          {String(col.key) === 'actions' ? (
                            
                            
                            cellContent(col, row)
                          ) : (
                            
                            
                            
                            <Box
                              style={{
                                display: 'block',
                                overflow: 'hidden',
                                textOverflow: 'ellipsis',
                                whiteSpace: 'nowrap',
                              }}
                            >
                              {cellContent(col, row)}
                            </Box>
                          )}
                        </Table.Td>
                      ))}
                    </Table.Tr>
                  );
                })
            )}
            </Table.Tbody>
          </Table>
      )}
      </Box>
  );

  return (
    <Box>
      <Group mb="sm" justify="space-between">
        <Group gap="xs">
          <TextInput
            placeholder="جستجو..."
            leftSection={<FaSearch size={14} />}
            value={query}
            onChange={(e) => {
              const value = e.currentTarget.value;
              setQuery(value);
              if (!serverMode) {
                setActivePage(1);
              } else {
                onPageChange?.(1);
              }
              
              
              onSearchChange?.(value);
            }}
            w="240px"
          />
          {filters}
        </Group>
        <Group gap="xs">
          {exportState && (
            <Group gap={6} bg="var(--mantine-color-blue-0)" px="sm" py={4} style={{ borderRadius: 8 }}>
              <Loader size="xs" color="blue" />
              <Text size="xs" c="blue">
                خروجی…{' '}
                {Math.min(100, Math.round((exportState.done / Math.max(1, exportState.total)) * 100))}٪
              </Text>
            </Group>
          )}
          {enableExcelExport && (
            <Tooltip label="خروجی Excel">
              <ActionIcon variant="light" color="green" onClick={exportToExcel}>
                <FaFileExcel />
              </ActionIcon>
            </Tooltip>
          )}
          {enablePdfExport && (
            <Tooltip label="خروجی PDF">
              <ActionIcon variant="light" color="red" onClick={exportToPDF}>
                <FaFilePdf />
              </ActionIcon>
            </Tooltip>
          )}
          {onRefresh && (
            <Tooltip label="بروزرسانی">
              <ActionIcon variant="light" onClick={onRefresh}>
                <FaSyncAlt />
              </ActionIcon>
            </Tooltip>
          )}
        </Group>
      </Group>

      {exportLimit !== undefined && (
        <Text size="xs" c="dimmed" mb="xs">
          خروجی حداکثر {exportLimit.toLocaleString('fa-IR')} رکورد را پشتیبانی می‌کند؛ برای خروجی کامل، جدول را فیلتر کنید.
        </Text>
      )}

      {}
      {useCards && (
        <Group justify="flex-end" mb="xs">
          <ActionIcon
            variant="light"
            size="lg"
            onClick={() => setShowTable((v) => !v)}
            aria-label={showTable ? 'نمایش کارتی' : 'نمایش جدول'}
            title={showTable ? 'نمایش کارتی' : 'نمایش جدول'}
          >
            {showTable ? <FaListUl /> : <FaTable />}
          </ActionIcon>
        </Group>
      )}

      {useCards && !showTable && !loading && mobileCardsView}

      {}
      {}
      {tableView}
      {totalPages > 1 && (
        <Group justify="center" mt="md">
          <Pagination
            value={currentPage}
            onChange={handlePageChange}
            total={totalPages}
            dir="rtl"
          />
        </Group>
      )}

    </Box>
  );
}
