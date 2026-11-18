using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// One-off pass: faint glow on the axe head and edge plus a small soft light at the head. Renders, then deletes itself.
[InitializeOnLoad]
public static class TempAxeGlow
{
    const string Shots = @"C:\Users\mechm\AppData\Local\Temp\claude\4127cfadc070c00b\600fb7d5-76a4-4721-99a5-705452281777\scratchpad\";
    static bool ran;

    static TempAxeGlow()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            EditorApplication.playModeStateChanged += s => { if (s == PlayModeStateChange.EnteredEditMode) EditorApplication.delayCall += Run; };
        else EditorApplication.delayCall += Run;
    }

    static void Glow(string path, Color c)
    {
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        m.EnableKeyword("_EMISSION");
        m.SetColor("_EmissionColor", c);
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(m);
    }

    static void Run()
    {
        if (ran) return;
        ran = true;
        try
        {
            var scene = SceneManager.GetSceneByPath("Assets/Scenes/WalkingSim.unity");
            if (!scene.isLoaded) scene = EditorSceneManager.OpenScene("Assets/Scenes/WalkingSim.unity", OpenSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            // the axe's own materials only (the handle shares the table's wood, so it's left alone)
            Glow("Assets/Materials/AxeIron.mat", new Color(0.07f, 0.075f, 0.09f));
            Glow("Assets/Materials/AxeEdge.mat", new Color(0.22f, 0.23f, 0.26f));
            AssetDatabase.SaveAssets();

            var axe = Object.FindFirstObjectByType<AxePickup>();
            var old = axe.transform.Find("Glow");
            if (old != null) Object.DestroyImmediate(old.gameObject);
            var go = new GameObject("Glow");
            go.transform.SetParent(axe.transform, false);
            go.transform.localPosition = new Vector3(0f, 0.6f, 0.12f);
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.8f, 0.85f, 1f);
            l.range = 1.2f;
            l.intensity = 0.6f;
            l.shadows = LightShadows.None;
            axe.glow = l;
            EditorUtility.SetDirty(axe);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            var camGo = new GameObject("Probe Cam") { hideFlags = HideFlags.HideAndDontSave };
            var cam = camGo.AddComponent<Camera>();
            cam.CopyFrom(axe.playerCamera);
            cam.targetTexture = new RenderTexture(1056, 600, 24);
            // inside lighting like in game
            Color amb = RenderSettings.ambientLight; float refl = RenderSettings.reflectionIntensity, fog = RenderSettings.fogDensity;
            var il = Object.FindFirstObjectByType<IndoorLighting>();
            if (il != null) { RenderSettings.ambientLight = amb * il.indoorAmbient; RenderSettings.reflectionIntensity = refl * il.indoorReflections; RenderSettings.fogDensity = fog * il.indoorFog; }
            cam.transform.position = new Vector3(0.6f, 1.7f, 2.2f);
            cam.transform.LookAt(new Vector3(1.5f, 1.0f, 4.7f));
            Shot(cam, "axe_glow_near.png");
            cam.transform.position = new Vector3(-1.5f, 1.7f, 0.5f);
            cam.transform.LookAt(new Vector3(1.5f, 1.0f, 4.7f));
            Shot(cam, "axe_glow_far.png");
            RenderSettings.ambientLight = amb; RenderSettings.reflectionIntensity = refl; RenderSettings.fogDensity = fog;
            Object.DestroyImmediate(camGo);
            Debug.Log("[AxeGlow] done");
            AssetDatabase.DeleteAsset("Assets/TextureTools");
        }
        catch (System.Exception e) { Debug.LogError("[AxeGlow] failed: " + e); }
    }

    static void Shot(Camera c, string file)
    {
        c.Render();
        RenderTexture.active = c.targetTexture;
        var t = new Texture2D(1056, 600, TextureFormat.RGB24, false);
        t.ReadPixels(new Rect(0, 0, 1056, 600), 0, 0);
        RenderTexture.active = null;
        File.WriteAllBytes(Shots + file, t.EncodeToPNG());
        Object.DestroyImmediate(t);
    }
}
