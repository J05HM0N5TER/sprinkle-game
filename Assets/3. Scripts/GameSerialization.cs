using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public partial class GameSerialization : MonoBehaviour
{
	public LayerMask layersToSerialize;

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
				if (((1 << item.gameObject.layer) & layersToSerialize) != 0
					//&& item.gameObject.GetComponent<Rigidbody>()
					/* item.name == "Crate_Small" */
				)
				{
					// Debug.Log(item.gameObject.layer);
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
				reader.ReadStartElement(); // Body
				reader.ReadStartElement(); // PlayerController
				playerController.ReadXml(reader);
				reader.ReadEndElement(); // PlayerController
				// xmlReader.ReadStartElement(); // ObjectData
				//(GameSave.ObjectData) objectDataWriter.Deserialize(xmlReader);
				GameSerialization.ObjectData readData = new ObjectData();
				readData.ReadXml(reader);
				Convert.Copy(from: readData, to: playerController.transform);
				reader.ReadEndElement(); // Body
				reader.ReadStartElement(); // Camera
				// xmlReader.ReadEndElement(); // ObjectData
				readData.ReadXml(reader);
				Convert.Copy(from: readData, to: camera.transform);
				reader.ReadEndElement(); // Camera
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

		foreach (var item in dynamicObjects)
		{
			int position = readObjects.BinarySearch(Convert.New(item));
			if (position >= 0)
			{
				Debug.Log($"Binary search found {position} for {item.GetInstanceID()}");
				Convert.Copy(readObjects[position], item);
			}
			else
			{
				// Debug.LogWarning($"Could not find {item.ToString()} in serialized objects");
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
			if (
				((1 << item.gameObject.layer) & layersToSerialize) != 0)
			{
				// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
				sortedList.Add(item);
			}
		}

		// Sort by the same rules that the Serialised objects are so they hopefully match up
		sortedList.Sort(
			delegate(Transform a, Transform b)
			{
				return a.GetInstanceID().CompareTo(b.GetInstanceID());
			});

		return sortedList;
	}
}
