using System;
using UnityEngine;

class MapClickHandler : MonoBehaviour {
    private LayerMask clickable_layers;
    private GameCore core;

    private new CameraController camera;

    void Awake()
    {
        if (camera == null) {
            camera = CameraController.Instance;
        }

        core = GameCore.Instance;

        clickable_layers = LayerMask.GetMask("CustomUI");
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) { return; }

        var mouse_world = camera._camera.ScreenToWorldPoint(Input.mousePosition);
        mouse_world.z = 0;
        var hit = Physics2D.OverlapPoint(mouse_world);

        Debug.Log($"hit: {hit}");


        if (hit == null) { return; }

        onMapRegionClicked(hit.gameObject);
    }

    private void onMapRegionClicked(GameObject region)
    {
        core.utils.drawBounds(region.GetComponent<Renderer>().bounds);
        Debug.Log($"clicked region: {region.name}");
    }
};
