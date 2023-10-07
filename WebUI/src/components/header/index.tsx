import {h} from 'preact';
import {Link} from 'preact-router/match';
import style from './style.css';
import translate from "../../translations/EmbeddedTranslator";
import Patreon from "./Patreon";
import {openUrl} from "../../integration/integration";

export interface Props {
  activeTab: number;
  showVideoGuide: boolean;

  setActiveTab(idx: number): void;
}

function Header({activeTab, setActiveTab, showVideoGuide}: Props) {
  const videoGuideVisible = showVideoGuide && translate('app.tab.discord').length > 0;
  return (
    <header class={style.header}>
      <nav>
        <Link activeClassName={style.active} className={activeTab===0 ? style.active : ''} href="#" onClick={() => setActiveTab(0)}>
          <svg d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"/> {translate('app.tab.items')}
        </Link>
        <Link activeClassName={style.active} className={activeTab===1 ? style.active : ''} href="#" onClick={() => setActiveTab(1)}>
          <svg d="M20 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm-5 14H4v-4h11v4zm0-5H4V9h11v4zm5 5h-4V9h4v9z"/> {translate('app.tab.collections')}
        </Link>
        <Link activeClassName={style.active} className={activeTab===2 ? style.active : ''} href="#" onClick={() => setActiveTab(2)}>
          <svg d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 17h-2v-2h2v2zm2.07-7.75l-.9.92C13.45 12.9 13 13.5 13 15h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H8c0-2.21 1.79-4 4-4s4 1.79 4 4c0 .88-.36 1.68-.93 2.25z"/> {translate('app.tab.help')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://grimdawn.evilsoft.net/crafting/?q=records/items/gearrelic/b005_relic.dbr")}>
          <svg d="M19 19H5V5h7V3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2v-7h-2v7zM14 3v2h3.59l-9.83 9.83 1.41 1.41L19 6.41V10h2V3h-7z"/> {translate('app.tab.crafting')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://grimdawn.evilsoft.net/enchantments/")}>
          <svg d="M19 19H5V5h7V3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2v-7h-2v7zM14 3v2h3.59l-9.83 9.83 1.41 1.41L19 6.41V10h2V3h-7z"/> {translate('app.tab.components')}
        </Link>
        {videoGuideVisible && <Link activeClassName={style.active} href="#" onClick={() => openUrl(translate('app.tab.videoGuideUrl'))}>
            <svg d="M4 6H2v14c0 1.1.9 2 2 2h14v-2H4V6zm16-4H8c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zm-8 12.5v-9l6 4.5-6 4.5z"/> {translate('app.tab.videoGuide')}
        </Link>}
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://discord.gg/5wuCPbB")}>
          <svg d="M19 2H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h4l3 3 3-3h4c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zm-6 16h-2v-2h2v2zm2.07-7.75l-.9.92C13.45 11.9 13 12.5 13 14h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H8c0-2.21 1.79-4 4-4s4 1.79 4 4c0 .88-.36 1.68-.93 2.25z"/> {translate('app.tab.discord')}
        </Link>
        <Link activeClassName={style.active} href="#" onClick={() => openUrl("https://www.patreon.com/itemassistant")}>
          <Patreon/> Patreon
        </Link>
      </nav>
    </header>
  );
};

export default Header;
