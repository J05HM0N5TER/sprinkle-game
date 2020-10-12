﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using GameSerialization;
using UnityEngine;

public partial class SerializationController : MonoBehaviour
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
		// XML sterilizers for all the classes needed
		XmlSerializer playerWriter = new XmlSerializer(playerController.GetType());
		XmlSerializer cameraWriter = new XmlSerializer(camera.GetType());
		XmlSerializer goDataListWriter = new XmlSerializer(typeof(List<GameObjectData>));
		XmlSerializer rbDataListWriter = new XmlSerializer(typeof(List<RigidbodyData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameObjectData));

		using(XmlWriter writer = XmlWriter.Create((Application.dataPath + "/Saves/Save.xml")))
		{
			// All of the brackets are just to help me keep track with where I am
			// in the file structure
			writer.WriteStartElement("root");
			{
				writer.WriteStartElement("Player");
				writer.WriteComment("This is the start of all the player data, position data is not stored here");
				{
					writer.WriteStartElement("Body");
					writer.WriteComment("This is for the main player body, which is in charge of player movement");
					{
						playerWriter.Serialize(writer, playerController);
						// objectDataWriter.Serialize(writer,
						// Convert.New(playerController.transform));
					}
					writer.WriteEndElement(); // Body
					writer.WriteStartElement("Camera");
					writer.WriteComment("The data controlling player camera movement and grabbing and interacting with objects");
					{
						cameraWriter.Serialize(writer, camera);
					}
					writer.WriteEndElement(); // PlayerCamera
				}
				writer.WriteEndElement(); // Player

				// Dynamic objects
				Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
				List<GameObjectData> dynamicObjects = new List<GameObjectData>();
				List<RigidbodyData> rigidbodies = new List<RigidbodyData>();

				foreach (var item in allObjects)
				{
					// If it has any of the layers that we are serializing
					if (((1 << item.gameObject.layer) & layersToSerialize) != 0)
					{
						// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
						dynamicObjects.Add(GameObjectData.New(item));

						Rigidbody rb = item.GetComponent<Rigidbody>();
						if (rb)
						{
							rigidbodies.Add(RigidbodyData.New(rb));
						}
					}
				}
				Debug.Log($"Serialized {dynamicObjects.Count} dynamic objects");

				dynamicObjects.Sort();
				// xmlWriter.WriteEndElement();
				writer.WriteStartElement(nameof(dynamicObjects));
				writer.WriteComment("This is all the objects that can move in the scene");
				{
					goDataListWriter.Serialize(writer, dynamicObjects);
				}
				writer.WriteEndElement();

				rigidbodies.Sort();
				// xmlWriter.WriteEndElement();
				writer.WriteStartElement(nameof(rigidbodies));
				writer.WriteComment("This is all the rigidbodies from moving objects");
				{
					rbDataListWriter.Serialize(writer, rigidbodies);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
	}

	public void LoadGame()
	{
		// The serializer for the different types
		PlayerController playerController = FindObjectOfType<PlayerController>();
		CameraControl camera = FindObjectOfType<CameraControl>();

		XmlSerializer goDataListWriter = new XmlSerializer(typeof(List<GameObjectData>));
		XmlSerializer rbDataListWriter = new XmlSerializer(typeof(List<RigidbodyData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameObjectData));

		List<GameObjectData> readGameObjects = new List<GameObjectData>();
		List<RigidbodyData> readRigidbodys = new List<RigidbodyData>();
		// Open file
		using(var stream = new FileStream(Application.dataPath + "/Saves/Save.xml", FileMode.Open))
		{
			// Open xml file reader
			using(XmlReader reader = XmlReader.Create(stream))
			{
				// All of the brackets are just to help me keep track with where I am
				// in the file structure
				reader.ReadStartElement("root"); // root
				{
					reader.ReadStartElement(); // Player
					{
						reader.ReadStartElement(); // Body
						{
							reader.ReadStartElement(); // PlayerController
							{
								// Read MonoBehaviour scripts though the method because Unity doesn't
								// allow 'new' keyword on MonoBehaviour, which the default
								// deserialization requires
								playerController.ReadXml(reader);
							}
							reader.ReadEndElement(); // PlayerController
						}
						reader.ReadEndElement(); // Body
						reader.ReadStartElement(); // Camera
						{
							// xmlReader.ReadEndElement(); // ObjectData
							reader.ReadStartElement(); // CameraController
							{
								camera.ReadXml(reader);
							}
							reader.ReadEndElement(); // CameraController
						}
						reader.ReadEndElement(); // Camera
					}
					reader.ReadEndElement(); // Player
					reader.ReadStartElement(); // DynamicObjects
					{
						readGameObjects = (List<GameObjectData>) goDataListWriter.Deserialize(reader);
					}
					reader.ReadEndElement(); // DynamicObjects
					reader.ReadStartElement(); // DynamicObjects
					{
						readRigidbodys = (List<RigidbodyData>) rbDataListWriter.Deserialize(reader);
					}
					reader.ReadEndElement(); // DynamicObjects
				}
				reader.ReadEndElement(); // root
			}
		}

		// Sort the read objects (even though it should already be sorted)
		readGameObjects.Sort();
		readRigidbodys.Sort();

		// Retrieve all objects in scene
		List<Transform> dynamicObjects = new List<Transform>(FindObjectsOfType<Transform>());

		List<Transform> loadObjects = new List<Transform>();
		// REmove all unwanted objects
		foreach (var item in dynamicObjects)
		{
			// If it has any of the layers that we are serializing
			if (((1 << item.gameObject.layer) & layersToSerialize) != 0)
			{
				// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
				loadObjects.Add(item);
			}
		}

		loadObjects.Sort(
			delegate(Transform a, Transform b)
			{
				return a.GetInstanceID().CompareTo(b.GetInstanceID());
			}
		);

		// Apply all loaded gameObject data
		foreach (var item in loadObjects)
		{
			int position = readGameObjects.BinarySearch(GameObjectData.New(item));
			if (position >= 0)
			{
				// Debug.Log($"Binary search found {position} for {item.GetInstanceID()}");
				GameObjectData.Copy(readGameObjects[position], item);
			}
			else
			{
				Debug.LogWarning($"Could not find {item.ToString()} in serialized objects");
			}
		}

		// Apply all loaded rigidbody data
		foreach (var item in loadObjects)
		{
			Rigidbody rb = item.GetComponent<Rigidbody>();
			if (rb)
			{
				int position = readRigidbodys.BinarySearch(RigidbodyData.New(rb));
				if (position >= 0)
				{
					// Debug.Log($"Binary search found {position} for {item.GetInstanceID()}");
					RigidbodyData.Copy(readRigidbodys[position], rb);
				}
				else
				{
					Debug.LogWarning($"Could not find {item.ToString()} in serialized objects");
				}
			}
		}
	}
}
