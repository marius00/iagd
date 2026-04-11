import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from './NotificationContainer.module.css';
import { CircleCheck, TriangleAlert, CircleAlert, X } from 'lucide-preact';
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
      return <CircleCheck className={styles.icon + " " + styles.success}/>;
    if (type === 'info')
      return <CircleCheck className={styles.icon + " " + styles.success} />;
    if (type === 'default')
      return <CircleCheck className={styles.icon + " " + styles.success}/>;
    if (type === 'warning')
      return <TriangleAlert className={styles.icon}/>;
    if (type === 'danger')
      return <CircleAlert className={styles.icon}/>;
    return "Bug!";
  }

  renderNotification = (notification: NotificationMessage, isLastElement: boolean) => {
    const toastStyle = isLastElement ? (styles.toast + " " + styles.toastLastElement) : styles.toast;
    return (
      <div class={toastStyle}>
        {this.renderIcon(notification.type)}
        {notification.message}
        <span class={styles.close} onClick={() => this.props.onClose(notification.id)}>
          <X/>
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