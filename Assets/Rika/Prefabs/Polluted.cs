using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData.MapElement;

public class Polluted : MonoBehaviour {
    public MeshRenderer[] Glows;
    public MeshRenderer Bottom;
    static public Color[] colors = { Color.black, Color.red, Color.blue };


    IEnumerator changeMaterial(Color tarC, float fadeDelay = .5f) {
        var delay = 0f;
        var initC = Glows[0].material.GetColor("_Color");

        var g = new Material(Glows[0].material);
        var b = new Material(Bottom.material);

        foreach (var glow in Glows)
            glow.material = g;
        Bottom.material = b;
        while (delay < fadeDelay) {
            delay += Time.fixedDeltaTime;
            var nowC = Color.Lerp(initC, tarC, delay / fadeDelay);
            g.SetColor("_Color", nowC);
            b.SetColor("_Color", new Color(nowC.r, nowC.g, nowC.b, .33f));
            yield return new WaitForEndOfFrame();
        }

    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
