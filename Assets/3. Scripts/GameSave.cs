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
	public LayerMask layersToSerialize;

	public class ObjectData : IXmlSerializable, IComparable<ObjectData>
	{
		public string name;
		public System.Numerics.Vector3 localPosition;
		public System.Numerics.Quaternion localRotation;
		public System.Numerics.Vector3 localScale;
		public ObjectData()
		{
			localPosition = System.Numerics.Vector3.Zero;
			localRotation = System.Numerics.Quaternion.Identity;
			localScale = System.Numerics.Vector3.Zero;
		}

		// Xml Serialization Infrastructure

		/// <summary>
		/// Used for serializing data
		/// </summary>
		/// <param name="reader">The XMLWriter that is in the correct position in memory</param>
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));
			XmlSerializer quaternionWriter = new XmlSerializer(typeof(System.Numerics.Quaternion));
			writer.WriteStartElement(nameof(name));
			writer.WriteValue(name);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(localPosition));
			vector3Writer.Serialize(writer, localPosition);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(localRotation));
			quaternionWriter.Serialize(writer, localRotation);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(localScale));
			vector3Writer.Serialize(writer, localScale);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Used for deserializing data
		/// </summary>
		/// <param name="reader">The XMLReader that is in the correct position in memory</param>
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));
			XmlSerializer quaternionWriter = new XmlSerializer(typeof(System.Numerics.Quaternion));
			reader.ReadStartElement(); // ObjectData
			name = reader.ReadElementContentAsString();
			// reader.ReadEndElement(); // Nam
			reader.ReadStartElement(); // Position
			localPosition = (System.Numerics.Vector3) vector3Writer.Deserialize(reader);
			reader.ReadEndElement(); // Position
			reader.ReadStartElement(); // Rotation
			localRotation = (System.Numerics.Quaternion) quaternionWriter.Deserialize(reader);
			reader.ReadEndElement(); // Rotation
			reader.ReadStartElement(); // LocalScale
			localScale = (System.Numerics.Vector3) vector3Writer.Deserialize(reader);
			reader.ReadEndElement(); // LocalScale
			reader.ReadEndElement(); // ObjectData
		}

		public XmlSchema GetSchema()
		{
			return (null);
		}

		/// <summary>
		/// Compares two ObjectData and sorts by name
		/// </summary>
		/// <param name="other">The other ObjectData that it is being compared against</param>
		/// <returns>-1 or 1 if they are not equal and 0 if they are</returns>
		public int CompareTo(ObjectData other)
		{
			int comparison = String.Compare(this.name, other.name, comparisonType : StringComparison.OrdinalIgnoreCase);
			if (comparison < 0)
			{
				return -1;
			}
			else if (comparison > 0)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}

	private void Start()
	{
		if (layersToSerialize == 0)
		{
			layersToSerialize = LayerMask.NameToLayer("Dynamic");
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
		// Create the directory because an error happens if it doesn't
		Directory.CreateDirectory(Application.dataPath + "/Saves/");
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
		XmlSerializer listWriter = new XmlSerializer(typeof(List<ObjectData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameSave.ObjectData));

		using(XmlWriter xmlWriter = XmlWriter.Create((Application.dataPath + "/Saves/Save.xml")))
		{
			xmlWriter.WriteStartElement("root");
			xmlWriter.WriteStartElement("Player");

			playerWriter.Serialize(xmlWriter, playerController);
			objectDataWriter.Serialize(xmlWriter, Convert.New(playerController.transform));
			xmlWriter.WriteEndElement(); // Player

			// Dynamic objects
			Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
			List<ObjectData> dynamicObjects = new List<ObjectData>();

			foreach (var item in allObjects)
			{
				// If it has any of the layers that we are serializing
				if ((item.gameObject.layer & layersToSerialize) != 0)
				{
					// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
					dynamicObjects.Add(Convert.New(item));
				}
			}
			dynamicObjects.Sort();
			// xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement(nameof(dynamicObjects));
			listWriter.Serialize(xmlWriter, dynamicObjects);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

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
		// The serializer for the different types
		PlayerController playerController = FindObjectOfType<PlayerController>();
		XmlSerializer listWriter = new XmlSerializer(typeof(List<ObjectData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameSave.ObjectData));
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());

		// try
		// {
		//Directory.CreateDirectory(Application.dataPath + "/Saves/");
		List<ObjectData> readObjects = new List<ObjectData>();
		// Open file
		using(var stream = new FileStream(Application.dataPath + "/Saves/Save.xml", FileMode.Open))
		{
			// Open xml file reader
			using(XmlReader xmlReader = XmlReader.Create(stream))
			{
				xmlReader.ReadStartElement("root"); // root

				xmlReader.ReadStartElement(); // Player
				xmlReader.ReadStartElement(); // PlayerController
				playerController.ReadXml(xmlReader);
				xmlReader.ReadEndElement(); // PlayerController
				// xmlReader.ReadStartElement(); // ObjectData
				//(GameSave.ObjectData) objectDataWriter.Deserialize(xmlReader);
				GameSave.ObjectData readData = new ObjectData();
				readData.ReadXml(xmlReader);
				Convert.Copy(from: readData, to: playerController.transform);
				// xmlReader.ReadEndElement(); // ObjectData
				xmlReader.ReadEndElement(); // Player
				xmlReader.ReadStartElement(); // DynamicObjects
				readObjects = (List<ObjectData>) listWriter.Deserialize(xmlReader);
				xmlReader.ReadEndElement(); // DynamicObjects

				xmlReader.ReadEndElement(); // root
			}
		}

		// Sort the read objects (even though it should already be sorted)
		readObjects.Sort();

		// Retrieve all objects in scene
		List<Transform> dynamicObjects = new List<Transform>(FindObjectsOfType<Transform>());
		// Remove all that are not being serialized
		dynamicObjects = dynamicObjects.FindAll(
			delegate(Transform item)
			{
				// Keep if it has any of the required layers
				return (item.gameObject.layer & layersToSerialize) != 0;
			}
		);

		// Sort by name with the same rules as ObjectData so that the order matches
		dynamicObjects.Sort(
			delegate(Transform a, Transform b)
			{
				int comparison = String.Compare(a.name, b.name, comparisonType : StringComparison.OrdinalIgnoreCase);
				if (comparison < 0)
				{
					return -1;
				}
				else if (comparison > 0)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		);

		for (int i = 0; i < dynamicObjects.Count; i++)
		{
			Convert.Copy(readObjects[i], dynamicObjects[i]);
		}

		// }
		// catch (System.Exception e)
		// {
		// 	Debug.LogError($"Failed to load at {e.TargetSite} Info: {e.ToString()}", this);
		// 	throw;
		// }
	}
}
