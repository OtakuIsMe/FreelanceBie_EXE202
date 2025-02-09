import axios, { AxiosInstance } from "axios";

export class ApiGateway {
	public static readonly API_Base: string = 'http://localhost:5147/';
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
			console.log(response.data)
			return response.data;
		} catch (error) {
			return null;
		}
	}
}
