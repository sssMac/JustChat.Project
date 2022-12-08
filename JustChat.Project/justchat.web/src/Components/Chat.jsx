import React, {useEffect, useRef, useState} from 'react';
import Message from "./Message";
import ActionPanel from "./ActionPanel";
import axios from "axios";

const Chat = (props) => {
    const [chatHistory, setChatHistory] = useState([])
    const latestChat = useRef(null);
    latestChat.current = chatHistory;

    useEffect(() => {
        if(props.currentUser){
            async function fetchChatHistory () {
                return await axios(
                    process.env.REACT_APP_URL + `/api/Chat/chathistory?user=${props.currentUser.connectionId}`,
                );
            }
            fetchChatHistory().then(r => setChatHistory(r.data))
        }
    }, [props.currentUser]);
    useEffect(() => {
        if (props.connection) {
            props.connection.on("ReceiveMessage", (message) => {

                const updatedChat = [...latestChat.current];
                updatedChat.push(message);
                setChatHistory(updatedChat);
            });
        }
    }, [props.connection]);

    return (
        <div className="chat_wrapper">
            <div className="info_header"></div>
            <div className="chat">
                { chatHistory
                    .sort((a,b) => b.message.publishDate - a.message.publishDate)
                    .map((chat) =>{
                        return (
                            <Message key={chat.message.messageId} message={chat} currentUser={props.currentUser}/>
                        )
                    })}

                <ActionPanel currentUser={props.currentUser} userName={props.userName}/>
            </div>
        </div>
    );
};

export default Chat;