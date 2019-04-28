import React from 'react';
import PropTypes from 'prop-types';
import { Helmet } from 'react-helmet-async';
import withStyles from '@material-ui/core/styles/withStyles';
import Typography from '@material-ui/core/Typography';

const styles = theme => ({
  text: {
    margin: 200,
    fontSize: 78,
    fontWeight: 700,
    color: '#ebebeb'
  }
});

function NotFound({classes}) {
  return <>
    <Helmet>
      <title>Not found</title>
    </Helmet>
    <Typography align='center' className={classes.text}>0x194</Typography>
  </>;
}

NotFound.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(NotFound);