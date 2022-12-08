import React, {useEffect, useState} from "react"
import {Routes, Route} from "react-router-dom";
import SupportChat from "./Pages/SupportChat";
import Login from "./Pages/Login";
import {HubConnectionBuilder} from "@microsoft/signalr";

function App() {
    const [userName, setUserName] = useState("")
    const [connection, setConnection] = useState()

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl(process.env.REACT_APP_URL + "/hub")
            .withAutomaticReconnect()
            .build();

        setConnection(connect);
    }, []);

    if(connection) {
        return (
            <div className="App">
                <Routes>
                    <Route path="/"
                           element={<Login setUserName={setUserName} userName={userName} connection={connection}/>}/>
                    <Route path="/SupportChat" element={<SupportChat userName={userName} connection={connection}/>}/>
                </Routes>

            </div>

        );
    }
    else{
        return (
            <div></div>
        );
    }
}

export default App;
