import {getMessages} from 'next-intl/server';
import Providers from '@/src/app/providers/providers';

export default async function RootLayout({
                                           children
                                         }: {
  children: React.ReactNode;
}) {
  const locale = 'ru';
  const messages = await getMessages();


  return (
    <html lang={locale}>
    <body>
    <Providers locale={locale} messages={messages}>
      {children}
    </Providers>
    </body>
    </html>
  );
}
