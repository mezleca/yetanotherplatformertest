using UnityEngine;

public class CameraController : MonoBehaviour
{
    private struct CameraData
    {
        public GameObject target;
        public Vector3 offset;
        public float duration, fov;
    }

    public new Camera camera;
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
            return;
        }

        float current_fov = get_fov();
        Vector3 current = transform.position, target = data.target.transform.position, offset = data.offset;
        
        if (current != target)
        {
            transform.position = new Vector3(
                Mathf.Lerp(current.x, target.x + offset.x, transition_speed * Time.deltaTime),
                Mathf.Lerp(current.y, target.y + offset.y, transition_speed * Time.deltaTime),
                Mathf.Lerp(current.z, target.z + offset.z - 5, transition_speed * Time.deltaTime)
            );
        }

        if (data.fov != current_fov)
        {
            set_fov(Mathf.Lerp(current_fov, data.fov, transition_speed * Time.deltaTime));
        }
    }

    private void set_fov(float value) => camera.fieldOfView = value;

    private float get_fov() => camera.fieldOfView;

    public void set_focus(GameObject target, float fov = 120.0f, Vector3 offset = new(), float duration = 0.0f)
    {
        data.target = target;
        data.offset = offset;
        data.duration = duration;
        data.fov = fov;
    }
};
