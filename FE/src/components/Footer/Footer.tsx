import './Footer.css'

// import LogoNSHOP from "../../../assets/Logo NSHOP.png";

import { FaYoutube } from "react-icons/fa";
import { IoLogoFacebook } from "react-icons/io";
import { AiFillTikTok } from "react-icons/ai";
import { IoLogoGithub } from "react-icons/io";

export default function Footer() {
	return (
		<div id="footer">
			<div className="footer-content">
				<div className="logo-contact">
					<div className="logo-title">
						{/* <img src={LogoNSHOP} alt="" className="logo" /> */}
						<span>FreelanceBie</span>
					</div>
					<div className="description">
						<span>Phone: 0707911476</span>
						<span>Email: hunglcse161248@fpt.edu.vn</span>
						<span>Address: 6th Floor, OneHub Saigon, High-Tech Park, Tan Phu Ward, District 9, Ho Chi Minh City</span>
					</div>
				</div>
				<div className="about block">
					<p className="title">About FreelanceBie</p>
					<div className="description">
						<p>About Us</p>
						<p>Privacy Policy</p>
						<p>Terms of Use</p>
						<p>Cookie Policy</p>
					</div>
				</div>
				<div className="product block">
					<p className="title">Product</p>
					<div className="description">
						<p>CV1</p>
					</div>
				</div>
				<div className="community block">
					<p className="title">Community</p>
					<div className="description">
						<a href="https://www.youtube.com/@hunghy_0852" target="_blank" rel="noopener noreferrer">
							<FaYoutube />
							<span>Youtube</span>
						</a>
						<a href="https://www.facebook.com/hunghyyy0852/" target="_blank" rel="noopener noreferrer">
							<IoLogoFacebook />
							<span>Facebook</span>
						</a>
						<a href="https://www.youtube.com/watch?v=dQw4w9WgXcQ&autoplay=1" target="_blank" rel="noopener noreferrer">
							<AiFillTikTok />
							<span>Tiktok</span>
						</a>
						<a href="https://github.com/HungHy1602" target="_blank" rel="noopener noreferrer">
							<IoLogoGithub />
							<span>GitHub</span>
						</a>
					</div>
				</div>
				<div className="account block">
					<p className="title">Account</p>
					<div className="description">
						<p>Create Account?</p>
						<p>Log In</p>
					</div>
				</div>
			</div>
			<div className="slogan">FreelanceBie : Where Creativity Meets Professionalism</div>
		</div>
	)
}
