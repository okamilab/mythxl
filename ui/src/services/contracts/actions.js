import * as api from './api';

export const CONTRACTS_REQUEST = 'CONTRACTS_REQUEST';
export const CONTRACTS_RECEIVE = 'CONTRACTS_RECEIVE';

export function fetchContracts(next) {
  return async (dispatch, _, client) => {
    dispatch({ type: CONTRACTS_REQUEST });
    const result = await api.fetchContracts(client, next);
    dispatch({
      type: CONTRACTS_RECEIVE,
      items: result.data,
      next: result.next
    });
  };
}