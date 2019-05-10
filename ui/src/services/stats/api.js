import { handleError } from '../api';

export async function fetchProcessingStat(client) {
  try {
    const { data } = await client.get(`/api/v0.1/stats/processing`);
    return data || {};
  } catch (error) {
    handleError(error);
  }
}

export async function fetchIssuesStat(client) {
  try {
    const { data } = await client.get(`/api/v0.1/stats/issues`);
    return data || [];
  } catch (error) {
    handleError(error);
  }
}