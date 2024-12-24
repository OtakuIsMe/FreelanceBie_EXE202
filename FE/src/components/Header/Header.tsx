import React from "react";
import { IoSearch } from "react-icons/io5"; // Search icon from react-icons
import "./Header.css";

const Header = () => {
	return (
		<header id="main_header">
			<div className="logo">FreelanceBie</div>
			<nav className="navbar">
				<ul>
					<li>
						<a href="#">Explore</a>
					</li>
					<li>
						<a href="#">Hire a Designer</a>
					</li>
					<li>
						<a href="#">Find Jobs</a>
					</li>
				</ul>
			</nav>
			<div className="search">
				<div className="search-input-wrapper">
					<input
						type="text"
						placeholder="What are you looking for?"
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
