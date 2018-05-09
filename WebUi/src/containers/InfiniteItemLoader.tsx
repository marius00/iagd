import { AnyAction, requestInitialItems } from '../actions';
import { connect, Dispatch } from 'react-redux';
import OnScrollLoader from '../components/OnScrollLoader';

interface InfiniteItemLoaderProps {
  onTrigger: () => void;
}

export function mapStateToProps(): {} {
  return {

  };
}

export function mapDispatchToProps(dispatch: Dispatch<AnyAction>): InfiniteItemLoaderProps {
  return {
    onTrigger: () => {
      dispatch(requestInitialItems());
    }
  };
}

export default connect<{}, InfiniteItemLoaderProps>(mapStateToProps, mapDispatchToProps)(OnScrollLoader);
