import { handleError } from '../api';

export async function fetchContracts(client, next, filter) {
  try {
    const { data } = await client.get(`/api/v0.1/contracts?t=${next || ''}&${filter || ''}`);
    return data || [];
  } catch (error) {
    handleError(error);
  }
}
