import {h} from "preact";
import Web from '@material-ui/icons/Web';
import Home from '@material-ui/icons/Home';
import OpenInNew from '@material-ui/icons/OpenInNew'; //https://material.io/resources/icons/?style=baseline
import VideoLibrary from '@material-ui/icons/VideoLibrary';
import LiveHelp from '@material-ui/icons/LiveHelp';
import Help from '@material-ui/icons/Help';
import Patreon from './Patreon';

import { openUrl } from '../../integration/integration';
import translate from '../../translations/EmbeddedTranslator';
import './Tabs.css';

export interface Props {
  activeTab: number;
  showVideoGuide: boolean;
  setActiveTab(idx: number): void;
}

export default function Tabs({activeTab, setActiveTab, showVideoGuide}: Props) {
  const renderTabEntry = (label: string, idx: number, icon: any, isActive?: boolean, feature?: string) =>
    <li class={'tab fancyTab ' + (isActive ? 'active' : '')} onClick={() => setActiveTab(idx)} data-feature={feature}>
      <div class="arrow-down">
        <div class="arrow-down-inner"></div>
      </div>
      <a id="tab0" href={'#tabBody' + idx} role="tab" aria-controls={'tabBody' + idx} aria-selected="true" data-toggle="tab" tabIndex={idx}>

        <span class="fa">
          {icon} <br/>
          <span class="hidden-xs">{label}</span>
        </span>
      </a>
      <div class="whiteBlock"></div>
    </li>;

  const renderLinkEntry = (label: string, idx: number, icon: any, url: string) =>
    <li class={'tab fancyTab'} onClick={() => openUrl(url)}>
      <div class="arrow-down">
        <div class="arrow-down-inner"></div>
      </div>
      <a id="tab0" href={'#tabBody' + idx} role="tab" aria-controls={'tabBody' + idx} aria-selected="true" data-toggle="tab" tabIndex={idx}>

        <span class="fa">
          {icon} <br/>
          <span class="hidden-xs">{label}</span>
        </span>

        <span class="external-link"><OpenInNew /></span>
      </a>
      <div class="whiteBlock"></div>
    </li>;


  let idx = 0;
  return <section id="fancyTabWidget" class="tabs t-tabs">
    <ul class="nav nav-tabs fancyTabs" role="tablist">

      {renderTabEntry(translate('app.tab.items'), idx++, <Home/>, activeTab === 0)}
      {renderTabEntry(translate('app.tab.collections'), idx++, <Web/>, activeTab === 1)}
      {renderTabEntry(translate('app.tab.help'), idx++, <Help/>, activeTab === 2, "HelpTab")}
      {renderLinkEntry(translate('app.tab.components'), idx++, <OpenInNew/>, 'https://grimdawn.evilsoft.net/enchantments/')}
      {showVideoGuide && translate('app.tab.videoGuide').length > 0 && renderLinkEntry(translate('app.tab.videoGuide'), idx++, <VideoLibrary/>, translate('app.tab.videoGuideUrl'))}

      {renderLinkEntry(translate('app.tab.discord'), idx++, <LiveHelp/>, 'https://discord.gg/5wuCPbB')}
      {renderLinkEntry('Patreon', idx++, <Patreon/>, 'https://www.patreon.com/itemassistant')}
    </ul>
  </section>;
} // TODO: Discord link should be hardcoded in C# - Not translateable nor duplicated.