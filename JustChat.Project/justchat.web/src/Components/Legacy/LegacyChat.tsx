import {useEffect, useState, useRef} from 'react';
import  './Chat.scss';
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import axios from "axios";
// @ts-ignore
import uuid from 'react-uuid';

const LegacyChat = () => {
    const [connection, setConnection] = useState<null | HubConnection>(null);

    const [inputText, setInputText] = useState<string>("");
    const [userName, setUserName] = useState<string>("")
    const [isLogin, setIsLogin] = useState(true)
    const latestChat = useRef(null);
    const latestUsers = useRef(null);
    const [file, setFile] = useState();
    const [onlineUsers, setOnlineUsers] = useState([]);
    const saveFile = (e) => {
        setFile(e.target.files[0]);
    };

    const [chatHistory, setChatHistory] = useState([])
    latestChat.current = chatHistory;
    latestUsers.current = onlineUsers;

    useEffect(() => {
        const fetchData = async () => {
            const result = await axios(
                process.env.REACT_APP_URL + '/api/Chat/chathistory',
            );
            setChatHistory(result.data);
            console.log(result.data)
        };

        const fetchOnlineUsers = async() => {
            const  result = await axios(
                process.env.REACT_APP_URL + '/api/Chat/onlineusers' ,
            );

            setOnlineUsers(result.data);
        }

        fetchData();
        fetchOnlineUsers();
    }, []);
    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl(process.env.REACT_APP_URL + "/hub")
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

                        const updatedChat = [...latestChat.current];
                        updatedChat.push(message);
                        setChatHistory(updatedChat);
                    });

                    connection.on("ReceiveJoinUser", (user) => {
                        const updatedUserList = [...latestUsers.current];
                        updatedUserList.push(user);
                        setOnlineUsers(updatedUserList);
                        console.log(user)

                    });

                    //connection.on("ReceiveLeaveUser", (user) => {
                    //    const updatedUserList = [...latestUsers.current];
                    //    updatedUserList.filter(function (ele){
                    //        return ele !== user
                    //    });
                    //    setOnlineUsers(updatedUserList);
                    //    console.log(onlineUsers)
                    //});
                })
                .catch((error) => console.log(error));
        }
    }, [connection]);

    function authHandler (state) {
        if(state) {
            setIsLogin(false)
            try {
                connection.invoke('JoinChat', userName).catch(err => console.error(err));

            }
            catch (err) {
                alert(err);
                console.log('Error while establishing connection: ' + { err })
            }
        }
        else{
            setIsLogin(true)
            try {
                connection.start().then(function () {
                    connection.invoke('LeaveChat', userName).catch(err => console.error(err));
                }).catch(function () {
                    console.log("Error while connecting to hub");
                });
            }
            catch (err) {
                alert(err);
                console.log('Error while establishing connection: ' + { err })
            }
        }



    };
    const sendMessage = async () => {
        if (connection){
            const formData = new FormData();
            formData.append("messageId", uuid());
            formData.append("userName", userName);
            formData.append("text", inputText);
            // @ts-ignore
            formData.append("publishDate", Math.floor(new Date().getTime() / 1000));
            formData.append("image", file);
            await axios.post(
                `https://localhost:7019/api/Chat/postmessage`, formData
            ).then(res => {
                console.log(res)
            })
        }
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
                                    <div className="name">Just LegacyChat</div>
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
                                             authHandler(true)
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
                                    <div className="name">Just LegacyChat</div>
                                    <div className="members">Talk with random people...</div>
                                </div>
                                <div className="messages">
                                    { chatHistory
                                        .sort((a,b) => b.message.publishDate - a.message.publishDate)
                                        .map((chat) =>{
                                            return (
                                                <div key={chat.message.messageId} className={chat.message.userName === userName ? "message income" : "message"}>
                                                    <div className="text">
                                                        <div className="sender">{chat.message.userName}</div>
                                                        <span>{chat.message.text}</span>
                                                        {
                                                            chat.rsp !== null ?
                                                                <div className="messageImg_container">
                                                                    <img className="messageImg" key={chat.rsp.id} src={ 'http://localhost:8000/justbucket/' + chat.rsp.titile } ></img>
                                                                </div>
                                                                : false
                                                        }
                                                    </div>
                                                </div>
                                            )
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
                                    <label className="custom-file-upload">
                                        <input type="file" onChange={saveFile} />
                                        <i className={file === undefined ? "fa fa-cloud-upload" : "fa fa-cloud-upload green"} />
                                    </label>
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

export default LegacyChat;
