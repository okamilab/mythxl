import { handleError } from '../api';

export async function fetchContracts(client, next) {
    try {
        const { data } = await client.get(`/api/v0.1/contracts?t=${next || ''}`);
        return data || [];
    } catch (error) {
        handleError(error);
    }
}
