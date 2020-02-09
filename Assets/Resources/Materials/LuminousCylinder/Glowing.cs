using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Glowing : MonoBehaviour {

    new Camera camera;
    public Shader GlowCore;
    public Shader GaussianBlur;
    Camera tempCam;
    const int iterationNum = 2 * 2;
    public Color GlowColor = Color.white;
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        tempCam.CopyFrom(camera);
        tempCam.clearFlags = CameraClearFlags.Color;
        tempCam.backgroundColor = Color.black;
        tempCam.cullingMask = 1 << LayerMask.NameToLayer("Effect");
        var tempRTFinal = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);
        var ts = new List<RenderTexture>();
        for (int i = 0; i < iterationNum + 1; i++) {
            ts.Add(RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8));
            ts[i].Create();
        }
        //tempRT.Create();
        tempCam.targetTexture = ts[0];

        tempCam.RenderWithShader(GlowCore, "");
        var m = new Material(GaussianBlur);
        for (int i = 0; i < iterationNum; i += 2) {
            Graphics.Blit(ts[i], ts[i + 1], m, 0);
            Graphics.Blit(ts[i + 1], ts[i + 2], m, 1);
        }
        m.SetTexture("_CoreTex", ts[0]);
        tempRTFinal.Create();
        Graphics.Blit(ts.Last(), tempRTFinal, m, 2);
        m.SetTexture("_GlowTex", tempRTFinal);
        m.SetColor("_GlowColor", GlowColor);
        Graphics.Blit(source, destination, m, 3);

        foreach (var t in ts) {
            t.Release();
        }
    }

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<Camera>();
        tempCam = new GameObject().AddComponent<Camera>();
        tempCam.enabled = false;
    }

    // Update is called once per frame
    void Update() {

    }
}
