import React from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";
import { withRouter } from 'react-router';
import withStyles from '@material-ui/core/styles/withStyles';
import { darken } from '@material-ui/core/styles/colorManipulator';
import Toolbar from '@material-ui/core/Toolbar';
import InputBase from '@material-ui/core/InputBase';
import MenuItem from '@material-ui/core/MenuItem';
import Menu from '@material-ui/core/Menu';
import IconButton from '@material-ui/core/IconButton';
import { FilterList, ArrowBack } from '@material-ui/icons';
import Divider from '@material-ui/core/Divider';
import Tooltip from '@material-ui/core/Tooltip';

import { invalidateContracts } from './../actions';

const styles = theme => ({
  mt3: {
    marginTop: theme.spacing.unit * 2,
  },
  search: {
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: darken(theme.palette.common.white, 0.15),
    '&:hover': {
      backgroundColor: darken(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing.unit * 2,
    marginLeft: 0,
    width: '100%',
  },
  inputRoot: {
    color: 'inherit',
    width: '100%',
  },
  inputInput: {
    paddingTop: theme.spacing.unit,
    paddingRight: theme.spacing.unit,
    paddingBottom: theme.spacing.unit,
    paddingLeft: theme.spacing.unit * 2,
    transition: theme.transitions.create('width'),
    width: '100%',
  }
});

class Filter extends React.Component {
  state = {
    anchorEl: null,
    redirect: null
  };

  handleFilterOpen = event => {
    this.setState({ anchorEl: event.currentTarget });
  };

  handleFilterClose = () => {
    this.setState({ anchorEl: null });
  };

  render() {
    const { anchorEl } = this.state;
    const { classes, history, dispatch, match } = this.props;
    const isMenuOpen = Boolean(anchorEl);

    return (
      <Toolbar className={classes.mt3}>
        <div className={classes.search}>
          <InputBase
            placeholder="Filter…"
            classes={{
              root: classes.inputRoot,
              input: classes.inputInput,
            }}
            onKeyPress={(ev) => {
              if (ev.key === 'Enter') {
                history.push(`/q=${ev.target.value}`);
                dispatch(invalidateContracts());
              }
            }}
          />
        </div>
        <Tooltip title="Filter">
          <IconButton
            color="inherit"
            onClick={this.handleFilterOpen}>
            <FilterList />
          </IconButton>
        </Tooltip>
        {match.params.filter ?
          <Tooltip title="Clean filter">
            <IconButton
              color="inherit"
              onClick={async () => {
                history.push("/");
                dispatch(invalidateContracts());
              }}>
              <ArrowBack />
            </IconButton>
          </Tooltip> : null}
        <Menu
          anchorEl={anchorEl}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          transformOrigin={{ vertical: 'top', horizontal: 'right' }}
          open={isMenuOpen}
          onClose={this.handleFilterClose}
        >
          <MenuItem onClick={async () => {
            history.push("/a=Finished");
            dispatch(invalidateContracts());
          }}>Analyses: Finished</MenuItem>
          <MenuItem onClick={async () => {
            history.push("/a=Error");
            dispatch(invalidateContracts());
          }}>Analyses: Error</MenuItem>
          <Divider />
          <MenuItem onClick={async () => {
            history.push("/s=Low");
            dispatch(invalidateContracts());
          }}>Severity: Low</MenuItem>
          <MenuItem onClick={async () => {
            history.push("/s=Medium");
            dispatch(invalidateContracts());
          }}>Severity: Medium</MenuItem>
          <MenuItem onClick={async () => {
            history.push("/s=High");
            dispatch(invalidateContracts());
          }}>Severity: High</MenuItem>
        </Menu>
      </Toolbar>
    );
  }
}

Filter.propTypes = {
  dispatch: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired
};

export default withRouter(
  connect()(withStyles(styles)(Filter))
);