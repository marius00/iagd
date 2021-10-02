import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from './NotificationContainer.css';
import CheckCircleOutlineIcon from '@material-ui/icons/CheckCircleOutline';
import WarningIcon from '@material-ui/icons/Warning';
import ErrorOutlineIcon from '@material-ui/icons/ErrorOutline';
import CloseIcon from '@material-ui/icons/Close';
import translate from "../translations/EmbeddedTranslator";

type NotificationTypes = 'success' | 'danger' | 'info' | 'default' | 'warning';

export interface NotificationMessage {
  id: string;
  message: string;
  type: NotificationTypes;
}

export interface NotificationProps {
  notifications: NotificationMessage[];
  onClose: (id?: string) => void;
}


class NotificationContainer extends PureComponent<NotificationProps, object> {
  renderIcon = (type: NotificationTypes) => {
    if (type === 'success')
      return <CheckCircleOutlineIcon className={styles.icon +  " " +  styles.success}/>;
    if (type === 'info')
      return <CheckCircleOutlineIcon className={styles.icon +  " " +  styles.success} />;
    if (type === 'default')
      return <CheckCircleOutlineIcon className={styles.icon +  " " +  styles.success}/>;
    if (type === 'warning')
      return <WarningIcon color={"secondary"} className={styles.icon}/>;
    if (type === 'danger')
      return <ErrorOutlineIcon color={"secondary"} className={styles.icon}/>;
    return "Bug!";
  }

  renderNotification = (notification: NotificationMessage, isLastElement: boolean) => {
    const toastStyle = isLastElement ? (styles.toast + " " + styles.toastLastElement) : styles.toast;
    return (
      <div class={toastStyle}>
        {this.renderIcon(notification.type)}
        {notification.message}
        <span class={styles.close} onClick={() => this.props.onClose(notification.id)}>
          <CloseIcon/>
        </span>
        {isLastElement && <span class={styles.closeAll} onClick={() => this.props.onClose()}>{translate('notification.clearall')}</span>}
      </div>
    );
  }

  render() {
    const lastElemIdx = this.props.notifications.length - 1;
    return (
      <div class={styles.NotificationContainer}>
        {this.props.notifications.map((elem, idx: number) => this.renderNotification(elem, idx === lastElemIdx))}
      </div>
    );
  }
}

export default NotificationContainer;