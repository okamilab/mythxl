import React from 'react';
import { Switch, Route } from 'react-router';
import { Helmet } from 'react-helmet-async';

import Layout from './components/Layout';
import Contracts from './services/contracts/pages/Contracts';

export default function App() {
  return (
    <React.Fragment>
      <Helmet titleTemplate='MythXL - %s' />
      <Layout>
        <Switch>
          <Route exact path='/' component={Contracts} />
        </Switch>
      </Layout>
    </React.Fragment>
  );
}
