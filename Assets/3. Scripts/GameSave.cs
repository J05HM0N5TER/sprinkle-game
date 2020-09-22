using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

// NOTE This whole class is going to be super dodgy

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

		XmlElement player = xmlDocument.CreateElement("Player");
		PlayerController playerController = FindObjectOfType<PlayerController>();
		// player.SetAttribute("Player", )
		// System.Xml.Serialization.XmlSerializer playerSerializer = new XmlSerializer(playerController.GetType());
		// FileStream playerStream = new FileStream(Application.dataPath + "/Saves/SavePlayer.xml", FileMode.Create);
		// playerSerializer.Serialize(playerStream, playerController);

		try
		{
			// XmlElement saveData = xmlDocument.CreateElement("Save");
			// File.Create(Application.dataPath + "/Saves/Save2.xml");
			// XmlTextWriter test = new XmlTextWriter(Application.dataPath + "/Saves/Save2.xml", System.Text.Encoding.UTF8);
			// test.Close();
			// test.Dispose();
			// saveData.SetAttribute("Test Vector3", new System.Numerics.Vector3(5.1f, 3f, 7f))
			//saveData.a
			Directory.CreateDirectory(Application.dataPath + "/Saves/");
			System.Numerics.Vector3 testVector3 = new System.Numerics.Vector3(5.1f, 3f, 7f);
			System.Xml.Serialization.XmlSerializer vector3Writer =
				new System.Xml.Serialization.XmlSerializer(typeof(System.Numerics.Vector3));

			System.Numerics.Vector3 temp = new System.Numerics.Vector3().OverWrite(transform.position);
			temp.OverWrite(transform.position);

			using(XmlWriter test2 = XmlWriter.Create(Application.dataPath + "/Saves/Save2.xml"))
			{
				vector3Writer.Serialize(test2, testVector3);
			}

		}
		catch (System.Exception e)
		{
			Debug.LogError("Failed to save " + e.ToString());
			throw;
		}

		xmlDocument.Save(Application.dataPath + "/Saves/Save.xml");
		if (File.Exists(Application.dataPath + "/Saves/Save.xml"))
		{
			Debug.Log("Game Saved", this);
		}
	}

	public void LoadGame()
	{

	}

	private List<string> ExtractVariables(Type classType)
	{
		BindingFlags bindingFlags = BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance |
			BindingFlags.Static;

		List<string> names = new List<string>();
		foreach (FieldInfo field in classType.GetFields(bindingFlags))
		{
			names.Add(field.Name);
		}

		return names;
	}
}
