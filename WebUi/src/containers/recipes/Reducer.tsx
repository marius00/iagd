import { RecipeAction } from './actions';
import { SET_RECIPES, GET_RECIPES, isEmbedded, GET_RECIPE_COMPONENTS, SET_RECIPE_COMPONENTS, RELIC_TEST_DATA, RELIC_TEST_DATA_2 } from '../../constants';
import { RecipeReducerState } from './types';

// tslint:disable-next-line
declare abstract class data {
  public static globalRequestRecipeList(): {};
  public static globalRequestRecipeComponents(record: string): {};
}

const initialState = {
  relics: [],
  misc: [],
  components: [],
  selectedRecipe: {
    name: '',
    bitmap: '',
    record: '',
    numRequired: 0,
    numOwned: 0,
    numCraftable: 0,
    isComplete: false,
    cost: []
  }
};

export function recipesReducer(state: RecipeReducerState = initialState, action: RecipeAction): RecipeReducerState {
  console.log('=============', state, action);

  if (action.type === SET_RECIPES) {
    return {
      ...state,
      relics: action.recipes.relics,
      misc: action.recipes.misc,
      components: action.recipes.components
    };
  }

  else if (action.type === GET_RECIPES) {
    if (isEmbedded) {
      data.globalRequestRecipeList();
    } else {
      return {
        ...state,
        relics: [
          {
            label: 'Mock relic recipe',
            record: 'no/record/exists/for/mock/recipe'
          }
        ],
        misc: [
          {
            label: 'Mock misc recipe',
            record: 'no/record/exists/for/mock/misc'
          }
        ],
        components: [
          {
            label: 'Mock component recipe',
            record: 'no/record/exists/for/mock/component'
          }
        ]
      };
    }
    return state;
  }
  else if (action.type === GET_RECIPE_COMPONENTS) {
    if (isEmbedded) {
      data.globalRequestRecipeComponents(action.record);
      return state;
    }
    else {
      if (action.record === 'no/record/exists/for/mock/misc') {
        return {...state, selectedRecipe: RELIC_TEST_DATA_2};
      }
      else {
        return {...state, selectedRecipe: RELIC_TEST_DATA};
      }
    }
  }
  else if (action.type === SET_RECIPE_COMPONENTS) {
    return {
      ...state,
      selectedRecipe: action.components
    };
  }

  return state;
}
