using UnityEngine;

public class Camera2 : MonoBehaviour
{
    [SerializeField] public Transform player_transform;
    [SerializeField] public float lerp_t = 100.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (player_transform == null)
        {
            return;
        }

        Vector3 target_pos = player_transform.position;
        target_pos.z -= 5;

        transform.position = Vector3.Lerp(transform.position, target_pos, lerp_t * Time.deltaTime);
    }
};
