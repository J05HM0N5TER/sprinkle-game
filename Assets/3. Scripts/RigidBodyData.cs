using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace GameSerialization
{
    public class RigidBodyData : IXmlSerializable, IComparable<RigidBodyData>, IComparable<Rigidbody>, IEquatable<Rigidbody>
    {
        public int instanceID;
        public int parentInstanceID;
        public string parentName;
        public System.Numerics.Vector3 velocity;
        public System.Numerics.Vector3 angularVelocity;
        public int CompareTo(RigidBodyData other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Rigidbody other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Rigidbody other)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
