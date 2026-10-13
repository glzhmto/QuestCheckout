using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// moves between the kiosk screens and remembers what the user picked
public class KioskUI : MonoBehaviour
{
    public KitManager kits;

    [Header("Screens")]
    public GameObject startScreen;
    public GameObject kitScreen;
    public GameObject durationScreen;
    public GameObject reviewScreen;
    public GameObject confirmScreen;
    public GameObject doneScreen;
    public GameObject alternativesScreen;

    [Header("Start")]
    public Button startButton;

    [Header("Choose kit")]
    public Button kitBackButton;
    public Text kitSubtitle;
    public Button[] kitButtons;
    public Text[] kitButtonTexts;
    public Text kitMessage;
    public GameObject noKitsBox;
    public Button seeAlternativesButton;

    [Header("Duration and contents")]
    public Button durationBackButton;
    public Text contentsTitle;
    public Button[] itemButtons;
    public Text[] itemTexts;
    public Button[] durationButtons;
    public Text[] durationTexts;
    public Text durationMessage;
    public Button nextButton;

    [Header("Review")]
    public Button reviewBackButton;
    public Text reviewKit;
    public Text reviewDuration;
    public Text reviewDeadline;
    public Button editKitButton;
    public Button editDurationButton;
    public Button reviewNextButton;

    [Header("Confirm")]
    public Button confirmBackButton;
    public Text confirmSummary;
    public Button lockButton;
    public Text confirmMessage;

    [Header("Done")]
    public Text doneLocker;
    public Text doneDeadline;
    public Button exitButton;

    [Header("Alternatives")]
    public Button altBackButton;
    public Text waitTimesText;
    public Button notifyButton;
    public Text notifyMessage;
    public Button altStartButton;

    [Header("Testing")]
	public GameObject testBar;
    public Button resetAButton;
    public Button resetBButton;

    string[] items = { "1x Headset - Meta Quest 3", "2x Motion Controllers - Left & Right", "1x USB-C Charger - Included cable" };
    int[] durations = { 90, 120, 180 };

    // only 90 min can be borrowed, the others are blocked by the scheduled class at 12
    bool[] durationAllowed = { true, false, false };

    // what the user picked, -1 means nothing yet
    int selectedKit = -1;
    int selectedDuration = -1;
    bool[] itemChecked = new bool[3];

    void Start()
    {
		testBar = GameObject.Find("TestBar"); 

        startButton.onClick.AddListener(GoToKits);

        kitBackButton.onClick.AddListener(GoToStart);
        seeAlternativesButton.onClick.AddListener(GoToAlternatives);
        for (int i = 0; i < kitButtons.Length; i++)
        {
            int kit = i; // copy for the listener
            kitButtons[i].onClick.AddListener(() => PickKit(kit));
        }

        durationBackButton.onClick.AddListener(GoToKits);
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int item = i;
            itemButtons[i].onClick.AddListener(() => CheckItem(item));
        }
        for (int i = 0; i < durationButtons.Length; i++)
        {
            int d = i;
            durationButtons[i].onClick.AddListener(() => PickDuration(d));
        }
        nextButton.onClick.AddListener(GoToReview);

        reviewBackButton.onClick.AddListener(GoToDuration);
        editKitButton.onClick.AddListener(GoToKits);
        editDurationButton.onClick.AddListener(GoToDuration);
        reviewNextButton.onClick.AddListener(GoToConfirm);

        confirmBackButton.onClick.AddListener(GoToReview);
        lockButton.onClick.AddListener(LockLoan);

        exitButton.onClick.AddListener(GoToStart);

        altBackButton.onClick.AddListener(GoToKits);
        notifyButton.onClick.AddListener(NotifyMe);
        altStartButton.onClick.AddListener(GoToStart);

        resetAButton.onClick.AddListener(ResetA);
        resetBButton.onClick.AddListener(ResetB);

        GoToStart();
    }

    void ShowScreen(GameObject screen)
    {
        startScreen.SetActive(screen == startScreen);
        kitScreen.SetActive(screen == kitScreen);
        durationScreen.SetActive(screen == durationScreen);
        reviewScreen.SetActive(screen == reviewScreen);
        confirmScreen.SetActive(screen == confirmScreen);
        doneScreen.SetActive(screen == doneScreen);
        alternativesScreen.SetActive(screen == alternativesScreen);

		if (testBar != null)
        {
            testBar.SetActive(screen == startScreen);
        }
    }

    // ---------- start ----------

    void GoToStart()
    {
        // new person, forget the old choices
        selectedKit = -1;
        selectedDuration = -1;
        itemChecked = new bool[3];
        ShowScreen(startScreen);
    }

    // ---------- choose kit ----------

    void GoToKits()
    {
        kitMessage.text = "";
        kits.FreeFinishedKits();
        int available = kits.CountAvailable();

        for (int i = 0; i < kitButtons.Length; i++)
        {
            string state = "AVAILABLE";
            if (kits.status[i] == KitManager.Charging) state = "CHARGING";
            if (kits.status[i] == KitManager.InUse) state = "IN USE";

            string mark = i == selectedKit ? "  (selected)" : "";
            kitButtonTexts[i].text = "#" + (i + 1).ToString("00") + "\n" + kits.GetKitName(i) + mark + "\n" + state;

            // greyed out when it can't be picked
            kitButtonTexts[i].color = kits.IsAvailable(i) ? Color.black : new Color(0.6f, 0.6f, 0.6f);
        }

        if (available == 0)
        {
            kitSubtitle.text = "0 OF 6 KITS AVAILABLE";
            noKitsBox.SetActive(true);
        }
        else
        {
            kitSubtitle.text = available + " OF 6 KITS AVAILABLE - TAP TO SELECT";
            noKitsBox.SetActive(false);
        }

        ShowScreen(kitScreen);
    }

    void PickKit(int kit)
    {
        // invalid kit, explain why instead of doing nothing
        if (!kits.IsAvailable(kit))
        {
            string why = kits.status[kit] == KitManager.Charging ? "is charging" : "is in use";
            kitMessage.text = kits.GetKitName(kit) + " " + why + " until " + FormatTime(kits.freeAt[kit]) + ". Please pick an available kit.";
            return;
        }

        // new kit means the contents have to be checked again, the duration is kept
        if (kit != selectedKit) itemChecked = new bool[3];
        selectedKit = kit;
        GoToDuration();
    }

    // ---------- duration and contents ----------

    void GoToDuration()
    {
        if (selectedKit == -1)
        {
            GoToKits();
            return;
        }

        contentsTitle.text = "ITEMS INCLUDED IN " + kits.GetKitName(selectedKit).ToUpper() + " - TAP EACH ONE TO CHECK IT";
        durationMessage.text = "120 & 180 MIN UNAVAILABLE - ALL KITS RESERVED FROM 12:00 PM FOR SCHEDULED CLASS";
        RefreshDuration();
        ShowScreen(durationScreen);
    }

    void CheckItem(int item)
    {
        itemChecked[item] = !itemChecked[item];
        RefreshDuration();
    }

    void PickDuration(int d)
    {
        if (!durationAllowed[d])
        {
            durationMessage.text = durations[d] + " MIN is not available, the kits are reserved at 12:00 PM. Please pick 90 MIN.";
            return;
        }
        selectedDuration = durations[d];
        RefreshDuration();
    }

    void RefreshDuration()
    {
        for (int i = 0; i < itemTexts.Length; i++)
        {
            itemTexts[i].text = (itemChecked[i] ? "[X]  " : "[  ]  ") + items[i];
        }

        for (int i = 0; i < durationTexts.Length; i++)
        {
            string text = durations[i] + "\nMIN";
            if (durationAllowed[i])
            {
                text += "\nby " + FormatTime(DateTime.Now.AddMinutes(durations[i]));
                durationTexts[i].color = Color.black;
            }
            else
            {
                text += "\nReserved 12:00 PM";
                durationTexts[i].color = new Color(0.6f, 0.6f, 0.6f);
            }
            if (durations[i] == selectedDuration) text = "> " + text;
            durationTexts[i].text = text;
        }

        // can only continue with all items checked and a duration picked
        nextButton.interactable = AllItemsChecked() && selectedDuration != -1;
    }

    bool AllItemsChecked()
    {
        for (int i = 0; i < itemChecked.Length; i++)
        {
            if (!itemChecked[i]) return false;
        }
        return true;
    }

    // ---------- review ----------

    void GoToReview()
    {
        if (selectedKit == -1 || selectedDuration == -1 || !AllItemsChecked())
        {
            GoToDuration();
            return;
        }

        reviewKit.text = kits.GetKitName(selectedKit);
        reviewDuration.text = selectedDuration + " Minutes";
        reviewDeadline.text = "By " + FormatTime(DateTime.Now.AddMinutes(selectedDuration));
        ShowScreen(reviewScreen);
    }

    // ---------- confirm ----------

    void GoToConfirm()
    {
        confirmMessage.text = "";
        confirmSummary.text = kits.GetKitName(selectedKit) + " - " + selectedDuration + " Minutes - Return by " + FormatTime(DateTime.Now.AddMinutes(selectedDuration));
        ShowScreen(confirmScreen);
    }

    void LockLoan()
    {
        // check again in case the kit was taken while the user was deciding
        if (!kits.Borrow(selectedKit, selectedDuration))
        {
            confirmMessage.text = kits.GetKitName(selectedKit) + " is not available anymore. Please choose another kit.";
            selectedKit = -1;
            return;
        }

        doneLocker.text = kits.GetLocker(selectedKit);
        doneDeadline.text = "Return by " + FormatTime(kits.freeAt[selectedKit]);
        ShowScreen(doneScreen);
    }

    // ---------- no kits / alternatives ----------

    void GoToAlternatives()
    {
        kits.FreeFinishedKits();
        int next = kits.NextFreeKit();
        if (next == -1)
        {
            waitTimesText.text = "No wait time information right now.";
        }
        else
        {
            string why = kits.status[next] == KitManager.Charging ? "finishes charging" : "is returned";
            waitTimesText.text = "The next kit will be ready in " + kits.MinutesUntilFree(next) + " min.\n"
                + kits.GetKitName(next) + " " + why + " at " + FormatTime(kits.freeAt[next]) + ".";
        }

        RefreshNotify();
        ShowScreen(alternativesScreen);
    }

    void NotifyMe()
    {
        PlayerPrefs.SetInt("notify", 1);
        PlayerPrefs.Save();
        RefreshNotify();
    }

    void RefreshNotify()
    {
        if (PlayerPrefs.GetInt("notify", 0) == 1)
        {
            int next = kits.NextFreeKit();
            string when = next == -1 ? "soon" : "at " + FormatTime(kits.freeAt[next]);
            notifyMessage.text = "You are on the list. We will send a message to your student email when a kit is ready (" + when + ").";
            notifyButton.interactable = false;
        }
        else
        {
            notifyMessage.text = "";
            notifyButton.interactable = true;
        }
    }

    // ---------- testing ----------

    void ResetA()
    {
        kits.ResetScenarioA();
        GoToStart();
    }

    void ResetB()
    {
        kits.ResetScenarioB();
        GoToStart();
    }

    // always "11:30 AM", no matter the language of the computer
    string FormatTime(DateTime time)
    {
        return time.ToString("h:mm tt", CultureInfo.InvariantCulture);
    }
}
