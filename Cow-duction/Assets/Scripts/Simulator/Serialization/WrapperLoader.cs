using UnityEngine;
using System.Collections;

public class WrapperLoader : MonoBehaviour {
	private SimulatorSerialization serializer;
	private TransformWrapper wrapper;
	// Use this for initialization
	void Start () {
		getActiveProfile();
		loadActiveProfile();
		activateWrapper();
	}
	
	void getActiveProfile() {
		serializer = GameObject.Find ("_MotionSerializer").GetComponent<SerializationClient>().getSerializer();
		if(serializer == null)
			serializer = new SimulatorSerialization();
	}
	
	void loadActiveProfile() {
		wrapper = this.gameObject.AddComponent<TransformWrapper>();
		serializer.setWrapper(ref wrapper);
	}
	
	void activateWrapper() {
		//wrapper.Connect();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
