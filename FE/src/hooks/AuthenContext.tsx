import { createContext, ReactNode, useContext } from "react";
import { ApiGateway } from "../services/api/ApiService";

interface AuthenContextProps {
	login: (email: string, password: string) => Promise<Boolean>;
	logout: () => void;
}

export const AuthenContext = createContext<AuthenContextProps | undefined>(undefined);


const AuthenProvider = ({ children }: { children: ReactNode }) => {
	const login = async (email: string, password: string): Promise<Boolean> => {
		return await ApiGateway.LoginDefault(email, password)
	};
	const logout = async (): Promise<void> => {
		localStorage.removeItem("token");
	}

	return (
		<AuthenContext.Provider value={{ login, logout }}>
			{children}
		</AuthenContext.Provider>
	);
}

export default AuthenProvider;

export const useAuthenContext = () => {
	const context = useContext(AuthenContext);
	if (!context) {
		throw new Error("useAuthenContext must be used within an AuthenProvider");
	}
	return context;
};
