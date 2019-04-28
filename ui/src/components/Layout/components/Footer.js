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
        MythXL is open security platform for smart contracts powered by MythX | <a href='https://github.com/aquiladev/mythxl'>GitHub</a>
      </Typography>
    </div>
  );
}


Footer.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Footer);
