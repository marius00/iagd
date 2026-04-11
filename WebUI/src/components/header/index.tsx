import {h} from 'preact';
import style from './style.module.css';
import translate from "../../translations/EmbeddedTranslator";
import Patreon from "./Patreon";
import { openUrl } from "../../integration/integration";
import { ExternalLink, MessageCircleQuestion, CircleHelp, Globe, House } from 'lucide-preact';

export interface Props {
  activeTab: number;
  setActiveTab(idx: number): void;
}

function Header({activeTab, setActiveTab}: Props) {
  return (
    <header class={style.header}>
      <nav>
        <a className={activeTab===0 ? style.active : ''} href="#" onClick={() => setActiveTab(0)}>
          <span class={style.navIcon}><House /></span> {translate('app.tab.items')}
        </a>
        <a className={activeTab===1 ? style.active : ''} href="#" onClick={() => setActiveTab(1)}>
          <span class={style.navIcon}><Globe /></span> {translate('app.tab.collections')}
        </a>
        <a className={activeTab===2 ? style.active : ''} href="#" onClick={() => setActiveTab(2)}>
          <span class={style.navIcon}><CircleHelp /></span> {translate('app.tab.help')}
        </a>
        <a href="#" onClick={() => openUrl("https://grimdawn.evilsoft.net/enchantments/")}>
          <span class={style.navIcon}><ExternalLink /></span> {translate('app.tab.components')}
        </a>
        <a href="#" onClick={() => openUrl("https://discord.gg/5wuCPbB")}>
          <span class={style.navIcon}><MessageCircleQuestion /></span> {translate('app.tab.discord')}
        </a>
        <a href="#" onClick={() => openUrl("https://www.patreon.com/itemassistant")}>
          <span><Patreon /></span> Patreon
        </a>
      </nav>
    </header>
  );
}

export default Header;
