import { describe, it, expect, beforeAll, afterAll, afterEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { setupServer } from 'msw/node';
import { http, HttpResponse } from 'msw';
import { MantineProvider } from '@mantine/core';
import CommitmentsComponent from './commitmentsComponent';
import { MemoryRouter } from 'react-router';

const mockData = {
  assetSummaries: [
    { assetClass: 'All', amount: '145M' },
    { assetClass: 'Infrastructure', amount: '45M' },
    { assetClass: 'Private Equity', amount: '100M' }
  ],
  commitments: [
    { 
      assetClass: 'Infrastructure', 
      currency: 'GBP', 
      amount: '45M'
    },
    { 
      assetClass: 'Private Equity', 
      currency: 'GBP', 
      amount: '100M'
    }
  ]
};

const INVESTOR_ID = '64823135-a9c9-af8f-7996-5c270ac80c56';

const server = setupServer(
  http.get(`*/commitments/investors/:investor_id`, () => {
    return HttpResponse.json(mockData);
  })
);

describe('CommitmentsComponent', () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  beforeAll(() => {
    server.listen({ onUnhandledRequest: 'error' });
  });

  afterEach(() => {
    queryClient.clear();
    server.resetHandlers();
  });

  afterAll(() => server.close());

  const renderComponent = () => {
    return render(
      <MantineProvider>
        <MemoryRouter initialEntries={[`/${INVESTOR_ID}`]}>
          <QueryClientProvider client={queryClient}>
            <CommitmentsComponent />
          </QueryClientProvider>
        </MemoryRouter>
      </MantineProvider>
    );
  };

  it('should render the component with initial data', async () => {
    renderComponent();

    // Wait for data to load
    expect(await screen.findByText('Commitments')).toBeTruthy();

    // Verify asset class controls are rendered
    expect(screen.getByText('All 145M')).toBeTruthy();
    expect(screen.getByText('Infrastructure 45M')).toBeTruthy();
    expect(screen.getByText('Private Equity 100M')).toBeTruthy();

    // Verify table headers
    expect(screen.getByText('Asset Class')).toBeTruthy();
    expect(screen.getByText('Currency')).toBeTruthy();
    expect(screen.getByText('Amount')).toBeTruthy();

    // Verify initial data rows
    expect(screen.getByText('Infrastructure')).toBeTruthy();
    expect(screen.getByText('45M')).toBeTruthy();
    expect(screen.getByText('Private Equity')).toBeTruthy();
    expect(screen.getByText('100M')).toBeTruthy();
  });

  it('should filter commitments when selecting an asset class', async () => {
    renderComponent();

    // Wait for data to load
    await screen.findByText('Commitments');

    // Click on Infrastructure filter
    fireEvent.click(screen.getByText('Infrastructure 45M'));

    // Should only show Infrastructure commitment
    expect(screen.getByText('Infrastructure')).toBeTruthy();
    expect(screen.getByText('45M')).toBeTruthy();

    // Private Equity commitment should not be visible
    expect(screen.queryByText('100M')).toBeNull();
  });

  it('should show all commitments when "All" is selected', async () => {
    renderComponent();

    // Wait for data to load
    await screen.findByText('Commitments');

    // First click Infrastructure to filter
    fireEvent.click(screen.getByText('Infrastructure 45M'));

    // Then click All to show everything
    fireEvent.click(screen.getByText('All 145M'));

    // Should show all commitments
    expect(screen.getByText('Infrastructure')).toBeTruthy();
    expect(screen.getByText('45M')).toBeTruthy();
    expect(screen.getByText('Private Equity')).toBeTruthy();
    expect(screen.getByText('100M')).toBeTruthy();
  });

  it('should maintain selected filter after data refresh', async () => {
    renderComponent();

    // Wait for initial data load
    await screen.findByText('Commitments');

    // Select Infrastructure filter
    fireEvent.click(screen.getByText('Infrastructure 45M'));

    // Trigger a refresh
    queryClient.invalidateQueries({ queryKey: ['investorCommitments'] });

    // Wait for refresh to complete and verify filter is maintained
    await screen.findByText('Infrastructure');
    expect(screen.queryByText('100M')).toBeNull();
  });
});