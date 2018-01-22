import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';

interface Props {
  onSelectionChanged: (selection: IDropdownEntry[]) => void;
}

class ResistanceSelector extends React.Component<Props, object> {
  state = {
    selectedOptions: []
  };

  private static getDamageOverTimeAttributes(damageType: string): string[] {
    const attributes = [
      `defensiveSlow${damageType}`,
      `defensiveSlow${damageType}Modifier`
    ];

    return attributes;
  }

  private static getFlatDamageAttributes(damageType: string): string[] {
    const isElemental = damageType === 'Fire' || damageType === 'Cold' || damageType === 'Lightning';

    if (isElemental) {
      return [
        `defensive${damageType}`,
        `defensive${damageType}Modifier`
      ];
    } else {
      return [
        `defensive${damageType}`,
        `defensive${damageType}Modifier`
      ];
    }
  }

  private static getDamageEntryKeys(): IDropdownEntry[] {
    const items: (IDropdownEntry)[] = [];

    const damageTypes = {
      Physical: 'Physical',
      Pierce: 'Piercing',
      Fire: 'Fire',
      Cold: 'Cold',
      Lightning: 'Lightning',
      Aether: 'Aether',
      Life: 'Vitality',
      Chaos: 'Chaos',
      Poison: 'Poison'
    };

    Object.keys(damageTypes).forEach((key) => {
      items.push({
        label: `${damageTypes[key]} Resistance`,
        value: ResistanceSelector.getFlatDamageAttributes(key).join(',')
      });
    });

    items.push({
      label: 'Elemental Damage',
      value: 'defensiveElementalResistance'
    });

    return items;
  }

  private static getDamageOverTimeEntryKeys(): IDropdownEntry[] {
    const items: (IDropdownEntry)[] = [];

    const damageTypes = {
      Physical: 'Bleeding Resistance',
      Fire: 'Burn Resistance',
      Cold: 'Frost Resistance',
      Lightning: 'Electrocute Resistance',
      Life: 'Vitality Decay Resistance'
    };

    Object.keys(damageTypes).forEach((key) => {
      items.push({
        label: damageTypes[key],
        value: ResistanceSelector.getDamageOverTimeAttributes(key).join(',')
      });
    });

    items.push({
      label: 'Life Leech',
      value: 'offensiveLifeLeechMin,offensiveSlowLifeLeachMin'
    });

    return items;
  }

  // tslint:disable-next-line: no-any
  handleSelectChange = (selectedOptions: any) => {
    this.setState({selectedOptions});
    this.props.onSelectionChanged(selectedOptions);
  }

  render() {
    let items: (IDropdownEntry)[] = [];
    items = items.concat(ResistanceSelector.getDamageEntryKeys());
    items = items.concat(ResistanceSelector.getDamageOverTimeEntryKeys());

    return (
      <div className="disableCaret">
        <Select
          name="resistance-selector"
          options={items}
          placeholder="Choose a Resistance"
          multi={true}
          value={this.state.selectedOptions}
          onChange={this.handleSelectChange}
        />
      </div>
    );
  }
}

export default ResistanceSelector;
