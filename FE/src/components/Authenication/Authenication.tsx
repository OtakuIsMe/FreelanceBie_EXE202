import React, { useContext, useState } from 'react';
import './Authenication.css';
import LoginSide from '../../assets/login-side.png'
import { useAuthenContext } from '../../hooks/AuthenContext';

interface AuthenicationProps {
	closeLogin: () => void;
	fetchUser: () => Promise<void>;
}

const Authenication: React.FC<AuthenicationProps> = ({ closeLogin, fetchUser }) => {
	const [emailInput, setEmailInput] = useState("");
	const [passwordInput, setPasswordInput] = useState("");
	const [isWrongSignIn, setIsWrongSignIn] = useState<boolean>(false);
	const [typeInput, setTypeInput] = useState<string>("Login")
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

	const ChangeStateType = (value: string) => {
		setTypeInput(value);
	}
	return (
		<div id="login">
			<div className="background" onClick={closeLogin}></div>
			<div className="login-form">
				<div className="form-side">
					<div className="form-center">
						{typeInput === "Login" && (
							<>
								<p className='logo'>FreelanceBie</p>
								<p className='welcomeback'>
									Holla, <br />
									Welcome Back
								</p>
								<p className="welcome-description">Hey, welcome back to your special place</p>
								<input type="text"
									className="email input"
									placeholder="Email"
									value={emailInput}
									onChange={onChangeEmail} />
								<input type="password"
									className="password input"
									placeholder="Password"
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
									<a className="forgot" onClick={() => ChangeStateType("Forgot")}>Forgot Password?</a>
								</div>
								<button className='sign-in' onClick={onClickSignIn}>Sign In</button>
								<p className="sign-up">Don't have an account? <span className='bold' onClick={() => ChangeStateType("SignUp")}>Sign Up</span></p>
							</>
						)}
						{typeInput === "SignUp" && (
							<>
								<p className='welcomeback'>
									Create <br />
									new account
								</p>
								<p className="welcome-description">Enter your details to create your account and get started</p>
								<input type="text"
									className="username input"
									placeholder="Username"
									value={emailInput}
									onChange={onChangeEmail} />
								<input type="text"
									className="email input"
									placeholder="Email"
									value={emailInput}
									onChange={onChangeEmail} />
								<input type="password"
									className="password input"
									placeholder="Password"
									value={passwordInput}
									onChange={onChangePassword} />
								<button className='sign-in'>Sign Up</button>
								<p className="sign-up">Already have an account <span className='bold' onClick={() => ChangeStateType("Login")}>Login</span></p>
							</>
						)}
						{typeInput === "Forgot" && (
							<>
								<p className='welcomeback forgot-title'>
									Forgot Password?
								</p>
								<p className="welcome-description forgot-des">
									Enter the email address you used when you joined and we’ll send you instructions to reset your password.
									<br /><br />
									For security reasons, we do NOT store your password. So rest assured that we will never send your password via email.
								</p>
								<input type="text"
									className="email-forgot input"
									placeholder="Email Address"
									value={emailInput}
									onChange={onChangeEmail} />
								<button className='sign-in'>Send Reset</button>
								<p className="sign-up">Remember Password? <span className='bold' onClick={() => ChangeStateType("Login")}>Login</span></p>
							</>
						)}
					</div>
				</div>
				<div className="side-image">
					<img src={LoginSide} alt="Image" />
				</div>
			</div>
		</div>
	)
};

export default Authenication;
