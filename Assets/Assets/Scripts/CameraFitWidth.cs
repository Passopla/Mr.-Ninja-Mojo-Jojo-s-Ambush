using UnityEngine;

// Keeps the full level width visible on any aspect ratio (phones crop the sides otherwise).
// Auto-attaches to the main camera at startup, so nothing to wire up in the scene.
public class CameraFitWidth : MonoBehaviour
{
    public float designWidth = 16f; // world units visible horizontally at 960x600 with ortho size 5
    float designSize;
    Camera cam;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Attach()
    {
        if (Camera.main != null && !Camera.main.GetComponent<CameraFitWidth>())
            Camera.main.gameObject.AddComponent<CameraFitWidth>();
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        designSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        // Never zoom in past the original size; only zoom out when the screen is narrower.
        cam.orthographicSize = Mathf.Max(designSize, designWidth * 0.5f / cam.aspect);
    }
}
