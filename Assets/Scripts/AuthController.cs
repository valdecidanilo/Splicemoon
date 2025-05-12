using DB;
using DB.Data;
using UnityEngine;
public class AuthController : MonoBehaviour
{
    public Database Database;
    
    public UserData Login(string email, string password)
    {
        return Database.LoginUser(email, password);
    }
    public (bool,string, int) Register(string email, string password, string nickname)
    {
        return Database.RegisterUser(email, password, nickname);
    }
}