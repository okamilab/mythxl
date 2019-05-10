import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import { withJob } from 'react-jobs';
import withStyles from '@material-ui/core/styles/withStyles';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import Grid from '@material-ui/core/Grid';
import Divider from '@material-ui/core/Divider';
import BubbleChart from '@weknow/react-bubble-chart-d3';

import Loader from './../../../components/Loader';
import { fetchProcessingStat, fetchIssuesStat } from './../actions';

const styles = theme => ({
  root: {
    ...theme.mixins.gutters(),
    marginTop: theme.spacing.unit * 4,
    paddingTop: theme.spacing.unit * 2,
    paddingBottom: theme.spacing.unit * 2,
  },
  stat: {
    paddingTop: theme.spacing.unit * 4,
  }
});

class Stats extends React.Component {
  render() {
    const { processing, issues, classes } = this.props;

    if (processing.isFetching || issues.isFetching) {
      return <Loader />
    }

    const issuesData = issues.data.map(x => {
      return { label: x.key, value: x.value }
    })

    return (
      <>
        <Paper className={classes.root} elevation={2}>
          <Typography variant="h5" component="h3">
            Processing stat
          </Typography>
          <Grid container spacing={24} className={classes.stat}>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Processed</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.processed}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Finished</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.finished}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Failed</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.failed}</Typography>
            </Grid>
            <Grid item xs={12}>
              <Divider />
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>No Issues</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.noIssues}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Low severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.lowSeverity}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Medium severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.mediumSeverity}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>High severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{processing.data.highSeverity}</Typography>
            </Grid>
          </Grid>
        </Paper>
        <Paper className={classes.root} elevation={2}>
          <Typography variant="h5" component="h3">
            Issues
          </Typography>
          <BubbleChart
            graph={{
              zoom: 1,
              offsetX: 0,
              offsetY: 0
            }}
            width={950}
            height={750}
            showLegend={true}
            legendPercentage={10}
            legendFont={{
              family: 'Arial',
              size: 12,
              color: '#000',
              weight: 'bold',
            }}
            valueFont={{
              family: 'Arial',
              size: 12,
              color: '#333',
              weight: 'bold',
            }}
            labelFont={{
              family: 'Arial',
              size: 16,
              color: '#fff',
              weight: 'bold',
            }}
            data={issuesData}
          />
        </Paper>
      </>
    );
  }
}

Stats.propTypes = {
  dispatch: PropTypes.func.isRequired,
  location: PropTypes.object,
  classes: PropTypes.object.isRequired,
  processing: PropTypes.object,
  issues: PropTypes.object
};

export default compose(
  connect(state => {
    const { processing, issues } = state.stats || {
      processing: {
        isFetching: true,
        data: {}
      },
      issues: {
        isFetching: true,
        data: []
      }
    };
    return { processing, issues };
  }),
  withJob({
    work: ({ dispatch }) => {
      dispatch(fetchProcessingStat())
      dispatch(fetchIssuesStat())
    },
    LoadingComponent: () => <div>Loading...</div>,
    error: function Error() { return <p>Error</p>; },
  }),
  withStyles(styles)
)(Stats);