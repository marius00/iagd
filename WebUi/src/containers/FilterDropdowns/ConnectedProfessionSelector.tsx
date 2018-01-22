import 'react-select/dist/react-select.css';
import { GlobalState } from '../../types';
import ProfessionSelector from './ProfessionSelector';
import { connect, Dispatch } from 'react-redux';
import IProfession from '../../interfaces/IProfession';
import { AnyAction } from '../../actions';

export interface ProvidedProps {
  onSelectionChanged?: (selection: string[]) => void;
}

interface DispatchProps {
  onSelectionChanged?: (selection: string[]) => void;
}

export interface AllProps {
  onSelectionChanged?: (selection: string[]) => void;
  classes: IProfession[];
}

export function mapStateToProps(state: GlobalState, providedProps: ProvidedProps): AllProps {
  return {
    classes: state.classes,
    onSelectionChanged: providedProps.onSelectionChanged
  };
}
// TODO: Remove this
export function mapDispatchToProps(dispatch: Dispatch<AnyAction>, x: ProvidedProps): DispatchProps {
  return {
    onSelectionChanged: x.onSelectionChanged
  };
}

export default connect<AllProps, ProvidedProps, DispatchProps>(mapStateToProps)(ProfessionSelector);
