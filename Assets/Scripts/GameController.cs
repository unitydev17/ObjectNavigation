using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public class GameController : MonoBehaviour
{

	public const int MAP_SIZE = 10;
	private const int MAX_TRIES = 100;
	private const float WHEEL_SENSIVITY = 20f;
	private const float ROTATE_SENSIVITY = 5f;
	private const float FIELD_OF_VIEW_MIN = 15f;
	private const float FIELD_OF_VIEW_MAX = 90f;

	private List<GameObject> prefabs;
	private GameObject root;


	public static event Action eventHandlers;


	private bool middleButtonDragged;


	void Start()
	{
		middleButtonDragged = false;

		Cursor.lockState = CursorLockMode.Confined;

		Loader.Load(out prefabs);
		root = new GameObject("root");

		for (int i = 0; i < 1; i++) {
			foreach (GameObject prefab in prefabs) {
				PlaceRandom(prefab);
			}
		}
		eventHandlers.Invoke();
	}


	void Update()
	{
		HandleMouse();
	}


	void HandleMouse()
	{
		if (Input.GetMouseButton(1)) {
			Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, Input.GetAxis("Mouse X") * ROTATE_SENSIVITY);
			Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, -Input.GetAxis("Mouse Y") * ROTATE_SENSIVITY);

		}

		var wheelValue = Input.GetAxis("Mouse ScrollWheel");
		if (wheelValue != 0) {
			float fov = Camera.main.fieldOfView;
			fov -= wheelValue * WHEEL_SENSIVITY;
			fov = Mathf.Clamp(fov, FIELD_OF_VIEW_MIN, FIELD_OF_VIEW_MAX);
			Camera.main.fieldOfView = fov;
		}

		if (Input.GetMouseButtonDown(2)) {
			middleButtonDragged = true;
		}

		if (Input.GetMouseButtonUp(2)) {
			middleButtonDragged = false;
		}

		if (middleButtonDragged) {
			DragMiddle();
		}

	}


	void DragMiddle()
	{
		Camera.main.transform.Translate(Camera.main.transform.right * Input.GetAxis("Mouse X"), Space.World);
		Vector3 directionY = Vector3.ProjectOnPlane(Camera.main.transform.forward, new Plane(Vector3.up, Vector3.zero).normal);

		Camera.main.transform.Translate(directionY * Input.GetAxis("Mouse Y"), Space.World);
		ClampPosition(Camera.main.transform);
	}


	public static void ClampPosition(Transform transform)
	{
		float posX = Mathf.Clamp(transform.position.x, -GameController.MAP_SIZE, GameController.MAP_SIZE);
		float posZ = Mathf.Clamp(transform.position.z, -GameController.MAP_SIZE, GameController.MAP_SIZE);
		float posY = transform.position.y;
		transform.position = new Vector3(posX, posY, posZ);
	}


	void PlaceRandom(GameObject prefab)
	{
		bool isCreated;
		int tries = 0;
		GameObject obj = Instantiate(prefab, GetObjectPosition(prefab), Quaternion.identity);
		obj.transform.parent = root.transform;

		do {
			if (IsVisibleWhole(obj) && IsNotCollides(obj)) {
				isCreated = true;
			} else {
				isCreated = false;
				obj.transform.position = GetObjectPosition(obj);
			}
			tries++;
		} while (!isCreated && tries < MAX_TRIES);

		if (tries == MAX_TRIES) {
			Destroy(obj);
		}
	}


	Vector3 GetObjectPosition(GameObject prefab)
	{
		var posX = MAP_SIZE * (2 * UnityEngine.Random.value - 1);
		var posZ = MAP_SIZE * (2 * UnityEngine.Random.value - 1);
		return new Vector3(posX, prefab.transform.position.y, posZ);
	}


	private bool IsVisible(GameObject obj)
	{
		Bounds bounds = obj.GetComponent<Renderer>().bounds;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		return GeometryUtility.TestPlanesAABB(planes, bounds);
	}


	private bool IsVisibleWhole(GameObject obj)
	{
		Vector3 pos = obj.transform.position;
		Bounds bounds = obj.GetComponent<Renderer>().bounds;

		List<Vector3> points = new List<Vector3>();
		points.Add(new Vector3(pos.x - bounds.extents.x, pos.y + bounds.extents.y, pos.z - bounds.extents.z));
		points.Add(new Vector3(pos.x - bounds.extents.x, pos.y + bounds.extents.y, pos.z + bounds.extents.z));
		points.Add(new Vector3(pos.x - bounds.extents.x, pos.y - bounds.extents.y, pos.z + bounds.extents.z));
		points.Add(new Vector3(pos.x - bounds.extents.x, pos.y - bounds.extents.y, pos.z - bounds.extents.z));

		points.Add(new Vector3(pos.x + bounds.extents.x, pos.y + bounds.extents.y, pos.z - bounds.extents.z));
		points.Add(new Vector3(pos.x + bounds.extents.x, pos.y + bounds.extents.y, pos.z + bounds.extents.z));
		points.Add(new Vector3(pos.x + bounds.extents.x, pos.y - bounds.extents.y, pos.z + bounds.extents.z));
		points.Add(new Vector3(pos.x + bounds.extents.x, pos.y - bounds.extents.y, pos.z - bounds.extents.z));

		foreach (Vector3 point in points) {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);
			if (screenPoint.z < 0 || !Camera.main.pixelRect.Contains(screenPoint)) {
				return false;
			}
		}
		return true;
	}



	private bool IsNotCollides(GameObject obj)
	{
		var extents = obj.GetComponent<Renderer>().bounds.extents;
		var colliders = Physics.OverlapBox(obj.transform.position, extents);
		return colliders.Length == 0; 
	}

}
