import React from 'react';
import ReactDOM from 'react-dom';
import dotenv from 'dotenv';
import { AppContainer } from 'react-hot-loader';
import { HelmetProvider } from 'react-helmet-async';
import { Provider } from 'react-redux';
import { BrowserRouter as Router } from 'react-router-dom';

import './index.css';
import { createClient } from './services/api/client';
import { configureStore } from './redux';
import App from './App';
import * as serviceWorker from './serviceWorker';

dotenv.config();

let client = createClient();
const store = configureStore({}, client);

ReactDOM.render(
  <AppContainer>
    <Router>
      <HelmetProvider>
        <Provider store={store}>
          <App />
        </Provider>
      </HelmetProvider>
    </Router>
  </AppContainer>,
  document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
