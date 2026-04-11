# WebUI

## Prerequisites
* Node.js 20 LTS
* Optional: Install [fnm](https://github.com/Schniz/fnm) and run `fnm use` in this directory

## The Web View
The Web view is written in Preact and TypeScript, built with Vite.
It can run both embedded inside IAGrim, and stand-alone in a normal browser for debugging purposes.
After first cloning the project, run `npm install` to download node packages.

When running the project on port 3000 (default), IA will load the live view instead of reading from file. (Only when starting IA in debug mode)

## Icons
The game icons need to be placed in `src\static` which is not checked into git.
They can be copied from `%appdata%\..\local\evilsoft\iagd\storage\static`.

For the **dev server**, the `vite.config.ts` automatically serves `src/static` at `/static/`.

Including icons in the production build requires them to be included in the C# project as resources.

_No good reason behind this, just not taken the time to do this properly._

## CLI Commands

* `build.cmd` Builds the project and copies the files to the debug folder in IAGD
* `deploy.cmd` Copies the files to both debug and release folders in IAGD

## NPM Commands
*   `npm install`: Installs dependencies
*   `npm run dev`: Run a development, HMR server (port 3000)
*   `npm run serve`: Run a production-like preview server (port 3000)
*   `npm run build`: Production-ready build (output to `build/`)
*   `npm run lint`: Lint TypeScript files using ESLint
