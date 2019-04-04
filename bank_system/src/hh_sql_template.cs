private static void runDatabase() {
	myConnection = new SqlConnection
		("Server= SA-INFO-23\\SQLEXPRESS; "+
		"Database= TestBD; "+
		"Integrated Security=True;");
	myConnection.Open();
}
protected SqlDataReader ExecCommandSafe(SqlCommand cmd, bool execReader)
{
	int errorCounter = 6;

	while (true)
	{

		if ((conSql2000.State == ConnectionState.Broken) || (conSql2000.State == ConnectionState.Closed))
		{
			conSql2000.Open();
		}
		SqlDataReader reader = null;
		try
		{
			if (execReader)
			{
				reader = cmd.ExecuteReader();
				return reader;
			}
			else
			{
				cmd.ExecuteNonQuery();

			}
			return null;
		}
		catch (SqlException e)
		{
			if (e.Number == 1205) //deadlock
			{
				if (execReader && reader != null)
				{
					reader.Close();
				}
				System.Threading.Thread.Sleep(1500);
				continue;
			}
			else if (e.Message.IndexOf("General network", StringComparison.InvariantCultureIgnoreCase) > -1)
			{
				
				conSql2000.Close();
				//_Log.WriteOperationError(e);
				if (execReader && reader != null)
				{
					reader.Close();
				}
				if (errorCounter.Equals(1))
				{
					SoundHelper.PlayInfo();
					Messages.ShowInfoMessage("Wystąpił problem z siecią.");
				}
			}
			else
			{
				throw;
			}

			if (errorCounter > 0)
			{
				//MessageBox.Show(e.Number.ToString());
				errorCounter--;
				System.Threading.Thread.Sleep(1000);
				continue;
			}
			else
			{
				throw;
			}
		}
		catch (Exception e)
		{

			HandleException(e);
		}

	}
}

public List<int> getPrzesunieciaNakazaneStrefy(int id)
{
	SqlConnection conn = null;
	SqlDataReader wyn = null;
	List<int> result = null;
	try
	{
		conn = new SqlConnection(Stale.ConnectionString());
		conn.Open();

		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
		cmd.CommandText = "select id_przesuniecia from hh2.nakazane_strefy where operator = " + id;
		wyn = ExecCommandSafe(cmd, true);
		result = new List<int>();
		while (wyn.Read())
		{
			
			if (!wyn.IsDBNull(0))
			{
				result.Add(wyn.GetInt32(0));
			}
		}

	}
	catch (Exception ee)
	{
		MessageBox.Show(ee.Message);
	}
	finally
	{
		if (conn != null && conn.State == ConnectionState.Open)
		{
			conn.Close();
		}
		if (wyn != null)
		{
			wyn.Close();
		}
	}
	return result;
}

public void ustawNakazanaStrefeById(int id, int id_przesuniecia)
{
	SqlDataReader wyn = null;

	try
	{
		CheckConnection();
		SqlCommand cmd = new SqlCommand("[hh2].[ustaw_nakazana_strefe_by_id]", conSql2000);
		cmd.Parameters.AddWithValue("@operator", id);
		cmd.Parameters.AddWithValue("@id_przesuniecia", id_przesuniecia);

		cmd.CommandType = CommandType.StoredProcedure;
		cmd.ExecuteNonQuery();
	}
	catch (SqlException e)
	{
		HandleSqlException(e);
	}
	catch (Exception e)
	{
		HandleException(e);
	}
	finally
	{
		if (wyn != null)
		{
			wyn.Close();
		}
	}
}

public void UstawNakazaneRozkladanie(int id, string strefa)
{
	SqlConnection conn = null;
	SqlDataReader wyn = null;
	try
	{
		conn = new SqlConnection(Stale.ConnectionString());
		conn.Open();

		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
		cmd.CommandText = "insert into hh2.nakazane_strefy values("+ id +",'"+ strefa +"', 2)";
		cmd.ExecuteNonQuery();

	}
	catch (Exception ee)
	{
		MessageBox.Show(ee.Message);
	}
	finally
	{
		if (conn != null && conn.State == ConnectionState.Open)
		{
			conn.Close();
		}
		if (wyn != null)
		{
			wyn.Close();
		}
	}
}

public NakazStrefy getOrderedArea(int id)
{
	SqlConnection conn = null;
	SqlDataReader wyn = null;
	NakazStrefy result = null;
	try
	{
		conn = new SqlConnection(Stale.ConnectionString());
		conn.Open();

		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
		cmd.CommandText = "select top 1 * from hh2.nakazane_strefy where operator = " + id;
		wyn = ExecCommandSafe(cmd, true);
		if (wyn.Read())
		{
			result = new NakazStrefy();
			if (!wyn.IsDBNull(1))
			{
				result.strefa = wyn.GetString(1);
			}
			if (!wyn.IsDBNull(2))
			{
				result.id_przekierowania = wyn.GetInt32(2);
			}
		}

	}
	catch (Exception ee)
	{
		MessageBox.Show(ee.Message);
	}
	finally
	{
		if (conn != null && conn.State == ConnectionState.Open)
		{
			conn.Close();
		}
		if (wyn != null)
		{
			wyn.Close();
		}
	}
	return result;
}

public string getLangId()
{
	SqlDataReader wyn = null;

	try
	{
		CheckConnection();
		SqlCommand cmd = new SqlCommand("hh2.test_lokalizacji", conSql2000);

		cmd.CommandType = CommandType.StoredProcedure;
		return cmd.ExecuteScalar().ToString();
	}
	catch (SqlException e)
	{
		HandleSqlException(e);
	}
	catch (Exception e)
	{
		HandleException(e);
	}
	finally
	{
		if (wyn != null)
		{
			wyn.Close();
		}
	}
	return "";
}

public void saveLps(List<KompInfo> ki, string Dane)
{
	String query = "insert into hh2.zlecenia_posortowane values (@koord, @miejsce_doc, @lp)";
	SqlCommand command = new SqlCommand(query, conSql2000);
	try
	{
		command.Parameters.Add("@koord", SqlDbType.VarChar);
		command.Parameters.Add("@miejsce_doc", SqlDbType.VarChar);
		command.Parameters.Add("@lp", SqlDbType.Int);
		foreach (KompInfo k in ki)
		{
			command.Parameters["@koord"].Value = k.koordynata_skad_komp;
			command.Parameters["@miejsce_doc"].Value = Dane;
			command.Parameters["@lp"].Value = k.najlepsza_pozycja;
			command.ExecuteNonQuery();
		}
	}
	catch (SqlException)
	{
		Messages.ShowInfoMessage("ex");
	}
	
}
public List<CollectedBook> getKomasListPlatform(string koordynata)
{

	SqlDataReader wyn = null;
	List<CollectedBook> list = null;
	Win32API.Cursor.WaitCursor(true);
	try
	{
		CheckConnection();
		SqlCommand cmd = new SqlCommand("[hh2].[komasacja_platforma_pobierz_zlecenia]", conSql2000);
		cmd.Parameters.AddWithValue("@koordynata", koordynata);
		//MessageBox.Show(koordynata + " " + typ_operacji + " " + kosz + " " + id_operator + " " + tryb_zbierania);

		cmd.CommandType = CommandType.StoredProcedure;
		wyn = ExecCommandSafe(cmd, true);

		list = new List<CollectedBook>();
		while (wyn.Read())
		{
			CollectedBook book = new CollectedBook();
			if (!wyn.IsDBNull(0))
			{
				book.place = wyn.GetString(0);
			}
			list.Add(book);
		}
		return list;
	}
	catch (SqlException e)
	{
		HandleSqlException(e);
	}
	catch (Exception e)
	{
		HandleException(e);
	}
	finally
	{
		if (wyn != null)
		{
			wyn.Close();
		}
		Win32API.Cursor.WaitCursor(false);
	}
	return list;
}
internal bool KPAnulujWydanie(int idWydania)
{
	SqlConnection conn = null;
	SqlDataReader wyn = null;
	try
	{
		conn = new SqlConnection(Stale.ConnectionString());
		conn.Open();

		SqlCommand cmd = new SqlCommand("struser.wydania_pal_anulacja", conSql2000);
		cmd.Parameters.AddWithValue("@id", idWydania);

		cmd.CommandType = CommandType.StoredProcedure;
		cmd.ExecuteNonQuery();
		return true;
	}
	catch (Exception ee)
	{
		MessageBox.Show(ee.Message);
		return false;
	}
	finally
	{
		if (conn != null && conn.State == ConnectionState.Open)
		{
			conn.Close();
		}
		if (wyn != null)
		{
			wyn.Close();
		}
	}
}
internal List<string[]> paletowanieListaDrukarek()
{
	SqlDataReader wyn = null;
	List<string[]> list = null;
	Win32API.Cursor.WaitCursor(true);
	try
	{
		CheckConnection();
		SqlCommand cmd = new SqlCommand("[hh2].[paletowanie_drukarki]", conSql2000);
		//MessageBox.Show(koordynata + " " + typ_operacji + " " + kosz + " " + id_operator + " " + tryb_zbierania);

		cmd.CommandType = CommandType.StoredProcedure;
		wyn = ExecCommandSafe(cmd, true);

		list = new List<string[]>();
		while (wyn.Read())
		{
			string obiekt = "";
			if (!wyn.IsDBNull(0))
			{
				obiekt = wyn.GetString(0);
			}
			list.Add(new string[] { obiekt });
		}
		return list;
	}
	catch (SqlException e)
	{
		HandleSqlException(e);
	}
	catch (Exception e)
	{
		HandleException(e);
	}
	finally
	{
		if (wyn != null)
		{
			wyn.Close();
		}
		Win32API.Cursor.WaitCursor(false);
	}
	return list;
}
internal void MMWczytajSkadDokad(int id, out string skad, out string dokad)
{
	SqlConnection conn = null;
	SqlDataReader wyn = null;
	skad = "";
	dokad = "";
	try
	{
		conn = new SqlConnection(Stale.ConnectionString());
		conn.Open();

		CheckConnection();
		SqlCommand cmd = new SqlCommand("[struser].[wydania_skad_dokad]", conSql2000);
		cmd.Parameters.AddWithValue("@id", id);


		SqlParameter skadSQL = cmd.Parameters.Add("@skad", SqlDbType.VarChar, 10);
		skadSQL.Direction = ParameterDirection.Output;

		SqlParameter dokadSQL = cmd.Parameters.Add("@dokad", SqlDbType.VarChar, 10);
		dokadSQL.Direction = ParameterDirection.Output;

		cmd.CommandType = CommandType.StoredProcedure;
		cmd.ExecuteNonQuery();
		if (skadSQL.Value != null)
		{
			skad = skadSQL.Value.ToString();
		}
		if (dokadSQL.Value != null)
		{
			dokad = dokadSQL.Value.ToString();
		}


	}
	catch (Exception ee)
	{
		MessageBox.Show(ee.Message);
	}
	finally
	{
		if (conn != null && conn.State == ConnectionState.Open)
		{
			conn.Close();
		}
		if (wyn != null)
		{
			wyn.Close();
		}
	}
}
		
		