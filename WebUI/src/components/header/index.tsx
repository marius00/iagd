import {h} from 'preact';
import {Link} from 'preact-router/match';
import style from './style.css';
import translate from "../../translations/EmbeddedTranslator";
import Patreon from "./Patreon";
import {openUrl} from "../../integration/integration";
import OpenInNew from '@material-ui/icons/OpenInNew'; //https://material.io/resources/icons/?style=baseline
import LiveHelp from '@material-ui/icons/LiveHelp';
import Help from '@material-ui/icons/Help';

import Web from '@material-ui/icons/Web';
import Home from '@material-ui/icons/Home';

export interface Props {
  activeTab: number;
  setActiveTab(idx: number): void;
}

function Header({activeTab, setActiveTab}: Props) {
  return (
    <header class={style.header}>
      <nav>
        <Link activeClassName={style.active} className={activeTab===0 ? style.active : ''} href="#" onClick={() => setActiveTab(0)}>
          <Home/> {translate('app.tab.items')}
        </Link>
        <Link activeClassName={style.active} className={activeTab===1 ? style.active : ''} href="#" onClick={() => setActiveTab(1)}>
          <Web/> {translate('app.tab.collections')}
        </Link>
        <Link activeClassName={style.active} className={activeTab===2 ? style.active : ''} href="#" onClick={() => setActiveTab(2)}>
          <Help/> {translate('app.tab.help')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://grimdawn.evilsoft.net/enchantments/")}>
          <OpenInNew/> {translate('app.tab.components')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://discord.gg/5wuCPbB")}>
          <LiveHelp/> {translate('app.tab.discord')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://www.patreon.com/itemassistant")}>
          <Patreon/> Patreon
        </Link>
      </nav>
    </header>
  );
}

export default Header;
