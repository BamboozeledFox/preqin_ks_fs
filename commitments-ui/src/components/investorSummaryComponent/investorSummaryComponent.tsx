import { Card, Loader, Table, Title } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import getApiClient from '../../client/getApiClient';
import { NavLink } from 'react-router';

interface InvestorCommitmentSummary {
    id: string,
    name: string,
    type: string,
    dateAdded: string,
    country: string,
    totalCommitment: string
};

function InvestorSummaryComponent() {
    const { isPending, error, data } = useQuery({
        queryKey: ['summaries'],
        queryFn: async () => {
            const client = getApiClient();
            const response = await client.get<InvestorCommitmentSummary[]>('/commitments/summaries');
            return response.data;
        }
    });

    return (
        isPending
            ? <Loader /> 
            : error === null
                ? <Card padding='xs' shadow='xs'>
                    <Title >Investors</Title>
                    <Table striped >
                        <Table.Thead>
                            <Table.Tr>
                                <Table.Th>Id</Table.Th>
                                <Table.Th>Name</Table.Th>
                                <Table.Th>Type</Table.Th>
                                <Table.Th>Date Added</Table.Th>
                                <Table.Th>Country</Table.Th>
                                <Table.Th>Total Commitment</Table.Th>
                            </Table.Tr>
                        </Table.Thead>
                        <Table.Tbody>
                            {data.map(el => (
                                <Table.Tr key={el.id}>
                                    <Table.Td>{el.id}</Table.Td>
                                    <Table.Td>{el.name}</Table.Td>
                                    <Table.Td>{el.type}</Table.Td>
                                    <Table.Td>{el.dateAdded}</Table.Td>
                                    <Table.Td>{el.country}</Table.Td>
                                    <Table.Td><NavLink to={`/${el.id}`} >{el.totalCommitment}</NavLink></Table.Td>
                                </Table.Tr>
                            ))}
                        </Table.Tbody>
                    </Table>
                </Card>
                : <p>Error: {error.message}</p>
    );
}

export default InvestorSummaryComponent;