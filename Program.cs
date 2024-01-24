using System;
using System.Collections;
using System.Configuration;
using System.Data;

namespace classMySQL;

class Program {

    public static async Task Main() {
        
        MySqlBd db = new MySqlBd();

        Dictionary<int, Dictionary<string, dynamic>>? array = new Dictionary<int, Dictionary<string, dynamic>>();


        array = await db.DataSelectionSQL("*", "users", "WHERE login = 'john'");

        if(array != null) {

            Dictionary<int, Dictionary<string, dynamic>>? arrayItems = new Dictionary<int, Dictionary<string, dynamic>>();


            arrayItems = await db.DataSelectionSQL("id", "items", "WHERE category = 'hats'");

            List<Dictionary<string, dynamic>> newArrayAddbase = new List<Dictionary<string, dynamic>>();

            if (arrayItems != null) {
                
                for (int i = 0; i < arrayItems.Count(); i++) {

                    Dictionary<string, dynamic> tmpArray = new Dictionary<string, dynamic>();

                    if (array != null && array.ContainsKey(0) && array[0].ContainsKey("id")) {
                        tmpArray.Add("idUsers", array[0]["id"]);

                        foreach (var el in arrayItems[i]) {

                            tmpArray.Add(el.Key, el.Value);

                        }

                        newArrayAddbase.Add(tmpArray);

                    }
                    

                }
            }

            Dictionary<string, string> tmpArrayAddBase = new Dictionary<string, string>();

            for (int i = 0; i < newArrayAddbase.Count(); i++) {

                Dictionary<string, string> tmp = new Dictionary<string, string>();

                tmp.Add("user_id", newArrayAddbase[i]["idUsers"].ToString());
                tmp.Add("item_id", newArrayAddbase[i]["id"].ToString());
                
                if (await db.DataAddRecordBaseSQL("orders", tmp)) {
                    System.Console.WriteLine("\nДобавил новую запись!");
                }else{
                    System.Console.WriteLine("Не добавил новую запись!");
                }
                
            }

        }else{
            System.Console.WriteLine("В базе с пользователями такого пользователя не существует!");
        }
    }


}