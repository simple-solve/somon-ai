'use client'

import React, {ReactNode} from 'react'
import {QueryClient} from "@tanstack/query-core";
import {QueryClientProvider} from "@tanstack/react-query";

const queryClient = new QueryClient()
const Providers = ({children}: { children: ReactNode }) => {

  return (
    <>
        <QueryClientProvider client={queryClient}>
          {children}
        </QueryClientProvider>
    </>
  )
}
export default Providers
