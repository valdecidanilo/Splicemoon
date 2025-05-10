using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlicemon : MonoBehaviour
{
    public Image iconSelect;
    public Image iconGender;
    public Image iconSplicemon;
    public Image containerImage;
    public Sprite[] containerSelect;
    public TMP_Text nameSplicemon;
    public TMP_Text levelSplicemon;
    public TMP_Text hpSplicemon;
    
    public HealthBar healthBar;
    public void SetIconSelected(Sprite sprite) => iconSelect.sprite = sprite;
    public void SelectGender(Sprite sprite) => iconGender.sprite = sprite;
    public void SetIconSplicemon(Sprite sprite) => iconSplicemon.sprite = sprite;
    public void SetSplicemon(string nameCurrentSplicemon, int level, int hp, int hpMax)
    {
        nameSplicemon.SetText(nameCurrentSplicemon);
        levelSplicemon.SetText($"Lv{level}");
        hpSplicemon.SetText($"{hp}/ {hpMax}");
    }
    public void SetSelectedContainer(bool isSelected) => containerImage.sprite = containerSelect[isSelected ? 1 : 0 ];
}
