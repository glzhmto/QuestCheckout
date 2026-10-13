using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Builds the Kiosk scene (canvas + all the screens) with the Unity API.
// Runs automatically the first time (if the scene does not exist) and from Tools > Build Kiosk Scene.
// Positions are in pixels from the top left, like in the Figma frames (1440 x 1024).
[InitializeOnLoad]
public static class KioskSceneBuilder
{
    const string ScenePath = "Assets/Scenes/Kiosk.unity";

    static Color background = new Color(0.93f, 0.93f, 0.91f);
    static Color box = new Color(0.88f, 0.88f, 0.86f);
    static Color dark = new Color(0.1f, 0.1f, 0.1f);
    static Color grey = new Color(0.45f, 0.45f, 0.45f);

    static Font font;

    static KioskSceneBuilder()
    {
        EditorApplication.delayCall += () =>
        {
            if (!System.IO.File.Exists(ScenePath) && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Build();
            }
        };
    }

    [MenuItem("Tools/Build Kiosk Scene")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");

        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // new scene already has a camera and a directional light
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = background;

        // canvas that scales like the figma frame
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1440, 1024);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // the project uses the new input system so the event system needs this module
        GameObject eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<InputSystemUIInputModule>();

        GameObject managerObj = new GameObject("KioskManager");
        KitManager kits = managerObj.AddComponent<KitManager>();
        KioskUI ui = managerObj.AddComponent<KioskUI>();
        ui.kits = kits;

        Transform root = canvasObj.transform;
        Text t;

        // ---------- start ----------
        GameObject start = MakeScreen("StartScreen", root);
        ui.startScreen = start;
        MakeText(start.transform, "MQ3", 420, 260, 600, 160, 140, true, TextAnchor.MiddleCenter, dark);
        MakeText(start.transform, "META QUEST 3 HEADSET LENDING", 320, 430, 800, 50, 30, false, TextAnchor.MiddleCenter, dark);
        MakeText(start.transform, "Your student ID has been verified.", 320, 480, 800, 40, 22, false, TextAnchor.MiddleCenter, grey);
        ui.startButton = MakeButton(start.transform, "TAP TO START", 470, 600, 500, 140, dark, Color.white, 44, out t);

        // ---------- choose kit ----------
        GameObject kitScreen = MakeScreen("KitScreen", root);
        ui.kitScreen = kitScreen;
        ui.kitBackButton = MakeHeader(kitScreen.transform, "STEP 1/4 - SELECT A KIT");
        MakeText(kitScreen.transform, "Choose a Headset Kit", 230, 140, 980, 70, 56, true, TextAnchor.MiddleLeft, dark);
        ui.kitSubtitle = MakeText(kitScreen.transform, "", 230, 215, 980, 40, 24, false, TextAnchor.MiddleLeft, dark);

        ui.kitButtons = new Button[6];
        ui.kitButtonTexts = new Text[6];
        for (int i = 0; i < 6; i++)
        {
            int col = i % 3;
            int row = i / 3;
            ui.kitButtons[i] = MakeButton(kitScreen.transform, "Kit " + (i + 1), 230 + col * 330, 280 + row * 240, 310, 220, box, dark, 28, out t);
            ui.kitButtonTexts[i] = t;
        }

        MakeText(kitScreen.transform, "KITS MARKED CHARGING OR IN USE CANNOT BE SELECTED", 230, 765, 980, 30, 18, false, TextAnchor.MiddleLeft, grey);
        ui.kitMessage = MakeText(kitScreen.transform, "", 230, 795, 980, 40, 22, true, TextAnchor.MiddleLeft, new Color(0.7f, 0.1f, 0.1f));

        GameObject noKits = MakeBox(kitScreen.transform, "NoKitsBox", 230, 840, 980, 160, dark);
        ui.noKitsBox = noKits;
        MakeText(noKits.transform, "No kits are available right now", 30, 20, 600, 50, 34, true, TextAnchor.MiddleLeft, Color.white);
        MakeText(noKits.transform, "ALL 6 KITS ARE CHARGING OR IN USE", 30, 80, 600, 40, 20, false, TextAnchor.MiddleLeft, Color.white);
        ui.seeAlternativesButton = MakeButton(noKits.transform, "SEE WAIT TIMES & ALTERNATIVES ->", 620, 30, 340, 100, Color.white, dark, 22, out t);

        // ---------- duration and contents ----------
        GameObject durationScreen = MakeScreen("DurationScreen", root);
        ui.durationScreen = durationScreen;
        ui.durationBackButton = MakeHeader(durationScreen.transform, "STEP 2/4 - DURATION & CONTENT");

        MakeText(durationScreen.transform, "1  Kit Contents", 60, 140, 620, 70, 48, true, TextAnchor.MiddleLeft, dark);
        ui.contentsTitle = MakeText(durationScreen.transform, "", 60, 210, 620, 40, 18, false, TextAnchor.MiddleLeft, dark);
        ui.itemButtons = new Button[3];
        ui.itemTexts = new Text[3];
        for (int i = 0; i < 3; i++)
        {
            ui.itemButtons[i] = MakeButton(durationScreen.transform, "Item " + (i + 1), 60, 260 + i * 120, 620, 100, box, dark, 26, out t);
            t.alignment = TextAnchor.MiddleLeft;
            t.rectTransform.offsetMin = new Vector2(25, 0);
            ui.itemTexts[i] = t;
        }

        MakeText(durationScreen.transform, "2  Loan Duration", 760, 140, 620, 70, 48, true, TextAnchor.MiddleLeft, dark);
        MakeText(durationScreen.transform, "PICK HOW LONG YOU NEED IT", 760, 210, 620, 40, 18, false, TextAnchor.MiddleLeft, dark);
        ui.durationButtons = new Button[3];
        ui.durationTexts = new Text[3];
        for (int i = 0; i < 3; i++)
        {
            ui.durationButtons[i] = MakeButton(durationScreen.transform, "Duration " + (i + 1), 760 + i * 210, 260, 190, 200, box, dark, 30, out t);
            ui.durationTexts[i] = t;
        }
        ui.durationMessage = MakeText(durationScreen.transform, "", 760, 480, 620, 100, 20, false, TextAnchor.UpperLeft, dark);

        ui.nextButton = MakeButton(durationScreen.transform, "DURATION & KIT CONFIRMED - NEXT ->", 24, 880, 1392, 110, dark, Color.white, 32, out t);

        // ---------- review ----------
        GameObject review = MakeScreen("ReviewScreen", root);
        ui.reviewScreen = review;
        ui.reviewBackButton = MakeHeader(review.transform, "STEP 3/4 - REVIEW ORDER");
        MakeText(review.transform, "Review Your Order", 230, 140, 980, 70, 56, true, TextAnchor.MiddleLeft, dark);
        MakeText(review.transform, "CONFIRM BEFORE CHECKING OUT", 230, 210, 980, 40, 22, false, TextAnchor.MiddleLeft, dark);

        MakeText(review.transform, "SELECTED KIT", 230, 290, 350, 60, 22, false, TextAnchor.MiddleLeft, grey);
        ui.reviewKit = MakeText(review.transform, "", 580, 290, 400, 60, 34, true, TextAnchor.MiddleLeft, dark);
        ui.editKitButton = MakeButton(review.transform, "EDIT KIT", 1000, 290, 210, 60, Color.white, dark, 22, out t);

        MakeText(review.transform, "DURATION", 230, 390, 350, 60, 22, false, TextAnchor.MiddleLeft, grey);
        ui.reviewDuration = MakeText(review.transform, "", 580, 390, 400, 60, 34, true, TextAnchor.MiddleLeft, dark);
        ui.editDurationButton = MakeButton(review.transform, "EDIT DURATION", 1000, 390, 210, 60, Color.white, dark, 22, out t);

        MakeText(review.transform, "RETURN DEADLINE", 230, 490, 350, 60, 22, false, TextAnchor.MiddleLeft, grey);
        ui.reviewDeadline = MakeText(review.transform, "", 580, 490, 400, 60, 34, true, TextAnchor.MiddleLeft, dark);

        MakeText(review.transform, "CONTENTS", 230, 590, 350, 60, 22, false, TextAnchor.MiddleLeft, grey);
        MakeText(review.transform, "1x Headset, 2x Controllers, 1x Charger", 580, 590, 630, 60, 26, false, TextAnchor.MiddleLeft, dark);

        MakeText(review.transform, "! LATE RETURNS MAY RESULT IN LENDING PRIVILEGES BEING SUSPENDED", 230, 700, 980, 40, 20, false, TextAnchor.MiddleLeft, dark);
        ui.reviewNextButton = MakeButton(review.transform, "CONFIRM ORDER ->", 24, 880, 1392, 110, dark, Color.white, 32, out t);

        // ---------- confirm ----------
        GameObject confirm = MakeScreen("ConfirmScreen", root);
        ui.confirmScreen = confirm;
        ui.confirmBackButton = MakeHeader(confirm.transform, "STEP 4/4 - FINAL CONFIRMATION");
        MakeText(confirm.transform, "Lock in Your Loan?", 220, 250, 1000, 80, 60, true, TextAnchor.MiddleCenter, dark);
        ui.confirmSummary = MakeText(confirm.transform, "", 220, 350, 1000, 50, 28, false, TextAnchor.MiddleCenter, dark);
        ui.lockButton = MakeButton(confirm.transform, "CONFIRM CHECKOUT - LOCK LOAN", 370, 480, 700, 130, dark, Color.white, 32, out t);
        ui.confirmMessage = MakeText(confirm.transform, "", 220, 660, 1000, 60, 24, true, TextAnchor.MiddleCenter, new Color(0.7f, 0.1f, 0.1f));

        // ---------- done ----------
        GameObject done = MakeScreen("DoneScreen", root);
        ui.doneScreen = done;
        MakeText(done.transform, "CHECKOUT COMPLETE", 420, 120, 600, 40, 22, false, TextAnchor.MiddleCenter, dark);
        MakeText(done.transform, "Collect Your Kit", 420, 160, 600, 80, 56, true, TextAnchor.MiddleCenter, dark);

        GameObject lockerBox = MakeBox(done.transform, "LockerBox", 470, 280, 500, 190, box);
        MakeText(lockerBox.transform, "PICKUP LOCATION", 0, 20, 500, 40, 20, false, TextAnchor.MiddleCenter, dark);
        ui.doneLocker = MakeText(lockerBox.transform, "", 0, 60, 500, 110, 72, true, TextAnchor.MiddleCenter, dark);

        GameObject deadlineBox = MakeBox(done.transform, "DeadlineBox", 470, 500, 500, 170, dark);
        MakeText(deadlineBox.transform, "HARD RETURN DEADLINE", 0, 15, 500, 40, 20, false, TextAnchor.MiddleCenter, Color.white);
        ui.doneDeadline = MakeText(deadlineBox.transform, "", 0, 55, 500, 70, 44, true, TextAnchor.MiddleCenter, Color.white);
        MakeText(deadlineBox.transform, "90-MINUTE LOAN", 0, 120, 500, 40, 20, false, TextAnchor.MiddleCenter, Color.white);

        ui.exitButton = MakeButton(done.transform, "EXIT / RETURN TO START", 470, 710, 500, 110, Color.white, dark, 28, out t);

        // ---------- alternatives ----------
        GameObject alt = MakeScreen("AlternativesScreen", root);
        ui.alternativesScreen = alt;
        ui.altBackButton = MakeHeader(alt.transform, "STEP 2/2 - ALTERNATIVES");
        MakeText(alt.transform, "No Headsets Available", 370, 140, 700, 70, 56, true, TextAnchor.MiddleLeft, dark);
        MakeText(alt.transform, "HERE IS WHEN KITS WILL BE FREE", 370, 210, 700, 40, 20, false, TextAnchor.MiddleLeft, dark);

        GameObject waitBox = MakeBox(alt.transform, "WaitBox", 370, 270, 700, 180, box);
        ui.waitTimesText = MakeText(waitBox.transform, "", 30, 20, 640, 140, 30, false, TextAnchor.MiddleLeft, dark);

        ui.notifyButton = MakeButton(alt.transform, "NOTIFY ME WHEN A KIT IS READY", 370, 490, 700, 110, dark, Color.white, 30, out t);
        ui.notifyMessage = MakeText(alt.transform, "", 370, 620, 700, 120, 24, false, TextAnchor.UpperLeft, dark);
        ui.altStartButton = MakeButton(alt.transform, "<- GO BACK TO START", 24, 880, 1392, 110, Color.white, dark, 32, out t);

        // ---------- reset buttons for testing, always on top ----------
        GameObject testBar = new GameObject("TestBar", typeof(RectTransform));
        testBar.transform.SetParent(root, false);
        Place(testBar.GetComponent<RectTransform>(), 1000, 30, 420, 60);
        MakeText(testBar.transform, "TEST:", 0, 0, 80, 60, 18, false, TextAnchor.MiddleLeft, grey);
        ui.resetAButton = MakeButton(testBar.transform, "RESET A", 80, 5, 160, 50, Color.white, dark, 18, out t);
        ui.resetBButton = MakeButton(testBar.transform, "RESET B", 255, 5, 160, 50, Color.white, dark, 18, out t);

        EditorUtility.SetDirty(ui);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[] { new EditorBuildSettingsScene(ScenePath, true) };
        Debug.Log("Kiosk scene built");
    }

    // full screen panel with the light background
    static GameObject MakeScreen(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        obj.GetComponent<Image>().color = background;
        return obj;
    }

    // back button + step text + line, like the top of every figma frame
    static Button MakeHeader(Transform parent, string step)
    {
        Text t;
        Button back = MakeButton(parent, "<- BACK", 24, 30, 140, 60, Color.white, dark, 22, out t);
        MakeText(parent, step, 190, 30, 700, 60, 22, false, TextAnchor.MiddleLeft, dark);
        GameObject line = MakeBox(parent, "Line", 0, 115, 1440, 2, grey);
        line.GetComponent<Outline>().enabled = false;
        return back;
    }

    static GameObject MakeBox(Transform parent, string name, float x, float y, float w, float h, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        Place(obj.GetComponent<RectTransform>(), x, y, w, h);
        obj.GetComponent<Image>().color = color;
        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = dark;
        outline.effectDistance = new Vector2(2, 2);
        return obj;
    }

    static Text MakeText(Transform parent, string text, float x, float y, float w, float h, int size, bool bold, TextAnchor align, Color color)
    {
        GameObject obj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        Place(obj.GetComponent<RectTransform>(), x, y, w, h);
        Text t = obj.GetComponent<Text>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        t.alignment = align;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    static Button MakeButton(Transform parent, string label, float x, float y, float w, float h, Color fill, Color textColor, int size, out Text text)
    {
        GameObject obj = MakeBox(parent, label, x, y, w, h, fill);
        Button button = obj.AddComponent<Button>();
        button.targetGraphic = obj.GetComponent<Image>();

        text = MakeText(obj.transform, label, 0, 0, w, h, size, true, TextAnchor.MiddleCenter, textColor);
        // text fills the whole button
        RectTransform rt = text.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return button;
    }

    // x, y from the top left corner of the parent
    static void Place(RectTransform rt, float x, float y, float w, float h)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(w, h);
    }
}
