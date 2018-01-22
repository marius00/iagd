import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';

interface Props {
  onSelectionChanged: (selection: IDropdownEntry[]) => void;
}

class DamageSelector extends React.Component<Props, object> {
    state = {
        selectedOptions: []
    };

    private static getDamageOverTimeAttributes(damageType: string): string[] {
        const attributes = [
            `offensiveSlow${damageType}`,
            `offensiveSlow${damageType}Modifier`,
            `offensiveSlow${damageType}ModifierChance`,
            `offensiveSlow${damageType}DurationModifier`,
            `retaliationSlow${damageType}Min`,
            `retaliationSlow${damageType}Chance`,
            `retaliationSlow${damageType}Duration`,
            `retaliationSlow${damageType}DurationMin`,
        ];

        return attributes;
    }

    private static getFlatDamageAttributes(damageType: string): string[] {
        const isElemental = damageType === 'Fire' || damageType === 'Cold' || damageType === 'Lightning';

        if (isElemental) {
            return [
                `offensive${damageType}`,
                `offensive${damageType}Modifier`,
                `offensiveElemental`,
                `offensiveElementalModifier`,
                `offensiveSlow${damageType}`,
                `offensiveSlow${damageType}Modifier`
            ];
        } else {
            return [
                `offensive${damageType}`,
                `offensive${damageType}Modifier`,
                `offensiveSlow${damageType}`,
                `offensiveSlow${damageType}Modifier`
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
            Poison: 'Poison',
            Elemental: 'Elemental'
        };

        Object.keys(damageTypes).forEach((key) => {
            items.push({
                label: `${damageTypes[key]} Damage`,
                value: DamageSelector.getFlatDamageAttributes(key).join(',')
            });
        });

        items.push({
            label: 'Total Damage',
            value: 'offensiveTotalDamageModifier'
        });

        return items;
    }

    private static getDamageOverTimeEntryKeys(): IDropdownEntry[] {
        const items: (IDropdownEntry)[] = [];

        const damageTypes = {
            Physical: 'Bleeding Damage',
            Fire: 'Burn Damage',
            Cold: 'Frost Damage',
            Lightning: 'Electrocute Damage',
            Life: 'Vitality Decay'
        };

        Object.keys(damageTypes).forEach((key) => {
            items.push({
                label: damageTypes[key],
                value: DamageSelector.getDamageOverTimeAttributes(key).join(',')
            });
        });

        items.push({
            label: 'Life Leech',
            value: 'offensiveLifeLeechMin,offensiveSlowLifeLeachMin'
        });

        return items;
    }

    // tslint:disable-next-line: no-any
    handleSelectChange = (selectedOptions: IDropdownEntry[]) => {
        this.setState({selectedOptions});
        this.props.onSelectionChanged(selectedOptions);
    }

    render() {
        let items: (IDropdownEntry)[] = [];
        items = items.concat(DamageSelector.getDamageEntryKeys());
        items = items.concat(DamageSelector.getDamageOverTimeEntryKeys());

        return (
        <div className="disableCaret">
            <Select
                name="damage-selector"
                options={items}
                placeholder="Choose a damage type"
                multi={true}
                value={this.state.selectedOptions}
                onChange={this.handleSelectChange}
            />
        </div>
    );
    }
}

export default DamageSelector;
