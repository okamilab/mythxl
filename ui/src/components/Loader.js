import React from 'react';
import CircularProgress from '@material-ui/core/CircularProgress';
import withStyles from '@material-ui/core/styles/withStyles';

const styles = _ => ({
  loader: {
    margin: 20,
    height: '100%',
    widows: '100%',
    textAlign: 'center'
  }
});

function Loader({ classes }) {
  return (
    <div className={classes.loader}>
      <CircularProgress />
    </div>
  );
}

export default withStyles(styles)(Loader);