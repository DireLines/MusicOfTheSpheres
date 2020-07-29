using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game {
    public static bool IsOnOSX = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);
    public static bool IsOnWindows = (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);
    public static bool IsOnLinux = (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer);

    //C# mod is not too useful. This one acts identically to the python one (and the math one)
    public static int correctmod(int a, int n) {
        return ((a % n) + n) % n;
    }
}
