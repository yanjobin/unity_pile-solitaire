using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StylesScreen : MonoBehaviour
{
    public EStyles StylesFolder;
    public GameObject Container;
    public GameObject StylePrefab;
    public bool atlas = false;

    void Start()
    {
        if (this.atlas)
        {
            //Atlas sprite, Card face styles
            string[] files = System.IO.Directory.GetFiles("Assets\\Resources\\" + GameStyles.GetPath(StylesFolder));

            foreach (string s in files)
            {
                if (!s.EndsWith(".png"))
                    continue;

                string resName = GameStyles.GetPath(StylesFolder) + System.IO.Path.GetFileNameWithoutExtension(s);

                Sprite[] cardAtlas = Resources.LoadAll<Sprite>(resName);

                GameObject newStyle = Instantiate(StylePrefab, Container.transform);
                newStyle.GetComponent<UIElement>().MainImage.GetComponent<Image>().sprite = cardAtlas[0];
                newStyle.GetComponent<UIElement>().MainImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                newStyle.GetComponent<UIElement>().MainImage.GetComponent<Image>().sprite.name = resName;

                if (StylesFolder == EStyles.Card)
                {
                    if (resName == GameStyles.CURRENT_CARD_ATLAS_NAME)
                        newStyle.GetComponent<UIElement>().Selected = true;
                }
            }
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(GameStyles.GetPath(StylesFolder));

            foreach (Sprite s in sprites)
            {
                GameObject newStyle = Instantiate(StylePrefab, Container.transform);
                newStyle.GetComponent<UIElement>().MainImage.GetComponent<Image>().sprite = s;
                newStyle.GetComponent<UIElement>().MainImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                if (StylesFolder == EStyles.Background)
                {
                    if (s == GameStyles.CURRENT_BACKGROUND)
                        newStyle.GetComponent<UIElement>().Selected = true;
                }
            }
        }
    }

    public void updateStyle(Sprite s)
    {
        if (StylesFolder == EStyles.Background)
        {
            GameStyles.CURRENT_BACKGROUND = s;
        }
        else if (StylesFolder == EStyles.Card)
        {
            GameStyles.CURRENT_CARD_ATLAS_NAME = s.name;
        }
    }

    public void CancelAllGlow()
    {
        foreach (Transform t in Container.transform)
        {
            t.GetComponent<Glow>().CancelGlow();
        }
    }

    public void setSelected(UIElement ui)
    {
        CancelAllGlow();
        foreach (Transform t in Container.transform)
        {
            t.GetComponent<UIElement>().Selected = false;
        }
        ui.GetComponent<UIElement>().Selected = true;
    }
}
