using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectController : MonoBehaviour
{

	public Material defaultMaterial;
	public Material hoverMaterial;
	public Material interceptMaterial;

	public Renderer cachedRenderer;
	private Plane plane;

	private int enteredColliders;
	private Vector3 offset;


	void Awake()
	{
		enteredColliders = 0;

		GetComponent<Collider>().enabled = false;
		cachedRenderer = GetComponent<Renderer>();
		plane = new Plane(Vector3.up, new Vector3(0, cachedRenderer.bounds.extents.y, 0));
		GameController.eventHandlers += OnEnableCollitions;
	}


	void OnEnableCollitions()
	{
		GetComponent<Collider>().enabled = true;
	}


	void Update()
	{
		GameController.ClampPosition(transform);
	}


	void OnMouseOver()
	{
		if (enteredColliders == 0) {
			cachedRenderer.material = hoverMaterial;
		}
	}


	void OnMouseExit()
	{
		if (enteredColliders == 0) {
			cachedRenderer.material = defaultMaterial;
		}
	}


	void OnMouseDown() {
		offset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
	}


	void OnMouseDrag()
	{
		var mousePosition = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(mousePosition - offset);
		float distance;


		if (plane.Raycast(ray, out distance)) {
			transform.position = ray.GetPoint(distance);
		}
	}


	void OnTriggerEnter(Collider other)
	{
		enteredColliders++;
		cachedRenderer.material = interceptMaterial;
	}


	void OnTriggerExit(Collider other)
	{
		enteredColliders--;
		if (enteredColliders == 0) {
			cachedRenderer.material = defaultMaterial;
		}
	}
}
