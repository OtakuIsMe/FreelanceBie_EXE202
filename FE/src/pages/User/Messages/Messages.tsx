import './Messages.css'

interface navProps {
    MessageId: string,
    UserImage: string,
    Username: string,
    LastMessageTime: string,
    LastMessage: string
}
interface Message {
    CreateAt: string
}

const Messages: React.FC = () => {
    return (
        <div id="messages">

        </div>
    )
}

export default Messages;
