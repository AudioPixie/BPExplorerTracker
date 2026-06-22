using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyChess : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    public Toggle AutoToggle;
    public List<Sprite> sprites = new List<Sprite>();
    public int currentIndex = 0;
    
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ChangeSprite(int index)
    {
        image.sprite = sprites[index];
        currentIndex = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left 
            && AutoToggle.isOn == false )
        {
            if (currentIndex == 6)
                currentIndex = 0;
            else
                currentIndex++;

            ChangeSprite(currentIndex);
        }

        if (eventData.button == PointerEventData.InputButton.Right 
            && AutoToggle.isOn == false )
        {
            if (currentIndex == 0)
                currentIndex = 6;
            else
                currentIndex--;

            ChangeSprite(currentIndex);
        }
    }
}
