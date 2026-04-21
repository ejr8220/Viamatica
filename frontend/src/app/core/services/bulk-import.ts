import { Injectable } from '@angular/core';
import { read, utils } from 'xlsx';

export interface ParsedBulkFile {
  headers: string[];
  rows: Record<string, string>[];
}

@Injectable({
  providedIn: 'root',
})
export class BulkImportService {
  async parseFile(file: File): Promise<ParsedBulkFile> {
    const buffer = await file.arrayBuffer();
    const workbook = read(buffer, { type: 'array' });
    const firstSheetName = workbook.SheetNames[0];

    if (!firstSheetName) {
      throw new Error('El archivo no contiene hojas para procesar.');
    }

    const worksheet = workbook.Sheets[firstSheetName];
    const rawRows = utils.sheet_to_json<Record<string, unknown>>(worksheet, { defval: '' });

    if (rawRows.length === 0) {
      throw new Error('El archivo no contiene registros para importar.');
    }

    const rows = rawRows.map((row) =>
      Object.fromEntries(
        Object.entries(row).map(([key, value]) => [this.normalizeHeader(key), String(value ?? '').trim()])
      )
    );

    return {
      headers: Object.keys(rows[0] ?? {}),
      rows,
    };
  }

  normalizeHeader(header: string): string {
    return header
      .normalize('NFD')
      .replace(/[̀-ͯ]/g, '')
      .replace(/[^a-zA-Z0-9]+/g, '_')
      .replace(/^_+|_+$/g, '')
      .toLowerCase();
  }
}
