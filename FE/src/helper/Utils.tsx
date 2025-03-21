export default class Utils {
	static generateRandomCode(): number {
		return Math.floor(1000000000 + Math.random() * 8000000000);
	}
	static convertUsdToVnd(amount: number): number {
		const exchangeRate = 25505;
		return amount * exchangeRate;
	}
}
