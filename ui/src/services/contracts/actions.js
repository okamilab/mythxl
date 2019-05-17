import * as api from './api';

export const CONTRACTS_REQUEST = 'CONTRACTS_REQUEST';
export const CONTRACTS_RECEIVE = 'CONTRACTS_RECEIVE';
export const CONTRACTS_INVALIDATE = 'CONTRACTS_INVALIDATE';

export function fetchContractsIfNeeded(next, filter) {
  return (dispatch, getState) => {
    if (shouldFetchContracts(getState())) {
      return dispatch(fetchContracts(next, filter));
    }
  };
}

function shouldFetchContracts(state) {
  const contracts = state.contracts;
  if (!contracts) {
    return true;
  } else if (contracts.isFetching) {
    return false;
  } else {
    return contracts.didInvalidate;
  }
}

export function fetchContracts(next, filter) {
  return async (dispatch, _, client) => {
    dispatch({ type: CONTRACTS_REQUEST, isFiltered: !!filter });
    const result = await api.fetchContracts(client, next, filter);
    dispatch({
      type: CONTRACTS_RECEIVE,
      items: result.data,
      next: result.next
    });
  };
}

export function fetchContract(id) {
  return async (dispatch, _, client) => {
    dispatch({ type: CONTRACTS_REQUEST });
    const result = await api.fetchContracts(client, null, `q=${id}`);
    dispatch({
      type: CONTRACTS_RECEIVE,
      items: result.data,
      invalidate: true
    });
    return result;
  };
}

export function invalidateContracts() {
  return { type: CONTRACTS_INVALIDATE };
}