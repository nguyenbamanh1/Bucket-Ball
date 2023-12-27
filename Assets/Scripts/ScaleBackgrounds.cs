using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBackgrounds : MonoBehaviour
{
    [SerializeField] Camera cam;


    private void Start()
    {
        Renderer sr = GetComponent<Renderer>();
        if (sr == null) return;

        if (cam == null)
            cam = Camera.main;

        // Line up sprite with camera
        transform.position = new Vector3(cam.transform.position.x, transform.position.y, transform.position.z);

        // Get viewport sizes
        float worldScreenHeight = cam.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        // Scale sprite
        var spriteSize = sr.bounds.size;
        Vector3 scale = Vector3.one;
        scale.x = worldScreenWidth / spriteSize.x;
        scale.y = worldScreenHeight / spriteSize.y;

        //sr.material.Set("_tiling") = new Vector2(scale.x, 0);
        sr.sharedMaterial.mainTextureScale = new Vector2(scale.x, 1);
        scale.x *= spriteSize.x;
        scale.y *= spriteSize.y;

        transform.localScale = scale;
    }
}
