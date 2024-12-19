using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataToCSV : MonoBehaviour
{
	private List<string[]> data = new List<string[]>();

	private string fileName = "SavedPlayerSpeed.csv";

	public List<float> times = new List<float>();

	public List<float> speeds = new List<float>();

	public RetrieveData retrieveData;

	public void SaveToCSV()
	{
		string folderPath = Path.Combine(Application.dataPath, "CSV_Data");
		string filePath = Path.Combine(folderPath, fileName);

		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		try
		{
			data.Add(new string[] { "PlayerSpeed", "Time" });

			speeds = retrieveData.playerSpeeds;
			for (int i = 0; i < speeds.Count; i++)
			{
				times.Add(i);
				data.Add(new string[] { speeds[i].ToString(), times[i].ToString() });
			}

			using (StreamWriter writer = new StreamWriter(filePath))
			{
				foreach (string[] row in data)
				{
					writer.WriteLine(string.Join(",", row));
				}
			}

			Debug.Log($"Dati salvati con successo in: {filePath}");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Errore durante il salvataggio: {ex.Message}");
		}
	}
}
