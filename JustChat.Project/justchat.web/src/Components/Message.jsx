import React from 'react';

const Message = (props) => {
    return (
        <div className={props.message.message.userName !== props.currentUser.id ? "message income" : "message"}>
        <div className="text">
            <div className="sender">{props.message.message.userName}</div>
            <span>{props.message.message.text}</span>
            {
                props.message.rsp !== null ?
                    <div className="messageImg_container">
                        <img className="messageImg" key={props.message.rsp.id} src={ 'http://localhost:9000/justbucket/' + props.message.rsp.titile } ></img>
                    </div>
                    : <div></div>
            }
        </div>
    </div>
);
};

export default Message;