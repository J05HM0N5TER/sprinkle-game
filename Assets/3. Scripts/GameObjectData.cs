using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace GameSerialization
{
	public class GameObjectData : IXmlSerializable, IComparable<GameObjectData>, IComparable<Transform>, IEquatable<Transform>
	{
		public string name;
		public int instanceID;
		public System.Numerics.Vector3 localPosition;
		public System.Numerics.Quaternion localRotation;
		public System.Numerics.Vector3 localScale;
		public GameObjectData()
		{
			localPosition = new System.Numerics.Vector3();
			localRotation = new System.Numerics.Quaternion();
			localScale = new System.Numerics.Vector3();
		}

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
			writer.WriteStartElement(nameof(instanceID));
			writer.WriteValue(instanceID);
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
			// reader.ReadStartElement(); // InstanceID
			instanceID = reader.ReadElementContentAsInt();
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
		/// Compares two ObjectData and sorts by ID
		/// </summary>
		/// <param name="other">The other ObjectData that it is being compared against</param>
		/// <returns>0 If equal and anything else if not</returns>
		public int CompareTo(GameObjectData other)
		{
			return instanceID.CompareTo(other.instanceID);
		}

		/// <summary>
		/// Compares a transform ID to an ObjectData ID and sorts by ID
		/// </summary>
		/// <param name="other">The transform that this is being compared against</param>
		/// <returns>0 If equal and anything else if not</returns>
		public int CompareTo(Transform other)
		{
			return instanceID.CompareTo(other.GetInstanceID());
		}

		public override bool Equals(object obj)
		{
			return obj is GameObjectData data &&
				instanceID == data.instanceID;
		}

		public override int GetHashCode()
		{
			int hashCode = -1470452096;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
			hashCode = hashCode * -1521134295 + instanceID.GetHashCode();
			hashCode = hashCode * -1521134295 + localPosition.GetHashCode();
			hashCode = hashCode * -1521134295 + localRotation.GetHashCode();
			hashCode = hashCode * -1521134295 + localScale.GetHashCode();
			return hashCode;
		}

		public bool Equals(Transform other)
		{
			return this.instanceID == other.GetInstanceID();
		}

		public override string ToString()
		{
			return $@"{{{name}, 
            instanceID: {instanceID}, localPosition: 
            {localPosition.ToString()}, 
            localRotation: {localRotation.ToString()}, 
            localScale: {localScale.ToString()}";
		}

		public static bool operator <(GameObjectData left, GameObjectData right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(GameObjectData left, GameObjectData right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(GameObjectData left, GameObjectData right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(GameObjectData left, GameObjectData right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static bool operator <(GameObjectData left, Transform right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(GameObjectData left, Transform right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(GameObjectData left, Transform right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(GameObjectData left, Transform right)
		{
			return left.CompareTo(right) >= 0;
		}

		/// <summary>
		/// Copies the values from ObjectData to a Transform
		/// </summary>
		/// <param name="from">The data the is being copies</param>
		/// <param name="to">The data that is being overwritten</param>
		/// <returns>A reference to the overwritten data</returns>
		public static UnityEngine.Transform Copy(GameObjectData from, UnityEngine.Transform to)
		{
			to.name = from.name;
			to.localPosition = Convert.Copy(from.localPosition, to.localPosition);
			to.localRotation = Convert.Copy(from.localRotation, to.localRotation);
			to.localScale = Convert.Copy(from.localScale, to.localScale);
			return to;
		}

		/// <summary>
		/// Copies the values from Transform to ObjectData
		/// </summary>
		/// <param name="from">The data the is being copies</param>
		/// <param name="to">The data that is being overwritten</param>
		/// <returns>A reference to the overwritten data</returns>
		public static GameObjectData Copy(UnityEngine.Transform from, GameObjectData to)
		{
			to.name = from.name;
			to.instanceID = from.GetInstanceID();
			to.localPosition = Convert.Copy(from.localPosition, to.localPosition);
			to.localRotation = Convert.Copy(from.localRotation, to.localRotation);
			to.localScale = Convert.Copy(from.localScale, to.localScale);
			return to;
		}
		
		/// <summary>
		/// Creates a new ObjectData instance with the data from a transform
		/// </summary>
		/// <param name="from">The data copied to the new ObjectData</param>
		/// <returns>The new ObjectData instance</returns>
		public static GameObjectData New(UnityEngine.Transform from)
		{
			GameObjectData newObjectData = new GameObjectData();
			Copy(from, newObjectData);
			return newObjectData;
		}
	}

}
