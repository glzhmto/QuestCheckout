using System;
using UnityEngine;

// keeps the 6 kits and saves them with PlayerPrefs so they stay after closing the app
public class KitManager : MonoBehaviour
{
    public const int KitCount = 6;

    // 0 = available, 1 = charging, 2 = in use
    public const int Available = 0;
    public const int Charging = 1;
    public const int InUse = 2;

    public int[] status = new int[KitCount];

    // when the kit is free again (return deadline or charging done)
    public DateTime[] freeAt = new DateTime[KitCount];

    void Awake()
    {
        // first time opening the app there is nothing saved yet
        if (!PlayerPrefs.HasKey("saved"))
        {
            ResetScenarioA();
        }
        else
        {
            Load();
        }
    }

    public bool IsAvailable(int kit)
    {
        if (kit < 0 || kit >= KitCount) return false;
        return status[kit] == Available;
    }

    public int CountAvailable()
    {
        int count = 0;
        for (int i = 0; i < KitCount; i++)
        {
            if (status[i] == Available) count++;
        }
        return count;
    }

    // returns false if the kit is not available anymore (so two loans can't happen on the same kit)
    public bool Borrow(int kit, int minutes)
    {
        if (!IsAvailable(kit)) return false;

        status[kit] = InUse;
        freeAt[kit] = DateTime.Now.AddMinutes(minutes);
        Save();
        return true;
    }

    // each kit has its own locker, kit 01 -> locker 01 etc
    public string GetLocker(int kit)
    {
        return "Locker #" + (kit + 1).ToString("00");
    }

    public string GetKitName(int kit)
    {
        return "Kit " + (kit + 1).ToString("00");
    }

    // minutes until the kit is free, 0 if it is already late
    public int MinutesUntilFree(int kit)
    {
        double minutes = (freeAt[kit] - DateTime.Now).TotalMinutes;
        if (minutes < 0) return 0;
        return (int)Math.Ceiling(minutes);
    }

    // kit that will be free first, -1 if there is none
    public int NextFreeKit()
    {
        int best = -1;
        for (int i = 0; i < KitCount; i++)
        {
            if (status[i] == Available) continue;
            if (best == -1 || freeAt[i] < freeAt[best]) best = i;
        }
        return best;
    }

    // same as the figma: 02, 03 and 06 available, 01 charging, 04 and 05 in use
    public void ResetScenarioA()
    {
        status = new int[] { Charging, Available, Available, InUse, InUse, Available };
        freeAt = new DateTime[KitCount];
        freeAt[0] = DateTime.Now.AddMinutes(16);
        freeAt[3] = DateTime.Now.AddMinutes(31);
        freeAt[4] = DateTime.Now.AddMinutes(45);
        PlayerPrefs.DeleteKey("notify");
        Save();
        Debug.Log("Reset to scenario A");
    }

    // no kits available, all charging or in use
    public void ResetScenarioB()
    {
        status = new int[] { Charging, Charging, InUse, InUse, InUse, InUse };
        freeAt = new DateTime[KitCount];
        freeAt[0] = DateTime.Now.AddMinutes(40);
        freeAt[1] = DateTime.Now.AddMinutes(16);
        freeAt[2] = DateTime.Now.AddMinutes(31);
        freeAt[3] = DateTime.Now.AddMinutes(55);
        freeAt[4] = DateTime.Now.AddMinutes(70);
        freeAt[5] = DateTime.Now.AddMinutes(85);
        PlayerPrefs.DeleteKey("notify");
        Save();
        Debug.Log("Reset to scenario B");
    }

    public void Save()
    {
        for (int i = 0; i < KitCount; i++)
        {
            PlayerPrefs.SetInt("kit" + i + "_status", status[i]);
            // DateTime can't be saved directly so we save the ticks as text
            PlayerPrefs.SetString("kit" + i + "_free", freeAt[i].Ticks.ToString());
        }
        PlayerPrefs.SetInt("saved", 1);
        PlayerPrefs.Save();
    }

    void Load()
    {
        for (int i = 0; i < KitCount; i++)
        {
            status[i] = PlayerPrefs.GetInt("kit" + i + "_status", Available);
            long ticks = long.Parse(PlayerPrefs.GetString("kit" + i + "_free", "0"));
            freeAt[i] = new DateTime(ticks);
        }
    }
}
