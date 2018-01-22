import { AnyAction } from '../actions';
import {ACTION_SET_ITEMS, ACTION_ADD_ITEMS, SET_MOCK_DATA, SET_CLASSES, REQUEST_CLASSES, SET_MODS, REQUEST_MODS, SET_SELECTED_MOD, SET_LOADING_STATUS} from '../constants';
import { GlobalState } from '../types';
import { getDefaultSelection } from '../interfaces/IMod';

// Detect if we're running embedded
declare abstract class cefsharp_CreatePromise {}
const isEmbedded = typeof cefsharp_CreatePromise === 'function';

declare abstract class data {
  public static globalRequestClasses(): {};
  public static globalRequestMods(): {};
}

export function setItemReducer(state: GlobalState, action: AnyAction): GlobalState {
  if (action.type === ACTION_SET_ITEMS) {
    console.log(`Setting ${action.items.length} items to the item view`);
    return {...state, items: action.items, isLoading: false};
  }

  else if (action.type === ACTION_ADD_ITEMS) {
    console.log(`Adding ${action.items.length} items to the item view`);
    return {...state, items: action.items, isLoading: false};
  }

  else if (action.type === SET_MOCK_DATA) {
    const items = JSON.parse(action.items);
    console.log('Setting mock items');
    return {...state, items: items, isLoading: false};
  }

  else if (action.type === SET_CLASSES) {
    const classes = action.classes;
    console.log('Setting classes to', classes);
    return {...state, classes: classes};
  }

  else if (action.type === SET_MODS) {
    console.log('Setting mods to', action.mods);
    return {
      ...state,
      mods: action.mods,
      selectedMod: getDefaultSelection(action.mods)
    };
  }

  else if (action.type === REQUEST_CLASSES) {
    if (isEmbedded) {
      console.log('Class update requested, forwarding to parent application');
      data.globalRequestClasses();
      return state;

    } else {
      const classes = [{'label':'Soldier (Mock Data)','value':'01'},{'label':'Demolitionist','value':'02'},{'label':'Occultist','value':'03'},{'label':'Nightblade','value':'04'},{'label':'Arcanist','value':'05'},{'label':'Shaman','value':'06'},{'label':'Inquisitor','value':'07'},{'label':'Necromancer','value':'08'}];
      console.log('Class update requested, sending mock data');
      return {...state, classes: classes};
    }
  }

  else if (action.type === REQUEST_MODS) {
    if (isEmbedded) {
      console.log('Mod listing requested, forwarding to parent application');
      data.globalRequestMods();
      return state;

    } else {
      const mods = [
        {label: 'Vanilla', path: '321/vanilla/file', isEnabled: true, isSelected: true, isHardcore: false, mod: 'this should be the value in search'},
        {label: 'Vanilla (HC)', path: '123/hc/file', isEnabled: true, isSelected: true, isHardcore: true, mod: 'this should be the value in hc search'}
      ];
      console.log('Mod listing requested, sending mock data');
      return {
        ...state,
        mods: mods,
        selectedMod: getDefaultSelection(mods)
      };
    }
  }

  else if (action.type === SET_SELECTED_MOD) {
    console.log('Update state for selectedMod to', action.mod);
    return {...state, selectedMod: action.mod};
  }

  else if (action.type === SET_LOADING_STATUS) {
    console.log('Update state for loading status to', action.status);
    return {...state, isLoading: action.status};
  }

  return state;
}
