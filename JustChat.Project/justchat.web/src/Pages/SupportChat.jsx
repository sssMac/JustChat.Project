import React, {useEffect, useState} from 'react';
import "../Styles/SupportChat.scss"
import SideBar from "../Components/SideBar";
import Chat from "../Components/Chat";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import Loader from "../Components/Loader";
import {useNavigate} from "react-router-dom";

const SupportChat = (props) => {
    const [loading, setLoading] = useState();
    const [currentUser, setCurrentUser] = useState()


    useEffect(() => {
        if(!props.isAdmin){
            props.connection.on("ReceiveMyUser", (user) => {
                setCurrentUser(user)
            });
        }
    }, [props.connection]);

    useEffect(() => {
        props.connection.invoke('LeaveRoom').catch(err => console.error(err));

        if(currentUser){
            props.connection.invoke('JoinRoom', currentUser.id).catch(err => console.error(err));
        }
    }, [currentUser]);

    if(!loading){
        return (
            <div className="main">
                <div className="header">
                    <div className="background">
                        <video autoPlay={true} muted>
                            <source
                                src="https://raw.githubusercontent.com/1dxrpz/video-player-test-js/main/videos/background.mp4"
                                type="video/mp4"/>
                        </video>
                    </div>
                    <div className="dot"></div>
                    <div className="heading"><img
                        src="https://raw.githubusercontent.com/1dxrpz/NetFlex-WebProject/e3088567e62e72f40583d4ddcca2503cc62ef4e4/NetFlex.WEB/wwwroot/images/Logo.svg"/>
                        <div className="text">Чач поддержки</div>
                    </div>
                </div>
                <div className="container">
                    {
                        props.isAdmin ? <SideBar connection={props.connection} userName={props.userName} setCurrentUser={setCurrentUser}/> : <div></div>
                    }
                    {
                        currentUser ? <Chat connection={props.connection} currentUser={currentUser} userName={props.userName} isAdmin={props.isAdmin}/> : <div></div>
                    }
                </div>
            </div>
        );
    }
    else{
        return(
            <Loader/>
        )
    }
};

export default SupportChat;