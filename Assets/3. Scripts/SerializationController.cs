using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using GameSerialization;
using UnityEngine;

public partial class SerializationController : MonoBehaviour
{
	private bool setUp = false;
	public LayerMask layersToSerialize;

	// Every object in the scene, sorted by InstanceID
	private SortedDictionary<int, Transform> allObjects = new SortedDictionary<int, Transform>();
	private List<int> allObjectToSerialize = new List<int>();

	public SortedDictionary<int, Transform> AllObjects { get => allObjects; }
	public List<int> AllObjectToSerialize { get => allObjectToSerialize; }

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
		// Create lookup tables
		PopulateLookup();

		Debug.Log("AllObjectsToSerilize size: " + AllObjectToSerialize.Count);
		Debug.Log("AllObjects size: " + allObjects.Count);

		// Find the currently active Living armour AI
		LivingArmourAI currentLivingArmour = FindObjectOfType<LivingArmourAI>();
		LivingArmourAI[] livingArmourAIs = FindObjectsOfType<LivingArmourAI>();
		foreach (var item in livingArmourAIs)
		{
			if (item.enabled)
			{
				currentLivingArmour = item;
			}
		}

		LurkerAi lurker = FindObjectOfType<LurkerAi>();

		PlayerController playerController = FindObjectOfType<PlayerController>();
		CameraControl camera = FindObjectOfType<CameraControl>();

		// Create the directory because an error happens if it doesn't
		Directory.CreateDirectory(Application.dataPath + "/Saves/");
		// XML sterilizers for all the classes needed
		XmlSerializer playerXML = new XmlSerializer(playerController.GetType());
		XmlSerializer cameraXML = new XmlSerializer(camera.GetType());
		XmlSerializer goDataListXML = new XmlSerializer(typeof(List<GameObjectData>));
		XmlSerializer rbDataListXML = new XmlSerializer(typeof(List<RigidbodyData>));
		XmlSerializer objectDataXML = new XmlSerializer(typeof(GameObjectData));
		XmlSerializer livingArmourXML = new XmlSerializer(typeof(LivingArmourAI));
		XmlSerializer lurkerXML = new XmlSerializer(typeof(LurkerAi));

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
						playerXML.Serialize(writer, playerController);
					}
					writer.WriteEndElement(); // Body
					writer.WriteStartElement("Camera");
					writer.WriteComment("The data controlling player camera movement and grabbing and interacting with objects");
					{
						cameraXML.Serialize(writer, camera);
					}
					writer.WriteEndElement(); // PlayerCamera
				}
				writer.WriteEndElement(); // Player

				writer.WriteStartElement("AI-Data");
				{
					// Only one living armour is active so record which one that is
					writer.WriteStartElement("LivingArmourInstanceID");
					writer.WriteValue(currentLivingArmour.gameObject.GetInstanceID());
					writer.WriteEndElement();
					livingArmourXML.Serialize(writer, currentLivingArmour);
					lurkerXML.Serialize(writer, lurker);
				}
				writer.WriteEndElement();

				// Dynamic objects
				List<GameObjectData> dynamicObjects = new List<GameObjectData>();
				List<RigidbodyData> rigidbodies = new List<RigidbodyData>();

				// Convert all objects to serialize into a data structure that can be saved
				foreach (var itemID in AllObjectToSerialize)
				{
					// dynamicObjects.con
					// Debug.Log($"Old: {item.position} New Object pos: {newObject.position}, rotation: {newObject.rotation}");
					dynamicObjects.Add(GameObjectData.New(AllObjects[itemID]));

					Rigidbody rb = AllObjects[itemID].GetComponent<Rigidbody>();
					if (rb)
					{
						rigidbodies.Add(RigidbodyData.New(rb));
					}
				}
				Debug.Log($"Serialized {dynamicObjects.Count} dynamic objects");

				dynamicObjects.Sort();
				// xmlWriter.WriteEndElement();
				writer.WriteStartElement(nameof(dynamicObjects));
				writer.WriteComment("This is all the objects that can move in the scene");
				{
					goDataListXML.Serialize(writer, dynamicObjects);
				}
				writer.WriteEndElement();

				rigidbodies.Sort();
				// xmlWriter.WriteEndElement();
				writer.WriteStartElement(nameof(rigidbodies));
				writer.WriteComment("This is all the rigidbodies from moving objects");
				{
					rbDataListXML.Serialize(writer, rigidbodies);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
		ClearAll();
	}

	public void LoadGame()
	{
		// Create all lookup tables
		PopulateLookup();

		// The serializer for the different types
		PlayerController playerController = FindObjectOfType<PlayerController>();
		CameraControl camera = FindObjectOfType<CameraControl>();

		XmlSerializer goDataListXML = new XmlSerializer(typeof(List<GameObjectData>));
		XmlSerializer rbDataListXML = new XmlSerializer(typeof(List<RigidbodyData>));
		XmlSerializer objectDataXML = new XmlSerializer(typeof(GameObjectData));
		XmlSerializer livingArmourXML = new XmlSerializer(typeof(LivingArmourAI));
		XmlSerializer lurkerXML = new XmlSerializer(typeof(LurkerAi));

		LurkerAi lurker = FindObjectOfType<LurkerAi>();

		LivingArmourAI[] livingArmours = FindObjectsOfType<LivingArmourAI>();

		List<GameObjectData> readGameObjects = new List<GameObjectData>();
		List<RigidbodyData> readRigidBodys = new List<RigidbodyData>();
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
					reader.ReadStartElement(); // AI-Data
					{
						int activeLivingArmourInstanceID = reader.ReadElementContentAsInt();
						reader.ReadStartElement();
						foreach (var item in livingArmours)
						{
							// If it is the one that was active
							if (item.gameObject.GetInstanceID() == activeLivingArmourInstanceID)
							{
								// Copy over the data
								item.ReadXml(reader);
							}
							// If it was disabled when it was saved
							else
							{
								// Make sure that it is disabled still
								item.enabled = false;
							}
						}
						reader.ReadEndElement();
						reader.ReadStartElement();
						// livingArmourXML.Deserialize(reader);
						lurker.ReadXml(reader);
						reader.ReadEndElement();
					}
					reader.ReadEndElement();
					reader.ReadStartElement(); // DynamicObjects
					{
						readGameObjects = (List<GameObjectData>) goDataListXML.Deserialize(reader);
					}
					reader.ReadEndElement(); // DynamicObjects
					reader.ReadStartElement(); // DynamicObjects
					{
						readRigidBodys = (List<RigidbodyData>) rbDataListXML.Deserialize(reader);
					}
					reader.ReadEndElement(); // DynamicObjects
				}
				reader.ReadEndElement(); // root
			}
		}

		// Apply all loaded gameObject data
		foreach (var itemID in AllObjectToSerialize)
		{
			// GameObject Data
			{
				int position = readGameObjects.BinarySearch(GameObjectData.New(AllObjects[itemID]));
				if (position >= 0)
				{
					// Debug.Log($"Binary search found {position} for {item.GetInstanceID()}");
					GameObjectData.Copy(readGameObjects[position], allObjects[itemID]);
				}
				else
				{
					Debug.LogWarning($"Could not find {itemID.ToString()} in serialized objects");
				}
			}

			// RigidBody Data
			Rigidbody rb = AllObjects[itemID].GetComponent<Rigidbody>();
			if (rb)
			{
				int position = readRigidBodys.BinarySearch(RigidbodyData.New(rb));
				if (position >= 0)
				{
					// Debug.Log($"Binary search found {position} for {item.GetInstanceID()}");
					RigidbodyData.Copy(readRigidBodys[position], rb);
				}
				else
				{
					Debug.LogWarning($"Could not find {AllObjects[itemID].ToString()} in serialized objects");
				}
			}
		}

		// Release all memory overhead
		ClearAll();
	}

	/// <summary>
	/// Converts an Instance ID to a Transform Reference
	/// </summary>
	/// <param name="instanceID">The ID of the object being retrieved</param>
	/// <returns>THe reference to the object</returns>
	/// <warning>If the instance is 0 then assumes that the value serialised was null</warning>
	public Transform InstanceIDToTransform(int instanceID)
	{
		if (instanceID == 0)
			return null;
		else
			return AllObjects[instanceID];
	}

	/// <summary>
	/// Populates the lookup tables for all of the objects in the scene
	/// </summary>
	public void RefetchAllObjects()
	{
		allObjects.Clear();
		Transform[] allTransforms = FindObjectsOfType<Transform>();
		foreach (var item in allTransforms)
		{
			allObjects.Add(item.GetInstanceID(), item);
		}
	}

	/// <summary>
	/// Populates the lookup tables for all of the objects in the layers to serialize
	/// </summary>
	public void PopulateAllObjectToSerialize()
	{
		allObjectToSerialize.Clear();
		foreach (var item in AllObjects)
		{
			if (((1 << item.Value.gameObject.layer) & layersToSerialize) != 0)
			{
				allObjectToSerialize.Add(item.Key);
			}
		}

	}

	/// <summary>
	/// Creates all lookup tables that are needed
	/// </summary>
	public void PopulateLookup()
	{
		RefetchAllObjects();
		PopulateAllObjectToSerialize();
		setUp = true;
	}

	/// <summary>
	/// Releases all the memory that the lookup tables are using
	/// </summary>
	public void ClearAll()
	{
		allObjects.Clear();
		allObjectToSerialize.Clear();
		setUp = false;
	}

	/// <summary>
	/// Deserializes gameObjectData, copies over the data and returns the transform
	/// </summary>
	/// <param name="reader">The XML Reader that is being used to deserialize the data</param>
	/// <returns>The reference to the transform that was being used</returns>
	public Transform DeserializeGameObject(XmlReader reader)
	{
		XmlSerializer gameObjectDataXML = new XmlSerializer(typeof(GameObjectData));
		GameObjectData data = (GameObjectData) gameObjectDataXML.Deserialize(reader);
		return GameObjectDataToTransform(data);
	}

	/// <summary>
	/// Copies over the data and return the transform of the gameObjectData
	/// </summary>
	/// <param name="from">The GameObjectData that the transform is retrieved from</param>
	/// <returns>The reference to the transform that was deserialized </returns>
	public Transform GameObjectDataToTransform(GameObjectData from)
	{
		if (!setUp)
			return null;
		Transform to = allObjects[from.instanceID];
		GameObjectData.Copy(from, to);
		return to;

	}
}
