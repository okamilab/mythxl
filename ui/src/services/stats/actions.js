import * as api from './api';

export const PROCESSING_STATS_REQUEST = 'PROCESSING_STATS_REQUEST';
export const PROCESSING_STATS_RECEIVE = 'PROCESSING_STATS_RECEIVE';

export function fetchProcessingStats() {
  return async (dispatch, _, client) => {
    dispatch({ type: PROCESSING_STATS_REQUEST });
    const data = await api.fetchProcessingStats(client);
    dispatch({
      type: PROCESSING_STATS_RECEIVE,
      data
    });
  };
}