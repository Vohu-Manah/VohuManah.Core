function isPersianChar(char: string): boolean {
  const code = char.charCodeAt(0);
  return code >= 0x0600 && code <= 0x06ff;
}

function isEnglishOrNumber(char: string): boolean {
  const code = char.charCodeAt(0);
  return (code >= 0x0020 && code <= 0x007f) || (code >= 0x0030 && code <= 0x0039);
}

export function detectTextDirection(text: string | null | undefined): 'rtl' | 'ltr' {
  if (!text || text.trim().length === 0) {
    return 'rtl';
  }

  let persianCount = 0;
  let englishCount = 0;

  for (let i = 0; i < text.length; i++) {
    const char = text[i];
    if (isPersianChar(char)) {
      persianCount++;
    } else if (isEnglishOrNumber(char)) {
      englishCount++;
    }
  }

  return persianCount > englishCount ? 'rtl' : 'ltr';
}

export function shouldBeLTR(fieldName: string): boolean {
  const ltrFields = [
    'isbn',
    'no',
    'number',
    'code',
    'id',
    'email',
    'username',
    'password',
    'url',
    'phone',
    'mobile',
    'year',
    'publishYear',
    'journalNo',
    'versionNo',
    'abbreviation',
    'bookShelfRow',
  ];

  const lowerFieldName = fieldName.toLowerCase();
  return ltrFields.some((field) => lowerFieldName.includes(field));
}

export function getInputDirection(
  fieldName: string,
  value: string | null | undefined,
): 'rtl' | 'ltr' {
  if (shouldBeLTR(fieldName)) {
    return 'ltr';
  }

  return detectTextDirection(value);
}
