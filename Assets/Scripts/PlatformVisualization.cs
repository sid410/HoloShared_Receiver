using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformVisualization : MonoBehaviour
{
    public extOSC.Examples.PlatformManager m_platformManager;
    public GameObject platformPlaneGO;
    
    void Update()
    {
        UpdatePlatformVisualization();
    }

    private void UpdatePlatformVisualization()
    {
        if (m_platformManager == null || platformPlaneGO == null)
        {
            return;
        }

        Dictionary<(int, int), int> platformData = m_platformManager.GetPlatformData();
        Texture2D texture = new Texture2D(120, 50);
        platformPlaneGO.GetComponent<Renderer>().material.mainTexture = texture;

        for (var x = 0; x < 120; x++)
        {
            for (var y = 0; y < 50; y++)
            {
                texture.SetPixel(x, y, new Color(platformData[(x, y)], 0, 0));
            }
        }

        texture.Apply();
    }
}
