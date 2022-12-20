import React, {useEffect, useState} from "react"
import {Routes, Route} from "react-router-dom";
import SupportChat from "./Pages/SupportChat";
import Login from "./Pages/Login";
import {HubConnectionBuilder} from "@microsoft/signalr";

function App() {
    const [userName, setUserName] = useState("")
    const [connection, setConnection] = useState()
    const [isAdmin, setIsAdmin] = useState(false)
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
                           element={<Login setUserName={setUserName} userName={userName} connection={connection} isAdmin={isAdmin} setIsAdmin={setIsAdmin}/>}/>
                    <Route path="/SupportChat" element={<SupportChat userName={userName} connection={connection} isAdmin={isAdmin}/>}/>
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
