import {h} from "preact";
import { CharacterListDto, getBackedUpCharacters, getCharacterDownloadUrl, openUrl } from '../integration/integration';
import './CharacterListContainer.css';
import {PureComponent} from "preact/compat";


class CharacterListContainer extends PureComponent<{}, object> {


  renderCharacter(c: CharacterListDto) {
    const clean = (c: string) => c.startsWith('_') ? c.substr(1) : c;
    const download = (c: string) => {
      console.log('Clickyetyclick');
      const url = getCharacterDownloadUrl(c);

      console.log(`Backup download URL for ${c} is ${url}`);
      if (url) {
        // @ts-ignore
        openUrl(url.url);
      }
    };


    return (
      <table key={c.name} onClick={() => download(c.name)}>
        <tr>
          <td>
            <svg d="M19.35 10.04C18.67 6.59 15.64 4 12 4 9.11 4 6.6 5.64 5.35 8.04 2.34 8.36 0 10.91 0 14c0 3.31 2.69 6 6 6h13c2.76 0 5-2.24 5-5 0-2.64-2.05-4.78-4.65-4.96zM17 13l-5 5-5-5h3V9h4v4h3z"/>
          </td>
          <td>
            <h2>{clean(c.name)}</h2>
            Last modified: {c.updatedAt}
          </td>
        </tr>
      </table>
    );
  }

  render() {
    let characters = getBackedUpCharacters();
    return <div className="characters">
      <h1>Backed up characters</h1>
      <ul>
        {characters.map(this.renderCharacter)}
      </ul>
    </div>;
  }
}

export default CharacterListContainer;