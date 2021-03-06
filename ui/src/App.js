import React from 'react';
import { Switch, Route } from 'react-router';
import { Helmet } from 'react-helmet-async';

import Layout from './components/Layout';
import Contracts from './services/contracts/pages/Contracts';
import Contract from './services/contracts/pages/Contract';
import Stats from './services/stats/pages/Stats';
import NotFound from './components/NotFound';

export default function App() {
  return (
    <React.Fragment>
      <Helmet titleTemplate='MythXL - %s' />
      <Layout>
        <Switch>
          <Route exact path='/' component={Contracts} />
          <Route exact path='/address/:address' component={Contract} />
          <Route exact path='/stats' component={Stats} />
          <Route exact path='/:filter' component={Contracts} />
          <Route exact path='**' component={NotFound} />
        </Switch>
      </Layout>
    </React.Fragment>
  );
}
