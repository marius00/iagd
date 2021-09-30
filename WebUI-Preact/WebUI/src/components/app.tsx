import { FunctionalComponent, h } from 'preact';
import { Route, Router } from 'preact-router';

import Home from '../routes/home';
import Profile from '../routes/profile';
import NotFoundPage from '../routes/notfound';
import Header from './header';
import Help from "../containers/help/Help";
import IItem from "../interfaces/IItem";
import ICollectionItem from "../interfaces/ICollectionItem";
import {PureComponent} from "preact/compat";
import {isEmbedded, requestMoreItems} from "../integration/integration";
import MockCollectionItemData from "../mock/MockCollectionItemData";
import ReactNotification, { store } from 'react-notifications-component';
import Spinner from "./Spinner";


interface ApplicationState {
    items: IItem[];
    isLoading: boolean;
    activeTab: number;
    collectionItems: ICollectionItem[];
    isDarkMode: boolean;
    helpSearchFilter: string;
    numItems: number;
    showBackupCloudIcon: boolean;
}


class App extends PureComponent<object, object> {
    state = {
        items: [],
        isLoading: true,
        activeTab: 0,
        collectionItems: [],
        isDarkMode: false,
        helpSearchFilter: '',
        numItems: 0,
        showBackupCloudIcon: true,
    } as ApplicationState;

    componentDidMount() {
        // Set the items to show
        // @ts-ignore: setItems doesn't exist on window
        window.setItems = (data: any, numItems: number) => {
            const items = typeof data === 'string' ? JSON.parse(data) : data;
            console.log(data, numItems);
            console.log('==========>', numItems);
            window.scrollTo(0, 0);

            this.setState({
                isLoading: false,
                items: items,
                numItems: numItems || 0
            });
        };

        // Add more items (typically scrolling)
        // @ts-ignore: setItems doesn't exist on window
        window.addItems = (data: any) => {
            const items = [...this.state.items];
            const newItems = typeof data === 'string' ? JSON.parse(data) : data;

            this.setState({
                isLoading: false,
                items: items.concat(newItems)
            });
        };

        // Set collection items
        // @ts-ignore: setCollectionItems doesn't exist on window
        window.setCollectionItems = (data: any) => {
            const collectionItems = typeof data === 'string' ? JSON.parse(data) : data;
            this.setState({
                collectionItems: collectionItems
            });
        };

        // Mock data for not embedded / dev mode
        if (!isEmbedded) {
            this.setState({collectionItems: MockCollectionItemData});
        }

        // Start showing the loading spinner
        // @ts-ignore: setIsLoading doesn't exist on window
        window.setIsLoading = (isLoading: boolean) => {
            this.setState({
                isLoading: isLoading
            });
        };

        // Toggle dark mode from C# container
        // @ts-ignore
        window.setIsDarkmode = (isDarkMode: boolean) => {
            this.setState({
                isDarkMode: isDarkMode
            });
        };

        // Show the help screen
        // @ts-ignore: setIsLoading doesn't exist on window
        window.showHelp = (tag: string) => {
            this.setState({
                activeTab: 2,
                helpSearchFilter: tag
            });
        };

        // Show the help screen
        // @ts-ignore: setIsLoading doesn't exist on window
        window.showCharacterBackups = () => {
            this.setState({
                activeTab: 3,
            });
        };

        // @ts-ignore:
        window.setOnlineBackupsEnabled = (enabled: boolean) => {
            this.setState({
                showBackupCloudIcon: enabled
            });
        };

        // Show a notification message such as "Item transferred" or "Too close to stash"
        // @ts-ignore: showMessage doesn't exist on window
        window.showMessage = (input: string) => {
            let s = JSON.parse(input);
            ShowMessage(s.message, s.type);
        };
    }


    // Used primarily for setting mock items for testing
    setItems(items: IItem[]) {
        this.setState({items: items, isLoading: false});
    }

    // Will eventually be moved into C# once the form supports darkmode
    toggleDarkmode() {
        this.setState({isDarkMode: !this.state.isDarkMode});
    }

    // reduceItemCount will reduce the number of items displayed, generally after an item has been transferred out.
    reduceItemCount(url: object[], numItems: number) {
        let items = [...this.state.items];
        var item = items.filter(e => e.url.join(':') === url.join(':'))[0];
        if (item !== undefined) {
            item.numItems -= numItems;
            this.setState({items: items});
        } else {
            console.warn('Attempted to reduce item count, but item could not be found', url);
        }
    }

    requestMoreItems() {
        console.log('More items it wantssssss?');
        this.setState({isLoading: true});
        requestMoreItems();
        // TODO: Fix this weird loop? This one will request more items.. which will end up in a call from C# to window.addItems().. is that how we wanna do this?
    }


    render() {
        return (
            <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
                <Header/>
                {this.state.isLoading && isEmbedded && <Spinner/>}
                <ReactNotification/>
                <Router>
                    <Route path="/" component={Home}/>
                    <Route path="/profile/" component={Profile} user="me"/>
                    <Route path="/profile/:user" component={Profile}/>
                    <NotFoundPage default/>
                </Router>


                {this.state.activeTab === 2 && <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.setState({helpSearchFilter: v})}/>}
            </div>
        );
    }
};


function ShowMessage(message: string, type: 'success' | 'danger' | 'info' | 'default' | 'warning') {
    let duration = 2500;
    if (type === 'danger') {
        duration = 4500;
    }

    store.addNotification({
        message: message,
        type: type,
        insert: 'top',
        container: 'bottom-center',
        animationIn: ['animate__animated', 'animate__fadeIn'],
        animationOut: ['animate__animated', 'animate__fadeOut'],
        dismiss: {
            duration: duration,
            onScreen: true
        }
    });
}


export default App;
