import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import { withJob } from 'react-jobs';
import { withRouter } from 'react-router-dom';
import withStyles from '@material-ui/core/styles/withStyles';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { CallMade } from '@material-ui/icons';

import Loader from './../../../components/Loader';
import AnalysesDetails from './../../analyses/components/Details';
import { fetchContract } from './../actions';
import { fetchAnalyses } from './../../analyses/actions';

const styles = theme => ({
  info: {
    padding: theme.spacing.unit * 2
  },
  title: {
    paddingTop: theme.spacing.unit * 4,
    paddingRight: theme.spacing.unit * 2,
    paddingLeft: theme.spacing.unit * 2
  },
  text_right: {
    textAlign: 'right'
  },
});

class Contract extends React.Component {
  render() {
    const { classes, contract, analysis } = this.props;

    if (contract.isFetching || analysis.isFetching) {
      return <Loader />
    }

    const data = contract.data;
    return (
      <>
        <Typography variant="h6" color="inherit" noWrap className={classes.title}>
          Address: {data.partitionKey}
        </Typography>
        <Grid container spacing={24} className={classes.info}>
          <Grid item xs={5}>
            <Typography noWrap>Analyses status: {data.analysisStatus}</Typography>
            <Typography noWrap>Severity: {data.severity || '-'}</Typography>
          </Grid>
          <Grid item xs={6}>
            {data.code ? <Typography noWrap>Code: {data.code}</Typography> : null}
          </Grid>
          <Grid item xs={1} className={classes.text_right}>
            <a href={'https://etherscan.io/address/' + data.partitionKey} target='_blank' rel='noopener noreferrer'>
              <CallMade />
            </a>
          </Grid>
        </Grid>
        {analysis.data ? <AnalysesDetails data={analysis.data} /> : null}
      </>
    );
  }
}

Contract.propTypes = {
  dispatch: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  contract: PropTypes.object.isRequired,
  analysis: PropTypes.object
};

export default compose(
  connect((state, props) => {
    const { contracts, analyses } = state || {
      contracts: {
        isFetching: false,
        items: []
      },
      analyses: {
        isFetching: false,
        itemsMap: {}
      }
    };
    const address = props.match.params.address;
    const item = contracts.items.find(c => c.partitionKey === address);
    const id = item && `${item.partitionKey}|${item.analysisId}`;
    return {
      contract: {
        isFetching: contracts.isFetching,
        data: item
      },
      analysis: {
        isFetching: analyses.isFetching,
        data: item && analyses.itemsMap[id]
      }
    };
  }),
  withJob({
    work: ({ dispatch, match, contract }) => {
      if (contract.data) {
        const item = contract.data;
        const id = `${item.partitionKey}|${item.analysisId}`;
        dispatch(fetchAnalyses(id));
        return;
      }

      dispatch(fetchContract(match.params.address))
        .then(x => {
          if (!x || !x.data || !x.data[0]) {
            return;
          }

          const item = x.data[0];
          const id = `${item.partitionKey}|${item.analysisId}`;
          dispatch(fetchAnalyses(id));
        });
    },
    LoadingComponent: () => <div>Loading...</div>,
    error: function Error() { return <p>Error</p>; },
  }),
  withStyles(styles)
)(withRouter(Contract));