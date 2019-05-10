import React from 'react';
import PropTypes from 'prop-types';
import withStyles from '@material-ui/core/styles/withStyles';
import CssBaseline from '@material-ui/core/CssBaseline';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import { BarChart } from '@material-ui/icons';
import { Link } from 'react-router-dom';

const styles = theme => ({
  appBar: {
    position: 'relative',
  },
  grow: {
    flexGrow: 1,
  }
});

function Header({ classes }) {
  return (
    <React.Fragment>
      <CssBaseline />
      <AppBar position="absolute" color="default" className={classes.appBar}>
        <Toolbar>
          <Typography variant="h6" color="inherit" noWrap className={classes.grow}>
            <Link to='/' style={{ textDecoration: 'none', color: 'black' }}>
              MythXL
            </Link>
          </Typography>
          <IconButton aria-haspopup="true" color="inherit">
            <Link to='/stats' style={{ textDecoration: 'none', color: 'black' }}>
              <BarChart />
            </Link>
          </IconButton>
        </Toolbar>
      </AppBar>
    </React.Fragment>
  );
}

Header.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Header);
