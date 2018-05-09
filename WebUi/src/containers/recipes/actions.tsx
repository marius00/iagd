import { SET_RECIPES, GET_RECIPES, GET_RECIPE_COMPONENTS, SET_RECIPE_COMPONENTS } from '../../constants';
import { RecipeList, Component } from './types';

export interface SetRecipes {
  type: SET_RECIPES;
  recipes: RecipeList;
}

export interface GetRecipes {
  type: GET_RECIPES;
}

export interface RequestRecipeComponents {
  type: GET_RECIPE_COMPONENTS;
  record: string;
}

export interface SetRecipeComponents {
  type: SET_RECIPE_COMPONENTS;
  components: Component;
}

export type RecipeAction = SetRecipes | GetRecipes | RequestRecipeComponents | SetRecipeComponents;

export function setRecipes(recipes: RecipeList): SetRecipes {
  console.log('Dispatching a request to set recipes');
  return {
    type: SET_RECIPES,
    recipes: recipes
  };
}

export function getRecipes(): GetRecipes {
  console.log('Dispatching a request to get recipes');
  return {
    type: GET_RECIPES
  };
}

export function requestRecipeComponents(record: string): RequestRecipeComponents {
  console.log('Dispatching a request to get components for recipe', record);
  return {
    type: GET_RECIPE_COMPONENTS,
    record: record
  };
}

export function setRecipeComponents(components: Component): SetRecipeComponents {
  console.log('Dispatching a request to set recipe components to', components);
  return {
    type: SET_RECIPE_COMPONENTS,
    components: components
  };
}