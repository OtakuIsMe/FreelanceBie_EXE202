import React, { useContext, useState } from 'react';
import './Login.css';
import LoginSide from '../../assets/login-side.png'
import { useAuthenContext } from '../../hooks/AuthenContext';

interface LoginProps {
	closeLogin: () => void;
	fetchUser: () => Promise<void>;
}

const Login: React.FC<LoginProps> = ({ closeLogin, fetchUser }) => {
	const [emailInput, setEmailInput] = useState("");
	const [passwordInput, setPasswordInput] = useState("");
	const [isWrongSignIn, setIsWrongSignIn] = useState<boolean>(false);
	const { login } = useAuthenContext();

	const onChangeEmail = (e: React.ChangeEvent<HTMLInputElement>) => {
		setEmailInput(e.target.value);
	}

	const onChangePassword = (e: React.ChangeEvent<HTMLInputElement>) => {
		setPasswordInput(e.target.value);
	}

	const onClickSignIn = async () => {
		const isSuccess = await login(emailInput, passwordInput);
		if (isSuccess) {
			closeLogin();
			await fetchUser();
		} else {
			setIsWrongSignIn(true)
		}
	}
	return (
		<div id="login">
			<div className="background" onClick={closeLogin}></div>
			<div className="login-form">
				<div className="form-side">
					<div className="form-center">
						<p className='logo'>FreelanceBie</p>
						<p className='welcomeback'>
							Holla, <br />
							Welcome Back
						</p>
						<p className="welcome-description">Hey, welcome back to your special place</p>
						<input type="text"
							className="email input"
							value={emailInput}
							onChange={onChangeEmail} />
						<input type="password"
							className="password input"
							value={passwordInput}
							onChange={onChangePassword} />
						<div className="remember-forget">
							{isWrongSignIn && (
								<p className='wrong-txt'>Incorrect username/email or password</p>
							)}
							<div className="remeber">
								<input type="checkbox" className='check-rem' />
								<span className='rem-span'>Remember me</span>
							</div>
							<a className="forgot">Forgot Password?</a>
						</div>
						<button className='sign-in' onClick={onClickSignIn}>Sign In</button>
						<p className="sign-up">Don't have an account? <span className='bold'>Sign Up</span></p>
					</div>
				</div>
				<div className="side-image">
					<img src={LoginSide} alt="Image" />
				</div>
			</div>
		</div>
	)
};

export default Login;
