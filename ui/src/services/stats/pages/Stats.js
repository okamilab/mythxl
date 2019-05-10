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

import Loader from './../../../components/Loader';
import { fetchProcessingStats } from './../actions';

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
    const { isFetching, data, classes } = this.props;

    console.log(data);

    if (isFetching) {
      return <Loader />
    }

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
              <Typography variant="subtitle1" noWrap>{data.count}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Finished</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.finished}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Errors</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.errors}</Typography>
            </Grid>
            <Grid item xs={12}>
              <Divider />
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>No Issues</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.noSeverity}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Low severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.lowSeverity}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>Medium severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.mediumSeverity}</Typography>
            </Grid>
            <Grid item xs={4}>
              <Typography variant="subtitle1" noWrap>High severity</Typography>
            </Grid>
            <Grid item xs={8}>
              <Typography variant="subtitle1" noWrap>{data.highSeverity}</Typography>
            </Grid>
          </Grid>
        </Paper>
      </>
    );
  }
}

Stats.propTypes = {
  dispatch: PropTypes.func.isRequired,
  location: PropTypes.object,
  classes: PropTypes.object.isRequired,
  data: PropTypes.object.isRequired
};

export default compose(
  connect(state => {
    const { isFetching, data } = state.stats || {
      isFetching: true,
      data: {}
    };
    return { isFetching, data };
  }),
  withJob({
    work: ({ dispatch }) => dispatch(fetchProcessingStats()),
    LoadingComponent: () => <div>Loading...</div>,
    error: function Error() { return <p>Error</p>; },
  }),
  withStyles(styles)
)(Stats);