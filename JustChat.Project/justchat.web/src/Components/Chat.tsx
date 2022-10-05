import {useEffect, useState, useRef} from 'react';
import  './Chat.scss';
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import axios from "axios";

const Chat = () => {
    const [connection, setConnection] = useState<null | HubConnection>(null);

    const [inputText, setInputText] = useState<string>("");
    const [userName, setUserName] = useState<string>("")
    const [isLogin, setIsLogin] = useState(true)
    const latestChat = useRef(null);


    const [chatHistory, setChatHistory] = useState([])
    latestChat.current = chatHistory;

    useEffect(() => {
        const fetchData = async () => {
            const result = await axios(
                'https://localhost:7019/api/Chat/chathistory',
            );
            setChatHistory(result.data);
        };
        fetchData();
    }, []);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl("https://localhost:7019/hub")
            .withAutomaticReconnect()
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(() => {
                    connection.on("ReceiveMessage", (message) => {
                        console.log(message)
                        const updatedChat = [...latestChat.current];
                        updatedChat.push(message);

                        setChatHistory(updatedChat);
                        console.log(message)
                    });
                })
                .catch((error) => console.log(error));
        }
    }, [connection]);

    const sendMessage = async () => {
        if (connection){
            console.log(`${userName} --- ${inputText}`)
            await connection.send("SendMessage",userName, inputText );}

        console.log(chatHistory)
        setInputText("");
    };


    return (
        <div  className="Chat">
            <div className="chat_wrapper">
                <div className="banner">
                    <img alt="" src="https://cdn.discordapp.com/attachments/789103816107884546/1024077753907159090/123.png"/>
                    <div className="links"><a className="github first" href="https://github.com/1dxrpz"><i
                        className="fa-brands fa-github"></i></a><a className="github second"
                                                                   href="https://github.com/sssMac"><i
                        className="fa-brands fa-github"></i></a></div>
                </div>
                {
                    (userName === "" || isLogin) ? (
                        <div className="chat">
                            <div className="messages_wrapper">
                                <div className="chatinfo">
                                    <div className="name">Just Chat</div>
                                    <div className="members">Please enter your nickname...</div>
                                </div>
                                <div className="login_wrapper">
                                    <input
                                        placeholder="Nickname"
                                        className="username"
                                        onChange = {(e) => setUserName(e.target.value)}
                                        value={userName}
                                    />
                                    <div className="connect_button" data-enabled="false"
                                         onClick={() => {
                                             setIsLogin(false)
                                         }}>
                                        <i className="fa-solid fa-chevron-right"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="chat logged">
                            <div className="messages_wrapper">
                                <div className="chatinfo">
                                    <div className="name">Just Chat</div>
                                    <div className="members">Talk with random people...</div>
                                </div>
                                <div className="messages">
                                    { chatHistory
                                        .sort((a,b) => b.publishDate - a.publishDate)
                                        .map((chat) =>{
                                            if(chat.userName === userName){
                                                return (
                                                    <div key={chat.messageId} className="message income">
                                                        <div className="text">
                                                            <div className="sender">{chat.userName}</div>
                                                            <span>{chat.text}</span>
                                                        </div>
                                                    </div>
                                                )}
                                            else{
                                                return  (<div key={chat.messageId} className="message">
                                                    <div className="text">
                                                        <div className="sender">{chat.userName}</div>
                                                        <span>{chat.text}</span>
                                                    </div>
                                                </div>)
                                            }
                                        })}
                                    <div className="message">
                                        <div className="info">You joined Support chat</div>
                                        <div className="spacer"></div>
                                    </div>
                                </div>
                                <div className="message_input">
                                    <div className="emoji"><i className="fa-regular fa-face-smile"></i>
                                        <div className="emoji_wrapper" data-enabled="false"></div>
                                        <input className="checkbox" type="checkbox" />
                                    </div>
                                    <input
                                        placeholder="Enter message"
                                        className="chat_input"
                                        onChange={(e) => setInputText(e.target.value)}
                                        value={inputText}
                                    />
                                    <div className="send_button" onClick={ sendMessage }><i
                                        className="fa-regular fa-paper-plane"></i></div>
                                </div>
                            </div>
                        </div>
                    )
                }
            </div>
        </div>
    )
};

export default Chat;
