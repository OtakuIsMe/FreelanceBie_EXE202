import React, { useState } from "react";
import { IoSearch } from "react-icons/io5"; // Search icon from react-icons
import { FaBars } from "react-icons/fa"; // Hamburger icon for mobile menu
import { Link } from "react-router-dom"; // Import Link from React Router
import "./Header.css";

const Header = () => {
	const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

	const toggleMobileMenu = () => {
		setIsMobileMenuOpen(!isMobileMenuOpen);
	};

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
			<div className="auth">
				<button className="btn login">Sign in</button>
				<button className="btn register">Sign up</button>
			</div>
		</header>
	);
};

export default Header;
