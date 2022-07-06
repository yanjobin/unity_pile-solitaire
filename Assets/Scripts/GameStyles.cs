using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStyles { Background, Card }

public class GameStyles : MonoBehaviour
{
    private static GameStyles INSTANCE;

    private static string BACKGROUND_STYLES_PATH = "Backgrounds\\";
    private static string CARDS_STYLES_PATH = "Cards\\";

    public static Sprite CURRENT_BACKGROUND;
#if UNITY_ANDROID
    private static string currentCardAtlasName = GameStyles.GetPath(EStyles.Card) + "cards2";
#else
    private static string currentCardAtlasName = GameStyles.GetPath(EStyles.Card) + "cards1";
#endif

    public static string CURRENT_CARD_ATLAS_NAME
    {
        get
        {
            return GameStyles.GetPath(EStyles.Card) + "cards2";
        }
        set
        {
            if (value != currentCardAtlasName)
            {
                currentCardAtlasName = value;
                INSTANCE.CardManager.GetComponent<CardManager>().updateAllCardSprite();
            }
        }
    }

    public GameObject CardManager;

    void Awake()
    {
        INSTANCE = this;

        Sprite[] sprites = Resources.LoadAll<Sprite>(GameStyles.GetPath(EStyles.Background));
        foreach (Sprite s in sprites)
        {
            CURRENT_BACKGROUND = s;
            break;
        }

        string[] files = System.IO.Directory.GetFiles("Assets\\Resources\\" + GameStyles.GetPath(EStyles.Card));
        foreach (string s in files)
        {
            currentCardAtlasName = GameStyles.GetPath(EStyles.Card) + System.IO.Path.GetFileNameWithoutExtension(s);
            break;
        }
    }

    public static string GetPath(EStyles style)
    {
        switch (style)
        {
            case EStyles.Background: return BACKGROUND_STYLES_PATH;
            case EStyles.Card: return CARDS_STYLES_PATH;
            default: return "";
        }
    }
}
