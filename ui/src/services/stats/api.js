import { handleError } from '../api';

export async function fetchProcessingStats(client) {
  try {
    const { data } = await client.get(`/api/v0.1/stats/processing`);
    return data || [];
  } catch (error) {
    handleError(error);
  }
}
