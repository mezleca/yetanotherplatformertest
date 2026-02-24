using UnityEngine;

public class CameraController : MonoBehaviour
{
    private struct CameraData
    {
        public GameObject target;
        public float duration, fov;
    }

    public Camera camera;
    private CameraData data;
    private readonly float transition_speed = 10.0f;

    void Awake()
    {
        camera = Camera.main;
    }
    
    void Update()
    {
        if (!data.target)
        {
            Debug.LogWarning("camera: no target...");
            return;
        }

        float current_fov = get_fov();
        Vector3 current = transform.position, target = data.target.transform.position;
        
        if (current != target)
        {
            transform.position = new Vector3(
                Mathf.Lerp(current.x, target.x, transition_speed * Time.deltaTime),
                Mathf.Lerp(current.y, target.y, transition_speed * Time.deltaTime),
                Mathf.Lerp(current.z, target.z - 5, transition_speed * Time.deltaTime)
            );
        }

        if (data.fov != current_fov)
        {
            set_fov(Mathf.Lerp(current_fov, data.fov, transition_speed * Time.deltaTime));
        }
    }

    private void set_fov(float value) => camera.fieldOfView = value;

    private float get_fov() => camera.fieldOfView;

    public void set_focus(GameObject target, float fov = 120.0f, float duration = 0.0f)
    {
        data.target = target;
        data.duration = duration;
        data.fov = fov;
    }
};
