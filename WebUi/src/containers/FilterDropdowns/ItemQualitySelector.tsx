import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';

interface Props {
  onSelectionChanged: (selection: string) => void;
}

class ItemQualitySelector extends React.Component<Props, object> {
  state = {
    selectedOption: ''
  };

  handleSelectChange = (selectedOption: any) => {
    this.setState({selectedOption});
    this.props.onSelectionChanged(selectedOption);
  }

  render() {
    let items: (IDropdownEntry)[] = [
      {
        label: 'All Qualities',
        value: ''
      },
      {
        label: 'Legendary',
        value: 'Legendary'
      },
      {
        label: 'Epic',
        value: 'Epic'
      },
      {
        label: 'Green',
        value: 'Green'
      },
      {
        label: 'Yellow',
        value: 'Yellow'
      },
    ];

    return (
      <div className="disableCaret">
        <Select
          name="item-quality-selector"
          options={items}
          simpleValue={true}
          clearable={false}
          value={this.state.selectedOption}
          onChange={this.handleSelectChange}
        />
      </div>
    );
  }
}

export default ItemQualitySelector;
