import {h} from "preact";
import { CharacterListDto, getBackedUpCharacters, getCharacterDownloadUrl, openUrl } from '../integration/integration';
import Icon from '@material-ui/icons/CloudDownload';
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
            <Icon/>
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