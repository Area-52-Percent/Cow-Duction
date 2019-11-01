using UnityEngine;
using System.Collections;

public class SerializationClient : MonoBehaviour {
	public SimulatorSerialization serializer;
	private int runVal = 0;

	// Use this for initialization
	void Start () {
		GameObject other;
		if((other = GameObject.Find("_MotionSerializer")) != null) { // This may not work, can't test it quite yet.
			if(	other.GetComponent<SerializationClient>().getRunVal() > runVal ) {
				Destroy(this.gameObject);
			} 
		}
		DontDestroyOnLoad(this);
		runVal = 1;
	}
	
	public int getRunVal() {
		return runVal;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public SimulatorSerialization getSerializer() {
		return serializer;	
	}
	
	void OnGUI() {
		if(GUI.Button(new Rect(Screen.width/2,Screen.height/2,200,200),"Load Test Profile")) {
			serializer = serializer.Load(serializer.profile);
		}
		
		if(GUI.Button(new Rect(Screen.width/2 + 250,Screen.height/2,200,200),"Save Test Profile")) {
			serializer.Save();
		}
	}
}
