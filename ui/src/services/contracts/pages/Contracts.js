import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import withStyles from '@material-ui/core/styles/withStyles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Collapse from '@material-ui/core/Collapse';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import LinearProgress from '@material-ui/core/LinearProgress';
import { ExpandLess, ExpandMore, CallMade, Done, Error } from '@material-ui/icons';
import InfiniteScroll from 'react-infinite-scroller';

import Loader from './../../../components/Loader';
import { fetchContractsIfNeeded, fetchContracts } from './../actions';
import { fetchAnalyses } from './../../analyses/actions';
import angry from './../../../images/emoticon-angry-outline.svg';
import neutral from './../../../images/emoticon-neutral-outline.svg';
import Filter from './../components/Filter';
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
  },
  404: {
    margin: 200,
    fontSize: 78,
    fontWeight: 700,
    color: '#ebebeb'
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

  componentDidMount() {
    const { dispatch, match } = this.props;
    dispatch(fetchContractsIfNeeded(null, match.params.filter));
  }

  componentDidUpdate() {
    const { dispatch, match } = this.props;
    dispatch(fetchContractsIfNeeded(null, match.params.filter));
  }

  openInfoClick = (index) => {
    const { items, itemsMap, dispatch } = this.props;
    const item = items[index];
    const id = `${item.partitionKey}|${item.analysisId}`;

    if (!this.state.open[index] && !itemsMap[id]) {
      dispatch(fetchAnalyses(id));
    }

    this.setState({ open: { ...this.state.open, [index]: !this.state.open[index] } });
  };

  render() {
    const { isFetching, items, itemsMap, next, classes, dispatch, match } = this.props;

    if (isFetching && !items.length) {
      return <Loader />
    }

    if (!items || !items.length) {
      return (
        <>
          <Filter />
          <Typography align='center' className={classes[404]}>0x194</Typography>
        </>
      )
    }

    return (
      <>
        <Filter />
        <InfiniteScroll
          loadMore={async (a) => {
            if (isFetching) return;
            await dispatch(fetchContracts(next, match.params.filter));
          }}
          hasMore={!!next}
          loader={<LinearProgress color='primary' variant='query' key={0} />}
        >
          <List>
            {items.map((x, i) => {
              const analyses = itemsMap[`${x.partitionKey}|${x.analysisId}`];

              return (
                <React.Fragment key={i}>
                  <ListItem button onClick={() => this.openInfoClick(i)} className={classes.mt3}>
                    <ListItemIcon>
                      {
                        x.analysisStatus === 'Error' ?
                          <Error titleAccess={x.analysisStatus} /> :
                          <Done titleAccess={x.analysisStatus} />
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
                  <Collapse in={this.state.open[i]} timeout='auto' unmountOnExit>
                    <Grid container spacing={24} className={classes.info}>
                      <Grid item xs={5}>
                        <Typography noWrap>Analyses status: {x.analysisStatus}</Typography>
                        <Typography noWrap>Severity: {x.severity || '-'}</Typography>
                      </Grid>
                      <Grid item xs={6}>
                        {x.code ? <Typography noWrap>Code: {x.code}</Typography> : null}
                      </Grid>
                      <Grid item xs={1} className={classes.text_right}>
                        <a href={'https://etherscan.io/address/' + x.partitionKey} target='_blank' rel='noopener noreferrer'>
                          <CallMade />
                        </a>
                      </Grid>
                    </Grid>
                    {
                      analyses ?
                        <AnalysesDetails data={analyses} /> :
                        <LinearProgress color='primary' variant='query' />
                    }
                  </Collapse>
                </React.Fragment>
              );
            })}
          </List>
        </InfiniteScroll>
      </>
    );
  }
}

Contracts.propTypes = {
  dispatch: PropTypes.func.isRequired,
  location: PropTypes.object,
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
  withStyles(styles)
)(Contracts);