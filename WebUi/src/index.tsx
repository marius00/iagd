import * as React from 'react';
import * as ReactDOM from 'react-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import './index.css';
import IItem from './interfaces/IItem';
import { setItems, addItems, setLoadingStatus, showMessage } from './actions';
import store from './store';
import { setRecipeComponents, setRecipes } from './containers/recipes/actions';
import { Component, RecipeList } from './containers/recipes/types';

/* == BEGIN MAGIC ==
* This little piece of magic enables the following function to be called globally:;
  data.globalStore.dispatch(data.globalSetItems());
  This allows CEFSharp embedded to update the redux state of the app
*/
// tslint:disable-next-line
declare abstract class data {
  public static globalStore: {};
  public static globalSetItems(items: IItem[]): {};
  public static globalAddItems(items: IItem[]): {};
  public static globalSetRecipes(recipes: RecipeList): {};
  public static globalSetRecipeIngredients(components: Component): {};
  public static globalSetIsLoading(v: boolean): {};
  public static showMessage(message: string, level: string): {};
}
if (typeof data === 'object') {
  data.globalStore = store;
  data.globalSetItems = setItems;
  data.globalAddItems = addItems;
  data.globalSetIsLoading = setLoadingStatus;
  data.showMessage = showMessage;
  data.globalSetRecipes = setRecipes;
  data.globalSetRecipeIngredients = setRecipeComponents;
}
/* == END MAGIC == */

ReactDOM.render(
  <App store={store} />,
  document.getElementById('root') as HTMLElement
);
registerServiceWorker();
