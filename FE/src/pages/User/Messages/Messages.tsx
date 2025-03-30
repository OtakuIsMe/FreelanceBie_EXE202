import { useEffect, useState } from 'react'
import { useParams } from "react-router-dom";
import { useNavigate } from 'react-router-dom';
import * as signalR from "@microsoft/signalr";
import './Messages.css'
import { ApiGateway } from '../../../services/api/ApiService';

interface navProps {
	messageId: string,
	userImage: string,
	username: string,
	lastMessageTime: string,
	lastMessage: string
}
interface MessagesProp {
	self: user
	partner: user
	messages: Message[]
}
interface Message {
	isSelf: boolean
	message: string
	createAt: string
}

interface user {
	id: string
	userImage: string,
	username: string,
}

const Messages: React.FC = () => {
	const navigate = useNavigate();
	const { id } = useParams();
	const [inputMes, setInputMes] = useState<string>("")
	const [navItems, setNavItems] = useState<navProps[]>([])
	const [messages, setMessages] = useState<MessagesProp | null>(null)

	const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

	function groupMessages(rawMessages: MessagesProp) {
		const groupedMessages: {
			isSelf: boolean;
			username: string;
			userImage: string;
			messages: { message: string; createAt: string }[];
		}[] = [];

		let currentGroup: {
			isSelf: boolean;
			username: string;
			userImage: string;
			messages: { message: string; createAt: string }[];
		} | null = null;

		for (const msg of rawMessages.messages) {
			const user = msg.isSelf ? rawMessages.self : rawMessages.partner;

			if (!currentGroup || currentGroup.isSelf !== msg.isSelf) {

				if (currentGroup) groupedMessages.push(currentGroup);

				currentGroup = {
					isSelf: msg.isSelf,
					username: user.username,
					userImage: user.userImage,
					messages: [],
				};
			}

			currentGroup.messages.push({ message: msg.message, createAt: msg.createAt });
		}

		if (currentGroup) groupedMessages.push(currentGroup);

		return groupedMessages;
	}

	useEffect(() => {
		const url = import.meta.env.VITE_API_SERVICE + 'chatHub'
		const newConnection = new signalR.HubConnectionBuilder()
			.withUrl(url, {
				accessTokenFactory: async () => localStorage.getItem("token") ?? "",
				withCredentials: true
			})
			.withAutomaticReconnect()
			.build();

		newConnection.start()
			.then(async () => {
				console.log("Connected to SignalR");
				if (id) {
					await newConnection.invoke("JoinGroup", id);
				} else {
					console.error("JoinGroup error: id is null or undefined");
				}
			})
			.catch(err => console.error("Error connecting SignalR: ", err));

		newConnection.on("ReceiveMessage", (message, senderId, createdAt) => {
			if (!messages || !messages.self?.id) {
				console.log("Messages being undefine wait a minute.");
				return;
			}

			if (String(senderId).trim() !== String(messages.self.id).trim()) {
				AddMessage(message, createdAt, false);
			}
		});
		setConnection(newConnection);

		return () => {
			newConnection.stop();
		};
	}, [id]);


	useEffect(() => {
		fetchingNavBar()
	}, [])
	useEffect(() => {
		fetchingMessages()
	}, [id])

	const fetchingNavBar = async () => {
		const data = await ApiGateway.ViewCommunication<navProps[]>()
		setNavItems(data)
	}

	const fetchingMessages = async () => {
		if (id) {
			const data = await ApiGateway.GetMessages<MessagesProp>(id)
			console.log(data)
			setMessages(data);
		}
	}

	const AddMessage = (message: string, createAt: string, isSelf: boolean) => {
		setMessages((prev) => {
			if (!prev) return null;

			return {
				...prev,
				messages: [
					...prev.messages,
					{
						isSelf: isSelf,
						message,
						createAt,
					},
				],
			};
		});
	};

	const handleSendBtn = async () => {
		if (!inputMes.trim()) return;
		await connection?.invoke("SendMessage", id, inputMes)
		setInputMes("")
		AddMessage(inputMes, new Date().toISOString(), true)
	}


	return (
		<div id="messages">
			<div className="logo-search block">
				<a href="/" className="logo">FreelanceBie</a>
				{/* <div className="search">
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" d="m21 21l-4.343-4.343m0 0A8 8 0 1 0 5.343 5.343a8 8 0 0 0 11.314 11.314" stroke-width="1" /></svg>
				</div> */}
			</div>
			<div className="user-control-bar block">
				{id &&
					<div className="user-info-container">
						<img src={messages?.partner.userImage} alt="" className="user-image" />
						<div className="info-status">
							<p className="name">{messages?.partner.username}</p>
							<p className="status">
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 48 48"><path fill="#4dc953" stroke="#4dc953" stroke-width="4" d="M24 33a9 9 0 1 0 0-18a9 9 0 0 0 0 18Z" /></svg>Online
							</p>
						</div>
					</div>
					/* <div className="detail">
						<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 48 48"><g fill="none"><path stroke="#b6b6b6" stroke-linejoin="round" stroke-width="4" d="M24 44c11.046 0 20-8.954 20-20S35.046 4 24 4S4 12.954 4 24s8.954 20 20 20Z" /><circle cx="14" cy="24" r="3" fill="#b6b6b6" /><circle cx="24" cy="24" r="3" fill="#b6b6b6" /><circle cx="34" cy="24" r="3" fill="#b6b6b6" /></g></svg>
					</div> */
				}
			</div>
			<div className="navbar block">
				<div className="title-nav">
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" fill-rule="evenodd"><path d="m12.594 23.258l-.012.002l-.071.035l-.02.004l-.014-.004l-.071-.036q-.016-.004-.024.006l-.004.01l-.017.428l.005.02l.01.013l.104.074l.015.004l.012-.004l.104-.074l.012-.016l.004-.017l-.017-.427q-.004-.016-.016-.018m.264-.113l-.014.002l-.184.093l-.01.01l-.003.011l.018.43l.005.012l.008.008l.201.092q.019.005.029-.008l.004-.014l-.034-.614q-.005-.019-.02-.022m-.715.002a.02.02 0 0 0-.027.006l-.006.014l-.034.614q.001.018.017.024l.015-.002l.201-.093l.01-.008l.003-.011l.018-.43l-.003-.012l-.01-.01z" /><path fill="#a3a3a3" d="M19 3a3 3 0 0 1 3 3v10a3 3 0 0 1-3 3H7.333L4 21.5c-.824.618-2 .03-2-1V6a3 3 0 0 1 3-3zm-8 9H8a1 1 0 1 0 0 2h3a1 1 0 1 0 0-2m5-4H8a1 1 0 0 0-.117 1.993L8 10h8a1 1 0 0 0 .117-1.993z" /></g></svg>
					<span className="title">All Message</span>
				</div>
				<div className="nav-container">
					{navItems.map((nav, index) => {
						return (
							<div className={`nav ${nav.messageId === id ? "focus" : ""}`} key={index}
								onClick={() => { navigate(`/messages/${nav.messageId}`) }}>
								<img src={nav.userImage} alt="" className="user-img-nav" />
								<div className="nav-info">
									<div className="username-message">
										<p className="username">{nav.username}</p>
										<p className="message">{nav.lastMessage}</p>
									</div>
									<div className="time">{new Date(nav.lastMessageTime).toLocaleString("vi-VN", {
										hour: "2-digit",
										minute: "2-digit",
										day: "2-digit",
										month: "2-digit",
										year: "numeric",
									})}</div>
								</div>
							</div>
						)
					})}
				</div>
			</div>
			<div className="message-container block">
				{id &&
					<>
						<div className="messages">{
							messages && (
								<>
									{
										groupMessages(messages).map((userMes, index) => {
											return (<>
												{userMes.isSelf ? (
													<div className="message-block right" key={index}>
														<div className="name-message">
															<div className="name-time">
																<span className='time'>{new Date(userMes.messages[0].createAt).toLocaleString("vi-VN", {
																	hour: "2-digit",
																	minute: "2-digit",
																	day: "2-digit",
																	month: "2-digit",
																	year: "numeric",
																})}</span>
																<span className="name">{userMes.username}</span>
															</div>
															<div className="messages-show">
																{userMes.messages.map((mes, index) => {
																	return (
																		<div key={index} className="message">
																			{mes.message}
																		</div>
																	)
																})}
															</div>
														</div>
														<div className="user-container">
															<img src={userMes.userImage} alt="" />
														</div>
													</div>
												) : (
													<div className="message-block left">
														<div className="user-container">
															<img src={userMes.userImage} alt="" />
														</div>
														<div className="name-message">
															<div className="name-time">
																<span className="name">{userMes.username}</span>
																<span className='time'>{userMes.messages[0].createAt}</span>
															</div>
															<div className="messages-show">
																{userMes.messages.map((mes, index) => {
																	return (
																		<div key={index} className="message">
																			{mes.message}
																		</div>
																	)
																})}
															</div>
														</div>
													</div>
												)
												}
											</>
											)
										})
									}
								</>
							)
						}
						</div>
						<div className="input-message">
							<div className="input-bar">
								<svg className='micro' xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 56 56"><path fill="#a3a3a3" d="M35.102 28V11.266c0-4.336-2.93-7.547-7.102-7.547s-7.102 3.21-7.102 7.547V28c0 4.313 2.93 7.547 7.102 7.547s7.102-3.234 7.102-7.547M17.055 48.742c-.938 0-1.758.844-1.758 1.781c0 .938.82 1.758 1.758 1.758h21.89c.938 0 1.758-.82 1.758-1.758c0-.937-.82-1.78-1.758-1.78h-9.187v-5.134c8.531-.75 14.18-7.054 14.18-15.632v-5.508c0-.914-.82-1.711-1.735-1.711c-.914 0-1.71.797-1.71 1.71v5.509c0 7.148-5.157 12.398-12.493 12.398s-12.492-5.25-12.492-12.398v-5.508c0-.914-.797-1.711-1.711-1.711s-1.735.797-1.735 1.71v5.509c0 8.578 5.649 14.882 14.157 15.632v5.133Z" /></svg>
								<input type="text" value={inputMes} onChange={(e) => { setInputMes(e.target.value) }} onKeyDown={(e) => { if (e.key === "Enter") { handleSendBtn(); } }} name="" id="" className="input" placeholder='Type a message' />
								<div className="send-btn" onClick={handleSendBtn}>
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="#a3a3a3" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M14.76 12H6.832m0 0c0-.275-.057-.55-.17-.808L4.285 5.814c-.76-1.72 1.058-3.442 2.734-2.591L20.8 10.217c1.46.74 1.46 2.826 0 3.566L7.02 20.777c-1.677.851-3.495-.872-2.735-2.591l2.375-5.378A2 2 0 0 0 6.83 12" /></svg>
								</div>
							</div>
						</div>
					</>
				}
			</div>
		</div>
	)
}

export default Messages;
