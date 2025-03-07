import { useState } from 'react';
import './CheckOut.css'

const CheckOut: React.FC = () => {
    const [selectedMethod, setSelectedMethod] = useState("credit_card");
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
                </div>
            </div>
        </div>
    )
}

export default CheckOut;
