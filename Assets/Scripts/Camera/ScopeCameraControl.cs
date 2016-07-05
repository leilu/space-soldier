using UnityEngine;

public class ScopeCameraControl : MonoBehaviour {

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

	void Update () {
        cam.orthographicSize = Camera.main.orthographicSize;
        transform.position = Camera.main.transform.position;	    
	}
}
