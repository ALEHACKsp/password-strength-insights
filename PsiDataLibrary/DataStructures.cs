using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsiDataStructures
{
    /* Base table structures */
    public struct AccountData
    {
        public int? Id { get; }
        public Guid Guid { get; }

        public AccountData(Guid guid)
        {
            this.Id = null;
            this.Guid = guid;
        }

        public AccountData(int id, Guid guid)
        {
            this.Id = id;
            this.Guid = guid;
        }
    }

    public struct PasswordData
    {
        public byte[] Nt { get; }
        public byte[] Lm { get; }
        public string Plaintext { get; }
        public float Strength { get; set; }

        public PasswordData(byte[] nt, byte[] lm, string plaintext = null, float strength = 0.0f)
        {
            this.Nt = nt;
            this.Lm = lm;
            this.Plaintext = plaintext;
            this.Strength = strength;
        }

        public PasswordData SetStrength(float strength)
        {
            return new PasswordData(this.Nt, this.Lm, this.Plaintext, strength);
        }
    }

    public struct RecordData
    {
        public bool Enabled { get; }
        public bool Deleted { get; }
        public DateTime Timestamp { get; }

        public RecordData(bool enabled, bool deleted, DateTime timestamp)
        {
            this.Enabled = enabled;
            this.Deleted = deleted;
            this.Timestamp = timestamp;
        }
    }

    /* Specialized overview classes */
    public class ValidAccount
    {
        public AccountData Account { get; }
        public string AccountNames { get; }
        public long RecordCount { get; }

        public ValidAccount(AccountData account, string account_names, long record_count)
        {
            this.Account = account;
            this.AccountNames = account_names;
            this.RecordCount = record_count;
        }
    }

    public class ValidEnvironment
    {
        public DateTime Timestamp { get; }
        public long RecordCount { get; }

        public ValidEnvironment(DateTime timestamp, long record_count)
        {
            this.Timestamp = timestamp;
            this.RecordCount = record_count;
        }
    }

    /* Specialized record classes */
    public class AccountRecord
    {
        public string AccountName { get; }
        public PasswordData Password { get; set; }
        public RecordData Details { get; }

        public AccountRecord(string account_name, PasswordData password, RecordData details)
        {
            this.AccountName = account_name;
            this.Password = password;
            this.Details = details;
        }
    }

    public class EnvironmentRecord : AccountRecord
    {
        public AccountData Account { get; }

        public EnvironmentRecord(AccountData account, string account_name, PasswordData password, RecordData details) :
            base(account_name, password, details)
        {
            this.Account = account;
        }
    }
}