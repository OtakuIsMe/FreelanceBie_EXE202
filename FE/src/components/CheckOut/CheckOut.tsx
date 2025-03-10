import { useEffect, useState } from 'react';
import './CheckOut.css'
import Cards, { Focused } from 'react-credit-cards-2';
import 'react-credit-cards-2/dist/es/styles-compiled.css';
import { ApiGateway } from '../../services/api/ApiService';
import VCB from '../../assets/vietcombank.png'
import confetti from '../../assets/confetti.png'

interface CardProps {
	number: string,
	expiry: string,
	cvc: string,
	name: string,
	focus: Focused | undefined,
}
interface CheckOutProps {
	amount: number
	closePopup: () => void;
	ActionAfterPayment: () => Promise<void>;
}
interface ReturnCheckPayment {
	status: boolean;
}
const CheckOut: React.FC<CheckOutProps> = (props) => {
	const [selectedMethod, setSelectedMethod] = useState<string>("qr");
	const [code, setCode] = useState<string>("")
	const [paymentCompleted, setPaymentCompleted] = useState(false);
	const [card, setCard] = useState<CardProps>({
		number: '',
		expiry: '',
		cvc: '',
		name: '',
		focus: undefined,
	})
	useEffect(() => {
		setCode(generateRandomCode());
	}, [])

	const generateRandomCode = (): string => {
		const randomNumbers = Math.floor(10000000 + Math.random() * 90000000); // Tạo số ngẫu nhiên 8 chữ số
		return `FLB${randomNumbers}`;
	};

	const handleInputChange = (evt: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = evt.target;

		setCard((prev) => ({ ...prev, [name]: value }));
	};

	const handleInputFocus = (e: React.FocusEvent<HTMLInputElement>) => {
		setCard((prev) => ({ ...prev, focus: e.target.name as Focused }));
	};

	useEffect(() => {
		if (paymentCompleted) return;

		const interval = setInterval(async () => {
			try {
				console.log(code);
				const response = await ApiGateway.CheckPayment<ReturnCheckPayment>(code);
				console.log(response);

				if (response.status) {
					setPaymentCompleted(true);
					props.ActionAfterPayment();
					clearInterval(interval);
				}
			} catch (error) {
				console.error("Error checking payment:", error);
			}
		}, 30000);

		return () => clearInterval(interval);
	}, [code, paymentCompleted]);

	const user = {
		name: "Nguyễn Mạnh Duy",
		number: 1026347842,
		amount: `$${props.amount}`,
		description: code,
		bank: "Vietcombank"
	}

	return (
		<div id="check-out">
			<div className="overlay" onClick={props.closePopup}>
				<div className="background-form" onClick={(e) => e.stopPropagation()}>
					{paymentCompleted ? (
						<div className="success-form">
							<div className='image-congrate-container'>
								<img src={confetti} alt="" />
								<div className="check-container">
									<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"><defs><mask id="ipSCheckOne0"><g fill="none" stroke-linejoin="round" stroke-width="4"><path fill="#fff" stroke="#fff" d="M24 44a19.94 19.94 0 0 0 14.142-5.858A19.94 19.94 0 0 0 44 24a19.94 19.94 0 0 0-5.858-14.142A19.94 19.94 0 0 0 24 4A19.94 19.94 0 0 0 9.858 9.858A19.94 19.94 0 0 0 4 24a19.94 19.94 0 0 0 5.858 14.142A19.94 19.94 0 0 0 24 44Z" /><path stroke="#000" stroke-linecap="round" d="m16 24l6 6l12-12" /></g></mask></defs><path fill="currentColor" d="M0 0h48v48H0z" mask="url(#ipSCheckOne0)" /></svg>
								</div>
							</div>
							<p className="title">Payment success!</p>
							<p className="des">Congratulation! Your payment was completed successfully</p>
							<button className='back' onClick={() => { window.location.href = '/' }}>Back to Home</button>
						</div>
					) : (
						<>
							<div className="title-select-method">
								<p className="title">Payment method</p>
								<div className="tab-container">
									<button
										className={`tab-button ${selectedMethod === "qr" ? "active" : ""}`}
										onClick={() => setSelectedMethod("qr")}
									>
										QR
									</button>
									<button
										className={`tab-button ${selectedMethod === "credit_card" ? "active" : ""}`}
										onClick={() => setSelectedMethod("credit_card")}
									>
										Credit card
									</button>
								</div>
							</div>
							{selectedMethod === "credit_card" && (
								<div className="card-input-container">
									<div className="card-container">
										<Cards
											number={card.number}
											expiry={card.expiry}
											cvc={card.cvc}
											name={card.name}
											focused={card.focus}
										/>
									</div>
									<div className="credit-card-info--form">
										<div className="input_container">
											<label className="input_label"
											>Card holder full name</label>
											<input
												placeholder="Enter your full name"
												title="Inpit title"
												name="name"
												type="text"
												className="input_field"
												id="password_field"
												value={card.name}
												onChange={handleInputChange}
												onFocus={handleInputFocus}
											/>
										</div>
										<div className="input_container">
											<label className="input_label">Card Number</label>
											<input
												placeholder="0000 0000 0000 0000"
												title="Inpit title"
												name="number"
												type="number"
												className="input_field"
												id="password_field"
												value={card.number}
												onChange={handleInputChange}
												onFocus={handleInputFocus}
											/>
										</div>
										<div className="input_container">
											<label className="input_label"
											>Expiry Date / CVV</label>
											<div className="split">
												<input
													placeholder="01/23"
													title="Expiry Date"
													name="expiry"
													type="text"
													className="input_field"
													id="password_field"
													value={card.expiry}
													onChange={handleInputChange}
													onFocus={handleInputFocus}
												/>
												<input
													placeholder="CVV"
													title="CVV"
													name="cvc"
													type="number"
													className="input_field"
													id="password_field"
													value={card.cvc}
													onChange={handleInputChange}
													onFocus={handleInputFocus}
												/>
											</div>
										</div>
										<div className="price-container">
											<span className="title">Total Price:</span>
											<span className="price-num">${props.amount}</span>
										</div>
										<button className="payment-btn">
											Pay Now
										</button>
									</div>
								</div>
							)}
							{selectedMethod === "qr" && (
								<div className="qr-container">
									<div className="image-container">
										<img src={`https://img.vietqr.io/image/VCB-1026347842-qr_only.png?amount=${props.amount * 25505}&addInfo=${code}`} />
									</div>
									<div className="info-container">
										<div className="block-info bank">
											<img src={VCB} alt="" />
											<div>
												<p className="title">Bank:</p>
												<p className="data">{user.bank}</p>
											</div>
										</div>
										<div className="block-info">
											<p className="title">Account owner:</p>
											<p className="data">{user.name}</p>
										</div>
										<div className="block-info">
											<p className="title">Account number:</p>
											<p className="data">{user.number}</p>
										</div>
										<div className="block-info">
											<p className="title">Amount:</p>
											<p className="data">{user.amount}</p>
										</div>
										<div className="block-info">
											<p className="title">Content:</p>
											<p className="data">{user.description}</p>
										</div>
									</div>
								</div>
							)}
						</>
					)}
				</div>
			</div>
		</div>
	)
}

export default CheckOut;
