import * as api from './api';

export const ANALYSES_REQUEST = 'ANALYSES_REQUEST';
export const ANALYSES_RECEIVE = 'ANALYSES_RECEIVE';

export function fetchAnalyses(id, next) {
  return async (dispatch, _, client) => {
    dispatch({ type: ANALYSES_REQUEST });
    const result = await api.fetchAnalyses(client, next, id);
    const map = {};

    result.data.forEach(x => { map[x.id] = x; });

    dispatch({
      type: ANALYSES_RECEIVE,
      itemsMap: map,
      next: result.next
    });
  };
}