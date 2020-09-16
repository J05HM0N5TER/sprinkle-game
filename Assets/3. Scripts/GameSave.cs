using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using System.IO;

public class GameSave : MonoBehaviour
{

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Debug.Log("Save button pressed");
			SaveGame();
		}
	}

	public void SaveGame()
	{
		XmlDocument xmlDocument = new XmlDocument();

		// Basic file info
		XmlElement fileInfo = xmlDocument.CreateElement("FileInfo");
		fileInfo.SetAttribute("FileName", "SaveData");
		fileInfo.SetAttribute("VersionNumber", "0.01");
		fileInfo.SetAttribute("Time", DateTime.Now.ToString());
		fileInfo.SetAttribute("TestID", this.GetInstanceID().ToString());

		xmlDocument.AppendChild(fileInfo);

		XmlAttribute saveData = xmlDocument.CreateAttribute("Save");
		//saveData.a

		xmlDocument.Save(Application.dataPath + "/Saves/Save.xml");
		if (File.Exists(Application.dataPath + "/Saves/Save.xml"))
		{
			Debug.Log("Game Saved", this);
		}
	}


	public void LoadGame()
	{

	}
}
