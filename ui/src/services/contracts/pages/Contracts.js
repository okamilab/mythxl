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
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import LinearProgress from '@material-ui/core/LinearProgress';
import { ExpandLess, ExpandMore, CallMade, Done, Error } from '@material-ui/icons';

import { fetchContracts } from './../actions';
import { fetchAnalyses } from './../../analyses/actions';
import angry from './../../../images/emoticon-angry-outline.svg';
import neutral from './../../../images/emoticon-neutral-outline.svg';
import AnalysesDetails from './../../analyses/components/Details';

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
  },
  w100: {
    width: '100%'
  },
  mt3: {
    marginTop: theme.spacing.unit * 2
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
    const { items, itemsMap, dispatch } = this.props;
    const item = items[index];
    const id = `${item.partitionKey}|${item.analyzeUUID}`;

    if (!this.state.open[index] && !itemsMap[id]) {
      dispatch(fetchAnalyses(id));
    }

    this.setState({ open: { ...this.state.open, [index]: !this.state.open[index] } });
  };

  render() {
    const { items, itemsMap, next, classes, dispatch } = this.props;

    return (
      <>
        <List>
          {items.map((x, i) => {
            const analyses = itemsMap[`${x.partitionKey}|${x.analyzeUUID}`];

            return (
              <React.Fragment key={i}>
                <ListItem button onClick={() => this.openInfoClick(i)} className={classes.mt3}>
                  <ListItemIcon>
                    {
                      x.analyzeStatus === 'Error' ?
                        <Error titleAccess={x.analyzeStatus} /> :
                        <Done titleAccess={x.analyzeStatus} />
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
                  {
                    analyses ?
                      <AnalysesDetails data={analyses} /> :
                      <LinearProgress color="primary" variant="query" />
                  }
                </Collapse>
              </React.Fragment>
            );
          })}
        </List>
        <div className={classes.text_center}>
          {next ? <Button onClick={() => {
            dispatch(fetchContracts(next));
          }} className={classes.w100}>Load more</Button> : null}
        </div>
      </>
    );
  }
}

Contracts.propTypes = {
  dispatch: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  items: PropTypes.array.isRequired,
  itemsMap: PropTypes.object,
  next: PropTypes.string
};

export default compose(
  connect(state => {
    const { isFetching, items, next } = state.contracts || {
      isFetching: true,
      items: [],
      next: ''
    };
    const { itemsMap } = state.analyses || {
      itemsMap: {}
    };
    return { isFetching, items, next, itemsMap };
  }),
  withJob({
    work: ({ dispatch }) => dispatch(fetchContracts()),
    LoadingComponent: () => <div>Loading...</div>,
    error: function Error() { return <p>Error</p>; },
  }),
  withStyles(styles)
)(Contracts);