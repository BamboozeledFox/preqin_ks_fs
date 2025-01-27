import { describe, it, expect, beforeAll, afterAll, afterEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { setupServer } from 'msw/node';
import { http, HttpResponse } from 'msw';
import InvestorSummaryComponent from './investorSummaryComponent';
import { BrowserRouter } from 'react-router';
import { MantineProvider } from '@mantine/core';

const mockData = [
  {
    id: '64823135-a9c9-af8f-7996-5c270ac80c56',
    name: 'Alice',
    type: 'asset manager',
    dateAdded: 'January 12, 2020',
    country: 'United Kingdom',
    totalCommitment: '45M'
  },
  {
    id: 'f64566da-2be2-f7f5-5974-dc7eed5fcd61',
    name: 'Bob',
    type: 'bank',
    dateAdded: 'March 07, 2022',
    country: 'United States',
    totalCommitment: '100M'
  }
];

const server = setupServer(
  http.get(`*/commitments/summaries`, () => {
    return HttpResponse.json(mockData);
  })
);

describe('InvestorSummaryComponent', () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  beforeAll(() => server.listen());

  afterEach(() => {
    queryClient.clear();
    server.resetHandlers();
  });

  afterAll(() => server.close());

  const renderComponent = () => {
    return render(
      <MantineProvider>
        <QueryClientProvider client={queryClient}>
          <BrowserRouter>
            <InvestorSummaryComponent />
          </BrowserRouter>
        </QueryClientProvider>
      </MantineProvider>
    );
  };

  it('should render the component with data successfully', async () => {
    renderComponent();

    // Verify title is rendered
    expect(await screen.findByText('Investors')).toBeTruthy();

    // Verify table headers are rendered
    expect(screen.getByText('Id')).toBeTruthy();
    expect(screen.getByText('Name')).toBeTruthy();
    expect(screen.getByText('Type')).toBeTruthy();
    expect(screen.getByText('Date Added')).toBeTruthy();
    expect(screen.getByText('Country')).toBeTruthy();
    expect(screen.getByText('Total Commitment')).toBeTruthy();

    // Verify data rows are rendered
    expect(screen.getByText('Alice')).toBeTruthy();
    expect(screen.getByText('asset manager')).toBeTruthy();
    expect(screen.getByText('United Kingdom')).toBeTruthy();
    expect(screen.getByText('45M')).toBeTruthy();

    expect(screen.getByText('Bob')).toBeTruthy();
    expect(screen.getByText('bank')).toBeTruthy();
    expect(screen.getByText('United Kingdom')).toBeTruthy();
    expect(screen.getByText('100M')).toBeTruthy();
  });

  it('should render links with correct URLs', async () => {
    renderComponent();

    await screen.findByText('Investors');

    const links = screen.getAllByRole('link');
    expect(links[0].getAttribute('href')).toBe('/64823135-a9c9-af8f-7996-5c270ac80c56');
    expect(links[1].getAttribute('href')).toBe('/f64566da-2be2-f7f5-5974-dc7eed5fcd61');
  });
});