import {getMessages} from 'next-intl/server';
import Providers from '@/src/app/providers/providers';
import Header from "@/src/components/layout/header";

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
      <Header />
      {children}
    </Providers>
    </body>
    </html>
  );
}
