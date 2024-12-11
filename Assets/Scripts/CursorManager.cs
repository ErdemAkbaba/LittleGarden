using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[Serializable]
public class CursorType
{
    public Texture2D cursorTexture;
    public string cursorName;
    public Vector2 origin;
}
public class CursorManager : MonoBehaviour
{
    public static CursorManager cursorManager;
    public List<CursorType> cursors = new List<CursorType>();
    public int index = 2;
    private Texture2D activeCursorTexture;

    void Awake()
    {
        cursorManager = this;
    }

    public void ChangeCursor()
    {
        Cursor.SetCursor(cursors[index].cursorTexture,
            cursors[index].origin,
            CursorMode.ForceSoftware);
        activeCursorTexture = cursors[index].cursorTexture;
    }
    
    public void ChangeCursortoDef()
    {
        Cursor.SetCursor(cursors[0].cursorTexture,
            new Vector2(20, 20),
            CursorMode.ForceSoftware);
        
        activeCursorTexture = cursors[0].cursorTexture;
    }

    private void Update()
    {
        if (GameManager.gameManager.currentTilePart != null)
        {
            if (cursors[index].cursorTexture != activeCursorTexture)
            {
                ChangeCursor();   
            }
            if (GameManager.gameManager.currentTilePart.isPlanted && index != 3)
            {
                ChangeCursortoDef();
            }
        }

        else if(cursors[0].cursorTexture != activeCursorTexture)
        {
            ChangeCursortoDef();
        }
    }
}
