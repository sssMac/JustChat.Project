import React, {useEffect, useRef, useState} from 'react';
import Message from "./Message";
import ActionPanel from "./ActionPanel";
import axios from "axios";
import Loader from "./Loader";

const Chat = (props) => {
    const [chatHistory, setChatHistory] = useState([])
    const [loading, setLoading] = useState(false);
    const latestChat = useRef(null);
    latestChat.current = chatHistory;

    const ChatHistoryFunction = async () =>{
        try{
            if(props.currentUser){
                async function fetchChatHistory () {
                    return await axios(
                        process.env.REACT_APP_URL + `/api/Chat/chathistory?user=${props.currentUser.connectionId}`,
                    );
                }
                fetchChatHistory().then(r => {
                    setChatHistory(r.data)
                })
            }
            setLoading(true);
        }
        catch (e){
            console.log(e)
        }
    }

    useEffect(() => {
        const timer = setTimeout(() => {
            ChatHistoryFunction();
        }, 3000);

        return () => clearTimeout(timer);
    }, [props.currentUser]);

    useEffect(() => {
        if (props.connection) {
            props.connection.on("ReceiveMessage", (message) => {
                const updatedChat = [...latestChat.current];
                updatedChat.push(message);
                setChatHistory(updatedChat);
            });
            if(props.isAdmin){
                console.log("--- I AM ADMIN ---")
                props.connection.on("ReceiveGroupMessage", (message) => {
                    const updatedChat = [...latestChat.current];
                    updatedChat.push(message);
                    setChatHistory(updatedChat);

                });
            }
        }
    }, [props.connection]);

    return(
        <div className="chat_wrapper">
            <div className="info_header"></div>
            {
                loading ? (
                    <div className="chat">
                        <div className="messages">
                            { chatHistory
                                .sort((a,b) => b.message.publishDate - a.message.publishDate)
                                .map((chat) =>{
                                    return (
                                        <Message key={chat.message.messageId} message={chat} currentUser={props.currentUser}/>
                                    )
                                })}
                        </div>
                        <ActionPanel currentUser={props.currentUser} userName={props.userName}/>
                    </div>
                ) : (
                    <Loader/>
                )
            }
        </div>
    )
};

export default Chat;