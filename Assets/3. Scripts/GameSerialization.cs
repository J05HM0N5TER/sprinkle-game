using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class GameSerialization : MonoBehaviour
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
			localPosition = new System.Numerics.Vector3();
			localRotation = new System.Numerics.Quaternion();
			localScale = new System.Numerics.Vector3();
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
		CameraControl camera = FindObjectOfType<CameraControl>();

		// Create the directory because an error happens if it doesn't
		Directory.CreateDirectory(Application.dataPath + "/Saves/");
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
		XmlSerializer listWriter = new XmlSerializer(typeof(List<ObjectData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameSerialization.ObjectData));

		using(XmlWriter writer = XmlWriter.Create((Application.dataPath + "/Saves/Save.xml")))
		{
			writer.WriteStartElement("root");
			writer.WriteStartElement("Player");

			writer.WriteStartElement("Body");
			playerWriter.Serialize(writer, playerController);
			objectDataWriter.Serialize(writer, Convert.New(playerController.transform));
			writer.WriteEndElement(); // Body
			writer.WriteStartElement("Camera");
			objectDataWriter.Serialize(writer, Convert.New(camera.transform));
			writer.WriteEndElement(); // PlayerCamera
			writer.WriteEndElement(); // Player

			// Dynamic objects
			Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
			List<ObjectData> dynamicObjects = new List<ObjectData>();

			foreach (var item in allObjects)
			{
				// If it has any of the layers that we are serializing
				if ((item.gameObject.layer & LayerMask.NameToLayer("Dynamic")) != 0 &&
					item.gameObject.GetComponent<Rigidbody>())
				{
					// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
					dynamicObjects.Add(Convert.New(item));
				}
			}
			Debug.Log($"Serialized {dynamicObjects.Count} dynamic objects");

			dynamicObjects.Sort();
			// xmlWriter.WriteEndElement();
			writer.WriteStartElement(nameof(dynamicObjects));
			listWriter.Serialize(writer, dynamicObjects);
			writer.WriteEndElement();
			writer.WriteEndElement();

			writer.WriteEndDocument();
		}
	}

	public void LoadGame()
	{
		// The serializer for the different types
		PlayerController playerController = FindObjectOfType<PlayerController>();
		CameraControl camera = FindObjectOfType<CameraControl>();

		XmlSerializer listWriter = new XmlSerializer(typeof(List<ObjectData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameSerialization.ObjectData));
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());

		List<ObjectData> readObjects = new List<ObjectData>();
		// Open file
		using(var stream = new FileStream(Application.dataPath + "/Saves/Save.xml", FileMode.Open))
		{
			// Open xml file reader
			using(XmlReader reader = XmlReader.Create(stream))
			{
				reader.ReadStartElement("root"); // root

				reader.ReadStartElement(); // Player
				reader.ReadStartElement(); // PlayerController
				playerController.ReadXml(reader);
				reader.ReadEndElement(); // PlayerController
				// xmlReader.ReadStartElement(); // ObjectData
				//(GameSave.ObjectData) objectDataWriter.Deserialize(xmlReader);
				GameSerialization.ObjectData readData = new ObjectData();
				readData.ReadXml(reader);
				Convert.Copy(from: readData, to: playerController.transform);
				// xmlReader.ReadEndElement(); // ObjectData
				readData.ReadXml(reader);
				Convert.Copy(from: readData, to: camera.transform);
				reader.ReadEndElement(); // Player
				reader.ReadStartElement(); // DynamicObjects
				readObjects = (List<ObjectData>) listWriter.Deserialize(reader);
				reader.ReadEndElement(); // DynamicObjects

				reader.ReadEndElement(); // root
			}
		}

		// Sort the read objects (even though it should already be sorted)
		readObjects.Sort();

		// Retrieve all objects in scene
		List<Transform> dynamicObjects = new List<Transform>(FindObjectsOfType<Transform>());

		List<Transform> itemsSerialised = new List<Transform>();
		
		SortSerialiseObjects(dynamicObjects);

		for (int i = 0; i < dynamicObjects.Count; i++)
		{
			if (dynamicObjects[i].name == readObjects[i].name)
			{
				Convert.Copy(readObjects[i], dynamicObjects[i]);
			}
			else
			{
				Debug.LogWarning($"Read object {readObjects[i].name} doesn't match up to {dynamicObjects[i].name}");
			}
		}
	}

	private List<Transform> SortSerialiseObjects(List<Transform> list)
	{
		List<Transform> sortedList = new List<Transform>();
		// REmove all unwanted objects
		foreach (var item in list)
		{
			// If it has any of the layers that we are serializing
			if ((item.gameObject.layer & LayerMask.NameToLayer("Dynamic")) != 0 &&
				item.gameObject.GetComponent<Rigidbody>())
			{
				// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
				//// Add the parent because that is what stores the transform
				sortedList.Add(item);
			}
		}

		// Sort by the same rules that the Serialised objects are so they hopefully match up
		sortedList.Sort(
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
			});

		return sortedList;
	}
}
