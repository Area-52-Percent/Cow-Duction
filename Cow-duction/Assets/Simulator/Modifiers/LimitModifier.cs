using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

class LimitModifier : MotionModifier {
	[XmlAttribute("lowX")]
	public float lowerX = 0f; 
	[XmlAttribute("highX")]
	public float higherX = 0f;
	[XmlAttribute("lowY")]
	public float lowerY = 0f; 
	[XmlAttribute("highY")]
	public float higherY = 0f;
	[XmlAttribute("lowZ")]
	public float lowerZ = 0f; 
	[XmlAttribute("highZ")]
	public float higherZ = 0f;
	
	
	// Checks if speed is above or below limits (speed = end - start)
	// If speed is negative the conditionals and low/high limits get flipped
	float modVal(float start, float end, float lowA, float highA) {
		float returnVal = 0f;
		if(end - start < 0) { // negative velocity
			returnVal = end - start < highA ? -highA : end;
			returnVal = end - start > lowA ? -lowA : end;
		} else {
			returnVal = end - start > highA ? highA : end;
			returnVal = end - start < lowA ? lowA : end;
		}
		
		return returnVal;
	}
	
	override public Packet modify(Packet thePacket, Vector3 destination) {
		thePacket.xLinVel = modVal ((float)thePacket.xLinVel,(float)destination.x,lowerX,higherX);
		thePacket.yLinVel = modVal ((float)thePacket.yLinVel,(float)destination.y,lowerY,higherY);
		thePacket.zLinVel = modVal ((float)thePacket.zLinVel,(float)destination.z,lowerZ,higherZ);
		
		return thePacket;
	}
}
