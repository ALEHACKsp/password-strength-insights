using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Net.NetworkInformation;

using DSInternals.Replication;

using PsiDataStructures;
using PsiDatabase;

namespace PsiDataCollection
{
    class Program
    {
        static bool PingHost(string host)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send(host);
                    return (reply.Status == IPStatus.Success);
                }
                catch (PingException)
                {
                    return false;
                }
            }
        }

        static string FindDomainController(string domain_name)
        {
            var domain_context = new DirectoryContext(DirectoryContextType.Domain, domain_name);
            var domain_controllers = Domain.GetDomain(domain_context).FindAllDomainControllers();

            /* Enumerate all domain controllers in the target domain */
            foreach (DomainController dc in domain_controllers)
            {
                /* Check if host is alive and reachable */
                if (PingHost(dc.IPAddress))
                {
                    /* Return only the name of a domain controller rather than full domain path */
                    if (dc.Name.IndexOf('.') != -1)
                        return dc.Name.Split('.')[0];
                    else
                        return dc.Name;
                }
            }

            return null;
        }

        static List<EnvironmentRecord> AcquireRecords(string domain_name, string naming_context)
        {
            List<EnvironmentRecord> records = new List<EnvironmentRecord>();

            try
            {
                string server = FindDomainController(domain_name);

                if (server == null)
                    Console.WriteLine("Could not find a domain controller with the given parameters.");
                else
                {
                    Console.WriteLine("Found domain controller: " + server);

                    using (var client = new DirectoryReplicationClient(server, RpcProtocol.TCP))
                    {
                        Console.WriteLine("Attempting to query Active Directory records...\n");

                        foreach (var account in client.GetAccounts(naming_context, null))
                        {
                            if (account.SamAccountType == DSInternals.Common.Data.SamAccountType.User && account.NTHash != null)
                            {
                                records.Add(new EnvironmentRecord(new AccountData(account.Guid), account.SamAccountName,
                                    new PasswordData(account.NTHash, account.LMHash),
                                    new RecordData(account.Enabled, account.Deleted, DateTime.Now)));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return records;
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 6)
                {
                    Console.WriteLine("Incorrect parameters. Usage is:");
                    Console.WriteLine("PsiDataCollection.exe <{0}> <{1}> <{2}> <{3}> <{4}> <{5}>", "Domain Name", "Naming Context", "DB Host", "DB Schema", "DB Username", "DB Password");
                }
                else
                {
                    List<EnvironmentRecord> records = AcquireRecords(args[0], args[1]);
                    LogRecords(records);

                    if (records != null && records.Count > 0)
                    {
                        DatabaseClient db = new DatabaseClient(args[2], args[3], args[4], args[5]);

                        if (!db.Connect())
                            Console.WriteLine("Could not connect to the database. Please check your input parameters.");
                        else
                        {
                            Console.WriteLine("Connected to the database. Storing records...");

                            foreach (EnvironmentRecord record in records)
                                db.StoreEnvironmentRecord(record);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void LogRecords(List<EnvironmentRecord> records)
        {
            Console.WriteLine("Acquired " + records.Count + " account records.");

            foreach (EnvironmentRecord record in records)
            {
                string name = string.Format("{0, " + (20/2 + record.AccountName.Length/2) + "}", record.AccountName);

                Console.WriteLine("AccountName: {0, -20} => Guid: {1}", name, record.Account.Guid);
            }

            Console.WriteLine();
        }
    }
}
