import { TextInput, type TextInputProps } from '@mantine/core';
import { detectTextDirection } from '@/app/helpers/textDirection';
import { useMemo, type CSSProperties } from 'react';

export interface SmartTextInputProps extends Omit<TextInputProps, 'dir'> {
  fieldName?: string;
}

type SmartInputStyleNames = 'wrapper' | 'input' | 'label';

export function SmartTextInput({ fieldName, value, ...props }: SmartTextInputProps) {
  const direction = useMemo(() => {
    const textValue = (value as string) || '';

    if (!textValue || textValue.trim().length === 0) {
      return 'rtl';
    }

    return detectTextDirection(textValue);
  }, [value]);

  
  const baseStyles: Partial<Record<SmartInputStyleNames, CSSProperties>> =
    typeof props.styles === 'object' && props.styles !== null ? props.styles : {};

  const sectionBox: CSSProperties = {
    position: 'absolute',
    top: 0,
    bottom: 0,
    margin: 0,
    padding: 0,
    width: '2.5rem',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  };

  return (
    <div
      data-smart-input-direction={direction}
      style={{
        direction: 'rtl',
        '--icon-position': direction === 'ltr' ? 'left' : 'right',
        '--icon-left': direction === 'ltr' ? '0' : 'auto',
        '--icon-right': direction === 'ltr' ? 'auto' : '0',
      } as CSSProperties}
    >
      <TextInput
        {...props}
        name={fieldName}
        value={value}
        dir={direction}
        leftSectionProps={{
          ...props.leftSectionProps,
          style: {
            ...sectionBox,
            left: direction === 'ltr' ? 0 : 'auto',
            right: direction === 'ltr' ? 'auto' : 0,
            ...props.leftSectionProps?.style,
          },
        }}
        rightSectionProps={{
          ...props.rightSectionProps,
          style: {
            ...sectionBox,
            left: direction === 'ltr' ? 'auto' : 0,
            right: direction === 'ltr' ? 0 : 'auto',
            ...props.rightSectionProps?.style,
          },
        }}
        styles={{
          ...baseStyles,
          wrapper: {
            ...baseStyles.wrapper,
            direction: 'rtl',
            position: 'relative',
          },
          input: {
            ...baseStyles.input,
            direction,
            textAlign: direction === 'rtl' ? 'right' : 'left',
            ...(props.rightSection && (direction === 'rtl'
              ? {
                  paddingRight: 'calc(var(--mantine-spacing-md) + 2.5rem)',
                  paddingLeft: 'var(--mantine-spacing-sm)',
                }
              : {
                  paddingLeft: 'calc(var(--mantine-spacing-md) + 2.5rem)',
                  paddingRight: 'var(--mantine-spacing-sm)',
                }
            )),
            ...(props.leftSection && (direction === 'rtl'
              ? {
                  paddingLeft: 'calc(var(--mantine-spacing-md) + 2.5rem)',
                  paddingRight: 'var(--mantine-spacing-sm)',
                }
              : {
                  paddingRight: 'calc(var(--mantine-spacing-md) + 2.5rem)',
                  paddingLeft: 'var(--mantine-spacing-sm)',
                }
            )),
          },
          label: {
            ...baseStyles.label,
            direction: 'rtl',
            textAlign: 'right',
          },
        }}
      />
    </div>
  );
}
