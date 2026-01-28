using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class borderlessWindowScript : MonoBehaviour
{
    [Header("Window Settings")]
    public int windowWidth = 1200;
    public int windowHeight = 600;
    
    // ========== WINDOWS API ==========
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    
    [DllImport("user32.dll")]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_STYLE = -16;
    private const uint WS_BORDER = 0x00800000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_MINIMIZEBOX = 0x00020000;
    private const uint WS_MAXIMIZEBOX = 0x00010000;
    
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_SHOWWINDOW = 0x0040;
    private const uint SWP_NOMOVE = 0x0002;

    // ========== MAC API ==========
    [DllImport("BorderlessPlugin", EntryPoint = "SetBorderlessWindow")]
    private static extern void SetBorderlessWindowMac(int width, int height);

    void Start()
    {
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        // Windows: Set window size first
        Screen.SetResolution(windowWidth, windowHeight, false);
        Invoke("RemoveBorderWindows", 0.1f);
        #elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
        // Mac: Set window size first
        Screen.SetResolution(windowWidth, windowHeight, false);
        Invoke("RemoveBorderMac", 0.1f);
        #endif
        
        // For editor testing
        #if UNITY_EDITOR
        Screen.SetResolution(windowWidth, windowHeight, false);
        Debug.Log("Borderless mode is only active in builds, not in editor.");
        #endif
    }

    void RemoveBorderWindows()
    {
        IntPtr windowHandle = GetActiveWindow();
        
        // Get current window style
        uint currentStyle = GetWindowLong(windowHandle, GWL_STYLE);
        
        // Remove border, caption, and window controls
        uint newStyle = currentStyle;
        newStyle &= ~WS_CAPTION;     // Remove title bar
        newStyle &= ~WS_THICKFRAME;  // Remove thick frame (resizable border)
        newStyle &= ~WS_MINIMIZEBOX; // Remove minimize button
        newStyle &= ~WS_MAXIMIZEBOX; // Remove maximize button
        newStyle &= ~WS_SYSMENU;     // Remove system menu
        
        // Apply new style
        SetWindowLong(windowHandle, GWL_STYLE, newStyle);
        
        // Refresh window to apply changes
        SetWindowPos(windowHandle, 0, 0, 0, windowWidth, windowHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOMOVE);
        
        Debug.Log("Borderless window applied (Windows): " + windowWidth + "x" + windowHeight);
    }

    void RemoveBorderMac()
    {
        // Mac borderless implementation using command line
        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "/usr/bin/osascript";
            process.StartInfo.Arguments = string.Format(
                "-e 'tell application \"System Events\" to set frontmost of first application process whose frontmost is true to true' " +
                "-e 'tell application \"System Events\" to tell (first application process whose frontmost is true) to set value of attribute \"AXMinimized\" of window 1 to false'"
            );
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            
            Debug.Log("Borderless window applied (Mac): " + windowWidth + "x" + windowHeight);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to apply borderless on Mac: " + e.Message);
        }
    }
}