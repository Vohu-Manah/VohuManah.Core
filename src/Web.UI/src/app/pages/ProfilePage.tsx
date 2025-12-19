import { useState } from 'react';
import { Button, Card, Stack, Text, Title, TextInput, Group, Select, Checkbox, NumberInput } from '@mantine/core';
import { DatePickerInput } from '@mantine/dates';
import { useForm, isNotEmpty, isEmail, hasLength } from '@mantine/form';
import { useAuth } from '../context/AuthContext';

interface ProfileFormValues {
  fullName: string;
  email: string;
  phone: string;
  age: number;
  gender: string;
  bio: string;
  isActive: boolean;
  birthDate: Date | null;
}

export default function ProfilePage() {
  const { user, logout } = useAuth();
  const [isEditing, setIsEditing] = useState(false);

  const form = useForm<ProfileFormValues>({
    initialValues: {
      fullName: user?.fullName ?? '',
      email: '',
      phone: '',
      age: 25,
      gender: '',
      bio: '',
      isActive: true,
      birthDate: null,
    },
    validate: {
      fullName: hasLength({ min: 2 }, 'نام باید حداقل ۲ کاراکتر باشد'),
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
    },
  });

  const handleSubmit = (values: ProfileFormValues) => {
    console.log('Profile updated:', values);
    // اینجا می‌توانید API call برای ذخیره پروفایل اضافه کنید
    setIsEditing(false);
  };

  const handleReset = () => {
    form.reset();
    setIsEditing(false);
  };

  if (isEditing) {
    return (
      <Card withBorder>
        <Stack>
          <Title order={4}>ویرایش پروفایل</Title>

          <form onSubmit={form.onSubmit(handleSubmit)}>
            <Stack>
              <TextInput
                label="نام و نام خانوادگی"
                placeholder="نام کامل خود را وارد کنید"
                required
                {...form.getInputProps('fullName')}
              />

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

              <DatePickerInput
                label="تاریخ تولد"
                placeholder="تاریخ تولد خود را انتخاب کنید"
                {...form.getInputProps('birthDate')}
              />

              <TextInput
                label="بیوگرافی"
                placeholder="درباره خود بنویسید..."
                {...form.getInputProps('bio')}
              />

              <Checkbox
                label="حساب فعال است"
                {...form.getInputProps('isActive', { type: 'checkbox' })}
              />

              <Group>
                <Button type="submit">ذخیره تغییرات</Button>
                <Button variant="light" onClick={handleReset}>لغو</Button>
              </Group>
            </Stack>
          </form>
        </Stack>
      </Card>
    );
  }

  return (
    <Card withBorder>
      <Stack>
        <Group justify="space-between">
          <Title order={4}>پروفایل کاربر</Title>
          <Button onClick={() => setIsEditing(true)}>ویرایش</Button>
        </Group>

        <Text>نام: {user?.fullName ?? '-'}</Text>
        <Text>نام کاربری: {user?.userName ?? '-'}</Text>

        <Button color="red" variant="light" onClick={() => logout()}>
          خروج
        </Button>
      </Stack>
    </Card>
  );
}

