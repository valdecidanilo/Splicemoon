using System.Collections.Generic;
using DB.Data;
using UnityEngine;

namespace Networking
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }
    
        public UserData CurrentUser { get; private set; }
        public List<SplicemonData> SplicemonDataList { get; private set; }
        public string Nickname { get; private set; }
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        public void SetSessionData(UserData userData, List<SplicemonData> splicemonDataList, string nickname)
        {
            CurrentUser = userData;
            SplicemonDataList = splicemonDataList;
            Nickname = nickname;
        }
    }
}