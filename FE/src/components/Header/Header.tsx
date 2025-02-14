import React, { useEffect, useState } from "react";
import { IoSearch } from "react-icons/io5"; // Search icon from react-icons
import { FaBars } from "react-icons/fa"; // Hamburger icon for mobile menu
import { Link } from "react-router-dom"; // Import Link from React Router
import "./Header.css";
import Authenication from "../Authenication/Authenication";
import { ApiGateway } from "../../services/api/ApiService";
import { useAuthenContext } from "../../hooks/AuthenContext";

const Header = () => {
	const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
	const [isLoginOpen, setIsLoginOpen] = useState(false);
	const [user, setUser] = useState<any>(null);
	const { logout } = useAuthenContext();
	useEffect(() => {
		fetchUser();
	}, [])

	const fetchUser = async (): Promise<void> => {
		var data = await ApiGateway.GetUser()
		setUser(data)
	}

	const LogOut = async (): Promise<void> => {
		await logout();
		setUser(null)
	}

	const toggleMobileMenu = () => {
		setIsMobileMenuOpen(!isMobileMenuOpen);
	};

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
						<Link to="#">Explore</Link>
					</li>
					<li>
						<Link to="#">Hire a Designer</Link>
					</li>
					<li>
						<Link to="#">Find Jobs</Link>
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
					<button className="btn login" onClick={openLogin}>Sign in</button>
					<button className="btn register">Sign up</button>
				</div>
			) : (
				<div className="auth">
					<div className="username">
						{user.username}
					</div>
					<button className="Btn" onClick={LogOut}>
						<div className="sign">
							<svg viewBox="0 0 512 512">
								<path
									d="M377.9 105.9L500.7 228.7c7.2 7.2 11.3 17.1 11.3 27.3s-4.1 20.1-11.3 27.3L377.9 406.1c-6.4 6.4-15 9.9-24 9.9c-18.7 0-33.9-15.2-33.9-33.9l0-62.1-128 0c-17.7 0-32-14.3-32-32l0-64c0-17.7 14.3-32 32-32l128 0 0-62.1c0-18.7 15.2-33.9 33.9-33.9c9 0 17.6 3.6 24 9.9zM160 96L96 96c-17.7 0-32 14.3-32 32l0 256c0 17.7 14.3 32 32 32l64 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-64 0c-53 0-96-43-96-96L0 128C0 75 43 32 96 32l64 0c17.7 0 32 14.3 32 32s-14.3 32-32 32z"
								></path>
							</svg>
						</div>
						{/* <div className="text">Logout</div> */}
					</button>
				</div>
			)}
			{isLoginOpen && (
				<Authenication closeLogin={closeLogin} fetchUser={fetchUser} />
			)}
		</header>
	);
};

export default Header;
