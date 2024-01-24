using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;

namespace classMySQL;

public class MySqlBd {

    private const string HOSTBD     = "localhost";  //Адрес сервера
    private const string PORTBD     = "3306";       //Порт сервера
    private const string USERBD     = "root";       //Пользователь БД
    private const string PASSWORDBD = "root";       //Пароль пользователя БД
    private const string NAMEBD     = "testbase"; //Название БД
    private       string connect    = $"Server={HOSTBD};Port={PORTBD};User={USERBD};Password={PASSWORDBD};Database={NAMEBD};";


    //Метод подключения к Базе данных который возвращает лож или истину.
    private async Task<bool> ConnectAddingRectification(string sql, Dictionary<string, string>? parameters = null) {

        try {

            using (MySqlConnection conn = new MySqlConnection(connect)) {
                
                await conn.OpenAsync();

                MySqlCommand command = new MySqlCommand(sql, conn);

                if (parameters != null) {
                    foreach (var el in parameters)
                    {
                        MySqlParameter param = new MySqlParameter($"@{el.Key}", el.Value);
                        command.Parameters.Add(param);
                    }
                }

                await command.ExecuteNonQueryAsync();
                return true;
            }
        
        } catch (Exception ) {
           return false;
        }

    } 

    //Соединения с базой для выборки данных
    private async Task<MySqlDataReader?> DataSelectConnectMysql(string sql) {

        try {
                MySqlConnection conn = new MySqlConnection(connect);   
                await conn.OpenAsync();

                MySqlCommand command = new MySqlCommand(sql, conn);

                return (MySqlDataReader) await command.ExecuteReaderAsync();
                   
        } catch (Exception) {
           return null;
        }

    } 


    //Запрос на добавление данных в таблицу
    public async Task<bool> DataAddRecordBaseSQL(string nameTable, Dictionary<string, string> arrayData) {

        string columns = ""; 
        string data    = "";

        if (arrayData.Count > 0) {

            foreach (var el in arrayData) {
                columns += el.Key + ", ";
                data    += "@" +el.Key + ", ";
            }

            columns = columns.TrimEnd(',', ' ');
            data    = data.   TrimEnd(',', ' ');

            string sql = $"INSERT INTO {nameTable} ({columns}) VALUES ({data})";

            return await ConnectAddingRectification(sql, arrayData);

        }else{
            return false;
        }

    }

    //Запрос на редактирование записей
    public async Task<bool> CreatedBaseRecord(string nameTable, Dictionary<string, string> arrayData, string? parameters = null) {
        
        string data = "";
        string sql  = "";

        if (arrayData.Count > 0) {

            foreach (var el in arrayData) {
                data    += el.Key + " = @" +el.Key + ", ";
            }

            data       = data.TrimEnd(',', ' ') + " ";
            if (parameters == null) {
                sql = $"UPDATE {nameTable} SET {data}";                
            }else{
                sql = $"UPDATE {nameTable} SET {data} WHERE {parameters}";
            }

            return await ConnectAddingRectification(sql, arrayData);

        }else{
            return false;
        }

    }

    //Запрос на выборку данных из базы данных
    public async Task<Dictionary<int, Dictionary<string, dynamic>>?> DataSelectionSQL(string nameСolumns, string baseName, string? condition = "") {
    
            string sql = $"SELECT {nameСolumns} FROM {baseName} {condition}";

            using (var reader = await DataSelectConnectMysql(sql)) {

                Dictionary<int, Dictionary<string, dynamic>> data = new Dictionary<int, Dictionary<string, dynamic>>();
                int id = 0;

                if (reader != null) {

                    while (await reader.ReadAsync()) {
                    
                        Dictionary<string, dynamic> row = new Dictionary<string, dynamic>();

                        for (int i = 0; i < reader.FieldCount; i++)  {
                            row.Add(reader.GetName(i), reader[i]);
                        }

                        data.Add(id, row);                    
                        id++;
                    }

                    reader.Close();
                    return data;
                }else{
                    return null;
                }
            }

    }


    //Запрос для создания таблиц
    public async Task<bool?> CreateTable(string baseName, string nameСolumns, string? engine = null) { // CREATE TABLE articles 1 param Name BD, 2 nameСolumns 3 engine"

        string sql = $"CREATE TABLE IF NOT EXISTS {baseName} ({nameСolumns}) {engine};";

        if(await ConnectAddingRectification(sql)) {
            return true;
        }else{
            return false;
        }
    }


    //Запрос для удаления данных в таблиц
    public async Task<bool> DeleteData(string baseName, string? condition = null) { // DELETE TABLE articles 1 param Name BD 2 param condition "WHERE id = 1 or WHERE title = `title`"

        if (condition == null) {
            string sql = $"DELETE FROM {baseName}";
            if(await ConnectAddingRectification(sql)) {
                return true;
            }else{
                return false;
            }
        }else{
             string sql = $"DELETE FROM {baseName} {condition}";
            if(await ConnectAddingRectification(sql)) {
                return true;
            }else{
                return false;
            }           
        }
    }


    //Запрос для удаления данных в таблиц
    public async Task<bool?> DeleteTable(string baseName) { // DROP TABLE articles 1 param Name BD"

        string sql = $"DROP FROM {baseName};";
        if(await ConnectAddingRectification(sql)) {
            return true;
        }else{
            return false;
        }
    }

}