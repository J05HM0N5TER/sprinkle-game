using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace GameSerialization
{
    public class RigidbodyData : IXmlSerializable, IComparable<RigidbodyData>, IComparable<Rigidbody>, IEquatable<Rigidbody>
    {
        public int instanceID;
        public int gameobjectInstanceID;
        public string gameobjectName;
        public System.Numerics.Vector3 velocity;
        public System.Numerics.Vector3 angularVelocity;
        public int CompareTo(RigidbodyData other)
        {
            return instanceID.CompareTo(other.instanceID);
        }

        public int CompareTo(Rigidbody other)
        {
            return instanceID.CompareTo(other.GetInstanceID());
        }

        public bool Equals(Rigidbody other)
        {
            return instanceID == other.GetInstanceID();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        /// <summary>
		/// Used for serializing data
		/// </summary>
		/// <param name="reader">The XMLWriter that is in the correct position in memory</param>
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
			writer.WriteStartElement(nameof(instanceID));
			writer.WriteValue(instanceID);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(gameobjectInstanceID));
			writer.WriteValue(gameobjectInstanceID);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(gameobjectName));
			writer.WriteValue(gameobjectName);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(velocity));
			vector3xml.Serialize(writer, velocity);
			writer.WriteEndElement();
			writer.WriteStartElement(nameof(angularVelocity));
			vector3xml.Serialize(writer, angularVelocity);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Used for deserializing data
		/// </summary>
		/// <param name="reader">The XMLReader that is in the correct position in memory</param>
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
			reader.ReadStartElement(); // ObjectData
			// reader.ReadEndElement(); // Nam
			instanceID = reader.ReadElementContentAsInt();
			gameobjectInstanceID = reader.ReadElementContentAsInt();
			gameobjectName = reader.ReadElementContentAsString();
			// reader.ReadStartElement(); // InstanceID
			reader.ReadStartElement(); // Position
			velocity = (System.Numerics.Vector3) vector3xml.Deserialize(reader);
			reader.ReadEndElement(); // Position
			reader.ReadStartElement(); // Rotation
			angularVelocity = (System.Numerics.Vector3) vector3xml.Deserialize(reader);
			reader.ReadEndElement(); // Rotation
			reader.ReadEndElement(); // ObjectData
		}

        public static bool operator <(RigidbodyData left, Rigidbody right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <(RigidbodyData left, RigidbodyData right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(RigidbodyData left, Rigidbody right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >(RigidbodyData left, RigidbodyData right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(RigidbodyData left, Rigidbody right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator <=(RigidbodyData left, RigidbodyData right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(RigidbodyData left, Rigidbody right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator >=(RigidbodyData left, RigidbodyData right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
		/// Copies the values from RigidbodyData to a Rigidbody
		/// </summary>
		/// <param name="from">The data the is being copies</param>
		/// <param name="to">The data that is being overwritten</param>
		/// <returns>A reference to the overwritten data</returns>
		public static UnityEngine.Rigidbody Copy(RigidbodyData from, UnityEngine.Rigidbody to)
		{
			to.gameObject.name = from.gameobjectName;
			to.velocity = Convert.Copy(from.velocity, to.velocity);;
			to.angularVelocity = Convert.Copy(from.angularVelocity, to.angularVelocity);
			return to;
		}

		/// <summary>
		/// Copies the values from Transform to ObjectData
		/// </summary>
		/// <param name="from">The data the is being copies</param>
		/// <param name="to">The data that is being overwritten</param>
		/// <returns>A reference to the overwritten data</returns>
		public static RigidbodyData Copy(UnityEngine.Rigidbody from, RigidbodyData to)
		{
			to.instanceID = from.GetInstanceID();
			to.gameobjectInstanceID = from.gameObject.GetInstanceID();
			to.gameobjectName = from.gameObject.name;
			to.instanceID = from.GetInstanceID();
			to.velocity = Convert.Copy(from.velocity, to.velocity);;
			to.angularVelocity = Convert.Copy(from.angularVelocity, to.angularVelocity);
			return to;
		}
		
		/// <summary>
		/// Creates a new ObjectData instance with the data from a transform
		/// </summary>
		/// <param name="from">The data copied to the new ObjectData</param>
		/// <returns>The new ObjectData instance</returns>
		public static RigidbodyData New(UnityEngine.Rigidbody from)
		{
			RigidbodyData newObjectData = new RigidbodyData();
			Copy(from, newObjectData);
			return newObjectData;
		}
    }

}
