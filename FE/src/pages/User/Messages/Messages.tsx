import { useState } from 'react'
import './Messages.css'

interface navProps {
    messageId: string,
    userImage: string,
    username: string,
    lastMessageTime: string,
    lastMessage: string
}
interface MessagesProp {
    first: user
    zero: user
    messages: Messange[]
}
interface Messange {
    message: string
    createAt: string
}

interface user {
    userImage: string,
    username: string,
}

const Messages: React.FC = () => {
    const [navItems, setNavItems] = useState<navProps[]>([])
    const [messages, setMessages] = useState<MessagesProp | null>(null)

    return (
        <div id="messages">
            <div className="logo-search">
                <a href="/" className="logo">FreelanceBie</a>
                <div className="search">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" d="m21 21l-4.343-4.343m0 0A8 8 0 1 0 5.343 5.343a8 8 0 0 0 11.314 11.314" stroke-width="1" /></svg>
                </div>
            </div>
            <div className="user-control-bar">
                <div className="user-info-container">
                    <img src="" alt="" className="user-image" />
                    <div className="info-status">
                        <p className="name"></p>
                        <p className="status"></p>
                    </div>
                </div>
                <div className="detail">

                </div>
            </div>
            <div className="navbar">
                <div className="title-nav">
                    <span className="title">All Message</span>
                </div>
                <div className="nav-container">
                    {navItems.map((nav, index) => {
                        return (
                            <div className="nav">
                                <img src={nav.userImage} alt="" className="user-img-nav" />
                                <div className="nav-info">
                                    <div className="username-message">
                                        <p className="username">{nav.username}</p>
                                        <p className="message">{nav.lastMessage}</p>
                                    </div>
                                    <div className="time"></div>
                                </div>
                            </div>
                        )
                    })}
                </div>
            </div>
            <div className="message-container">

            </div>
        </div>
    )
}

export default Messages;
