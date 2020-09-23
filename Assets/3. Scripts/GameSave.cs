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
		PlayerController playerController = FindObjectOfType<PlayerController>();

		try
		{
			System.Xml.Serialization.XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
			Directory.CreateDirectory(Application.dataPath + "/Saves/");
			System.Xml.Serialization.XmlSerializer vector3Writer =
				new System.Xml.Serialization.XmlSerializer(typeof(System.Numerics.Vector3));

			System.Numerics.Vector3 temp = new System.Numerics.Vector3().OverWrite(transform.position);
			temp.OverWrite(transform.position);

			using(XmlWriter xmlWriter = XmlWriter.Create(Application.dataPath + "/Saves/Save.xml"))
			{
				xmlWriter.WriteStartElement("root");
				playerWriter.Serialize(xmlWriter, playerController);
				vector3Writer.Serialize(xmlWriter, temp);
				xmlWriter.WriteEndElement();
			}

		}
		catch (System.Exception e)
		{
			Debug.LogError("Failed to save " + e.ToString());
			throw;
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
