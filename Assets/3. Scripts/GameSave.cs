using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

// NOTE This whole class is going to be super dodgy

[System.Serializable]

public class GameSave : MonoBehaviour
{
	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F1))
		{
			Debug.Log ("Save button pressed");
			SaveGame ();
		}
	}

	public void SaveGame ()
	{
		XmlDocument xmlDocument = new XmlDocument ();

		// Basic file info
		XmlElement fileInfo = xmlDocument.CreateElement ("FileInfo");
		fileInfo.SetAttribute ("FileName", "SaveData");
		fileInfo.SetAttribute ("VersionNumber", "0.01");
		fileInfo.SetAttribute ("Time", DateTime.Now.ToString ());
		fileInfo.SetAttribute ("TestID", this.GetInstanceID ().ToString ());

		xmlDocument.AppendChild (fileInfo);

		XmlElement player = xmlDocument.CreateElement ("Player");
		PlayerController playerController = FindObjectOfType<PlayerController> ();
		// player.SetAttribute("Player", )
		System.Xml.Serialization.XmlSerializer playerSerializer = new XmlSerializer (playerController.GetType ());
		FileStream playerStream = new FileStream (Application.dataPath + "/Saves/SavePlayer.xml", FileMode.Create);
		playerSerializer.Serialize (playerStream, playerController);

		XmlAttribute saveData = xmlDocument.CreateAttribute ("Save");
		//saveData.a

		xmlDocument.Save (Application.dataPath + "/Saves/Save.xml");
		if (File.Exists (Application.dataPath + "/Saves/Save.xml"))
		{
			Debug.Log ("Game Saved", this);
		}
	}

	public void LoadGame ()
	{

	}

	private List<string> ExtractVariables (Type classType)
	{
		BindingFlags bindingFlags = BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance |
			BindingFlags.Static;

		List<string> names = new List<string> ();
		foreach (FieldInfo field in classType.GetFields (bindingFlags))
		{
			names.Add (field.Name);
		}

		return names;
	}
}