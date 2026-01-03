import {Pathnames} from 'next-intl/routing';

export const routing = {
  locales: ['en', 'ru', 'tj'],
  defaultLocale: 'en',
  localePrefix: 'never'
} satisfies Pathnames<readonly ['en', 'ru', 'tj']>;

export type Locale = (typeof routing.locales)[number];
