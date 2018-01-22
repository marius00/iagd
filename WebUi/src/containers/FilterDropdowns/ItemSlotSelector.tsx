import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';

interface Props {
  onSelectionChanged: (selection: string) => void;
}

class ItemSlotSelector extends React.Component<Props, object> {
  state = {
    selectedOption: {
      label: 'Any Slot',
      value: ''
    }
  };

  private static getEntryKeys(): IDropdownEntry[] {
    const items: (IDropdownEntry)[] = [
      {
        label: 'Any Slot',
        value: ''
      },
      {
        label: 'Head',
        value: 'ArmorProtective_Head'
      },
      {
        label: 'Hands',
        value: 'ArmorProtective_Hands'
      },
      {
        label: 'Feet',
        value: 'ArmorProtective_Feet'
      },
      {
        label: 'Legs',
        value: 'ArmorProtective_Legs'
      },
      {
        label: 'Chest',
        value: 'ArmorProtective_Chest'
      },
      {
        label: 'Waist/Belt',
        value: 'ArmorProtective_Waist'
      },
      {
        label: 'Medal',
        value: 'ArmorJewelry_Medal'
      },
      {
        label: 'Ring',
        value: 'ArmorJewelry_Ring'
      },
      {
        label: 'Shoulders',
        value: 'ArmorProtective_Shoulders'
      },
      {
        label: 'Amulet',
        value: 'ArmorJewelry_Amulet'
      },
      {
        label: 'Weapon (1H)',
        value: 'WeaponMelee_Dagger,WeaponMelee_Mace,WeaponMelee_Axe,WeaponMelee_Scepter,WeaponMelee_Sword'
      },
      {
        label: 'Weapon (2H)',
        value: 'WeaponMelee_Sword2h,WeaponMelee_Mace2h,WeaponMelee_Axe2h'
      },
      {
        label: 'Weapon (Ranged)',
        value: 'WeaponHunting_Ranged2h,WeaponHunting_Ranged1h'
      },
      {
        label: 'Offhand',
        value: 'WeaponArmor_Offhand'
      },
      {
        label: 'Shield',
        value: 'WeaponArmor_Shield'
      },
      {
        label: 'Component',
        value: 'ItemRelic' // This is correct, "Relic" => Component
      },
      {
        label: 'Relic',
        value: 'ItemArtifact' // This is correct, "Artifact" => Relic
      },
    ];

    return items; // TODO: This list should be sorted by label
  }

  // tslint:disable-next-line: no-any
  handleSelectChange = (selectedOption: any) => {
    this.setState({selectedOption});

    if (selectedOption) {
      this.props.onSelectionChanged(selectedOption);
    } else {
      this.props.onSelectionChanged('');
    }
  }

  render() {
    const items: (IDropdownEntry)[] = ItemSlotSelector.getEntryKeys();

    return (
      <div className="disableCaret">
        <Select
          name="item-slot-selector"
          placeholder="Select a slot"
          options={items}
          simpleValue={true}
          clearable={false}
          onBlurResetsInput={false}
          value={this.state.selectedOption}
          onChange={this.handleSelectChange}
        />
      </div>
    );
  }
}

export default ItemSlotSelector;
