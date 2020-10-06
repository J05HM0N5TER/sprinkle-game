using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class GameSave : MonoBehaviour
{

	public class ObjectData : IXmlSerializable
	{
		public string name;
		public System.Numerics.Vector3 position;
		public System.Numerics.Quaternion rotation;
		public System.Numerics.Vector3 localScale;
		public ObjectData()
		{
			position = System.Numerics.Vector3.Zero;
			rotation = System.Numerics.Quaternion.Identity;
			localScale = System.Numerics.Vector3.Zero;
		}

		// Xml Serialization Infrastructure

		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));
			XmlSerializer quaternionWriter = new XmlSerializer(typeof(System.Numerics.Quaternion));
			writer.WriteStartElement(nameof(name));
			writer.WriteValue(name);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(position));
			vector3Writer.Serialize(writer, position);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(rotation));
			quaternionWriter.Serialize(writer, rotation);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(localScale));
			vector3Writer.Serialize(writer, localScale);
			writer.WriteEndElement();

		}

		public void ReadXml(XmlReader reader)
		{
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));
			XmlSerializer quaternionWriter = new XmlSerializer(typeof(System.Numerics.Quaternion));
			name = reader.ReadElementContentAsString();
			position = (System.Numerics.Vector3) vector3Writer.Deserialize(reader);
			rotation = (System.Numerics.Quaternion) quaternionWriter.Deserialize(reader);
			localScale = (System.Numerics.Vector3) vector3Writer.Deserialize(reader);
		}

		public XmlSchema GetSchema()
		{
			return (null);
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Debug.Log("Save button pressed");
			SaveGame();
		}
		else if (Input.GetKeyDown(KeyCode.F2))
		{
			Debug.Log("Load button pressed");
			LoadGame();
		}
	}

	public void SaveGame()
	{
		PlayerController playerController = FindObjectOfType<PlayerController>();

		// try
		// {
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
		Directory.CreateDirectory(Application.dataPath + "/Saves/");
		XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));
		XmlSerializer listWriter = new XmlSerializer(typeof(List<ObjectData>));

		using(XmlWriter xmlWriter = XmlWriter.Create((Application.dataPath + "/Saves/Save.xml")))
		{
			xmlWriter.WriteStartElement("root");
			playerWriter.Serialize(xmlWriter, playerController);

			// Dynamic objects
			Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
			List<ObjectData> dynamicObjects = new List<ObjectData>();

			foreach (var item in allObjects)
			{
				if (item.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
				{
					// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
					dynamicObjects.Add(Convert.New(item));
				}
			}


			System.Numerics.Vector3 test = new System.Numerics.Vector3();
			Vector3 test2 = new Vector3(1, 5, 4);
			Debug.Log("Vector is " + test.ToString());
			Debug.Log("Other Vector is " + test2.ToString());
			test = Convert.Copy(test2, test);
			Debug.Log("Vector is " + test.ToString());

			// xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("DynamicObjects");
			// xmlWriter.WriteValue(dynamicObjects);
			// XmlSerializer objectDataWriter = new XmlSerializer(typeof(ObjectData));
			// objectDataWriter.Serialize(xmlWriter, new ObjectData().CopyFrom(this.transform));
			xmlWriter.WriteEndElement();
			listWriter.Serialize(xmlWriter, dynamicObjects);
			// xmlWriter.WriteEndElement();

			xmlWriter.WriteEndDocument();
		}

		// }
		// catch (System.Exception e)
		// {
		// 	Debug.LogError($"Failed to load at {e.Source} Info: {e.ToString()}");
		// 	throw;
		// }
	}

	public void LoadGame()
	{
		PlayerController playerController = FindObjectOfType<PlayerController>();

		try
		{
			XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
			//Directory.CreateDirectory(Application.dataPath + "/Saves/");
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));


			using(var stream = new FileStream(Application.dataPath + "/Saves/Save.xml", FileMode.Open))
			{
				using(XmlReader xmlReader = XmlReader.Create(stream))
				{
					xmlReader.ReadStartElement("root"); // root

					xmlReader.ReadStartElement(nameof(PlayerController)); // PlayerController
					playerController.ReadXml(xmlReader);
					xmlReader.ReadEndElement(); // PlayerController

					xmlReader.ReadEndElement(); // root
				}
			}

		}
		catch (System.Exception e)
		{
			Debug.LogError($"Failed to load at {e.TargetSite} Info: {e.ToString()}", this);
			throw;
		}
	}
}
