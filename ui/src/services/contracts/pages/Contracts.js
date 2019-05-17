import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import withStyles from '@material-ui/core/styles/withStyles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Typography from '@material-ui/core/Typography';
import LinearProgress from '@material-ui/core/LinearProgress';
import { Done, Error } from '@material-ui/icons';
import InfiniteScroll from 'react-infinite-scroller';

import Loader from './../../../components/Loader';
import { fetchContractsIfNeeded, fetchContracts } from './../actions';
import angry from './../../../images/emoticon-angry-outline.svg';
import neutral from './../../../images/emoticon-neutral-outline.svg';
import Filter from './../components/Filter';

const styles = theme => ({
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
  componentDidMount() {
    const { dispatch, match } = this.props;
    dispatch(fetchContractsIfNeeded(null, match.params.filter));
  }

  componentDidUpdate() {
    const { dispatch, match } = this.props;
    dispatch(fetchContractsIfNeeded(null, match.params.filter));
  }

  render() {
    const { isFetching, items, next, classes, dispatch, match } = this.props;

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
              return (
                <Link key={i}
                  to={`/address/${x.partitionKey}`}
                  style={{ textDecoration: 'none' }}>
                  <ListItem className={classes.mt3}>
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
                  </ListItem >
                  
                </Link>
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
  withStyles(styles)
)(Contracts);