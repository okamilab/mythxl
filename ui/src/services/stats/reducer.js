import {
  PROCESSING_STATS_REQUEST,
  PROCESSING_STATS_RECEIVE
} from './actions';

export const initialState = {
  isFetching: false,
  didInvalidate: true,
  data: {}
};

export default function reduce(state = initialState, action) {
  switch (action.type) {
    case PROCESSING_STATS_REQUEST:
      return Object.assign({}, state, {
        isFetching: true,
        didInvalidate: false
      });
    case PROCESSING_STATS_RECEIVE:
      return Object.assign({}, state, {
        isFetching: false,
        didInvalidate: false,
        data: action.data
      });
    default:
      return state;
  }
}