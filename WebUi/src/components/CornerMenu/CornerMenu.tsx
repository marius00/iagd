import * as React from 'react';
import { Button, DropdownButton, MenuItem } from 'react-bootstrap';
import './CornerMenu.css';
import { isEmbedded } from '../../constants';
import IMod from '../../interfaces/IMod';
import { connect } from 'react-redux';
import { GlobalState } from '../../types';
import { setSelectedMod } from '../../actions';
import * as Guid from 'guid';

const Image = require('./paypal.png');

interface Props {
  mods: IMod[];
  selectedMod: IMod;
}

interface DispatchedProps {
  onModChanged: (mod: IMod) => void;
}

class CornerMenu extends React.Component<Props & DispatchedProps, {}> {
  zoomLevel: number = 1.0;
  zoomIncrement: number = 0.25;
  errorLabel: 'Error';

  incrementZoom() {
    this.zoomLevel += this.zoomIncrement;
    if (isEmbedded) {
      document.location.href = `setZoom://${this.zoomLevel}`;
    } else {
      console.log(`IncrementZoom to ${this.zoomLevel}`);
    }
  }

  decrementZoom() {
    this.zoomLevel = Math.max(0.1, this.zoomLevel - this.zoomIncrement);
    if (isEmbedded) {
      document.location.href = `setZoom://${this.zoomLevel}`;
    } else {
      console.log(`DecrementZoom to ${this.zoomLevel}`);
    }
  }

  openPaypal() {
    if (isEmbedded) {
      document.location.href = 'http://grimdawn.dreamcrash.org/ia/?donate';
    } else {
      window.open('http://grimdawn.dreamcrash.org/ia/?donate');
    }
  }

  onModSelection(mod: IMod) {
    console.log(mod.path);
    if (mod.path !== this.props.selectedMod.path) {
      this.setState({selectedItem: mod});
      this.props.onModChanged(mod);
    }
  }

  render() {
    console.log('render with', this.props);
    const title = this.props.selectedMod && this.props.selectedMod.label ? this.props.selectedMod.label : '- Loading.. -';
    return (
      <div className="corner-menu">
        <DropdownButton title={title} id="mod-selection-dropdown">
          {this.props.mods.map((mod) =>
            <MenuItem key={Guid.raw()} onClick={(e) => this.onModSelection(mod)} disabled={!mod.isEnabled}>
              {mod.label}
            </MenuItem>)
          }
        </DropdownButton>
        <Button onClick={() => this.decrementZoom()}>-</Button>
        <Button onClick={() => this.incrementZoom()}>+</Button>
        <Button onClick={() => this.openPaypal()}><img src={Image} height="16" /></Button>
      </div>
    );
  }
}

const mapStateToProps = (state: GlobalState): Props => {
  return {
    mods: state.mods,
    selectedMod: state.selectedMod
  };
};

const mapDispatchToProps = (dispatch) => {
  return { onModChanged: (m: IMod) => dispatch(setSelectedMod(m)) } as DispatchedProps;
};

export default connect<Props, DispatchedProps>(mapStateToProps, mapDispatchToProps)(CornerMenu);
