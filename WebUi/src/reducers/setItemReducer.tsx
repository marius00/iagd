import { AnyAction } from '../actions';
import {
  ACTION_SET_ITEMS,
  ACTION_ADD_ITEMS,
  SET_MOCK_DATA,
  SET_LOADING_STATUS,
  isEmbedded,
  REQUEST_INITIAL_ITEMS, SET_MOCK_COLLECTION_DATA, ACTION_SET_COLLECTIONITEMS
} from '../constants';
import { ApplicationState } from '../types';
import * as ReactTooltip from 'react-tooltip';

// tslint:disable-next-line
declare abstract class data {
  public static globalRequestInitialItems(): {};
}

const initialState = {
  clickCounter: 1,
  items: [],
  collectionItems: [],
  isLoading: false
};

export function setItemReducer(state: ApplicationState = initialState, action: AnyAction): ApplicationState {
  console.log('=============', state, action);
  if (action.type === ACTION_SET_ITEMS) {
    console.log(`Setting ${action.items.length} items to the item view`);
    return {...state, items: action.items, isLoading: false};
  }

  else if (action.type === ACTION_ADD_ITEMS) {
    console.log(`Adding ${action.items.length} items to the item view`);
    console.log(action.items);
    return {...state, items: state.items.concat(action.items), isLoading: false};
  }

  else if (action.type === SET_MOCK_DATA) {
    const items = JSON.parse(action.items);
    console.log('Setting mock items');
    return {...state, items: items, isLoading: false};
  }

  else if (action.type === SET_MOCK_COLLECTION_DATA ) {
    const items = JSON.parse(action.items);
    console.log('Setting mock collection items');
    setTimeout(() => ReactTooltip.rebuild(), 250); // TODO: This seems like a stupid way to solve tooltip issues.
    return {...state, collectionItems: items, isLoading: false};
  }

  else if (action.type === ACTION_SET_COLLECTIONITEMS) {
    console.log('Setting collection items to:', action.items);
    setTimeout(() => ReactTooltip.rebuild(), 1250); // TODO: This seems like a stupid way to solve tooltip issues.
    return {...state, collectionItems: action.items, isLoading: false};
  }

  else if (action.type === SET_LOADING_STATUS) {
    console.log('Update state for loading status to', action.status);
    return {...state, isLoading: action.status};
  }

  else if (action.type === REQUEST_INITIAL_ITEMS) {
    if (isEmbedded) {
      console.log('Initial items requested, forwarding to parent application');
      data.globalRequestInitialItems();
      return state;

    }
  }

  return state;
}
