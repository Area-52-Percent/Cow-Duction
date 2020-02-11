using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;


public class TransformWrapper : Wrapper {
	
	public float rollAmpFactor = 2.0f;
	private Vector3 targetVelocity;
	private Vector3 lastPosition;
	private float targetPitch;
	private float targetRoll;
	//private float velocityAdjustWeight = 0;
	
	public float smoothSpeed = 1.0f;
	
	new void Start(){
		base.Start();
		
		lastPosition = this.transform.position;
	}
	
	void Update () {
		//There is no need to update in editor
		if(!Application.isPlaying)
			return;
		
		if(Time.timeScale != 0 && Time.deltaTime != 0)
			targetVelocity = (transform.position - lastPosition) / Time.deltaTime;		
		
		lastPosition = transform.position;  //print (targetVelocity.ToString());
	
		Packet p = simClient.packet;

		// Bypasses the modifiers
		p.xLinVel = targetVelocity.x;
		p.yLinVel = targetVelocity.y;
		p.zLinVel = targetVelocity.z;

		// This should be for smoothing motion, does not seem to be working...
		// TODO: Fix/implement correctly
//		foreach(MotionModifier mod in modifiers) {
//			p = mod.modify(p, targetVelocity);
//		}
		
		Vector3 rotation = transform.rotation.eulerAngles;
		float pitch = rotation.x > 180F ? rotation.x - 360F : rotation.x;
		float roll = rotation.z > 180F ? rotation.z - 360F : rotation.z;
		AssignRotation(pitch, roll);
	}
}
