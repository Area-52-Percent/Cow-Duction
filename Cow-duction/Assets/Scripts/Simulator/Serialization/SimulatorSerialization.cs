using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Wrapper Profile")]
public class SimulatorSerialization {
	[XmlAttribute("profile")]
	public string profile;
	[XmlArray("Motion Modifiers"),XmlArrayItem("Modiier")]
	public List<MotionModifier> modifiers = new List<MotionModifier>();
	[XmlAttribute("rollModifier")]
	public float rollPosModifier;
	[XmlAttribute("pitchposModifier")]
	public float pitchPosModifier;
	[XmlAttribute("useTransformRotation")]
	public bool useTransformRotation = true;
	[XmlAttribute("rollAmpFactor")]
	public float rollAmpFactor;
	
	public SimulatorSerialization() {}
	
	public SimulatorSerialization(TransformWrapper wrapper) {
		LoadWrapper(wrapper);	
	}
	
	public void LoadWrapper(TransformWrapper wrapper) {
		profile = wrapper.profile;
		modifiers = wrapper.modifiers;
		rollPosModifier = wrapper.rollPosModifier;
		pitchPosModifier = wrapper.pitchPosModifier;
		useTransformRotation = wrapper.useTransformRotation;
		rollAmpFactor = wrapper.rollAmpFactor;
	}
	
	public void Save() {
		string path = Application.dataPath + "./Profiles/" + profile;
		XmlSerializer serializer = new XmlSerializer(typeof(SimulatorSerialization));
 		using(FileStream stream = new FileStream(path, FileMode.Create))
 		{
 			serializer.Serialize(stream, this);
 		}
	}
	
	
	public void setWrapper(ref TransformWrapper wrapper) {
		wrapper.rollAmpFactor = rollAmpFactor;
		wrapper.profile = profile;
		wrapper.modifiers = modifiers;
		wrapper.pitchPosModifier = pitchPosModifier;
		wrapper.rollPosModifier = rollPosModifier;
		wrapper.useTransformRotation = useTransformRotation;
	}
	
	public SimulatorSerialization Load(string simProfile) {
		string path = Application.dataPath + "./Profiles/" + simProfile;
		XmlSerializer serializer = new XmlSerializer(typeof(SimulatorSerialization));
 		using(FileStream stream = new FileStream(path, FileMode.Open))
 		{
 			return serializer.Deserialize(stream) as SimulatorSerialization;
 		}
	}
}
