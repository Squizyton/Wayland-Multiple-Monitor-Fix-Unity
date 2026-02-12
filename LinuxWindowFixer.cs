using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
// ReSharper disable once CheckNamespace
public static class LinuxWindowFixer
{
    // Threshold to catch windows near the top-left corner (4, 28 etc.)
    private const float OriginThreshold = 50f;
    private static HashSet<int> handledWindowIds = new HashSet<int>();

    private const bool MoveToCursor = false;
    private const bool MoveToCenterWindowWithRandomOffset = true;
    private const bool LOGOutActiveWindows = false;
    private const bool LogOutWhatThisSaves = false;
    static LinuxWindowFixer()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        List<string> ignoreList = new List<string>()
        {
            "Scene",
            "Hierarchy",
            "Console",
            "UnityEditor.MainToolbarWindow",
            "Game"
        };


        List<string> containsKeywordList = new List<string>()
        {
            "UnityEngine",
            "UnityEditor"
        };
        
        EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

        foreach (var window in allWindows)
        {
            
            
            if (LOGOutActiveWindows)
#pragma warning disable CS0162 // Unreachable code detected
            {
                Debug.Log($"Active Window: {window.titleContent.text}");            
            }
#pragma warning restore CS0162 // Unreachable code detected

            
            //ANYTHING UNITY RELATED (Built by Unity, should be ignored for the most part)
            if (ignoreList.Contains(window.titleContent.text) 
                || window.titleContent.text.Contains("UnityEngine") 
                || window.titleContent.text.Contains("UnityEditor"))
                continue;
            
            int id = window.GetInstanceID();
            Rect pos = window.position;

            // 1. Check if window is 'stuck' near the origin (XWayland bug)
            // 2. Ensure we haven't already moved THIS specific window instance this session
            if (pos.x < OriginThreshold && pos.y < OriginThreshold && !handledWindowIds.Contains(id))
            {
                if (MoveToCursor)
#pragma warning disable CS0162 // Unreachable code detected
                {
                    MoveWindowToCursor(window);
                }
#pragma warning restore CS0162 // Unreachable code detected
                else
                {
                    MoveToCenterWindow(window, MoveToCenterWindowWithRandomOffset);
                }

                handledWindowIds.Add(id);
            }
        }
    }
    
    private static void MoveToCenterWindow(EditorWindow window, bool randomizeOffset)
    {
        Vector2 centerPosition = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(i => i.titleContent.text == "UnityEditor.MainToolbarWindow")!.position.center;

        window.position = randomizeOffset ? 
            new Rect(centerPosition.x + Random.Range(-100, 100), centerPosition.y + Random.Range(-100, 100), window.position.width, window.position.height) 
            : new Rect(centerPosition.x, centerPosition.y, window.position.width, window.position.height);
        window.Repaint();

    }

    private static void MoveWindowToCursor(EditorWindow window)
    {
        // Get the absolute screen position of the mouse across all monitors
        Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current != null ? 
                           Event.current.mousePosition : Vector2.zero);

        // If Event.current is null (common in Update), fallback to a safer method
        if (mousePos == Vector2.zero)
        {
            // Fallback: Just push it significantly away from the corner
            window.position = new Rect(500, 300, window.position.width, window.position.height);
        }
        else
        {
            // Center the window on the cursor
            float newX = mousePos.x - (window.position.width / 2);
            float newY = mousePos.y - (window.position.height / 2);
            window.position = new Rect(newX, newY, window.position.width, window.position.height);
        }

        window.Repaint();
        
        if(LogOutWhatThisSaves)
            //Turning this to true will make this reachable
#pragma warning disable CS0162 // Unreachable code detected
            Debug.Log($"[WaylandFix] Rescued {window.titleContent.text} from origin.");
#pragma warning restore CS0162 // Unreachable code detected
    }
}
