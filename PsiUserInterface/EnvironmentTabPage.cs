using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using PsiDatabase;
using PsiDataStructures;

namespace PsiUserInterface
{
    public partial class EnvironmentTabPage : UserControl
    {
        private DatabaseClient db;
        private List<ValidEnvironment> environments;

        public EnvironmentTabPage(Control c, Rectangle r, DatabaseClient db)
        {
            InitializeComponent();

            this.ClientSize = r.Size;
            this.Parent = c;

            if ((this.environments = (this.db = db).FetchValidEnvironments()) != null)
            {
                this.environmentsListView.Items.Clear();

                foreach (ValidEnvironment environment in this.environments)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.GetDate(environment.Timestamp)));
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, environment.RecordCount.ToString()));

                    this.environmentsListView.Items.Add(lvi);
                }
            }
        }

        private void environmentsListView_ItemActivate(object sender, EventArgs e)
        {
            int index = this.environmentsListView.SelectedItems[0].Index;
            DateTime time = this.environments[index].Timestamp;

            /* Display record list */
            var records = this.db.FetchEnvironmentRecords(time);

            this.recordsListView.Items.Clear();

            if (records == null || records.Count == 0)
                this.recordsListView.Enabled = false;
            else
            {
                this.recordsListView.Enabled = true;

                foreach (EnvironmentRecord record in records)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, record.Account.Guid.ToString()));
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

                    foreach (EnvironmentStatistic statistic in statistics)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, statistic.Description));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.DoubleOrAlternative(statistic.All)));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.DoubleOrAlternative(statistic.Enabled)));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, Utility.DoubleOrAlternative(statistic.Disabled)));

                        this.statisticsListView.Items.Add(lvi);
                    }
                }

                /* Evolution graph */
                this.evolutionGraph.Series.Clear();

                this.AddGraphSeries_PasswordStrength(index);

                /* Basic actions */
                this.Parent.Text = Utility.GetDate(time) + Form1.padding_spaces;
            }
        }

        private void AddGraphSeries_PasswordStrength(int index)
        {
            List<ValidEnvironment> timeline = null;

            if (index >= 0)
            {
                timeline = this.environments.GetRange(index, this.environments.Count - index);
                timeline.Reverse();

                var series_all = AddGraphSeries("Password Strength (all)");
                var series_enabled = AddGraphSeries("Password Strength (enabled)");
                var series_disabled = AddGraphSeries("Password Strength (disabled)");

                foreach (var environment in timeline)
                {
                    var records = this.db.FetchEnvironmentRecords(environment.Timestamp);

                    foreach (var record in records)
                    {
                        if (record.Password.Strength == 0.0f)
                            record.Password = record.Password.SetStrength(PasswordStrength.EvaluatePassword(record.Password.Plaintext));
                    }

                    var statistics = PasswordStatistics.Compute(records);

                    this.AddGraphPoint(series_all, statistics[1].All, Utility.GetDate(environment.Timestamp));
                    this.AddGraphPoint(series_enabled, statistics[1].Enabled, Utility.GetDate(environment.Timestamp));
                    this.AddGraphPoint(series_disabled, statistics[1].Disabled, Utility.GetDate(environment.Timestamp));
                }

                this.evolutionGraph.Series.Add(series_all);
                this.evolutionGraph.Series.Add(series_enabled);
                this.evolutionGraph.Series.Add(series_disabled);
            }
        }

        private Series AddGraphSeries(string name)
        {
            return new Series(name)
            {
                ChartType = SeriesChartType.Spline
            };
        }

        private void AddGraphPoint(Series series, double y, string label = null)
        {
            series.Points.Add(new DataPoint()
            {
                YValues = new double[1] { y },
                Label = y.ToString("G3"),
                AxisLabel = label,

                MarkerStyle = MarkerStyle.Circle,
                MarkerColor = Color.Black
            });
        }
    }
}
