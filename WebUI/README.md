# WebUI

## Prerequisites
* Nodejs 12 (will _not_ compile on nodejs 18+)


## The Web View
The Web view is written in React and Typescript.
It can run both Embedded inside IAGrim, and stand-alone in a normal browser for debugging purposes.
After first cloning the project, the developer should run "npm install" to download node packages.

When running the project on port 3000 (default), IA will load the live view instead of reading from file. (Only when starting IA in debug mode)

## CLI Commands

* `build.cmd` Builds the project and copies the files to the debug folder in IAGD (also does some post-processing)
* `release.cmd` Copies the files to the release folder in IAGD

## NPM Commands
*   `npm install`: Installs dependencies

*   `npm run dev`: Run a development, HMR server

*   `npm run serve`: Run a production-like server

*   `npm run build`: Production-ready build

*   `npm run lint`: Pass TypeScript files using ESLint

*   `npm run test`: Run Jest and Enzyme with
    [`enzyme-adapter-preact-pure`](https://github.com/preactjs/enzyme-adapter-preact-pure) for
    your tests


For detailed explanation on how things work, checkout the [CLI Readme](https://github.com/developit/preact-cli/blob/master/README.md).
