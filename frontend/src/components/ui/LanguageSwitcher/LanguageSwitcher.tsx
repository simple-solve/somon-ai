'use client';

import {useRouter} from 'next/navigation';
import React from 'react';

const LanguageSwitcher = () => {
  const router = useRouter();

  const changeLocale = (locale: string) => {
    document.cookie = `locale=${locale}; path=/; max-age=31536000`; // 1 год
    router.refresh();
  };

  return (
    <div>
      <button onClick={() => changeLocale('en')}>EN</button>
      <button onClick={() => changeLocale('ru')}>RU</button>
      <button onClick={() => changeLocale('tj')}>TJ</button>
    </div>
  );
};

export default LanguageSwitcher;
