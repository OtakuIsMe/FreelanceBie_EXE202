import axios, { AxiosInstance } from "axios";
import { postData } from "../../pages/User/PostJob/PostJob";
import CryptoJS from "crypto-js";

export class ApiGateway {
	public static readonly API_Base: string = import.meta.env.VITE_API_SERVICE + 'api/v1/';
	private static axiosInstance: AxiosInstance = axios.create({
		baseURL: ApiGateway.API_Base,
		headers: {
			'Content-Type': 'application/json',
		},
	});

	private static getToken(): string | null {
		return localStorage.getItem("token");
	}

	private static setAuthHeader(): void {
		const token = this.getToken();
		if (token) {
			this.axiosInstance.defaults.headers.common["Authorization"] = `Bearer ${token}`;
		}
	}

	public static async LoginDefault<T>(email: string, password: string): Promise<Boolean> {
		try {
			const response = await this.axiosInstance.get<T>(`user/Login?Email=${email}&Password=${password}`);
			console.log(response.data)
			const token = (response as any).data.message;
			localStorage.setItem("token", token);
			return true;
		} catch (error) {
			return false;
		}
	}

	public static async GetUser<T>(): Promise<T | null> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`user/getInfo`)
			return response.data;
		} catch (error) {
			return null;
		}
	}
	public static async GetTagsByQuery<T>(query: string): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`specialty/view-specialties?query=${query}`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}
	public static async PublishShot<T>(
		Title: string,
		Specialties: string[],
		Html: string,
		Images: string[]
	): Promise<T> {
		this.setAuthHeader();
		try {
			const formData = new FormData();
			formData.append("title", Title);
			formData.append("html", Html);
			Specialties.map((Specialty, index) => {
				formData.append(`specialties[${index}]`, Specialty);
			})

			await Promise.all(
				Images.map(async (image, index) => {
					let fileToUpload: File | string = image;
					if (image.startsWith("blob:")) {
						const blob = await fetch(image).then((res) => res.blob());
						fileToUpload = new File([blob], `image-${index}.png`, { type: blob.type });
					}
					formData.append(`images[${index}].replace`, image);
					if (fileToUpload instanceof File) {
						formData.append(`images[${index}].file`, fileToUpload);
					}
				})
			);

			const response = await this.axiosInstance.post<T>(`shot/AddShotData`, formData, {
				headers: {
					"Content-Type": "multipart/form-data",
				},
			});
			return response.data;
		} catch (error) {
			console.error("Error publishing shot:", error);
			throw error;
		}
	}
	public static async ShotOwner<T>(): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`shot/ShotOwner`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}
	public static async ShotDetail<T>(shotCode: string): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`shot/ShotDetail?shotCode=${shotCode}`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}

	public static async ShotOther<T>(shotCode: string): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`shot/other-shots?ShotId=${shotCode}`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}

	public static async ShotRandom<T>(item: number): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`shot/shot-random?item=${item}`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}

	public static async CheckPayment<T>(code: string): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`transaction/Check-Payment?code=${code}`)
			return response.data
		} catch (error) {
			console.log(error)
			throw error
		}
	}

	public static async AddPostJob<T>(
		data: postData
	): Promise<T> {
		this.setAuthHeader();
		const formData = new FormData();

		formData.append("title", data.title);
		formData.append("description", data.description);
		formData.append("workType", data.workType.toString());
		formData.append("workLocation", data.workLocation);
		formData.append("companyName", data.companyName);
		formData.append("employmentType", data.employmentType.toString());
		formData.append("experience", data.experience.toString());
		formData.append("specialty", data.specialty);
		formData.append("companyLink", data.companyLink);
		formData.append("payment", data.payment.toString());

		if (data.companyLogo) {
			formData.append("companyLogo", data.companyLogo);
		}

		if (data.files && data.files.length > 0) {
			data.files.forEach((file) => {
				formData.append("files", file);
			});
		}

		try {
			const response = await this.axiosInstance.post<T>("post/PostingJob", formData, {
				headers: {
					...this.axiosInstance.defaults.headers.common,
					"Content-Type": "multipart/form-data",
				},
			});
			return response.data;
		} catch (error) {
			console.error("Error posting job:", error);
			throw error;
		}
	}

	public static async JobDetail<T>(id: string): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`post/PostJobDetail?postCode=${id}`)
			return response.data;
		} catch (error) {
			console.error("Error job detail:", error);
			throw error;
		}
	}

	public static async ListPost<T>(item: number, page: number): Promise<T> {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`post/list-post-card?item=${item}&page=${page}`)
			return response.data;
		} catch (error) {
			console.error("Error List Job:", error)
			throw error;
		}
	}

	public static async ListDesigner<T>(item: number, page: number, countImg: number): Promise<T> {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`user/list-designer?item=${item}&page=${page}&countImg=${countImg}`);
			return response.data;
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ListPostOwner<T>() {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`post/list-post-owner`);
			return response.data;
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ListApply<T>(id: string) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`post/ListApply?postId=${id}`)
			return response.data;
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async PostEmployeeDetail<T>(id: string) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`post/PostEmployeeDetail?postId=${id}`)
			return response.data;
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ApplyJob<T>(id: string) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.post<T>(`post/ApplyJob?postId=${id}`);
			return response.data;
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async SignUp<T>(username: string, email: string, password: string) {
		this.setAuthHeader()
		try {
			const user = {
				userName: username,
				email: email,
				password: password
			}
			const response = await this.axiosInstance.post<T>(`user/register`, user);
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async LikeShot<T>(shotId: string, state: boolean) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.put<T>(`shot/LikeShot?shotId=${shotId}&state=${state}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async LikedShot<T>() {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`shot/ListShotLiked`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ListShot<T>(page: number, count: number) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`shot/ListShot?page=${page}&count=${count}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async Profile<T>() {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`user/view-profile`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ApplyJobStatus<T>(applyId: string, status: boolean) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.put<T>(`post/ApplyJobStatus?applyId=${applyId}&status=${status}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async PostStatus<T>(postId: string, status: boolean) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.put<T>(`post/PostStatus?postId=${postId}&status=${status}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async GetInTouch<T>(userId: string) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.post<T>(`communication/get-in-touch?ownerId=${userId}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async ViewCommunication<T>() {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`communication/view-all-communications`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}
	public static async GetMessages<T>(communicationId: string) {
		this.setAuthHeader()
		try {
			const response = await this.axiosInstance.get<T>(`communication/view-messages?communicationId=${communicationId}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async PaymentUrl(id: number, amount: number, description: string, transactionType: string) {
		this.setAuthHeader()
		try {
			const CLIENT_ID = import.meta.env.VITE_CLIENT_ID;
			const API_KEY = import.meta.env.VITE_API_KEY;
			const CHECKSUM_KEY = import.meta.env.VITE_CHECKSUM_KEY;
			const CANCLE_URL = import.meta.env.VITE_API_CLIENT + "cancel-payment"
			const RETURN_URL = import.meta.env.VITE_API_CLIENT + `payment-success?transactionType=${transactionType}`

			const rawData = `amount=${amount}&cancelUrl=${CANCLE_URL}&description=${description}&orderCode=${id}&returnUrl=${RETURN_URL}`;

			const signature = CryptoJS.HmacSHA256(rawData, CHECKSUM_KEY).toString(CryptoJS.enc.Hex);

			const data = {
				amount: amount,
				orderCode: id,
				description: description,
				cancelUrl: CANCLE_URL,
				returnUrl: RETURN_URL,
				signature: signature
			}
			const response = await axios.post("https://api-merchant.payos.vn/v2/payment-requests", data, {
				headers: {
					'Content-Type': 'application/json',
					'x-client-id': CLIENT_ID,
					'x-api-key': API_KEY
				}
			});
			return response.data.data.checkoutUrl
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}
	public static async ViewControl<T>(shotId: string) {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`shot/ViewControl?shotId=${shotId}`);
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async BuyMembership<T>() {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.post<T>(`user/BuyMembership`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}

	public static async CheckMembership<T>() {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`user/CheckMembership`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}
	public static async CheckApply<T>(id: string) {
		this.setAuthHeader();
		try {
			const response = await this.axiosInstance.get<T>(`user/CheckApply?jobId=${id}`)
			return response.data
		} catch (error) {
			console.error("Error List Designer:", error)
			throw error;
		}
	}
}
