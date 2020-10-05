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
		else if (Input.GetKeyDown(KeyCode.F2))
		{
			LoadGame();
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

			System.Numerics.Vector3 temp = new System.Numerics.Vector3().CopyFrom(transform.position);
			temp.CopyFrom(transform.position);

			using(XmlWriter xmlWriter = XmlWriter.Create((Application.dataPath + "/Saves/Save.xml")))
			{
				xmlWriter.WriteStartElement("root");
				playerWriter.Serialize(xmlWriter, playerController);
				// xmlWriter.WriteEndElement();
				vector3Writer.Serialize(xmlWriter, temp);
				// xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}

		}
		catch (System.Exception e)
		{
			Debug.LogError($"Failed to load at {e.Source} Info: {e.ToString()}");
			throw;
		}
	}

	public void LoadGame()
	{
		PlayerController playerController = FindObjectOfType<PlayerController>();

		try
		{
			XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
			//Directory.CreateDirectory(Application.dataPath + "/Saves/");
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));

			System.Numerics.Vector3 temp = new System.Numerics.Vector3().CopyFrom(transform.position);
			temp.CopyFrom(transform.position);

			using(var stream = new FileStream(Application.dataPath + "/Saves/Save.xml", FileMode.Open))
			{
				using(XmlReader xmlReader = XmlReader.Create(stream))
				{
					xmlReader.ReadStartElement("root");

					playerController.ReadXml(xmlReader);

					// playerController = (PlayerController) playerWriter.Deserialize(xmlReader);
					temp = (System.Numerics.Vector3) vector3Writer.Deserialize(xmlReader);
					xmlReader.ReadEndElement();
				}
			}

		}
		catch (System.Exception e)
		{
			Debug.LogError($"Failed to load at {e.TargetSite} Info: {e.ToString()}", this);
			throw;
		}
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
