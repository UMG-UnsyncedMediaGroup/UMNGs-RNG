using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

class Program
{
    class PlayerData
    {
        public int Balance { get; set; }
        public List<string> Collection { get; set; } = new List<string>();
        public int PityCounter { get; set; } = 0;
    }

    class SaveFile
    {
        public PlayerData Data { get; set; }
        public string Hash { get; set; }
    }

    class Femboy
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
    }

    static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("FemboysGetMeHotForTheLadyboysOng");

    static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = EncryptionKey;
        aes.GenerateIV();

        using MemoryStream ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter sw = new StreamWriter(cs))
            sw.Write(plainText);

        return Convert.ToBase64String(ms.ToArray());
    }

    static string Decrypt(string encryptedText)
    {
        byte[] fullBytes = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();
        aes.Key = EncryptionKey;

        byte[] iv = new byte[16];
        Array.Copy(fullBytes, iv, 16);
        aes.IV = iv;

        using MemoryStream ms = new MemoryStream(fullBytes, 16, fullBytes.Length - 16);
        using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    static string ComputeHash(string input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes); 
        }
    }

    static void Main()
    {
        Random rng = new Random();
        string saveFile = "playerData.json";
        PlayerData player = new PlayerData { Balance = 100 };

        if (File.Exists(saveFile))
        {
            try
            {
                string fileContents = File.ReadAllText(saveFile);
                SaveFile loaded = null;
                bool loadedOk = false;

                try
                {
                    string decrypted = Decrypt(fileContents);
                    loaded = JsonSerializer.Deserialize<SaveFile>(decrypted);
                }
                catch
                {
                    
                }

                if (loaded == null)
                {
                    try
                    {
                        loaded = JsonSerializer.Deserialize<SaveFile>(fileContents);
                    }
                    catch
                    {
                        loaded = null;
                    }
                }

                if (loaded != null)
                {
                    string recalculatedHash = ComputeHash(JsonSerializer.Serialize(loaded.Data));
                    if (!string.IsNullOrEmpty(loaded.Hash) && loaded.Hash == recalculatedHash)
                    {
                        player = loaded.Data; 
                        loadedOk = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Save File Has Been Edited Or Corrupted, Resetting Progress.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                if (!loadedOk && loaded == null)
                {
                    Console.WriteLine("No Valid Save File Found, Starting fresh.");
                }
            }
            catch
            {
                Console.WriteLine("Failed To Load Save, Starting Fresh.");
            }
        }

        string[] slotSymbols = { "X", "K", "L", "D", "PP" };

        List<Femboy> femboys = new List<Femboy>()
        {
            new Femboy { Name = "Matt",    Rarity = "Common" },
            new Femboy { Name = "Dylan",   Rarity = "Common" },
            new Femboy { Name = "Blake",   Rarity = "Uncommon" },
            new Femboy { Name = "Cody",    Rarity = "Rare" },
            new Femboy { Name = "Dante",   Rarity = "Epic" },
            new Femboy { Name = "Elliot",  Rarity = "Legendary" },
            new Femboy { Name = "Nathan",  Rarity = "Ultra" }
        };

        Dictionary<string, int> rarityWeights = new Dictionary<string, int>()
        {
            { "Common", 50 },
            { "Uncommon", 20 },
            { "Rare", 15 },
            { "Epic", 10 },
            { "Legendary", 4 },
            { "Ultra", 1 }
        };

        string GetWeightedRarity()
        {
            int total = 0;
            foreach (var r in rarityWeights.Values)
                total += r;

            int roll = rng.Next(total);
            int running = 0;

            foreach (var pair in rarityWeights)
            {
                running += pair.Value;
                if (roll < running)
                    return pair.Key;
            }
            return "Common";
        }

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("$$\\   $$\\ $$\\   $$\\ $$\\      $$\\  $$$$$$\\  $$\\               $$$$$$$\\  $$\\   $$\\  $$$$$$\\  \r\n$$ |  $$ |$$$\\  $$ |$$$\\    $$$ |$$  __$$\\ $  |              $$  __$$\\ $$$\\  $$ |$$  __$$\\ \r\n$$ |  $$ |$$$$\\ $$ |$$$$\\  $$$$ |$$ /  \\__|\\_/$$$$$$$\\       $$ |  $$ |$$$$\\ $$ |$$ /  \\__|\r\n$$ |  $$ |$$ $$\\$$ |$$\\$$\\$$ $$ |$$ |$$$$\\   $$  _____|      $$$$$$$  |$$ $$\\$$ |$$ |$$$$\\ \r\n$$ |  $$ |$$ \\$$$$ |$$ \\$$$  $$ |$$ |\\_$$ |  \\$$$$$$\\        $$  __$$< $$ \\$$$$ |$$ |\\_$$ |\r\n$$ |  $$ |$$ |\\$$$ |$$ |\\$  /$$ |$$ |  $$ |   \\____$$\\       $$ |  $$ |$$ |\\$$$ |$$ |  $$ |\r\n\\$$$$$$  |$$ | \\$$ |$$ | \\_/ $$ |\\$$$$$$  |  $$$$$$$  |      $$ |  $$ |$$ | \\$$ |\\$$$$$$  |\r\n \\______/ \\__|  \\__|\\__|     \\__| \\______/   \\_______/       \\__|  \\__|\\__|  \\__| \\______/ \r\n                                                                                           \r\n                                                                                           \r\n                                                                                           ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"Balance: {player.Balance} Bucks | Collection: {player.Collection.Count} Femboys");
            Console.WriteLine("Choose A Game:");
            Console.WriteLine("1. Slot Machine (10 Bucks Per Spin)");
            Console.WriteLine("2. Femboy Gacha (20 Bucks Per Pull)");
            Console.WriteLine("3. Quit\n");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                while (true)
                {
                    if (player.Balance < 10)
                        Console.WriteLine("Not Enough Cash To Spin! Type 'Back' To Leave.");

                    Console.WriteLine($"\nBalance: {player.Balance} Bucks");
                    Console.Write("Press ENTER To Spin Or Type 'Back' To Return: ");
                    string input = Console.ReadLine();

                    if (input.ToLower() == "back")
                        break;

                    if (player.Balance < 10)
                        continue;

                    player.Balance -= 10;

                    string s1 = slotSymbols[rng.Next(slotSymbols.Length)];
                    string s2 = slotSymbols[rng.Next(slotSymbols.Length)];
                    string s3 = slotSymbols[rng.Next(slotSymbols.Length)];

                    Console.WriteLine($"\n[ {s1} | {s2} | {s3} ]");

                    if (s1 == s2 && s2 == s3)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("JACKPOT! +100 Bucks!");
                        player.Balance += 100;
                    }
                    else if (s1 == s2 || s2 == s3 || s1 == s3)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Matched Two! +20 Bucks!");
                        player.Balance += 20;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No Match, Lmfao Noob.");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            else if (choice == "2")
            {
                while (true)
                {
                    if (player.Balance < 20)
                        Console.WriteLine("Not Enough Cash To Pull! Type 'Back' To Leave.");

                    Console.WriteLine($"\nBalance: {player.Balance} Bucks | Collection: {player.Collection.Count}");
                    Console.Write("Press ENTER To Pull Or Type 'Back': ");
                    string ginput = Console.ReadLine();

                    if (ginput.ToLower() == "back")
                        break;

                    if (player.Balance < 20)
                        continue;

                    player.Balance -= 20;
                    player.PityCounter++;

                    string rarityPulled;

                    if (player.PityCounter >= 50)
                    {
                        rarityPulled = "Legendary";
                        player.PityCounter = 0;

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPITY ACTIVATED! You'll Get A Guaranteed Legendary, Loser.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        rarityPulled = GetWeightedRarity();
                    }

                    var pool = femboys.FindAll(f => f.Rarity == rarityPulled);

                    Femboy chosen;
                    if (pool.Count == 0)
                    {
                        chosen = femboys[rng.Next(femboys.Count)];
                    }
                    else
                    {
                        chosen = pool[rng.Next(pool.Count)];
                    }

                    string result = $"{chosen.Name} ({chosen.Rarity})";
                    player.Collection.Add(result);

                    Console.ForegroundColor =
                        chosen.Rarity switch
                        {
                            "Common" => ConsoleColor.Gray,
                            "Uncommon" => ConsoleColor.Green,
                            "Rare" => ConsoleColor.Blue,
                            "Epic" => ConsoleColor.Magenta,
                            "Legendary" => ConsoleColor.Yellow,
                            "Ultra" => ConsoleColor.Cyan,
                            _ => ConsoleColor.White
                        };

                    Console.WriteLine($"\nYou Pulled: {result}!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid Choice Please Try Again.");
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nYou Cashed Out With {player.Balance} Bucks And {player.Collection.Count} Femboys!");

        try
        {
            string dataJson = JsonSerializer.Serialize(player);
            string hash = ComputeHash(dataJson);

            SaveFile save = new SaveFile
            {
                Data = player,
                Hash = hash
            };

            string plainJson = JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true });

            string encrypted = Encrypt(plainJson);
            File.WriteAllText(saveFile, encrypted);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Game Saved.");
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed To Save Game.");
        }

        Console.ResetColor();
    }
}
