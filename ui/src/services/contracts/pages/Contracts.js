import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import { withJob } from 'react-jobs';
import withStyles from '@material-ui/core/styles/withStyles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Collapse from '@material-ui/core/Collapse';
import ErrorIcon from '@material-ui/icons/Error';
import DoneIcon from '@material-ui/icons/Done';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { ExpandLess, ExpandMore, CallMade } from '@material-ui/icons';
import Button from '@material-ui/core/Button';

import { fetchContracts } from './../actions';
import angry from './../../../images/emoticon-angry-outline.svg';
import neutral from './../../../images/emoticon-neutral-outline.svg';

const styles = theme => ({
  info: {
    padding: theme.spacing.unit * 2
  },
  text_right: {
    textAlign: 'right'
  },
  text_center: {
    textAlign: 'center'
  },
  img: {
    height: 20,
    paddingLeft: 8
  }
});

class Contracts extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      open: {}
    };

    this.openInfoClick = this.openInfoClick.bind(this);
  }

  openInfoClick = (index) => {
    this.setState({ open: { ...this.state.open, [index]: !this.state.open[index] } });
  };

  render() {
    const { items, next, classes, dispatch } = this.props;

    return (
      <>
        <List>
          {items.map((x, i) => {
            return (
              <React.Fragment key={i}>
                <ListItem button onClick={() => this.openInfoClick(i)}>
                  <ListItemIcon>
                    {
                      x.analyzeStatus === 'Error' ?
                        <ErrorIcon titleAccess={x.analyzeStatus} /> :
                        <DoneIcon titleAccess={x.analyzeStatus} />
                    }
                  </ListItemIcon>
                  <ListItemText primary={
                    <Typography noWrap>
                      {x.partitionKey}
                      {x.severity === 'Medium' ? <img src={neutral} alt={x.severity} className={classes.img} /> : null}
                      {x.severity === 'High' ? <img src={angry} alt={x.severity} className={classes.img} /> : null}
                    </Typography>
                  } />
                  {this.state.open[i] ? <ExpandLess /> : <ExpandMore />}
                </ListItem >
                <Collapse in={this.state.open[i]} timeout="auto" unmountOnExit>
                  <Grid container spacing={24} className={classes.info}>
                    <Grid item xs={5}>
                      <Typography noWrap>Analyses status: {x.analyzeStatus}</Typography>
                      <Typography noWrap>Severity: {x.severity || '-'}</Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography noWrap>Code: {x.code}</Typography>
                    </Grid>
                    <Grid item xs={1} className={classes.text_right}>
                      <a href={'https://etherscan.io/address/' + x.partitionKey} target="_blank" rel="noopener noreferrer">
                        <CallMade />
                      </a>
                    </Grid>
                  </Grid>
                </Collapse>
              </React.Fragment>
            );
          })}
        </List>
        <div className={classes.text_center}>
          {next ? <Button onClick={() => {
            dispatch(fetchContracts(next));
          }}>Load more</Button> : null}
        </div>
      </>
    );
  }
}

Contracts.propTypes = {
  dispatch: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  items: PropTypes.array.isRequired,
  next: PropTypes.string
};

export default compose(
  connect(state => {
    const { isFetching, items, next } = state.contracts || {
      isFetching: true,
      items: [],
      next: ''
    };
    return { isFetching, items, next };
  }),
  withJob({
    work: ({ dispatch }) => dispatch(fetchContracts()),
    LoadingComponent: () => <div>Loading...</div>,
    error: function Error() { return <p>Error</p>; },
  }),
  withStyles(styles)
)(Contracts);