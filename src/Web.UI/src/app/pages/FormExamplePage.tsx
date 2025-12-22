import { useState } from 'react';
import { Button, Card, Stack, Title, TextInput, Select, Checkbox, NumberInput, Group, Textarea, MultiSelect, FileInput, JsonInput, Divider } from '@mantine/core';
import { useForm, isNotEmpty, isEmail, hasLength } from '@mantine/form';
import { PersianDatePicker } from '../components/common/PersianDatePicker';
import { DatePickerTest } from '../components/common/DatePickerTest';

interface ComplexFormValues {
  // Personal Info
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  age: number;
  gender: string;
  bio: string;

  // Preferences
  notifications: boolean;
  theme: string;
  language: string;

  // Advanced
  skills: string[];
  experience: number;
  salary: number;
  availableFrom: Date | null;

  // Files & JSON
  resume: File | null;
  settings: string;

  // Dynamic
  addresses: Array<{
    street: string;
    city: string;
    zipCode: string;
  }>;
}

export default function FormExamplePage() {
  const [submittedData, setSubmittedData] = useState<ComplexFormValues | null>(null);

  const form = useForm<ComplexFormValues>({
    initialValues: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      age: 25,
      gender: '',
      bio: '',
      notifications: true,
      theme: 'light',
      language: 'fa',
      skills: [],
      experience: 0,
      salary: 0,
      availableFrom: null,
      resume: null,
      settings: JSON.stringify({ autoSave: true, showHints: false }, null, 2),
      addresses: [{ street: '', city: '', zipCode: '' }],
    },
    validate: {
      firstName: hasLength({ min: 2 }, 'نام باید حداقل ۲ کاراکتر باشد'),
      lastName: hasLength({ min: 2 }, 'نام خانوادگی باید حداقل ۲ کاراکتر باشد'),
      email: isEmail('ایمیل معتبر نیست'),
      phone: (value) => {
        if (!value) return 'شماره تلفن الزامی است';
        if (!/^09\d{9}$/.test(value)) return 'شماره تلفن باید با ۰۹ شروع شود و ۱۱ رقم باشد';
        return null;
      },
      age: (value) => {
        if (value < 18) return 'سن باید حداقل ۱۸ باشد';
        if (value > 120) return 'سن نمی‌تواند بیش از ۱۲۰ باشد';
        return null;
      },
      gender: isNotEmpty('جنسیت الزامی است'),
      salary: (value) => {
        if (value < 0) return 'حقوق نمی‌تواند منفی باشد';
        return null;
      },
      addresses: {
        street: isNotEmpty('خیابان الزامی است'),
        city: isNotEmpty('شهر الزامی است'),
        zipCode: (value) => {
          if (!value) return 'کد پستی الزامی است';
          if (!/^\d{10}$/.test(value)) return 'کد پستی باید ۱۰ رقم باشد';
          return null;
        },
      },
    },
  });

  const handleSubmit = (values: ComplexFormValues) => {
    console.log('Form submitted:', values);
    setSubmittedData(values);
  };

  const addAddress = () => {
    form.insertListItem('addresses', { street: '', city: '', zipCode: '' });
  };

  const removeAddress = (index: number) => {
    form.removeListItem('addresses', index);
  };

  return (
    <Stack p="md" maw={800} mx="auto">
      <Title order={2} ta="center" mb="lg">
        نمونه فرم پیشرفته با @mantine/form
      </Title>

      <Card withBorder shadow="sm" p="lg">
        <form onSubmit={form.onSubmit(handleSubmit)}>
          <Stack>
            {/* Personal Information */}
            <Title order={4}>اطلاعات شخصی</Title>

            <Group grow>
              <TextInput
                label="نام"
                placeholder="نام خود را وارد کنید"
                required
                {...form.getInputProps('firstName')}
              />

              <TextInput
                label="نام خانوادگی"
                placeholder="نام خانوادگی خود را وارد کنید"
                required
                {...form.getInputProps('lastName')}
              />
            </Group>

            <Group grow>
              <TextInput
                label="ایمیل"
                placeholder="ایمیل خود را وارد کنید"
                required
                {...form.getInputProps('email')}
              />

              <TextInput
                label="شماره تلفن"
                placeholder="مثال: 09123456789"
                required
                {...form.getInputProps('phone')}
              />
            </Group>

            <Group grow>
              <NumberInput
                label="سن"
                placeholder="سن خود را وارد کنید"
                min={18}
                max={120}
                required
                {...form.getInputProps('age')}
              />

              <Select
                label="جنسیت"
                placeholder="جنسیت خود را انتخاب کنید"
                data={[
                  { value: 'male', label: 'مرد' },
                  { value: 'female', label: 'زن' },
                  { value: 'other', label: 'سایر' },
                ]}
                required
                {...form.getInputProps('gender')}
              />
            </Group>

            <Textarea
              label="بیوگرافی"
              placeholder="درباره خود بنویسید..."
              minRows={3}
              {...form.getInputProps('bio')}
            />

            {/* Preferences */}
            <Title order={4}>تنظیمات</Title>

            <Group>
              <Checkbox
                label="دریافت اعلان‌ها"
                {...form.getInputProps('notifications', { type: 'checkbox' })}
              />

              <Select
                label="تم"
                data={[
                  { value: 'light', label: 'روشن' },
                  { value: 'dark', label: 'تیره' },
                  { value: 'auto', label: 'خودکار' },
                ]}
                {...form.getInputProps('theme')}
              />

              <Select
                label="زبان"
                data={[
                  { value: 'fa', label: 'فارسی' },
                  { value: 'en', label: 'English' },
                ]}
                {...form.getInputProps('language')}
              />
            </Group>

            {/* Professional Info */}
            <Title order={4}>اطلاعات حرفه‌ای</Title>

            <MultiSelect
              label="مهارت‌ها"
              placeholder="مهارت‌های خود را انتخاب کنید"
              data={[
                { value: 'react', label: 'React' },
                { value: 'typescript', label: 'TypeScript' },
                { value: 'nodejs', label: 'Node.js' },
                { value: 'python', label: 'Python' },
                { value: 'docker', label: 'Docker' },
              ]}
              {...form.getInputProps('skills')}
            />

            <Group grow>
              <NumberInput
                label="سال‌های تجربه"
                placeholder="سال‌های تجربه کاری"
                min={0}
                {...form.getInputProps('experience')}
              />

              <NumberInput
                label="حقوق مورد انتظار (تومان)"
                placeholder="حقوق مورد انتظار"
                min={0}
                step={1000000}
                {...form.getInputProps('salary')}
              />
            </Group>

            <PersianDatePicker
              label="تاریخ شروع همکاری"
              placeholder="تاریخ شروع همکاری خود را انتخاب کنید"
              value={form.values.availableFrom}
              onChange={(date) => form.setFieldValue('availableFrom', date)}
              error={typeof form.errors.availableFrom === 'string' ? form.errors.availableFrom : undefined}
            />

            {/* File Upload */}
            <Title order={4}>فایل‌ها</Title>

            <FileInput
              label="رزومه"
              placeholder="فایل رزومه خود را انتخاب کنید"
              accept=".pdf,.doc,.docx"
              {...form.getInputProps('resume')}
            />

            <JsonInput
              label="تنظیمات پیشرفته"
              placeholder="تنظیمات پیشرفته (JSON)"
              minRows={4}
              {...form.getInputProps('settings')}
            />

            {/* Dynamic Addresses */}
            <Title order={4}>آدرس‌ها</Title>

            {form.values.addresses.map((_, index) => (
              <Card key={index} withBorder p="sm">
                <Group justify="space-between" mb="sm">
                  <Title order={5}>آدرس {index + 1}</Title>
                  {form.values.addresses.length > 1 && (
                    <Button
                      size="xs"
                      color="red"
                      variant="light"
                      onClick={() => removeAddress(index)}
                    >
                      حذف
                    </Button>
                  )}
                </Group>

                <Stack>
                  <TextInput
                    label="خیابان"
                    placeholder="آدرس خیابان"
                    required
                    {...form.getInputProps(`addresses.${index}.street`)}
                  />

                  <Group grow>
                    <TextInput
                      label="شهر"
                      placeholder="نام شهر"
                      required
                      {...form.getInputProps(`addresses.${index}.city`)}
                    />

                    <TextInput
                      label="کد پستی"
                      placeholder="۱۰ رقم"
                      required
                      {...form.getInputProps(`addresses.${index}.zipCode`)}
                    />
                  </Group>
                </Stack>
              </Card>
            ))}

            <Button variant="light" onClick={addAddress}>
              افزودن آدرس جدید
            </Button>

            {/* Submit */}
            <Group mt="xl">
              <Button type="submit" size="lg">ارسال فرم</Button>
              <Button variant="light" onClick={() => form.reset()}>
                ریست فرم
              </Button>
            </Group>
          </Stack>
        </form>
      </Card>

      <Divider my="xl" />

      {/* Persian DatePicker Test Component */}
      <DatePickerTest />

      {/* Display submitted data */}
      {submittedData && (
        <Card withBorder>
          <Title order={4} mb="md">داده‌های ارسال شده:</Title>
          <pre style={{ fontSize: '12px', overflow: 'auto' }}>
            {JSON.stringify(submittedData, null, 2)}
          </pre>
        </Card>
      )}
    </Stack>
  );
}
