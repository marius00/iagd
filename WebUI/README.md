# WebUI

## Prerequisites
* Nodejs 12 (will _not_ compile on nodejs 18+)


## The Web View
The Web view is written in React and Typescript.
It can run both Embedded inside IAGrim, and stand-alone in a normal browser for debugging purposes.
After first cloning the project, the developer should run "npm install" to download node packages.

When running the project on port 3000 (default), IA will load the live view instead of reading from file. (Only when starting IA in debug mode)

## Icons
The icons needs to be placed in `src\static` which is not checked into git.
They can be copied from `%appdata%\..\local\evilsoft\iagd\storage\static`.

Including icons in the project requires them to be included in the C# project as resources.

_No good reason behind this, just not taken the time to do this properly._ 

## CLI Commands

* `build.cmd` Builds the project and copies the files to the debug folder in IAGD (also does some post-processing)
* `release.cmd` Copies the files to the release folder in IAGD

## NPM Commands
*   `npm install`: Installs dependencies
*   `npm run dev`: Run a development, HMR server
*   `npm run serve`: Run a production-like server
*   `npm run build`: Production-ready build
*   `npm run lint`: Pass TypeScript files using ESLint
