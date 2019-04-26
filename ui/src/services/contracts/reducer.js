import {
  CONTRACTS_REQUEST,
  CONTRACTS_RECEIVE,
} from './actions';

export const initialState = {
  isFetching: false,
  didInvalidate: true,
  items: [],
  next: ''
};

export default function reduce(state = initialState, action) {
  switch (action.type) {
    case CONTRACTS_REQUEST:
      return Object.assign({}, state, {
        isFetching: true,
        didInvalidate: false
      });
    case CONTRACTS_RECEIVE:
      return Object.assign({}, state, {
        isFetching: false,
        didInvalidate: false,
        items: [...state.items, ...action.items],
        next: action.next
      });
    default:
      return state;
  }
}