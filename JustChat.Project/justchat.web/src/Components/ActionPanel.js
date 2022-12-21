import React, {useState} from 'react';
import axios from "axios";
import uuid from 'react-uuid';

const ActionPanel = (props) => {
    const [inputText, setInputText] = useState("");
    const [file, setFile] = useState(null);

    const saveFile = (e) => {
        setFile(e.target.files[0]);
    };

    const sendMessage = async () => {
        const formData = new FormData();
        formData.append("messageId", uuid());
        formData.append("userName", props.userName);
        formData.append("whom", props.currentUser.id);
        formData.append("text", inputText);
        formData.append("publishDate", Math.floor(new Date().getTime() / 1000));
        formData.append("image", file);
        const receiver = props.currentUser.connectionId;
        console.log(formData, receiver)
        await axios.post(
            process.env.REACT_APP_URL + `/api/Chat/postmessage?receiver=${receiver}`, formData
        ).then(res => {
            console.log(res)
        })
        setInputText("");
        setFile(null)

    };

    return (
        <div className="input">
            <input className="text"
                   placeholder="Введите сообщение..."
                   onChange={(e) => setInputText(e.target.value)}
                   value={inputText}
            />
            {
                file === null ? (
                    <div className="button">
                        <input className="file"
                               type="file"
                               onChange={saveFile}
                        />
                        <i className="fa-solid fa-paperclip"></i>
                    </div>
                ) : (
                    <div className="button green">
                        <a className="file" onClick={() => setFile(null)}></a>
                        <i className="fa-solid fa-xmark"></i>
                    </div>
                )
            }
            <div className={"button"} onClick={ sendMessage }>
                <i className="fa-regular fa-paper-plane"></i>
            </div>
        </div>
    );
};

export default ActionPanel;