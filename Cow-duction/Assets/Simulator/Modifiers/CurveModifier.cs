using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class CurveModifier : MotionModifier {
	[XmlAttribute("AnimCurve")]
	public AnimationCurve theCurve = new AnimationCurve();
	[XmlAttribute("smoothVal")]
	public float smoothValue = 1.0f;
	
	// Takes a packet and modifies its values to according to the animation curve
	// Curve created in inspector
	override public Packet modify(Packet thePacket, Vector3 destination) {
		thePacket.xLinVel = thePacket.xLinVel + (destination.x - thePacket.xLinVel) * theCurve.Evaluate(Time.deltaTime);
		thePacket.yLinVel = thePacket.yLinVel + (destination.y - thePacket.yLinVel) * theCurve.Evaluate(Time.deltaTime);
		thePacket.zLinVel = thePacket.zLinVel + (destination.z - thePacket.zLinVel) * theCurve.Evaluate(Time.deltaTime);
		return thePacket;
	}
}
