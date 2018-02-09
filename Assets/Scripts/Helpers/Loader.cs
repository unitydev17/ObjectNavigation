using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Loader
{
	private const string CUBE_PATH = "Prefabs/Cube";
	private const string SPHERE_PATH = "Prefabs/Sphere";
	private const string CAPSULE_PATH = "Prefabs/Capsule";



	public static void Load(out List<GameObject> prefabs)
	{
		prefabs = new List<GameObject>();
		prefabs.Add(Resources.Load(CUBE_PATH) as GameObject);
		prefabs.Add(Resources.Load(SPHERE_PATH) as GameObject);
		prefabs.Add(Resources.Load(CAPSULE_PATH) as GameObject);
	}

}
