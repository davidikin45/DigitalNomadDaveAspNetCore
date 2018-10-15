# Getting Started with React

## Setting up Environment
1. [Download and install Visual Studio Code](https://code.visualstudio.com/)
2. [Download and install Node.js](https://nodejs.org/en/download/)
3. Open command prompt and run
```
npm install npm@latest -g
```
4. Open Visual Studio Code
5. Click File > Auto Save
6. Click View > Terminal
7. Open extensions (Ctrl+Shift+X) 
8. Install the [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) extension
9. Install the [Simple React Snippets](https://marketplace.visualstudio.com/items?itemName=burkeholland.simple-react-snippets) (See below for commands)
10. Install [Chrome React Developer Tools](https://chrome.google.com/webstore/detail/react-developer-tools/fmkadmapgofadopljbjfkapdkoienihi?hl=en)

## Creating a new App with create-react-react, npm and git repo
1. To create a new app open a command line and use the [create-react-app](https://reactjs.org/docs/create-a-new-react-app.html#create-react-app) command where my-app is the name of the new app.
```
npx create-react-app my-app --use-npm
```
2. Install extra packages
```
npm install bootstrap@4
npm install --save prop-types
npm install --save react-router-dom
```
3. Add the following import to src/index.js
```
import '../node_modules/bootstrap/dist/css/bootstrap.min.css';
```
4. Create the following folders.
```
views\application
components
assets\css
assets\fonts
assets\images
assets\svg
```
5. Copy the following files into views\application. Update the index.js to import the App from /views/application.
```
App.js
App.test.js
```
6. Copy the following files into assets\svg. Update the App.js to import the logo from /assets/svg/.
```
logo.svg
```
7. Copy the following files into assets\css. Update the App.js to import the css from /assets/css/.
```
App.css
```
8. To generate the build scripts and config run the following commands. Alternatively will depend on react-scripts.
```
cd my-app
npm run eject
```

## Progressive Web App
* By default the create-react-app creates a fully functional Progressive Web App which works offline by using the service worker. Great for occasionally connected devices without internet but may require extra coding.
* All requests will go to the service worker and at regular interval the service worker will see if there are any updates
* This can be switched on/off by changed the last list in index.js
```
serviceWorker.register();
serviceWorker.unregister();
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

## Launching App for Development from Command Prompt
1. Open a command prompt and direct to the my-app directory.
2. To launch the app on http://localhost:3000/ run the npm start command.
```
cd my-app
npm install
npm start
```

## Launching App for Development with Visual Studio Code
1. Open a command prompt and direct to the my-app directory.
2. Launch Visual Studio Code

```
code .
```
3.  Click View > Terminal
4. Run the following command
```
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

## React Routing
1. install react-router-dom
```
npm install --save react-router-dom
```
2. Add the BrowserRouter as top level App component and a Switch component within
```
import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import { BrowserRouter, Link, Switch, Route } from 'react-router-dom'

const Root = () => (
  <h2>Home component</h2>
)

const Search = () => (
  <h2>Search component</h2>
)

const List = () => (
  <h2>List component</h2>
)

class App extends Component {
  render() {
    return (
      <BrowserRouter>
        <div className="App">
          <header className="App-header">
            <img src={logo} className="App-logo" alt="logo" />
            <h1 className="App-title">Welcome to React</h1>
          </header>
          <ul>
              <li><Link to='/'>Home</Link></li>
              <li><Link to='/search'>Search</Link></li>
              <li><Link to='/list'>List</Link></li>
            </ul>
          <Switch>
              <Route exact path='/' component={Root} />
              <Route path='/search' component={Search} />
              <Route path='/list' component={List} />
            </Switch>
        </div>
      </BrowserRouter>
    );
  }
}

export default App;

```


## React Development Workflow
 JSX > Babel (react-scripts) > React JavaScript (react) > React Dom (react-dom) > Html

## React Component
1. Props
2. State
3. Render

## React vs Angular vs Vue
| React        | Angular           | Vue  |
| ------------- |:-------------:| --------:|
| Library     | Framework | Library |
| Facebook      | Google      |   Community |
| MIT | MIT      |    MIT |
| 1-way data binding | 2-way data binding      |    1-way data binding |
| UI in JavaScript | UI in HTML      |    UI in HTML |

## Import/Export Module Components
```
export component;
import {component} from "./module";

export default component;
import comp from "./module";

export default component, x;
import comp, {x} from "./module";
```

## What is State?
Private Data for the Component
Calling setState triggers re-render of component
Calling setState only updates that particular state item
if the data doesn't need to change (not used in the Render method) then store it in a private property such as this.property
Initialize state with state = {};

## Example Function Component (View Component)
```
import React from 'react';
import logo from '../../assets/svg/logo.svg';

const Header = (props) => (
 <header className="row">
    <div className="col-md-5">
        <img src={logo} className="logo" alt="logo" />
    </div>
    <div className="col-md-7 mt-5 subtitle">
        Providing houses world wide
    </div>
 </header>
);

export default Header; 
```

## Simple React Snippet for Function Component
imr + tab = import React
```
import React from 'react';
```
sfc = stateless function component
```
const FeaturedHouse = (props) => {
    return (  );
}
 
export default FeaturedHouse;
```

## Example Class Component (State Component)
```
import React, { Component } from 'react';
import '../../assets/css/App.css';
import Header from '../../components/header/header';

class App extends Component {
  state = {};

  componentDidMount(){
    //Good place to fetch data
    this.fetchHouses();
  }

  fetchHouses =() =>{
    fetch('/houses.json')
    .then(rsp => rsp.json()) 
    .then(allHouses => {
      this.allHouses = allHouses;
      this.determineFeaturedHouse();
    })
  }

  determineFeaturedHouse = () => {
    if (this.allHouses) {
      const randomIndex = Math.floor(Math.random() * this.allHouses.length);
      const featuredHouse =  this.allHouses[randomIndex];
      this.setState({ featuredHouse });
    };
  }

  render() {
    return (
     <div className="container">
     <Header subtitle="Providing houses all over the world"/>
     </div>
    );
  }
}

export default App;

```

## Simple React Snippet for Class Component
imrc + tab = import React Component
```
import React, { Component } from 'react';
```
cc = class component
```
class House extends Component {
    state = {  }
    render() { 
        return (  );
    }
}
 
export default House;
```

## Lifecycle Methods Mounting
```
constructor()
render()
componentDidMount()
```

## Lifecycle Methods Updating
```
getDerivedStateFromProps()
shouldComponentUpdate()
render()
componentDidUpdate()
```

## Lifecycle Methods Unmount
```
componentWillUnmount()
```

## Lifecycle Methods Error
```
componentDidCatch()
```

## Props Type Safety
```
npm install --save prop-types
import PropTypes from 'prop-types';
House.propTypes = {house: PropTypes.object.isRequired}
```

## Testing with [Jest](https://jestjs.io/docs/en/tutorial-react.html)
1. Run the following command
```
npm test
```

## Flux and [Redux](https://redux.js.org)
* Flux is an architectural pattern
* Redux is an implementation of the pattern
* For complex applications
* New concept around state and UI updates
* State stored outside of components
* Action 1 > 1 Dispatcher 1 > m Store  > 1 View

## New Projects with .NET Core
* [dotnet new react -o my-new-app](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-2.1&tabs=visual-studio)
* [dotnet new reactredux -o my-new-app](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa/react-with-redux?view=aspnetcore-2.1)

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