import axios, { AxiosInstance } from "axios";

export class ApiGateway {
	public static readonly API_Base: string = 'http://localhost:5000/';
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
}
