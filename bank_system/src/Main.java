class Main{
	public static void main(String[] args){
		BankSystem a = new BankSystem();
		a.readFromFile("data.csv");
		a.transferCash(5, 6, 0.15);
		a.storeToFile("data.csv");
	}
}
