import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';

interface Props {
  onSelectionChanged: (selection: IDropdownEntry[]) => void;
}

class MiscSkillSelector extends React.Component<Props, object> {
  state = {
    selectedOptions: []
  };

  private static getEntryKeys(): IDropdownEntry[] {
    const items: (IDropdownEntry)[] = [
      {
        label: 'Attack Speed',
        value: 'characterAttackSpeedModifier,characterAttackSpeed,characterTotalSpeedModifier'
      },
      {
        label: 'Cast Speed',
        value: 'characterSpellCastSpeedModifier,characterTotalSpeedModifier'
      },
      {
        label: 'Run Speed',
        value: 'characterRunSpeedModifier,characterTotalSpeedModifier'
      },
      {
        label: 'Experience Gain',
        value: 'characterIncreasedExperience'
      },
      {
        label: 'Damage Reflect',
        value: 'defensiveReflect'
      },
      {
        label: 'Life Leech',
        value: 'offensiveLifeLeechMin,offensiveSlowLifeLeachMin'
      },
      {
        label: 'Health / Life',
        value: 'characterLifeModifier,characterLife'
      },
      {
        label: 'Defensive Ability',
        value: 'characterDefensiveAbilityModifier,characterDefensiveAbility'
      },
      {
        label: 'Offensive Ability',
        value: 'characterOffensiveAbilityModifier,characterOffensiveAbility'
      },
      {
        label: 'Augment Mastery',
        value: 'augmentMastery1,augmentMastery2'
      },
      {
        label: 'Pet Bonuses',
        value: 'petBonusName'
      },
      {
        label: 'Set Bonus',
        value: 'setName,itemSetName'
      },
      {
        label: 'Shield / Block',
        value: 'blockAbsorption,defensiveBlock,defensiveBlockChance,defensiveBlockModifier,defensiveBlockAmountModifier'
      }
    ];

    return items;
  }

  // tslint:disable-next-line: no-any
  handleSelectChange = (selectedOptions: any) => {
    this.setState({selectedOptions});
    this.props.onSelectionChanged(selectedOptions);
  }

  render() {
    const items: (IDropdownEntry)[] = MiscSkillSelector.getEntryKeys();

    return (
      <div className="disableCaret">
        <Select
          name="misc-skill-selector"
          options={items}
          placeholder="Choose a stat"
          multi={true}
          value={this.state.selectedOptions}
          onChange={this.handleSelectChange}
        />
      </div>
    );
  }
}

export default MiscSkillSelector;
