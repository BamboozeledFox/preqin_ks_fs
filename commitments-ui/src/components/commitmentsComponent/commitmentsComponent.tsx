import getApiClient from "../../client/getApiClient"
import { Card, Center, FloatingIndicator, Loader, Table, Text, Title, UnstyledButton } from "@mantine/core"
import { useQuery } from "@tanstack/react-query"
import { useState } from "react"
import { useParams } from "react-router"
import classes from './commitmentComponent.module.css'

interface InvestorCommitments {
    assetSummaries: AssetSummary[],
    commitments: Commitment[]
}

interface AssetSummary {
    assetClass: AssetClass,
    amount: string
}

type AssetClass = 'All' | 'Infrastructure' | 'Hedge Funds' | 'Private Equity' | 'Natural Resources' | 'Real Estate' | 'Private Debt';

interface Commitment {
    assetClass: AssetClass,
    currency: string,
    amount: string
}


function CommitmentsComponent() {
    const { investorId } = useParams();

    const [rootRef, setRootRef] = useState<HTMLDivElement | null>(null);
    const [controlsRefs, setControlsRefs] = useState<Record<string, HTMLButtonElement | null>>({});
    const [assetClass, setAssetClass] = useState<AssetClass>('All');

    const { isPending, error, data } = useQuery({
        queryKey: ['investorCommitments', investorId],
        queryFn: async () => {
            const client = getApiClient();
            const result = await client.get<InvestorCommitments>(`commitments/investors/${investorId}`);
            return result.data;
        },
        select(data) {
            if (assetClass === 'All') {
                return data;
            }

            return {
                assetSummaries: data.assetSummaries,
                commitments: data.commitments.filter(el => el.assetClass === assetClass)
            }
        },
    });

    const setControlRef = (index: number) => (node: HTMLButtonElement) => {
        controlsRefs[index] = node;
        setControlsRefs(controlsRefs);
    };

    const controls = data?.assetSummaries.map((item, index) => (
        <UnstyledButton
            key={item.assetClass}
            className={classes.control}
            ref={setControlRef(index)}
            onClick={() => setAssetClass(item.assetClass)}
            mod={{ active: item.assetClass === assetClass }}
        >
            <Text className={classes.controlLabel}>{`${item.assetClass} ${item.amount}`}</Text>
        </UnstyledButton>
    ));


    return (
        isPending
            ? <Loader />
            : error === null
                ? <Card padding='xs' shadow='xs'>
                    <Card.Section>
                        <Title>Commitments</Title>
                        <Center>
                            <div className={classes.root} ref={setRootRef}>
                                {controls}

                                <FloatingIndicator
                                    target={controlsRefs[assetClass]}
                                    parent={rootRef}
                                />
                            </div>
                        </Center>
                    </Card.Section>

                    <Table striped >
                        <Table.Thead>
                            <Table.Tr>
                                <Table.Th>Asset Class</Table.Th>
                                <Table.Th>Currency</Table.Th>
                                <Table.Th>Amount</Table.Th>
                            </Table.Tr>
                        </Table.Thead>
                        <Table.Tbody>
                            {data.commitments.map((el, i) => (
                                <Table.Tr key={i}>
                                    <Table.Td>{el.assetClass}</Table.Td>
                                    <Table.Td>{el.currency}</Table.Td>
                                    <Table.Td>{el.amount}</Table.Td>
                                </Table.Tr>
                            ))}
                        </Table.Tbody>
                    </Table>
                </Card>
                : <p>Error: {error.message} </p>
    );
}

export default CommitmentsComponent;