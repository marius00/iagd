import { ApplicationState } from './types';
import { setItemReducer } from './reducers/setItemReducer';
import { Store } from 'react-redux';
import { combineReducers, createStore } from 'redux';
import { reducer as notifications } from 'react-notification-system-redux';

const reducers = combineReducers({
  setItemReducer,
  notifications
});

/// https://redux.js.org/docs/api/combineReducers.html
// tslint:disable-next-line
const enhancer = window['devToolsExtension'] ? window['devToolsExtension']()(createStore) : createStore;

const store: Store<ApplicationState> = enhancer(reducers, {});

export default store;
