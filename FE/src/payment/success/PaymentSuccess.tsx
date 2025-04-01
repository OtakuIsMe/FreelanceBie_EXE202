import Header from '../../components/Header/Header';
import './PaymentSuccess.css'
import success from '../../assets/success.jpg'
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useEffect } from 'react';
import { ApiGateway } from '../../services/api/ApiService';
import { postData } from '../../pages/User/PostJob/PostJob';


const PaymentSuccess: React.FC = () => {
	const [searchParams] = useSearchParams();
	const navigate = useNavigate();
	const transactionType = searchParams.get("transactionType");

	useEffect(() => {
		const timer = setTimeout(() => {
			navigate("/");
		}, 5000);

		return () => clearTimeout(timer);
	}, [navigate]);

	useEffect(() => {
		if (transactionType === "membership") {
			BuyMembership();
		} else if (transactionType === "post") {
			PostJob();
		}
	}, [transactionType]);

	const BuyMembership = async () => {
		await ApiGateway.BuyMembership();
	}

	const PostJob = async () => {
		try {
			// 🔹 Lấy dữ liệu JSON từ localStorage
			const rawData = localStorage.getItem("pendingPostJob");
			if (!rawData) {
				console.warn("No pending post job found!");
				return;
			}
			const postData = JSON.parse(rawData) as Omit<postData, "companyLogo" | "files">;

			// 🔹 Lấy File từ sessionStorage
			const logoBase64 = sessionStorage.getItem("pendingPostJob_companyLogo");
			const companyLogo = logoBase64 ? dataURLToFile(logoBase64, "company-logo.png") : null;

			const files: File[] = [];
			let index = 0;
			while (true) {
				const fileBase64 = sessionStorage.getItem(`pendingPostJob_files_${index}`);
				if (!fileBase64) break;
				files.push(dataURLToFile(fileBase64, `file-${index}.png`)); // Đặt tên file tạm
				index++;
			}

			// 🔹 Tạo dữ liệu hoàn chỉnh để gửi API
			const fullPostData: postData = {
				...postData,
				companyLogo,
				files: files.length > 0 ? files : null
			};

			// 🔹 Gửi request lên API
			const response = await ApiGateway.AddPostJob(fullPostData);
			console.log("Job posted successfully:", response);

			// 🔹 Xóa dữ liệu sau khi đăng thành công
			localStorage.removeItem("pendingPostJob");
			sessionStorage.removeItem("pendingPostJob_companyLogo");
			for (let i = 0; i < files.length; i++) {
				sessionStorage.removeItem(`pendingPostJob_files_${i}`);
			}
		} catch (error) {
			console.error("Error posting job:", error);
		}
	};

	// 🔹 Chuyển Base64 về File
	const dataURLToFile = (dataUrl: string, filename: string): File => {
		const arr = dataUrl.split(",");
		const mimeMatch = arr[0].match(/:(.*?);/);
		const mime = mimeMatch ? mimeMatch[1] : "application/octet-stream";
		const bstr = atob(arr[1]);
		let n = bstr.length;
		const u8arr = new Uint8Array(n);
		while (n--) {
			u8arr[n] = bstr.charCodeAt(n);
		}
		return new File([u8arr], filename, { type: mime });
	};

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
