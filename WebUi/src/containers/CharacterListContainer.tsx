import * as React from 'react';
import { getBackedUpCharacters, getCharacterDownloadUrl, openUrl } from '../integration/integration';

let characters = getBackedUpCharacters();
class CharacterListContainer extends React.PureComponent<{}, object> {



  renderCharacter(c: string) {
    const clean = (c: string) => c.startsWith("_") ? c.substr(1) : c;
    const download = (c: string) => {
      console.log('Clickyetyclick');
      const url = getCharacterDownloadUrl(c);

      console.log(`Backup download URL for ${c} is ${url}`);
      if (url) {
        // @ts-ignore
        openUrl(url.url);
      }
    };


    return <li key={c}>
      <a href='#' onClick={() => download(c)}>{clean(c)}</a>
    </li>;
  }

  render() {
    return <div>
      <h1>Backed up characters</h1>
      <ul>
        {characters.map(this.renderCharacter)}
      </ul>
    </div>;
  }
}

export default CharacterListContainer;