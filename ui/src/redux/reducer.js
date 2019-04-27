import { combineReducers } from 'redux';

import contracts from '../services/contracts/reducer';
import analyses from '../services/analyses/reducer';

export default combineReducers({ contracts, analyses });