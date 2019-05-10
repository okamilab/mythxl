import * as api from './api';

export const PROCESSING_STAT_REQUEST = 'PROCESSING_STAT_REQUEST';
export const PROCESSING_STAT_RECEIVE = 'PROCESSING_STAT_RECEIVE';
export const ISSUES_STAT_REQUEST = 'ISSUES_STAT_REQUEST';
export const ISSUES_STAT_RECEIVE = 'ISSUES_STAT_RECEIVE';

export function fetchProcessingStat() {
  return async (dispatch, _, client) => {
    dispatch({ type: PROCESSING_STAT_REQUEST });
    const data = await api.fetchProcessingStat(client);
    dispatch({
      type: PROCESSING_STAT_RECEIVE,
      data
    });
  };
}

export function fetchIssuesStat() {
  return async (dispatch, _, client) => {
    dispatch({ type: ISSUES_STAT_REQUEST });
    const data = await api.fetchIssuesStat(client);
    dispatch({
      type: ISSUES_STAT_RECEIVE,
      data
    });
  };
}