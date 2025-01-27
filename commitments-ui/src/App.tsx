import { MantineProvider } from '@mantine/core';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import InvestorSummaryComponent from './components/investorSummaryComponent/investorSummaryComponent';
import './App.css'
import '@mantine/core/styles.css';
import { BrowserRouter, Route, Routes } from 'react-router';
import CommitmentsComponent from './components/commitmentsComponent/commitmentsComponent';

function App() {
  const queryClient = new QueryClient();

  return (
    <MantineProvider defaultColorScheme='light' >
      <QueryClientProvider client={queryClient} >
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<InvestorSummaryComponent />} />
            <Route path=':investorId' element={<CommitmentsComponent/>} />
          </Routes>
        </BrowserRouter>
      </QueryClientProvider>
    </MantineProvider>
  )
}

export default App
