import {LanguageSwitcher} from '@/components/language-switcher';
import {getTranslations} from 'next-intl/server';
import React from 'react';

const Page = async () => {
  const t = await getTranslations('Home');

  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-6 p-6 text-center">
      <div className="space-y-3">
        <h1 className="text-3xl font-semibold md:text-4xl">{t('title')}</h1>
        <p className="text-lg text-gray-600 md:text-xl">{t('description')}</p>
      </div>
      <LanguageSwitcher />
    </main>
  );
};

export default Page;
