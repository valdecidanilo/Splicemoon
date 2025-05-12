using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DB.Data;
using Inventory;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

namespace DB
{
    public class Database
    {
        private SQLiteConnection db;
        //Local : C:/Users/valde/AppData/LocalLow/
        public Database()
        {
            var dbPath = Path.Combine(Application.persistentDataPath, "splicemon.db");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<SplicemonData>();
            db.CreateTable<UserData>();
        }
        string regexEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public (bool, string, int) RegisterUser(string email, string password, string nickname)
        {
            if (password.Length < 6) return (false, "Senha muito curta", -1);
            if(nickname.Length < 3) return (false, "Nickname muito curto", -1);
            if(!System.Text.RegularExpressions.Regex.IsMatch(email, regexEmail)) return (false, "Email inválido", -1);
            if (db.Table<UserData>().Any(u => u.Email == email))
            {
                return (false, "Usuário já existe", -1);
            }

            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(password, salt);

            var newUser = new UserData
            {
                Email = email,
                PasswordHash = hash,
                Salt = salt,
                Nickname = nickname
            };

            db.Insert(newUser);
            return (true, "Usuário registrado com sucesso", newUser.Id);
        }

        public UserData LoginUser(string email, string password)
        {
            var user = db.Table<UserData>().FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            string hash = PasswordHasher.HashPassword(password, user.Salt);
            if (hash == user.PasswordHash)
            {
                Logger.Log("Login bem-sucedido.");
                return user;
            }

            Logger.Log("Senha incorreta.");
            return null;
        }

        public void SaveSplicemonForUser(SplicemonData mon, int userId)
        {
            db.Insert(mon);
        }

        public SplicemonData GetFirstSplicemon(int userId)
        {
            return db.Table<SplicemonData>().FirstOrDefault(x => x.UserId == userId);
        }
        
        public List<SplicemonData> GetSplicemonsByUser(int userId)
        {
            return db.Table<SplicemonData>().Where(x => x.UserId == userId).ToList();
        }

        public void DeleteAll()
        {
            db.DeleteAll<SplicemonData>();
        }
        public SplicemonData SaveData(SpliceMon sp)
        {
            return new SplicemonData
            {
                Name = sp.nameSpliceMon,
                Level = sp.level,
                Nature = sp.nature,
                Hp = sp.stats.hpStats.currentStat,
                Attack = sp.stats.attackStats.currentStat,
                Defense = sp.stats.defenseStats.currentStat,
                SpAttack = sp.stats.specialAttackStats.currentStat,
                SpDefense = sp.stats.specialDefenseStats.currentStat,
                Speed = sp.stats.speedStats.currentStat,
                Experience = sp.experience,
                BaseExperience = sp.baseExperience,
                FrontSprite = sp.frontSprite,
                BackSprite = sp.backSprite,
                CrySound = sp.crySound,
                GrowthRate = sp.growthRate.ToString(),

                MovesJson = JsonConvert.SerializeObject(new MoveListWrapper { moves = sp.stats.movesAttack }),
                PossibleMovesJson = JsonConvert.SerializeObject(new StringListWrapper { items = sp.stats.possiblesMoveAttack })
            };
        }
    }
}