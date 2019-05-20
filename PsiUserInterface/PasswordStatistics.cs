using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PsiDataStructures;

namespace PsiUserInterface
{
    public struct EnvironmentStatistic
    {
        public string Description { get; }
        public double All { get; }
        public double Enabled { get; }
        public double Disabled { get; }

        public EnvironmentStatistic(string description, 
            List<EnvironmentRecord> all,
            List<EnvironmentRecord> enabled, 
            List<EnvironmentRecord> disabled, 
            Func<List<EnvironmentRecord>, double> functor)
        {
            this.Description = description;
            this.All = functor(all);
            this.Enabled = functor(enabled);
            this.Disabled = functor(disabled);
        }
    };

    public struct AccountStatistic
    {
        public string Description { get; }
        public double Value { get; }

        public AccountStatistic(string description, double value)
        {
            this.Description = description;
            this.Value = value;
        }
    };

    class PasswordStatistics
    {
        public static List<EnvironmentStatistic> Compute(List<EnvironmentRecord> records)
        {
            var enabled = new List<EnvironmentRecord>();
            var disabled = new List<EnvironmentRecord>();

            foreach (var record in records)
            {
                (record.Details.Enabled ? enabled : disabled).Add(record);
            }

            return new List<EnvironmentStatistic>
            {
                new EnvironmentStatistic("Amount of account records", records, enabled, disabled, RecordCount),
                new EnvironmentStatistic("Average password strength", records, enabled, disabled, AverageStrength)
            };
        }

        private static double RecordCount(List<EnvironmentRecord> list)
        {
            return Convert.ToDouble(list.Count);
        }

        private static double AverageStrength(List<EnvironmentRecord> list)
        {
            int count = 0;
            double sum = 0.0;

            foreach (var record in list)
            {
                if (record.Password.Strength != 0.0f)
                {
                    count++;
                    sum += record.Password.Strength;
                }
            }

            return (sum / count);
        }

        public static List<AccountStatistic> Compute(List<AccountRecord> records)
        {
            return new List<AccountStatistic>
            {
                new AccountStatistic("Average password strength", AverageStrength(records))
            };
        }

        private static double AverageStrength(List<AccountRecord> list)
        {
            int count = 0;
            double sum = 0.0;

            foreach (var record in list)
            {
                if (record.Password.Strength != 0.0f)
                {
                    count++;
                    sum += record.Password.Strength;
                }
            }

            return (sum / count);
        }

        /* Environment statistics */

        /* Account statistics */

    }
}
