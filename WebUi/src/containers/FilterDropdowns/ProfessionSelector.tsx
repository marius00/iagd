import * as React from 'react';
import Select from 'react-select';
import 'react-select/dist/react-select.css';
import IDropdownEntry from '../../interfaces/IDropdownEntry';
import IProfession from '../../interfaces/IProfession';

interface Props {
  onSelectionChanged?: (selection: string[]) => void;
  classes: IProfession[];
}

class ProfessionSelector extends React.Component<Props, object> {
  state = {
    selectedOptions: []
  };

  handleSelectChange = (selectedOptions: IDropdownEntry[]) => {
    this.setState({selectedOptions});
    if (this.props.onSelectionChanged) {
      this.props.onSelectionChanged(selectedOptions.map((e) => e.value));
    }
  }

  render() {
    let items: (IDropdownEntry)[] = this.props.classes;

    return (
      <div className="disableCaret">
        <Select
          name="class-selector"
          options={items}
          placeholder="Choose a class"
          multi={true}
          value={this.state.selectedOptions}
          onChange={this.handleSelectChange}
        />
      </div>
    );
  }
}
export default ProfessionSelector;
