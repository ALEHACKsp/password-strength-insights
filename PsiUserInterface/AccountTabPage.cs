using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using PsiDatabase;
using PsiDataStructures;

namespace PsiUserInterface
{
    public partial class AccountTabPage : UserControl
    {
        private DatabaseClient db;
        private List<ValidAccount> accounts;

        public AccountTabPage(Control c, Rectangle r, DatabaseClient db)
        {
            InitializeComponent();

            this.ClientSize = r.Size;
            this.Parent = c;

            if ((this.accounts = (this.db = db).FetchValidAccounts()) != null)
            {
                this.accountsListView.Items.Clear();
                
                foreach (ValidAccount account in this.accounts)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, account.AccountNames));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, account.RecordCount.ToString()));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, account.Account.Guid.ToString()));

                    this.accountsListView.Items.Add(lvi);
                }
            }
        }

        private void accountsListView_ItemActivate(object sender, EventArgs e)
        {
            int index = this.accountsListView.SelectedItems[0].Index;
            var records = this.db.FetchAccountRecords(this.accounts[index].Account.Id.Value);

            /* Display record list */
            this.recordsListView.Items.Clear();

            if (records == null || records.Count == 0)
                this.recordsListView.Enabled = false;
            else
            {
                this.recordsListView.Enabled = true;

                foreach (AccountRecord record in records)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, record.AccountName));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, record.Details.Enabled.ToString()));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, record.Details.Deleted.ToString()));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.GetTime(record.Details.Timestamp)));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.StringOrAlternative(record.Password.Plaintext)));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.FloatOrAlternative((record.Password = record.Password.SetStrength(PasswordStrength.EvaluatePassword(record.Password.Plaintext))).Strength)));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.ByteStringOrNull(record.Password.Nt)));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.ByteStringOrNull(record.Password.Lm)));

                    this.recordsListView.Items.Add(lvi);
                }

                /* Calculate statistics */
                var statistics = PasswordStatistics.Compute(records);

                this.statisticsListView.Items.Clear();

                if (statistics == null || statistics.Count == 0)
                    this.statisticsListView.Enabled = false;
                else
                {
                    this.statisticsListView.Enabled = true;

                    foreach (AccountStatistic statistic in statistics)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, statistic.Description));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.DoubleOrAlternative(statistic.Value)));

                        this.statisticsListView.Items.Add(lvi);
                    }
                }

                /* Basic actions */
                this.Parent.Text = this.accounts[index].AccountNames + Form1.padding_spaces;
            }
        }
    }
}
