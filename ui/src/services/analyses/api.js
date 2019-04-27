import { handleError } from '../api';

export async function fetchAnalyses(client, next, id) {
    try {
        const { data } = await client.get(`/api/v0.1/analyses?t=${next || ''}&id=${id}`);
        return data || [];
    } catch (error) {
        handleError(error);
    }
}
