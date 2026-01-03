'use client';

import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from '@/components/ui/select';
import {Locale} from '@/i18n/routing';
import {usePathname, useRouter} from '@/i18n/navigation';
import {useLocale, useTranslations} from 'next-intl';
import React from 'react';

const languages: {value: Locale; labelKey: 'en' | 'ru' | 'tj';}[] = [
  {value: 'en', labelKey: 'en'},
  {value: 'ru', labelKey: 'ru'},
  {value: 'tj', labelKey: 'tj'}
];

export function LanguageSwitcher() {
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const t = useTranslations('LanguageSelector');

  const handleChange = (value: Locale) => {
    router.replace(pathname, {locale: value});
  };

  return (
    <div className="w-full max-w-xs space-y-2">
      <div className="text-left text-sm font-medium text-gray-700">
        {t('label')}
      </div>
      <Select value={locale} onValueChange={handleChange}>
        <SelectTrigger aria-label={t('label')}>
          <SelectValue placeholder={t('placeholder')} />
        </SelectTrigger>
        <SelectContent>
          {languages.map((language) => (
            <SelectItem key={language.value} value={language.value}>
              {t(language.labelKey)}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
