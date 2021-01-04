## WebUI

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

If IAGrim is started in debug mode, it will also default to the localhost:3000 server before running the prebuilt version.

### `npm build`
Builds the solution

### `npm start:online`

Runs the app for the online items site

### `npm build:online`
Builds the app for online items



## Integration
The C# integration is as follows:
* `window.setItems(items: IItem[])` can be called by C# to update the item list  
* `window.setIsLoading(bool)` can be called by C# to toggle the loading animation
* `window.setIsDarkmode(bool)` can be called by C# to toggle dark mode
* `window.showHelp(string)` can be called by C# to switch to the help tab and search
* `window.showMessage("{message: m, type: t}")` can be called by C# to show a message