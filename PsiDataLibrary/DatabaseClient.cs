using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using PsiDataStructures;

namespace PsiDatabase
{
    public class DatabaseClient
    {
        private MySqlConnection connection;

        public DatabaseClient(string server = "localhost", string database = "psi", string username = "root", string password = "Pa$$w0rd")
        {
            this.connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};", server, database, username, password));
        }

        public bool Connect()
        {
            try
            {
                this.connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to database. Connection failed.");
                        break;

                    case 1045:
                        Console.WriteLine("Cannot connect to database. Wrong username or password.");
                        break;
                }

                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                this.connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool IsConnected()
        {
            try
            {
                return this.connection.Ping();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<ValidAccount> FetchValidAccounts()
        {
            string query = "SELECT a.`id`, a.`guid`, GROUP_CONCAT(DISTINCT n.`name` ORDER BY n.`id` DESC SEPARATOR ', ') `names`, COUNT(*) `count` FROM `Records` r " +
                "JOIN `AccountNames` n ON r.`account_name_id` = n.`id` " +
                "JOIN `Accounts` a ON n.`account_id` = a.`id` " +
                    "GROUP BY a.`guid` " +
                    "ORDER BY a.`id` ASC;";

            return this.ReadObjects<ValidAccount>(query, (MySqlDataReader reader) =>
            {
                AccountData account = new AccountData(
                    ReadNullable<int>(reader["id"]),
                    new Guid(ReadNullable<byte[]>(reader["guid"])));

                return new ValidAccount(account,
                    ReadNullable<string>(reader["names"]),
                    ReadNullable<long>(reader["count"]));
            });
        }

        public List<AccountRecord> FetchAccountRecords(int account_id)
        {
            string query = "SELECT n.`name`, p.`nt`, p.`lm`, p.`plaintext`, r.`account_enabled` `enabled`, r.`account_deleted` `deleted`, r.`timestamp` FROM `Records` r " +
                "JOIN `AccountNames` n ON r.`account_name_id` = n.`id` " +
                "JOIN `Accounts` a ON n.`account_id` = a.`id` " +
                "JOIN `Passwords` p ON r.`password_id` = p.`id` " +
                    "WHERE a.`id` = @account_id " +
                    "ORDER BY r.`timestamp` DESC, n.`id` ASC;";

            return this.ReadObjects<AccountRecord>(query, (MySqlDataReader reader) =>
            {
                PasswordData password = new PasswordData(
                    ReadNullable<byte[]>(reader["nt"]),
                    ReadNullable<byte[]>(reader["lm"]),
                    ReadNullable<string>(reader["plaintext"]));

                RecordData details = new RecordData(
                    ReadNullable<bool>(reader["enabled"]),
                    ReadNullable<bool>(reader["deleted"]),
                    ReadNullable<DateTime>(reader["timestamp"]));

                return new AccountRecord(
                    ReadNullable<string>(reader["name"]),
                    password, details);
            },
                new KeyValuePair<string, object>("@account_id", account_id));
        }

        public List<ValidEnvironment> FetchValidEnvironments()
        {
            string query = "SELECT r.`timestamp`, COUNT(*) `count` FROM `Records` r " +
                "JOIN `AccountNames` n ON r.`account_name_id` = n.`id` " +
                "JOIN `Accounts` a ON n.`account_id` = a.`id`" +
                    "GROUP BY DATE(r.`timestamp`) " +
                    "ORDER BY DATE(r.`timestamp`) DESC;";

            return this.ReadObjects<ValidEnvironment>(query, (MySqlDataReader reader) =>
            {
                return new ValidEnvironment(
                    ReadNullable<DateTime>(reader["timestamp"]),
                    ReadNullable<long>(reader["count"]));
            });
        }

        public List<EnvironmentRecord> FetchEnvironmentRecords(DateTime timestamp)
        {
            string query = "SELECT a.`id`, a.`guid`, n.`name`, p.`nt`, p.`lm`, p.`plaintext`, r.`account_enabled` `enabled`, r.`account_deleted` `deleted`, r.`timestamp` FROM `Records` r " +
                "JOIN `AccountNames` n ON r.`account_name_id` = n.`id` " +
                "JOIN `Accounts` a ON n.`account_id` = a.`id` " +
                "JOIN `Passwords` p ON r.`password_id` = p.`id` " +
                    "WHERE DATE(r.`timestamp`) = DATE(@timestamp) " +
                    "ORDER BY r.`timestamp` DESC, a.`id` ASC, n.`id` ASC;";

            return this.ReadObjects<EnvironmentRecord>(query, (MySqlDataReader reader) =>
            {
                AccountData account = new AccountData(
                    ReadNullable<int>(reader["id"]),
                    new Guid(ReadNullable<byte[]>(reader["guid"])));

                PasswordData password = new PasswordData(
                    ReadNullable<byte[]>(reader["nt"]),
                    ReadNullable<byte[]>(reader["lm"]),
                    ReadNullable<string>(reader["plaintext"]));

                RecordData details = new RecordData(
                    ReadNullable<bool>(reader["enabled"]),
                    ReadNullable<bool>(reader["deleted"]),
                    ReadNullable<DateTime>(reader["timestamp"]));

                return new EnvironmentRecord(account,
                    ReadNullable<string>(reader["name"]),
                    password, details);
            },
                new KeyValuePair<string, object>("@timestamp", timestamp));
        }

        public void StoreEnvironmentRecord(EnvironmentRecord record)
        {
            long account_id = this.StoreAccount(record.Account);
            long account_name_id = this.StoreAccountName(record.AccountName, account_id);
            long password_id = this.StorePassword(record.Password);
            long record_id = this.StoreRecord(record.Details, account_name_id, password_id);
        }

        /* `Accounts` table */
        private long FetchAccountId(Guid guid)
        {
            return this.ReadId("SELECT `id` FROM `Accounts` WHERE `guid` = @guid;",
                new KeyValuePair<string, object>("@guid", guid.ToByteArray()));
        }

        private long StoreAccount(AccountData account)
        {
            return this.InsertObject("INSERT INTO `Accounts`(`guid`) VALUES (@guid);", () => FetchAccountId(account.Guid),
                new KeyValuePair<string, object>("@guid", account.Guid.ToByteArray()));
        }

        /* `AccountNames` table */
        private long FetchAccountNameId(long account_id, string account_name)
        {
            return this.ReadId("SELECT `id` FROM `AccountNames` WHERE `account_id` = @account_id AND `name` = @account_name;",
                new KeyValuePair<string, object>("@account_id", account_id),
                new KeyValuePair<string, object>("@account_name", account_name));
        }

        private long StoreAccountName(string account_name, long account_id)
        {
            return this.InsertObject("INSERT INTO `AccountNames`(`account_id`, `name`) VALUES (@account_id, @account_name);", () => FetchAccountNameId(account_id, account_name),
                new KeyValuePair<string, object>("@account_id", account_id),
                new KeyValuePair<string, object>("@account_name", account_name));
        }

        /* `Passwords` table */
        private long FetchPasswordId(byte[] nt_hash)
        {
            return this.ReadId("SELECT `id` FROM `Passwords` WHERE `nt` = @nt_hash;",
                new KeyValuePair<string, object>("@nt_hash", nt_hash));
        }

        private long StorePassword(PasswordData password)
        {
            return this.InsertObject("INSERT INTO `Passwords`(`nt`, `lm`) VALUES (@nt_hash, @lm_hash);", () => FetchPasswordId(password.Nt),
                new KeyValuePair<string, object>("@nt_hash", password.Nt),
                new KeyValuePair<string, object>("@lm_hash", password.Lm));
        }
        
        /* `Records` table */
        private long StoreRecord(RecordData details, long account_name_id, long password_id)
        {
            return this.InsertObject("INSERT INTO `Records`(`account_name_id`, `password_id`, `account_enabled`, `account_deleted`, `timestamp`) VALUES (@account_name_id, @password_id, @account_enabled, @account_deleted, CURRENT_TIMESTAMP);", null,
                new KeyValuePair<string, object>("@account_name_id", account_name_id),
                new KeyValuePair<string, object>("@password_id", password_id),
                new KeyValuePair<string, object>("@account_enabled", details.Enabled),
                new KeyValuePair<string, object>("@account_deleted", details.Deleted));
        }

        /* Base functionality */
        private List<T> ReadObjects<T>(string query, Func<MySqlDataReader, T> make_object, params KeyValuePair<string, object>[] parameters)
        {
            if (this.IsConnected())
            {
                try
                {
                    using (MySqlCommand cmd = PrepareStatement(query, parameters))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            List<T> results = new List<T>();

                            while (reader.Read())
                                results.Add(make_object(reader));

                            return results;
                        }
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        private long ReadId(string query, params KeyValuePair<string, object>[] parameters)
        {
            if (this.IsConnected())
            {
                try
                {
                    using (MySqlCommand cmd = PrepareStatement(query, parameters))
                        return Convert.ToInt64(cmd.ExecuteScalar());
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return 0;
        }

        private long InsertObject(string query, Func<long> fetch_id, params KeyValuePair<string, object>[] parameters)
        {
            if (this.IsConnected())
            {
                try
                {
                    using (MySqlCommand cmd = PrepareStatement(query, parameters))
                    {
                        if (cmd.ExecuteNonQuery() == 1)
                            return cmd.LastInsertedId;
                    }
                }
                catch (MySqlException e)
                {
                    if (e.Number != 1062)
                        Console.WriteLine(e.Message);
                    else if (fetch_id != null)
                        return fetch_id();
                }
            }

            return 0;
        }

        private bool UpdateObjects(string query, params KeyValuePair<string, object>[] parameters)
        {
            if (this.IsConnected())
            {
                try
                {
                    using (MySqlCommand cmd = PrepareStatement(query, parameters))
                    {
                        if (cmd.ExecuteNonQuery() != 0)
                            return true;
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return false;
        }

        private MySqlCommand PrepareStatement(string query, params KeyValuePair<string, object>[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, this.connection);

            foreach (KeyValuePair<string, object> parameter in parameters)
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

            cmd.Prepare();

            return cmd;
        }

        private T ReadNullable<T>(object o)
        {
            return (o != DBNull.Value ? (T)o : default(T));
        }
    }
}