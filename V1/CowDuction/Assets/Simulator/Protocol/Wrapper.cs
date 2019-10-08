using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
abstract public class Wrapper : MonoBehaviour {
	public string profile = "JimBob";
	public List<MotionModifier> modifiers = new List<MotionModifier>();
	protected Client simClient; // Recreated on load, it isn't saved with the xml
	public bool kill = false; // Recreated on load
	public float rollPosModifier = 0.0f;
	public float pitchPosModifier = 0.0f;
	public bool useTransformRotation = true;

	protected void Start () {
		simClient = new Client(4001, "10.10.101.90", 4001);
		if(simClient == null)
			print("Client failed");
		
		StartCoroutine(checkTimer());
	}
	
	public void Connect () {
		simClient = new Client(4001, "10.10.101.90", 4001);
		if(simClient == null)
			print("Client failed");
		
		StartCoroutine(checkTimer());
	}
	
	protected void AssignRotation(float pitch, float roll) {
		float newPitch = pitchPosModifier;
		float newRoll = rollPosModifier;
		if(useTransformRotation) {
			newPitch += pitch;
			newRoll += roll;
		}
		simClient.packet.pitchPos = -Mathf.Clamp(newPitch, -20, 20);
		simClient.packet.rollPos = Mathf.Clamp(newRoll, -30, 30);
	}

	protected void checkKill() {
		if(Input.GetButtonDown("Kill")) {
			kill = true;
		}		
	}
	
	protected IEnumerator checkTimer() {
		while(true) {
			if(kill) {
				Debug.Log("KILLING!");
				GetComponent<AudioSource>().Play();
				break;
			}
			yield return new WaitForSeconds(0.033f);
			simClient.sendData();
		}
	}
}
