using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Motion Wrapper")]
abstract public class MotionModifier{
	public MotionModifier(){}
	
	void Start() {}
	void Update() {}
	// Modify function to be applied to a packet created by a wrapper
	abstract public Packet modify(Packet thePacket, Vector3 destination);
}
