import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from './NotificationContainer.css';
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

const CheckCircleOutlineIcon = <svg ></svg>

class NotificationContainer extends PureComponent<NotificationProps, object> {
  renderIcon = (type: NotificationTypes) => {
    if (type === 'success')
      return <svg className={styles.icon +  " " +  styles.success} d="M16.59 7.58L10 14.17l-3.59-3.58L5 12l5 5 8-8zM12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"/>;
    if (type === 'info')
      return <svg className={styles.icon +  " " +  styles.success} d="M16.59 7.58L10 14.17l-3.59-3.58L5 12l5 5 8-8zM12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"/>;
    if (type === 'default')
      return <svg className={styles.icon +  " " +  styles.success} d="M16.59 7.58L10 14.17l-3.59-3.58L5 12l5 5 8-8zM12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"/>;
    if (type === 'warning')
      return <svg  className={styles.icon} d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"/>;
    if (type === 'danger')
      return <svg className={styles.icon} d="M11 15h2v2h-2zm0-8h2v6h-2zm.99-5C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"/>;
    return "Bug!";
  }

  renderNotification = (notification: NotificationMessage, isLastElement: boolean) => {
    const toastStyle = isLastElement ? (styles.toast + " " + styles.toastLastElement) : styles.toast;
    return (
      <div class={toastStyle}>
        {this.renderIcon(notification.type)}
        {notification.message}
        <span class={styles.close} onClick={() => this.props.onClose(notification.id)}>
          <svg d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/>
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