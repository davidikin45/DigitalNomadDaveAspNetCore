# Getting Started with React
## Setting up Environment
1. [Download and install Visual Studio Code](https://code.visualstudio.com/)
2. [Download and install Node.js](https://nodejs.org/en/download/)
3. Open Visual Studio Code
4. Open extensions (Ctrl+Shift+X) 
5. Install the [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) extension
6. Install [Chrome React Developer Tools](https://chrome.google.com/webstore/detail/react-developer-tools/fmkadmapgofadopljbjfkapdkoienihi?hl=en)

## Creating a new App and Git Repository
1. To create a new app open a command line and use the [create-react-app](https://reactjs.org/docs/create-a-new-react-app.html#create-react-app) command where my-app is the name of the new app.
```
npx create-react-app my-app
```
## Setting up App for Development
1. Open a command prompt and direct to the my-app directory.
2. Launch Visual Studio Code by running the following command
```
cd my-app
code .
```
3. Open extensions (Ctrl+Shift+X) 
4. Install the [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) extension
5. Press Reload
6. To configure the [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) press (Ctrl+Shift+D)
7. Click the cog
8. Choose Chrome from the environment dropdown
9. Set launch.json to the following.  --disable-web-security will bypass CORS.
```
{
    "version": "0.2.0",
    "configurations": [
        
        {
            "type": "chrome",
            "request": "launch",
            "name": "Launch Chrome against localhost",
            "url": "http://localhost:3000",
            "webRoot": "${workspaceRoot}/src",
            "runtimeArgs": ["--disable-web-security"]
        }
    ]
}
```

## Launching App for Development
1. Open a command prompt and direct to the my-app directory.
2. To launch the app on http://localhost:3000/ run the npm start command.
```
cd my-app
npm install
npm start
```

## Building App for Production
1. Open a command prompt and direct to the my-app directory.
2. run the npm run build command.
```
npm run build
```
3. app will be output to my-app\build

## Running Production Build
1. Open a command prompt and direct to the my-app directory.
2. Install static server
```
yarn global add serve
```
3. Launch the static server
```
serve -s build
```

## Playground
* https://jscomplete.com/repl

## Package Managers
Lets you take advantage of a vast ecosystem of third-party packages, and easily install or update them.
* [Yarn](https://yarnpkg.com/)
* [npm](https://www.npmjs.com/)

## bundlers
Lets you write modular code and bundle it together into small packages to optimize load time.
* [webpack](https://webpack.js.org/)
* [Parcel](https://parceljs.org/)

## Compilers
Lets you write modern JavaScript code that still works in older browsers.
* [Babel](http://babeljs.io/)

## PluralSight Courses
* [React.js: Getting Started](https://www.pluralsight.com/courses/react-js-getting-started)
* [A Practical Start with React](https://app.pluralsight.com/library/courses/react-practical-start/table-of-contents)
* [Building a Website with React and ASP.NET Core](https://www.pluralsight.com/courses/aspdotnet-core-react-building-website)

## Authors

* **David Ikin**