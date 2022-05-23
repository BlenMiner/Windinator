using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum CSSCursors
{
    alias = 0,
    all_scroll,
    auto,
    cell,
    context_menu,
    col_resize,
    copy,
    crosshair,
    @default,
    e_resize,
    ew_resize,
    grab,
    grabbing,
    help,
    move,
    n_resize,
    ne_resize,
    nesw_resize,
    ns_resize,
    nw_resize,
    nwse_resize,
    no_drop,
    none,
    not_allowed,
    pointer,
    progress,
    row_resize,
    s_resize,
    se_resize,
    sw_resize,
    text,
    w_resize,
    wait,
    zoom_in,
    zoom_out
}

public static class Cursorinator
{
    private struct CursorTH
    {
        public Texture2D Cursor;

        public Vector2 Spot;
    }

    static string[] WebGLCursorsCached = new string[] {
        "alias",
        "all-scroll",
        "auto",
        "cell",
        "context-menu",
        "col-resize",
        "copy",
        "crosshair",
        "default",
        "e-resize",
        "ew-resize",
        "grab",
        "grabbing",
        "help",
        "move",
        "n-resize",
        "ne-resize",
        "nesw-resize",
        "ns-resize",
        "nw-resize",
        "nwse-resize",
        "no-drop",
        "none",
        "not-allowed",
        "pointer",
        "progress",
        "row-resize",
        "s-resize",
        "se-resize",
        "sw-resize",
        "text",
        "w-resize",
        "wait",
        "zoom-in",
        "zoom-out"
    };

    static Dictionary<CSSCursors, CursorTH> WindowsCachedCursors = 
        new Dictionary<CSSCursors, CursorTH>();

    [DllImport("__Internal")]
    private static extern void SetCursor(string cursor);

    static string ToFileName(CSSCursors cursor) => cursor switch
    {
        CSSCursors.pointer => "link",
        CSSCursors.text  => "ibeam",
        CSSCursors.not_allowed  => "no",
        CSSCursors.crosshair  => "cross",
        CSSCursors.move  => "move",
        CSSCursors.ns_resize  => "vertical",
        _ => null
    };

    static Vector2 ToOffset(CSSCursors cursor) => cursor switch
    {
        CSSCursors.pointer => new Vector2(10, 0),
        _ => new Vector2(16, 16)
    };

    public static void SetCursor(CSSCursors cursor)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            SetCursor(WebGLCursorsCached[(int)cursor]);
            return;
        }
        else
        {
            if (!WindowsCachedCursors.TryGetValue(cursor, out var cursorTH))
            {
                var fileName = ToFileName(cursor);
                var offset = ToOffset(cursor);

                cursorTH = new CursorTH {
                    Cursor = Resources.Load<Texture2D>(fileName),
                    Spot = offset
                };

                if (fileName == null)
                     WindowsCachedCursors.Add(cursor, default);
                else WindowsCachedCursors.Add(cursor, cursorTH);
            }

            Cursor.SetCursor(cursorTH.Cursor, cursorTH.Spot, CursorMode.Auto);
        }
    }
}
