import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { Button, Input, notification } from "antd";
import 'antd/dist/antd.css';
import { useEffect, useState } from "react";

export const Notify = () => {
    const [connection, setConnection] = useState<null | HubConnection>(null);
    const [inputText, setInputText] = useState("");

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
                        notification.open({
                            message: "New Notification",
                            description: message,
                        });
                    });
                })
                .catch((error) => console.log(error));
        }
    }, [connection]);

    const sendMessage = async () => {
        if (connection) await connection.send("SendMessage", inputText);
        console.log(inputText)
        setInputText("");
    };

    return (
        <>
            <Input
                value={inputText}
                onChange={(input) => {
                    setInputText(input.target.value);
                }}
            />
            <Button onClick={sendMessage} type="primary">
                Send
            </Button>
        </>
    );
};