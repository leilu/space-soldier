using UnityEngine;

public class CameraSync : MonoBehaviour {

    [SerializeField]
    private bool syncSize = true;
    [SerializeField]
    private bool syncPosition = true;
    [SerializeField]
    private float scalingFactor = 1;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

	void Update () {
        if (syncSize)
        {
            cam.orthographicSize = Camera.main.orthographicSize * scalingFactor;
        }

        if(syncPosition)
        {
            transform.position = Camera.main.transform.position;
        }
    }
}
