using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class PasswordUtility : MonoBehaviour
    {
        public TMP_InputField password;
        public Image eyeIcon;
        public Sprite eye;
        public Sprite eyeSlash;
        public void TogglePassword()
        {
            password.contentType = password.contentType == TMP_InputField.ContentType.Password ? 
                TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            eyeIcon.sprite = password.contentType == TMP_InputField.ContentType.Password ? eyeSlash : eye;
            password.ForceLabelUpdate();
        }
    }
}
