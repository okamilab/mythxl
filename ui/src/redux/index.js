import { applyMiddleware, createStore, compose as reduxCompose } from 'redux';
import thunk from 'redux-thunk';

import reducer from './reducer';

export function configureStore(initialState, client) {
  // Use compose function provided by Redux DevTools if the extension is installed.
  const compose = (process.env.NODE_ENV === 'development'
    && typeof window === 'object'
    && window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__)
    || reduxCompose;

  const middleware = [thunk.withExtraArgument(client)];
  const enhancers = [applyMiddleware(...middleware)];
  return createStore(reducer, initialState, compose(...enhancers));
}