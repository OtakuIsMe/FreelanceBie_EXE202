import Header from '../../components/Header/Header';
import './PaymentCancel.css'
import cancel from '../../assets/cancel.jpg'
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

const PaymentCancel: React.FC = () => {
	const navigate = useNavigate();

	useEffect(() => {
		const timer = setTimeout(() => {
			navigate("/"); // Chuyển hướng sau 5 giây
		}, 5000);

		return () => clearTimeout(timer); // Dọn dẹp khi component unmount
	}, [navigate]);
	return (
		<div id="payment-cancel">
			<Header />
			<div className="container">
				<img src={cancel} alt="" />
				<p className="title">Something went wrong!</p>
				<p className="des">We cant complete the payment</p>
				<p className="des">Please try again later</p>
			</div>
		</div>
	)
}

export default PaymentCancel;
