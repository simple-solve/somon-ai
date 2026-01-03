'use client';

import {ReactNode} from 'react';
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {AbstractIntlMessages, NextIntlClientProvider} from 'next-intl';
import {MantineProvider} from "@mantine/core";
import '@mantine/core/styles.css';

const queryClient = new QueryClient();

type Props = {
  children: ReactNode;
  locale: string;
  messages: AbstractIntlMessages;
};


export default function Providers({children, locale, messages}: Props) {
  return (
    <NextIntlClientProvider locale={locale} messages={messages}>
      <QueryClientProvider client={queryClient}>
        <MantineProvider>
          {children}
        </MantineProvider>
      </QueryClientProvider>
    </NextIntlClientProvider>
  );
}
