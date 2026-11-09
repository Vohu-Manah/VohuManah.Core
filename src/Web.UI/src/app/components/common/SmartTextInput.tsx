import { TextInput, type TextInputProps } from '@mantine/core';
import { detectTextDirection } from '@/app/helpers/textDirection';
import { useMemo } from 'react';

export interface SmartTextInputProps extends Omit<TextInputProps, 'dir'> {
  fieldName?: string;
}

export function SmartTextInput({ fieldName, value, ...props }: SmartTextInputProps) {
  const direction = useMemo(() => {
    const textValue = (value as string) || '';

    if (!textValue || textValue.trim().length === 0) {
      return 'rtl';
    }

    return detectTextDirection(textValue);
  }, [value]);

  return (
    <div data-smart-input-direction={direction} className="smart-textinput">
      <TextInput
        {...props}
        name={fieldName}
        value={value}
        dir={direction}
        classNames={{
          wrapper: 'smart-textinput__wrapper',
          input: 'smart-textinput__field',
          label: 'smart-textinput__label',
        }}
      />
    </div>
  );
}
