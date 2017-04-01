class ClientData{
	private int clientNumber;
	private String clientName;
	private String clientSurname;
	private String clientPesel;
	private String clientAdress;
	private double clientCash;
	private boolean isCreated = false;
	protected void addNewUser(int cNu, String cNa, String cS, String cP, String cA, double cC){
		clientNumber = cNu;
		clientName = cNa;
		clientSurname = cS;
		clientPesel = cP;
		clientAdress = cA;
		clientCash = cC;
		isCreated = true;
	}
	protected boolean depositCash(double howMuch){
		if(isCreated){
			clientCash += howMuch;
			return true;
		}
		else{
			System.out.println("Cannot deposit. This user is not created.");
			return false;
		}
	}
	protected boolean withdrawCash(double howMuch){
		if(isCreated){
			if(clientCash >= howMuch){
				clientCash -= howMuch;
				return true;
			}
			else{
				System.out.println("Cannot withdraw. This user has not such amount of cash.");
			}
		}
		else{
			System.out.println("Cannot withdraw. This user is not created.");
		}
		return false;
		
	}
	protected void displayClient(){
		System.out.format("%4d %10s %15s   %11s %20s    %.2fzl\n",clientNumber,clientName,clientSurname,clientPesel,clientAdress,clientCash);
	}
	protected int getClientNumber(){
		return clientNumber;
	}
	protected double getClientBalance(){
		return clientCash;
	}
	protected String toString(String sep){
		return clientNumber + sep + clientName + sep + clientSurname + sep + clientPesel + sep + clientAdress + sep + clientCash;
	}
	protected String getSpecificData(int index) {
		switch(index) {
			case 0:
				return Integer.toString(clientNumber);
			case 1:
				return clientName;
			case 2:
				return clientSurname;
			case 3:
				return clientPesel;
			case 4:
				double noDig = Math.floor(clientCash);
				String strNoDig = Double.toString(noDig);
				String digits = Double.toString(clientCash);
				if((digits.length() - strNoDig.length()) == 1) {
					return digits;
				}
				else {
					return digits + "0";
				}
			default:
				System.out.println("Unrecognized column.");
		}
		return null;
	}
}










