import * as React from 'react';
import 'react-select/dist/react-select.css';
import { Button, Checkbox } from 'react-bootstrap';
import DamageSelector from './FilterDropdowns/DamageSelector';
import ResistanceSelector from './FilterDropdowns/ResistanceSelector';
import MiscSkillSelector from './FilterDropdowns/MiscSkillSelector';
import ItemSlotSelector from './FilterDropdowns/ItemSlotSelector';
import IDropdownEntry from '../interfaces/IDropdownEntry';
import ConnectedProfessionSelector from './FilterDropdowns/ConnectedProfessionSelector';
import './FilterView.css';
import ItemQualitySelector from './FilterDropdowns/ItemQualitySelector';
import { setItems, setLoadingStatus } from '../actions/index';
import { connect } from 'react-redux';
import IMod from '../interfaces/IMod';
import { GlobalState } from '../types';

interface UserProvidedProps {
  isEmbedded: boolean;
}
interface Props {
  isEmbedded: boolean;
  onSearch?: () => void;
  mod: IMod;
}
interface DispatchProps {
  onSearch?: () => void;
}
interface InjectedProps {
  mod: IMod;
}

class FilterView extends React.Component<Props & UserProvidedProps, any> {
  constMaximumLevel = 100;

  state = {
    selectedDamageEntries: [],
    selectedResistanceEntries: [],
    selectedMiscEntries: [],
    selectedProfessions: [],
    selectedItemSlot: '',
    duplicatesOnly: false,
    socketedOnly: false,
    retaliationOnly: false,
    itemName: '',
    minimumLevel: 0,
    maximumLevel: this.constMaximumLevel,
    itemQuality: ''
  };

  search() {
    let requirements = this.state.selectedDamageEntries.map((e: IDropdownEntry) => e.value)
      .concat(this.state.selectedResistanceEntries.map((e: IDropdownEntry) => e.value))
      .concat(this.state.selectedMiscEntries.map((e: IDropdownEntry) => e.value))
      .map((e) => e.split(','));

    let searchParameters = {
      wildcard: this.state.itemName.replace(' ', '%'),
      minimumLevel: this.state.minimumLevel || 0,
      maximumLevel: this.state.maximumLevel || this.constMaximumLevel,
      rarity: this.state.itemQuality,
      slot: this.state.selectedItemSlot ? this.state.selectedItemSlot.split(',') : [],
      mod: this.props.mod ? this.props.mod.mod : '',
      stash: this.props.mod.path,
      isHardcore: this.props.mod.isHardcore,
      filters: requirements,
      duplicatesOnly: this.state.duplicatesOnly, // TODO: Move into misc along with petbonuses
      socketedOnly: this.state.socketedOnly, // TODO: Move into misc along with petbonuses
      isRetaliation: this.state.retaliationOnly, // TODO: Move into misc along with petbonuses
      petBonuses: false, // TODO: Add this to 'misc' and return a more complex object from the filter
      classes: this.state.selectedProfessions.map((p) => `class${p}`)
    };

    const base64 = btoa(JSON.stringify(searchParameters));
    const url = `search://${base64}`;
    console.log(searchParameters);

    if (this.props.onSearch) {
      this.props.onSearch();
    }
    if (this.props.isEmbedded) {
      document.location.href = url;
    } else {
      console.log('Search', url);
    }
  }

  onDamageSelectionChanged(selection: IDropdownEntry[]) {
    this.setState({selectedDamageEntries: selection});
  }

  onResistanceSelectionChanged(selection: IDropdownEntry[]) {
    this.setState({selectedResistanceEntries: selection});
  }

  onMiscSelectionChanged(selection: IDropdownEntry[]) {
    this.setState({selectedMiscEntries: selection});
  }

  onProfessionSelectionChanged(selection: string[]) {
    this.setState({selectedProfessions: selection});
  }

  onCheckChangedDuplicatesOnly(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({duplicatesOnly: event.target.checked});
  }

  onCheckChangedSocketedOnly(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({socketedOnly: event.target.checked});
  }

  onCheckChangedRetaliationOnly(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({retaliationOnly: event.target.checked});
  }

  onItemSlotSelectionChanged(selection: string) {
    console.log('Item slot selection has changed to', selection);
    this.setState({selectedItemSlot: selection});
  }

  onItemQualitySelectionChanged(selection: string) {
    this.setState({itemQuality: selection});
  }

  onItemNameChange(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({itemName: event.target.value});
  }

  onMinimumLevelChange(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({minimumLevel: parseInt(event.target.value, 10) || 0});
  }

  onMaximumLevelChange(event: React.ChangeEvent<HTMLInputElement>) {
    this.setState({maximumLevel: parseInt(event.target.value, 10) || this.constMaximumLevel});
  }

  render() {
    return (
        <div className="disableCaret left-side-filter">
          <input type="text" className="form-control" placeholder="Item name" onChange={(e) => this.onItemNameChange(e as any)} />
          <DamageSelector onSelectionChanged={(x) => this.onDamageSelectionChanged(x)} />
          <ResistanceSelector onSelectionChanged={(x) => this.onResistanceSelectionChanged(x)} />
          <MiscSkillSelector onSelectionChanged={(x) => this.onMiscSelectionChanged(x)} />
          <ConnectedProfessionSelector onSelectionChanged={(x) => this.onProfessionSelectionChanged(x)} />

          <ItemSlotSelector onSelectionChanged={(x) => this.onItemSlotSelectionChanged(x)} />
          <ItemQualitySelector onSelectionChanged={(x) => this.onItemQualitySelectionChanged(x)} />
          <div className="input-group">
            <input type="text" min="0" max={this.constMaximumLevel} className="max-width-50p form-control" placeholder="Min level" onChange={(e) => this.onMinimumLevelChange(e as any)} />
            <input type="text" min="0" max={this.constMaximumLevel} className="max-width-50p form-control" placeholder="Max level" onChange={(e) => this.onMaximumLevelChange(e as any)} />
          </div>
          <Checkbox size={14} onChange={(e) => this.onCheckChangedDuplicatesOnly(e as any)}>Duplicates only</Checkbox>
          <Checkbox size={14} onChange={(e) => this.onCheckChangedSocketedOnly(e as any)}>Socketed only</Checkbox>
          <Checkbox size={14} onChange={(e) => this.onCheckChangedRetaliationOnly(e as any)}>Retaliation items</Checkbox>
          <Button onClick={() => this.search()}>Search</Button>
          <Button disabled={true}>Clear</Button>
        </div>
    );
  }
}

const mapStateToProps = (globalState: GlobalState, state: Props) => {
  return {
    isEmbedded: state.isEmbedded,
    mod: globalState.selectedMod
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    onSearch: () => {
      dispatch(setItems([]));
      dispatch(setLoadingStatus(true));
    }
  } as DispatchProps;
};

// See: https://stackoverflow.com/questions/42657792/typescript-react-redux-property-xxx-does-not-exist-on-type-intrinsicattrib
export default connect<InjectedProps, {}, UserProvidedProps>(mapStateToProps, mapDispatchToProps)(FilterView);
