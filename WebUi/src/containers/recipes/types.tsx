export interface ComponentListEntry {
  label: string;
  record: string;
}

export interface RecipeList {
  relics: ComponentListEntry[];
  misc: ComponentListEntry[];
  components: ComponentListEntry[];
}

export interface RecipeReducerState {
  relics: ComponentListEntry[];
  misc: ComponentListEntry[];
  components: ComponentListEntry[];
  selectedRecipe: Component;
}

export interface Component {
  name: string;
  bitmap: string;
  record: string;
  numRequired: number;
  numOwned: number;
  numCraftable: number;
  isComplete: boolean;
  cost: Component[];
}