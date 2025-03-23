import { useEffect, useState } from "react";
import { IoSearch } from "react-icons/io5";
import { Link } from "react-router-dom";
import "./Header.css";
import Authenication from "../Authenication/Authenication";
import { ApiGateway } from "../../services/api/ApiService";
import { useAuthenContext } from "../../hooks/AuthenContext";

interface UserProps {
	username: string,
	imageVideos: {
		url: string
	}[]
}

const Header = () => {
	const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
	const [isLoginOpen, setIsLoginOpen] = useState(false);
	const [user, setUser] = useState<UserProps | null>(null);
	const [type, setType] = useState<string>("Login");
	const { logout } = useAuthenContext();
	useEffect(() => {
		fetchUser();
	}, [])

	const fetchUser = async (): Promise<void> => {
		var data = await ApiGateway.GetUser<UserProps>()
		setUser(data)
	}

	const LogOut = async (): Promise<void> => {
		await logout();
		setUser(null);
		window.location.href = '/'
	}

	const openLogin = () => {
		setIsLoginOpen(true)
	}

	const closeLogin = () => {
		setIsLoginOpen(false)
	}



	return (
		<header id="main_header">
			<Link to="/" className="logo">FreelanceBie</Link>
			<nav className={`navbar ${isMobileMenuOpen ? "open" : ""}`}>
				<ul>
					<li>
						<Link to="/inspiration">Explore</Link>
					</li>
					<li>
						<Link to="/search-designer">Hire a Designer</Link>
					</li>
					<li>
						<Link to="/find-job">Find Jobs</Link>
					</li>
				</ul>
			</nav>
			<div className="search">
				<div className="search-input-wrapper">
					<input
						type="text"
						placeholder="Search..."
						className="search-input"
					/>
					<IoSearch size={20} className="search-icon" />
				</div>
			</div>
			{(user == null) ? (
				<div className="auth">
					<button className="btn login" onClick={() => { openLogin(); setType("Login") }}>Sign in</button>
					<button className="btn register" onClick={() => { openLogin(); setType("SignUp") }}>Sign up</button>
				</div>
			) : (
				<div className="auth">
					<div className="notification">
						<button className="button">
							<svg viewBox="0 0 448 512" className="bell"><path d="M224 0c-17.7 0-32 14.3-32 32V49.9C119.5 61.4 64 124.2 64 200v33.4c0 45.4-15.5 89.5-43.8 124.9L5.3 377c-5.8 7.2-6.9 17.1-2.9 25.4S14.8 416 24 416H424c9.2 0 17.6-5.3 21.6-13.6s2.9-18.2-2.9-25.4l-14.9-18.6C399.5 322.9 384 278.8 384 233.4V200c0-75.8-55.5-138.6-128-150.1V32c0-17.7-14.3-32-32-32zm0 96h8c57.4 0 104 46.6 104 104v33.4c0 47.9 13.9 94.6 39.7 134.6H72.3C98.1 328 112 281.3 112 233.4V200c0-57.4 46.6-104 104-104h8zm64 352H224 160c0 17 6.7 33.3 18.7 45.3s28.3 18.7 45.3 18.7s33.3-6.7 45.3-18.7s18.7-28.3 18.7-45.3z"></path></svg>
						</button>
					</div>
					<a href="/pro" className="pro">Go Pro</a>
					<div className="user-container">
						<img src={user.imageVideos[0].url} alt="" className="avatar" />
						<div className="user-dropdown">
							<div className="background">
								<img src={user.imageVideos[0].url} alt="" className="avatar" />
								<p className="name">{user.username}</p>
								<a className="card profile" href="/profile">Profile</a>
								<a className="card profile" href="/post/manage">Manage Post</a>
								<div className="card logout" onClick={LogOut}>Sign Out</div>
							</div>
						</div>
					</div>
				</div>
			)}
			{isLoginOpen && (
				<Authenication closeLogin={closeLogin} fetchUser={fetchUser} type={type} />
			)}
		</header>
	);
};

export default Header;
