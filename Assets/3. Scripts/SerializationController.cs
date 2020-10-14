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
		ClearAll();
	}

	public void LoadGame()
	{
		// Create all lookup tables
		PopulateLookup();

		// The serializer for the different types
		PlayerController playerController = FindObjectOfType<PlayerController>();
		CameraControl camera = FindObjectOfType<CameraControl>();

		XmlSerializer goDataListWriter = new XmlSerializer(typeof(List<GameObjectData>));
		XmlSerializer rbDataListWriter = new XmlSerializer(typeof(List<RigidbodyData>));
		XmlSerializer objectDataWriter = new XmlSerializer(typeof(GameObjectData));

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
					reader.ReadStartElement(); // DynamicObjects
					{
						readGameObjects = (List<GameObjectData>) goDataListWriter.Deserialize(reader);
					}
					reader.ReadEndElement(); // DynamicObjects
					reader.ReadStartElement(); // DynamicObjects
					{
						readRigidBodys = (List<RigidbodyData>) rbDataListWriter.Deserialize(reader);
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
	public Transform InstanceIDToTransform(int instanceID)
	{
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
			if ((( 1 << item.Value.gameObject.layer) & layersToSerialize) != 0)
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
	}

	/// <summary>
	/// Releases all the memory that the lookup tables are using
	/// </summary>
	public void ClearAll()
	{
		allObjects.Clear();
		allObjectToSerialize.Clear();
	}
}
