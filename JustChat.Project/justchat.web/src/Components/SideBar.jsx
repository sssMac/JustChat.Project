import React, {useEffect, useRef, useState} from 'react';
import OnlineUsers from "./OnlineUsers";
import {HubConnectionBuilder} from "@microsoft/signalr";
import axios from "axios";

const SideBar = (props) => {
    const [users, setUsers] = useState([])
    const latestUsers = useRef(null);
    latestUsers.current = users;

    useEffect(() => {
        async function fetchOnlineUsers (){
            return await axios(
                process.env.REACT_APP_URL + '/api/Chat/onlineusers',
            );
        }
        fetchOnlineUsers().then(r => setUsers(r.data))
    }, []);

    useEffect(() => {
        if (props.connection) {
            props.connection.on("ReceiveJoinUser", (user) => {
                const updatedUserList = [...latestUsers.current];
                updatedUserList.push(user);
                setUsers(updatedUserList);
            });

            props.connection.on("ReceiveLeaveUser", (user) => {
                console.log(user)
                setUsers(current =>
                    current.filter(u => {
                        return u.id !== user.id;
                    }))
            });
        }
    }, [props.connection]);



    return (
        <div className="users_wrapper">
            {
               users
                   .filter(v => v.id !== props.userName )
                   .map(user => <OnlineUsers key={user.connectionId} user={user} setCurrentUser={props.setCurrentUser}/>)
            }
        </div>
    );
};

export default SideBar;