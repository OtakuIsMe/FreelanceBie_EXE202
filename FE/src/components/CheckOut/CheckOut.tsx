import { useState } from 'react';
import './CheckOut.css'
import Cards, { Focused } from 'react-credit-cards-2';
import 'react-credit-cards-2/dist/es/styles-compiled.css';

interface CardProps {
	number: string,
	expiry: string,
	cvc: string,
	name: string,
	focus: Focused | undefined,
}
const CheckOut: React.FC = () => {
	const [selectedMethod, setSelectedMethod] = useState("credit_card");
	const [card, setCard] = useState<CardProps>({
		number: '',
		expiry: '',
		cvc: '',
		name: '',
		focus: undefined,
	})
	const handleInputChange = (evt: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = evt.target;

		setCard((prev) => ({ ...prev, [name]: value }));
	};

	const handleInputFocus = (e: React.FocusEvent<HTMLInputElement>) => {
		setCard((prev) => ({ ...prev, focus: e.target.name as Focused }));
	};
	return (
		<div id="check-out">
			<div className="overlay">
				<div className="background-form">
					<div className="title-select-method">
						<p className="title">Payment method</p>
						<div className="tab-container">
							<button
								className={`tab-button ${selectedMethod === "paypal" ? "active" : ""}`}
								onClick={() => setSelectedMethod("paypal")}
							>
								PayPal
							</button>
							<button
								className={`tab-button ${selectedMethod === "credit_card" ? "active" : ""}`}
								onClick={() => setSelectedMethod("credit_card")}
							>
								Credit card
							</button>
						</div>
					</div>
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
								<span className="price-num">$20.00</span>
							</div>
							<button className="payment-btn">
								Pay Now
							</button>
						</div>
					</div>
				</div>
			</div>
		</div>
	)
}

export default CheckOut;
