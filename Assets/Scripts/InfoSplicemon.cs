using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoSplicemon : MonoBehaviour
{
    [SerializeField] private TMP_Text nameSplicemon;
    [SerializeField] private TMP_Text levelSplicemon;
    [SerializeField] private Image genderSplicemon;
    [SerializeField] private Sprite[] genderSprites;
    [SerializeField] private Image spriteSplicemon;
    public ExpBar expBar;
    public HealthBar healthBar;
    public Animator animationSplicemon;
    
    public void SetName(string currentName) => nameSplicemon.text = currentName;
    public void SetLevel(int currentLevel) => levelSplicemon.text = $"Lv{currentLevel}";
    public void SetGender(bool isFemale) => genderSplicemon.sprite = isFemale ? genderSprites[1] : genderSprites[0];
    public void SetSprite(Sprite sprite) => spriteSplicemon.sprite = sprite;
}
