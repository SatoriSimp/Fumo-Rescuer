using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public new SpriteRenderer renderer;

    public Sprite PlayerDefaultSprite;
    public Sprite PlayerDisabledSprite;
    public Sprite PlayerReadySprite;

    public enum SPRITE_STATUS
    {
        DEFAULT  = 0,
        DISABLED = 1,
        READY = 2,
    }

    public void ChangePlayerSprite(SPRITE_STATUS changeMethod)
    {
        switch (changeMethod)
        {
            case SPRITE_STATUS.DEFAULT:
                renderer.sprite = PlayerDefaultSprite;
                break;
            case SPRITE_STATUS.DISABLED:
                renderer.sprite = PlayerDisabledSprite;
                break;
            case SPRITE_STATUS.READY:
                renderer.sprite = PlayerReadySprite;
                break;
        }
    }
}
