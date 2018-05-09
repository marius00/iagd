import * as React from 'react';
import { connect } from 'react-redux';
import { GlobalReducerState } from '../types';
import * as Notifications from 'react-notification-system-redux';

interface Props {
  notifications: object[];
}

class NotificationContainer extends React.Component<Props, {}> {

  render() {
    const {notifications} = this.props;

    // Optional styling
    const style = {
      NotificationItem: { // Override the notification item
        DefaultStyle: { // Applied to every notification, regardless of the notification level
          margin: '10px 5px 2px 1px'
        }
      }
    };

    return (
      <Notifications
        notifications={notifications}
        style={style}
      />
    );
  }
}

export function mapStateToProps(state: GlobalReducerState): Props {
  return {
    notifications: state.notifications
  };
}
export default connect(mapStateToProps)(NotificationContainer);
