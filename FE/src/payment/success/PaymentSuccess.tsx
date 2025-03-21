import Header from '../../components/Header/Header';
import './PaymentSuccess.css'
import success from '../../assets/success.jpg'
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

const PaymentSuccess: React.FC = () => {
	const navigate = useNavigate();

	useEffect(() => {
		const timer = setTimeout(() => {
			navigate("/"); // Chuyển hướng sau 5 giây
		}, 5000);

		return () => clearTimeout(timer); // Dọn dẹp khi component unmount
	}, [navigate]);

	return (
		<div id="payment-success">
			<Header />
			<div className="container">
				<img src={success} alt="" />
				<p className="title">Payment Successful!</p>
				<p className="des">The payment has been done Successfully</p>
				<p className="des">Thanks for being there with us</p>
			</div>
		</div>
	)
}

export default PaymentSuccess;
