import IItem from '../interfaces/IItem';
import { RecipeReducerState } from '../containers/recipes/types';

export interface ApplicationState {
  clickCounter: number;
  items: IItem[];
  isLoading: boolean;
}


export interface GlobalReducerState {
  setItemReducer: ApplicationState;
  notifications: Notification[];
  recipes: RecipeReducerState;
}
