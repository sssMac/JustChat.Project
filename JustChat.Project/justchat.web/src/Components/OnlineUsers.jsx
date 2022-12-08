import React from 'react';

const OnlineUsers = (props) => {
    return (
        <div className="user_card_wrapper">
            <input className="checkbox"
                   type="radio"
                   name="user"
                   onChange={e => props.setCurrentUser(JSON.parse(e.target.value))}
                   value={JSON.stringify(props.user)}
            />
            <div className="user_card">
                <div className="avatar"><img
                    src={`https://ui-avatars.com/api/?background=0D8ABC&color=fff&bold=true&name=${props.user.id}&size=256&length=2`}/>
                </div>
                <div className="name">{props.user.id}</div>
            </div>
        </div>
    );
};

export default OnlineUsers;