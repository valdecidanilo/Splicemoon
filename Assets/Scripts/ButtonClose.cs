using CustomButton;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClose : MonoBehaviour
{
    public CustomButtonClass button;
    public Image iconSelect;
    public Image container;
    public Sprite[] containerSelect;
    public void SetContainerSelected(int index) => container.sprite = containerSelect[index == 0 ? 1 : 0];
    public void SetIconSelected(Sprite spr) => iconSelect.sprite = spr;
}