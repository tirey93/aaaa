import java.util.ArrayList;
import java.io.IOException;
import java.nio.file.Files;
import java.util.Scanner;
import java.nio.file.Path;
import java.nio.file.Paths;
class BankSystem extends ClientData{
	private ArrayList<ClientData> dataBase;
	private Path path;
	private int cNu = 0; 
	BankSystem(){
		dataBase = new ArrayList<>();
	}
	public void addNewAccount(){
		String cNa;
		String cS; 
		String cP; 
		String cA;
		int cC;
		Scanner scan = new Scanner(System.in);
		cNu++;
		System.out.println("Enter new user name: ");
		System.out.printf(">");
		cNa = scan.nextLine();
		
		System.out.println("Enter new user surname: ");
		System.out.printf(">");
		cS = scan.nextLine();
		
		System.out.println("Enter new user PESEL: ");
		System.out.printf(">");
		cP = scan.nextLine();
		
		System.out.println("Enter new user adress: ");
		System.out.printf(">");
		cA = scan.nextLine();
		
		cC = 0;
		
		ClientData cD = new ClientData();
		cD.addNewUser(cNu,cNa,cS,cP,cA,cC);
		dataBase.add(cD);
		System.out.println("New account has been created.");
		System.out.println("Client Number: " + cNu);
		System.out.println("Balance: " + cC);
	}
	public void removeAccount(int index) {
		int toRemove = getListID(index);
		if(toRemove != -1) {
			dataBase.remove(toRemove);
			System.out.println("Account has ben deleted.");
		}
	}
	public void transferCash(int from, int to, double howMuch) {
		int indexOfFrom = getListID(from+1);
		int indexOfTo = getListID(to+1);
		boolean statusWithdraw = false;
		boolean statusDeposit = false;
		
		statusWithdraw = withdrawFromClient(howMuch, indexOfFrom);
		if(statusWithdraw) {
			statusDeposit = depositToClient(howMuch, indexOfTo);
			if(statusDeposit) {
				System.out.println("Succecfully transfer from " + from + " to " + to + ".");
			}
			else {
				System.out.println("Operation has been cancelled.");
			}
		}
		else {
			System.out.println("Operation has been cancelled.");
		}
		
	}
	private int getListID(int index) {
		int i = 0;
		boolean found = false;
		for(ClientData cD : dataBase) {
			if(cD.getClientNumber() == index) {
				found = true;
				return i;
			}
			i++;
		}
		if(!found) {
			System.out.println("Account not found.");;
		}
		return -1;
	}
	public void displayAllAccounts(){
		displayHeadline();
		for(ClientData cD : dataBase){
			cD.displayClient();
		}
	}
	public boolean depositToClient(double howMuch, int cNu){
		boolean found = false;
		double bal = 0;
		int a = 0;
		for(ClientData cD : dataBase){
			a = cD.getClientNumber();
			if(a == cNu){
				if(!cD.depositCash(howMuch)) {
					return false;
				}
				bal = cD.getClientBalance();
				found = true;
			}
		}
		if(found){
			System.out.println("Succefully added " + howMuch + " to " + cNu + " account. Balance: " + bal);
			return true;
		}
		else{
			System.out.println("Cannot find user: " + cNu + ". Deposit has not been finished.");
			return false;
		}
	}
	public boolean withdrawFromClient(double howMuch, int cNu){
		boolean found = false;
		double bal = 0;
		for(ClientData cD : dataBase){
			if(cD.getClientNumber() == cNu){
				if(!cD.withdrawCash(howMuch)) {
					return false;
				}
				bal = cD.getClientBalance();
				found = true;
			}
		}
		if(found){
			System.out.println("Succefully subtract " + howMuch + " from " + cNu + " account. Balance: " + bal);
			return true;
		}
		else{
			System.out.println("Cannot find user: " + cNu + ". Withdraw has not been finished.");
			return false;
		}
	}
	public void storeToFile(String p1){
		path = Paths.get(p1);
		ArrayList<String> out = new ArrayList<String>();
		for(ClientData cD : dataBase) {
			out.add(cD.toString(";"));
		}
		try {
			Files.write(path, out);
			System.out.print("Data was successfully stored.");
			
		}
		catch( IOException a) {
			System.out.print("Can't write to file.");
		}
	}
	public void readFromFile(String p1) {
		path = Paths.get(p1);
		dataBase.clear();
		try {
			ArrayList<String> in = (ArrayList<String>) Files.readAllLines(path);
			for(String list : in) {
				dataBase.add(setObject(list));
			}
			cNu = in.size();
			System.out.println("Data was read from file.");
		}
		catch(IOException ex) {
			System.out.println("Cannot read the file.");
		}
	}
	private ClientData setObject(String in){
		ClientData out = new ClientData();
		String[] val = in.split(";");
		out.addNewUser(
				Integer.parseInt(val[0]), 
				val[1], 
				val[2], 
				val[3], 
				val[4], 
				Double.parseDouble(val[5]));
		return out;
	}
	public void findRecords(int col,String key) {
		boolean found = false;
		displayHeadline();
		for(ClientData cD : dataBase) {
			if(key.equals(cD.getSpecificData(col))) {
				cD.displayClient();
				found = true;
			}
		}
		if(!found) {
			System.out.println("No records found.");
		}
	}
	private void displayHeadline() {
		System.out.format("%4s %10s %15s   %11s %20s    %s\n\n", "ID","Name","Surname","PESEL","Adress","Balance");
	}
}






