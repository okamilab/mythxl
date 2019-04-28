import React from 'react';
import PropTypes from 'prop-types';
import withStyles from '@material-ui/core/styles/withStyles';
import Typography from '@material-ui/core/Typography';

const styles = theme => ({
  footer: {
    textAlign: 'center',
    paddingTop: theme.spacing.unit * 3,
    paddingBottom: theme.spacing.unit * 2
  }
});

function Footer({ classes }) {
  return (
    <div className={classes.footer}>
      <Typography noWrap>
        MythXL is a tool for analyzing contracts via MythX on Ethereum mainnet
      </Typography>
    </div>
  );
}


Footer.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Footer);
