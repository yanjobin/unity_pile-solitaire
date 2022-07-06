using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ECardValue { A, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }
public enum ECardSuit { Club, Diamond, Heart, Spade }

public class CardValue : MonoBehaviour
{
    public Sprite CardBackgroundImage;
    public ECardValue Value { get; set; }
    public ECardSuit Suit { get; set; }

    void Start()
    {
        updateSprite();
    }

    void Update()
    {
        
    }

    public void updateSprite()
    {
        if (GetComponent<CardState>().Flipped)
        {
            transform.Find("CardImage").GetComponent<Image>().sprite = CardBackgroundImage;
        }
        else
        {
            Sprite[] atlas = Resources.LoadAll<Sprite>(GameStyles.CURRENT_CARD_ATLAS_NAME);
            int id = (int)Value + ((int)Suit * ((int)ECardValue.King + 1));
            transform.Find("CardImage").GetComponent<Image>().sprite = atlas[id];
        }        
    }
}
