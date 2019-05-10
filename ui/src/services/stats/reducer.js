import {
  PROCESSING_STAT_REQUEST,
  PROCESSING_STAT_RECEIVE,
  ISSUES_STAT_REQUEST,
  ISSUES_STAT_RECEIVE
} from './actions';

export const initialState = {
  processing: {
    isFetching: false,
    didInvalidate: true,
    data: {}
  },
  issues: {
    isFetching: false,
    didInvalidate: true,
    data: []
  }
};

export default function reduce(state = initialState, action) {
  switch (action.type) {
    case PROCESSING_STAT_REQUEST:
      return Object.assign({}, state, {
        processing: {
          isFetching: true,
          didInvalidate: false
        }
      });
    case PROCESSING_STAT_RECEIVE:
      return Object.assign({}, state, {
        processing: {
          isFetching: false,
          didInvalidate: false,
          data: action.data
        }
      });
      case ISSUES_STAT_REQUEST:
        return Object.assign({}, state, {
          issues: {
            isFetching: true,
            didInvalidate: false
          }
        });
      case ISSUES_STAT_RECEIVE:
        return Object.assign({}, state, {
          issues: {
            isFetching: false,
            didInvalidate: false,
            data: action.data
          }
        });
    default:
      return state;
  }
}