using UnityEngine;
using UnityEditor;
using System.Collections;


[ExecuteInEditMode]
public class ParticleToolbox : MonoBehaviour {



	public float pDuration = 5f;
	public PropertyFields.TwoFloatField pStartLifetime;
	public PropertyFields.TwoFloatField pStartSize;
	public PropertyFields.TwoFloatField pStartRotation;
	//public PropertyFields.ColorField pStartColor;
	public Color pStartColor = Color.white;
	public Gradient pColorOverLifetime = new Gradient();
	public float pStartSpeed = 15f;

	public float pStartLifetime1 = 3f;
	public float pStartLifetime2 = 3f;
	public float pStartSize1 = 1f;
	public float pStartSize2 = 1f;
	public float pStartRotation1 = 0f;
	public float pStartRotation2 = 0f;

	public int particlesPerSecond = 100;
	public int maxParticles = 1000;

	public Vector3 birthOffset = new Vector3(0,0,0);
	public bool looping = true;
	public bool prewarm = false;
	public float startDelay = 0f;

	public float gravityModifier = 0.98f;

	public AnimationCurve velocityLifetimeX = AnimationCurve.Linear(0,0,1,0);
	public AnimationCurve velocityLifetimeY = AnimationCurve.Linear(0,0,1,0);
	public AnimationCurve velocityLifetimeZ = AnimationCurve.Linear(0,0,1,0);


	private ParticleSystem particleSystem;
	private ParticleSystem.Particle[] particles; // = new ParticleSystem.Particle[this.particleSystem.particleCount];




	[MenuItem("GameObject/Custom ParticleSystem", false, -100)]
	static void CreateParticleSystem(MenuCommand menuCommand){

		GameObject particleSystem = new GameObject ("CustomParticleSystem");
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(particleSystem, menuCommand.context as GameObject);
		particleSystem.AddComponent<ParticleToolbox> ();
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(particleSystem, "Create " + particleSystem.name);
		Selection.activeObject = particleSystem;
	}



	public void OnEnable() {
		if (particleSystem == null)
			particleSystem = GetComponent<ParticleSystem>();
			if (particleSystem == null) {
				particleSystem = gameObject.AddComponent<ParticleSystem>() as ParticleSystem;
			}
		particleSystem.hideFlags = HideFlags.HideAndDontSave;

		if (particles == null || particles.Length < particleSystem.maxParticles)
			particles = new ParticleSystem.Particle[particleSystem.maxParticles]; 

	}


	public void Start(){

		var psEmission = particleSystem.emission;
		var psEmmitterShape = particleSystem.shape;
		var psVelocityOverLifetime = particleSystem.velocityOverLifetime;
		var psColorOverLifetime = particleSystem.colorOverLifetime;
		var psSizeOverLifetime = particleSystem.sizeOverLifetime;

		psEmission.enabled = true;
		psEmmitterShape.enabled = true;
		psVelocityOverLifetime.enabled = true;
		psColorOverLifetime.enabled = true;
		psSizeOverLifetime.enabled = true;

		Quaternion rotation = Quaternion.Euler (-90f, 0f, 0f);
		particleSystem.transform.rotation = rotation;

		pColorOverLifetime.SetKeys( new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) }, 
			new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.2f), 
				new GradientAlphaKey(1.0f, 0.75f), new GradientAlphaKey(0.0f, 1.0f) } );
		psColorOverLifetime.color = pColorOverLifetime;

		psEmmitterShape.shapeType = ParticleSystemShapeType.Cone;
		psEmmitterShape.radius = 0.01f;
		psEmmitterShape.angle = 8f;
		psEmmitterShape.length = 0f;
	
	}


	public void LateUpdate (){

		particleSystem.GetParticles (particles);

		UpdateParticleSystem ();
		//PositionParticles ();

		particleSystem.SetParticles (particles, particles.Length);
	}


	void OnDestroy(){
		print (particleSystem + " destroyed");
	}



	public void UpdateParticleSystem(){

		var psEmission = particleSystem.emission;
		//var psEmmitterShape = system.shape;
		//var psVelocityOverLifetime = system.velocityOverLifetime;
		var psColorOverLifetime = particleSystem.colorOverLifetime;
		//var psSizeOverLifetime = system.sizeOverLifetime;

		//system.duration = pDuration;
		particleSystem.startLifetime = pStartLifetime1;
		particleSystem.startSize = pStartSize1;
		particleSystem.startColor = pStartColor;
		particleSystem.startSpeed = pStartSpeed;
		particleSystem.maxParticles = maxParticles;
		particleSystem.gravityModifier = gravityModifier;



		psEmission.rate = particlesPerSecond;
		psColorOverLifetime.color = pColorOverLifetime;

	}



	public float RandomBetweenTwoConstants(float value1, float value2){

		float newValue = value1 * ( (Time.time - Time.deltaTime) / value2);
			return newValue;
	}



	private void PositionParticles () {

		//Vector3 position = new Vector3 (0, 0, 0);
		for (int i = 0; i < particles.Length; i++) {
			//Debug.Log("Birthtime::  " + particles[i].startLifetime);
			Vector3 position = new Vector3 (0, 0, 0);
			position += birthOffset * i;
			particles [i].position += position;

			//Debug.Log ("");


		}
	}





	/*
	[System.Serializable]
	public struct TwoFloatField{
		public float value1;
		public float value2;
	}

	[System.Serializable]
	public struct ColorField{
		public Color color;
	}

	[System.Serializable]
	public struct TwoColorField{
		public Color color1;
		public Color color2;
	}
	*/	
}



namespace OptionsTypes
{
	public enum EmitterType
	{
		Point,
		Sphere,
		Hemisphere,
		Cone,
		Box,
		Circle,
		Edge
	}


}

namespace PropertyFields
{
	[System.Serializable]
	public class TwoFloatField{
		public float value1;
		public float value2;
	}

	[System.Serializable]
	public class ColorField{
		public Color color;
	}

	[System.Serializable]
	public class TwoColorField{
		public Color color1;
		public Color color2;
	}
}