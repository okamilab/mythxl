import {
  ANALYSES_REQUEST,
  ANALYSES_RECEIVE,
} from './actions';

export const initialState = {
  isFetching: false,
  didInvalidate: true,
  itemsMap: {},
  next: ''
};

export default function reduce(state = initialState, action) {
  switch (action.type) {
    case ANALYSES_REQUEST:
      return Object.assign({}, state, {
        isFetching: true,
        didInvalidate: false
      });
    case ANALYSES_RECEIVE:
      return Object.assign({}, state, {
        isFetching: false,
        didInvalidate: false,
        itemsMap: {...state.itemsMap, ...action.itemsMap},
        next: action.next
      });
    default:
      return state;
  }
}