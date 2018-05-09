import * as React from 'react';
import { Tree } from './Tree';
import './CraftingContainer.css';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import { connect, Dispatch } from 'react-redux';
import { Component, ComponentListEntry } from './types';
import { GlobalReducerState } from '../../types';
import { AnyAction } from '../../actions';
import { requestRecipeComponents } from './actions';

export interface StateProps {
  relics: ComponentListEntry[];
  misc: ComponentListEntry[];
  components: ComponentListEntry[];
  selectedRecipe: Component;
}

export interface DispatchProps {
  onChanged: (record: string) => void;
}

class CraftingContainer extends React.Component<StateProps & DispatchProps, {}> {
  state = {
    selectedRelicOption: '',
    selectedMiscOption: '',
    selectedComponentOption: ''
  };

  onRelicRecipeChanged = (selectedOption) => {
    this.setState({selectedRelicOption: selectedOption, selectedMiscOption: '', selectedComponentOption: ''});
    this.props.onChanged(selectedOption.record);
  }

  onMiscRecipeChanged = (selectedOption) => {
    this.setState({selectedRelicOption: '', selectedMiscOption: selectedOption, selectedComponentOption: ''});
    this.props.onChanged(selectedOption.record);
  }

  onComponentRecipeChanged = (selectedOption) => {
    this.setState({selectedRelicOption: '', selectedMiscOption: '', selectedComponentOption: selectedOption});
    this.props.onChanged(selectedOption.record);
  }

  render() {
    return (
      <div className="crafting-tab">
        <div className="recipe-dropdowns">
          <Select
            onChange={this.onRelicRecipeChanged}
            value={this.state.selectedRelicOption}
            clearable={false}
            options={this.props.relics}
          />

          <Select
            onChange={this.onMiscRecipeChanged}
            value={this.state.selectedMiscOption}
            clearable={false}
            options={this.props.misc}
          />

          <Select
            onChange={this.onComponentRecipeChanged}
            value={this.state.selectedComponentOption}
            clearable={false}
            options={this.props.components}
          />

        </div>

        <Tree selectedRecipe={this.props.selectedRecipe} />
      </div>
    );
  }
}

export function mapStateToProps({ recipes }: GlobalReducerState): StateProps {
  const state = recipes;
  return {
    components: state.components,
    misc: state.misc,
    relics: state.relics,
    selectedRecipe: state.selectedRecipe
  };
}

export function mapDispatchToProps(dispatch: Dispatch<AnyAction>): DispatchProps {
  return {
    onChanged: (record: string) => {
      dispatch(requestRecipeComponents(record));
    }
  };
}

export default connect<StateProps, DispatchProps>(mapStateToProps, mapDispatchToProps)(CraftingContainer);
