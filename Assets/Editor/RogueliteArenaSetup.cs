#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Genera escena Arena_MVP, prefabs mínimos y referencias. Menú: Roguelite → Generar escena Arena (MVP).
/// </summary>
public static class RogueliteArenaSetup
{
    const string ScenePath = "Assets/Scenes/Arena_MVP.unity";
    const string PrefabDir = "Assets/_Generated/Prefabs";

    [MenuItem("Roguelite/Generar escena Arena (MVP)")]
    public static void Generate()
    {
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/_Generated");
        EnsureFolder(PrefabDir);

        EnsureEnemyLayer();

        Sprite sprPlayer = LoadOrCreateCircleSprite("spr_Player", new Color(0.35f, 0.85f, 0.45f));
        Sprite sprEnemyChase = LoadOrCreateCircleSprite("spr_EnemyChase", new Color(0.95f, 0.35f, 0.35f));
        Sprite sprEnemyShoot = LoadOrCreateCircleSprite("spr_EnemyShoot", new Color(0.45f, 0.55f, 0.95f));
        Sprite sprBullet = LoadOrCreateCircleSprite("spr_Bullet", Color.white, 16);

        GameObject projectileContent = BuildProjectileContent(sprBullet);
        GameObject projectileAsset = SavePrefab(projectileContent, $"{PrefabDir}/Projectile_MVP.prefab");
        Object.DestroyImmediate(projectileContent);

        GameObject chaserContent = BuildChaserContent(sprEnemyChase);
        GameObject chaserPrefab = SavePrefab(chaserContent, $"{PrefabDir}/Enemy_Chaser.prefab");
        Object.DestroyImmediate(chaserContent);

        GameObject shooterContent = BuildShooterContent(sprEnemyShoot);
        GameObject shooterPrefab = SavePrefab(shooterContent, $"{PrefabDir}/Enemy_Shooter.prefab");
        Object.DestroyImmediate(shooterContent);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateMainCamera();
        CreateDirectionalLight();
        CreateArenaBootstrap();

        GameObject systems = new GameObject("Systems");
        var waveManager = systems.AddComponent<WaveManager>();
        systems.AddComponent<RewardManager>();
        systems.AddComponent<UpgradeManager>();

        GameObject player = BuildPlayerInScene(sprPlayer, projectileAsset);

        int enemyMask = LayerMask.GetMask("Enemy");
        WireWaveManager(waveManager, chaserPrefab, shooterPrefab, systems.GetComponent<RewardManager>());

        var weapon = player.transform.Find("Weapon").GetComponent<WeaponController>();
        SerializedObject soWeapon = new SerializedObject(weapon);
        soWeapon.FindProperty("projectilePrefab").objectReferenceValue = projectileAsset;
        soWeapon.FindProperty("firePoint").objectReferenceValue = weapon.transform;
        soWeapon.FindProperty("enemyLayer").intValue = enemyMask;
        soWeapon.ApplyModifiedPropertiesWithoutUndo();

        Projectile projComp = projectileAsset.GetComponent<Projectile>();
        if (projComp != null)
        {
            SerializedObject soProjectile = new SerializedObject(projComp);
            soProjectile.FindProperty("enemyMask").intValue = enemyMask;
            soProjectile.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SavePrefabAsset(projectileAsset);
        }

        CreateHudCanvas(waveManager);
        CreateRewardCanvas();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);

        SetStartSceneInBuildSettings(ScenePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Roguelite MVP",
            "Listo.\n\nEscena: " + ScenePath + "\n\nEn Unity abre esa escena (doble clic) y pulsa Play.",
            "OK");
    }

    static void EnsureEnemyLayer()
    {
        if (LayerMask.NameToLayer("Enemy") >= 0)
            return;

        Object[] objs = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (objs == null || objs.Length == 0)
            return;

        SerializedObject tagManager = new SerializedObject(objs[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null)
            return;

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty sp = layers.GetArrayElementAtIndex(i);
            if (sp != null && string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = "Enemy";
                tagManager.ApplyModifiedProperties();
                Debug.Log("Capa 'Enemy' creada en el slot " + i + ".");
                return;
            }
        }

        Debug.LogWarning("No hay slot libre en Layers para 'Enemy'. Añádela a mano en Project Settings → Tags and Layers.");
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(parent) || parent == "Assets")
        {
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder("Assets", name);
            return;
        }

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    static Sprite LoadOrCreateCircleSprite(string baseName, Color c, int size = 32)
    {
        EnsureFolder("Assets/_Generated");
        string path = $"Assets/_Generated/{baseName}.png";
        string diskPath = Path.Combine(Application.dataPath, "_Generated", baseName + ".png");

        if (!File.Exists(diskPath))
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size - 1, size - 1) * 0.5f;
            float r = size * 0.45f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, d < r ? c : Color.clear);
            }

            tex.Apply();
            File.WriteAllBytes(diskPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = size;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        var spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (spr != null)
            return spr;

        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (o is Sprite s)
                return s;
        }

        return null;
    }

    static GameObject BuildProjectileContent(Sprite s)
    {
        var go = new GameObject("ProjectileContent");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = 5;
        go.transform.localScale = Vector3.one * 0.35f;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.45f;

        go.AddComponent<Projectile>();
        return go;
    }

    static GameObject BuildChaserContent(Sprite s)
    {
        var go = new GameObject("EnemyChaserContent");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = 2;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.45f;

        go.AddComponent<ChaserEnemy>();
        int layer = LayerMask.NameToLayer("Enemy");
        if (layer >= 0)
            go.layer = layer;
        return go;
    }

    static GameObject BuildShooterContent(Sprite s)
    {
        var go = new GameObject("EnemyShooterContent");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = 2;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.45f;

        go.AddComponent<ShooterEnemy>();
        int layer = LayerMask.NameToLayer("Enemy");
        if (layer >= 0)
            go.layer = layer;
        return go;
    }

    static GameObject SavePrefab(GameObject content, string prefabPath)
    {
        return PrefabUtility.SaveAsPrefabAsset(content, prefabPath);
    }

    static void CreateMainCamera()
    {
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.11f);
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;
        camGo.AddComponent<AudioListener>();
    }

    static void CreateDirectionalLight()
    {
        var go = new GameObject("Directional Light");
        var l = go.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1f;
        go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    static void CreateArenaBootstrap()
    {
        var go = new GameObject("ArenaBootstrap");
        go.AddComponent<ArenaBootstrap>();
    }

    static GameObject BuildPlayerInScene(Sprite spr, GameObject projectilePrefabAsset)
    {
        var player = new GameObject("Player");
        player.transform.position = Vector3.zero;

        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.sortingOrder = 3;

        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = player.AddComponent<CircleCollider2D>();
        col.radius = 0.45f;

        player.AddComponent<PlayerController>();

        var weaponGo = new GameObject("Weapon");
        weaponGo.transform.SetParent(player.transform, false);
        weaponGo.transform.localPosition = Vector3.zero;
        weaponGo.AddComponent<WeaponController>();

        return player;
    }

    static void WireWaveManager(WaveManager waveManager, GameObject chaser, GameObject shooter, RewardManager rm)
    {
        SerializedObject so = new SerializedObject(waveManager);
        var arr = so.FindProperty("enemyPrefabs");
        arr.arraySize = 2;
        arr.GetArrayElementAtIndex(0).objectReferenceValue = chaser;
        arr.GetArrayElementAtIndex(1).objectReferenceValue = shooter;
        so.FindProperty("rewardManager").objectReferenceValue = rm;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static Canvas CreateHudCanvas(WaveManager waveManager)
    {
        var canvasGo = new GameObject("Canvas_HUD");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = GetUiFont();

        Text hp = CreateUiText(canvasGo.transform, "HPText", "HP: —", font, 18, TextAnchor.UpperLeft,
            new Vector2(0, 1), new Vector2(320, 40), new Vector2(20, -20));
        Text dash = CreateUiText(canvasGo.transform, "DashText", "Dash: —", font, 18, TextAnchor.UpperLeft,
            new Vector2(0, 1), new Vector2(320, 40), new Vector2(20, -50));
        Text wave = CreateUiText(canvasGo.transform, "WaveText", "Oleada: —", font, 18, TextAnchor.UpperLeft,
            new Vector2(0, 1), new Vector2(320, 40), new Vector2(20, -80));

        var hud = canvasGo.AddComponent<GameHUD>();
        SerializedObject soHud = new SerializedObject(hud);
        soHud.FindProperty("hpText").objectReferenceValue = hp;
        soHud.FindProperty("dashText").objectReferenceValue = dash;
        soHud.FindProperty("waveText").objectReferenceValue = wave;
        soHud.FindProperty("waveManager").objectReferenceValue = waveManager;
        soHud.ApplyModifiedPropertiesWithoutUndo();

        return canvas;
    }

    static void CreateRewardCanvas()
    {
        var canvasGo = new GameObject("Canvas_Reward");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = GetUiFont();

        var panel = new GameObject("RewardPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var panelRt = panel.AddComponent<RectTransform>();
        StretchFull(panelRt);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);

        Text title = CreateUiText(panel.transform, "TitleText", "Oleada completada — elige una mejora", font, 22,
            TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(800, 60), new Vector2(0, -40));

        var nameTexts = new Text[3];
        var descTexts = new Text[3];
        var buttons = new Button[3];

        float startY = -140f;
        float step = 150f;

        for (int i = 0; i < 3; i++)
        {
            float y = startY - step * i;
            var row = new GameObject($"Row_{i}");
            row.transform.SetParent(panel.transform, false);
            var rowRt = row.AddComponent<RectTransform>();
            rowRt.anchorMin = new Vector2(0.5f, 1f);
            rowRt.anchorMax = new Vector2(0.5f, 1f);
            rowRt.pivot = new Vector2(0.5f, 1f);
            rowRt.sizeDelta = new Vector2(700, 130);
            rowRt.anchoredPosition = new Vector2(0f, y);

            nameTexts[i] = CreateUiText(row.transform, $"Name_{i}", "—", font, 20, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(520, 32), new Vector2(20, -10));
            descTexts[i] = CreateUiText(row.transform, $"Desc_{i}", "", font, 16, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(520, 70), new Vector2(20, -42));

            var btnGo = new GameObject($"ChooseButton_{i}");
            btnGo.transform.SetParent(row.transform, false);
            var btnRt = btnGo.AddComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(1f, 0.5f);
            btnRt.anchorMax = new Vector2(1f, 0.5f);
            btnRt.pivot = new Vector2(1f, 0.5f);
            btnRt.sizeDelta = new Vector2(140, 48);
            btnRt.anchoredPosition = new Vector2(-20f, 0f);

            var btnImage = btnGo.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.55f, 0.95f, 1f);
            buttons[i] = btnGo.AddComponent<Button>();
            var colors = buttons[i].colors;
            colors.highlightedColor = new Color(0.35f, 0.68f, 1f);
            colors.pressedColor = new Color(0.15f, 0.4f, 0.85f);
            buttons[i].colors = colors;

            var btnLabel = CreateUiText(btnGo.transform, "Label", "Elegir", font, 16, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(120, 40), Vector2.zero);
            btnLabel.alignment = TextAnchor.MiddleCenter;
        }

        var rewardUi = canvasGo.AddComponent<RewardUI>();
        SerializedObject soR = new SerializedObject(rewardUi);
        soR.FindProperty("panelRoot").objectReferenceValue = panel;
        soR.FindProperty("titleText").objectReferenceValue = title;
        soR.FindProperty("nameTexts").arraySize = 3;
        soR.FindProperty("descTexts").arraySize = 3;
        soR.FindProperty("buttons").arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            soR.FindProperty("nameTexts").GetArrayElementAtIndex(i).objectReferenceValue = nameTexts[i];
            soR.FindProperty("descTexts").GetArrayElementAtIndex(i).objectReferenceValue = descTexts[i];
            soR.FindProperty("buttons").GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        }

        soR.ApplyModifiedPropertiesWithoutUndo();
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static Text CreateUiText(Transform parent, string name, string text, Font font, int size, TextAnchor anchor,
        Vector2 anchorMin, Vector2 sizeDelta, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMin;
        rt.pivot = anchorMin;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;

        var t = go.AddComponent<Text>();
        t.font = font;
        t.fontSize = size;
        t.color = Color.white;
        t.text = text;
        t.alignment = anchor;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    static Font GetUiFont()
    {
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f != null)
            return f;
        return Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Liberation Sans", "Helvetica" }, 16);
    }

    static void SetStartSceneInBuildSettings(string scenePath)
    {
        var scen = new EditorBuildSettingsScene(scenePath, true);
        if (EditorBuildSettings.scenes != null && EditorBuildSettings.scenes.Length > 0)
        {
            bool found = false;
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.path == scenePath)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                list.Insert(0, scen);
                EditorBuildSettings.scenes = list.ToArray();
            }
        }
        else
        {
            EditorBuildSettings.scenes = new[] { scen };
        }
    }
}
#endif
