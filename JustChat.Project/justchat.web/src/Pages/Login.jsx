import React, {useState} from 'react';
import {useNavigate} from "react-router-dom";
import "../Styles/Login.scss"

const Login = (props) => {
    let navigate = useNavigate();
    const routeChange = () =>{
        try {
            props.connection.start().then(() => {
                props.connection.invoke('JoinChat', props.userName).catch(err => console.error(err));
            })
        }
        catch (err) {
            navigate("/")
            alert(err);
            console.log('Error while establishing connection: ' + { err })
        }
        let path = `/SupportChat`;
        navigate(path);
    }

    return (
        <div className="login_wrapper">
            <input
                placeholder="Nickname"
                className="username"
                onChange = {(e) => props.setUserName(e.target.value)}
                value={props.userName}
            />
            <div className="connect_button" data-enabled="false"
                 onClick={routeChange}>
                <i className="fa-solid fa-chevron-right"></i>
            </div>
        </div>
    );
};

export default Login;