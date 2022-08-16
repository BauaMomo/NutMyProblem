using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChandelierManager : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public Sprite DamagedChandalier;

    // Start is called before the first frame update

    public void ChangeSprite()
    {
        spriteRenderer.sprite = DamagedChandalier;
    }
}
