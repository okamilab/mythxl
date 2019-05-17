import {
  CONTRACTS_REQUEST,
  CONTRACTS_RECEIVE,
  CONTRACTS_INVALIDATE
} from './actions';

export const initialState = {
  isFetching: false,
  didInvalidate: true,
  isFiltered: false,
  items: [],
  next: ''
};

export default function reduce(state = initialState, action) {
  switch (action.type) {
    case CONTRACTS_REQUEST:
      return Object.assign({}, state, {
        isFetching: true,
        didInvalidate: false,
        isFiltered: action.isFiltered
      });
    case CONTRACTS_RECEIVE:
      return Object.assign({}, state, {
        isFetching: false,
        didInvalidate: false || action.invalidate,
        items: [...state.items, ...action.items],
        next: action.next
      });
    case CONTRACTS_INVALIDATE: {
      return Object.assign({}, state, {
        didInvalidate: true,
        isFiltered: false,
        items: []
      });
    }
    default:
      return state;
  }
}