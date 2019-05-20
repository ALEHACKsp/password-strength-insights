using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using PsiDatabase;

namespace PsiUserInterface
{
    public partial class Form1 : Form
    {
        private DatabaseClient db = new DatabaseClient();

        private Size ResizeDelta;
        public const string padding_spaces = "     ";

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public Form1()
        {
            InitializeComponent();

            if (!this.db.Connect())
                MessageBox.Show("Failed to connect to the PSI database.");

            const int TCM_SETMINTABWIDTH = 0x1300 + 49;

            SendMessage(this.environmentTabControl.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)24);
            SendMessage(this.accountTabControl.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)24);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CreateTabPage(this.environmentTabControl);
            this.CreateTabPage(this.accountTabControl);
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            this.ResizeDelta = ((Control)sender).Size;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Size delta = ((Control)sender).Size - this.ResizeDelta;

            for (int i = 0; i < this.environmentTabControl.TabPages.Count - 1; i++)
            {
                if (this.environmentTabControl.TabPages[i].Controls.Count > 0)
                    this.environmentTabControl.TabPages[i].Controls[0].Size += delta;
            }

            for (int i = 0; i < this.accountTabControl.TabPages.Count - 1; i++)
            {
                if (this.accountTabControl.TabPages[i].Controls.Count > 0)
                    this.accountTabControl.TabPages[i].Controls[0].Size += delta;
            }

            this.ResizeDelta += delta;
        }

        private int OffsetPosX(Rectangle parent, int distance)
        {
            return (parent.Right - distance);
        }

        private int CenterPosX(Rectangle parent, int distance)
        {
            return (parent.Left + 2 + ((parent.Width - distance) / 2));
        }

        private int CenterPosY(Rectangle parent, int distance)
        {
            return (parent.Top + 1 + ((parent.Height - distance) / 2));
        }

        private void DrawDefaults(TabControl control, DrawItemEventArgs e)
        {
            VisualStyleRenderer render = null;

            if (e.State == DrawItemState.Selected)
            {
                if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.Tab.TabItem.Pressed))
                    render = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Pressed);
            }
            else
            {
                if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.Tab.TabItem.Normal))
                    render = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Normal);
            }

            if (render != null)
            {
                render.DrawBackground(e.Graphics, e.Bounds);
                render.DrawText(e.Graphics, e.Bounds, control.TabPages[e.Index].Text, false, TextFormatFlags.SingleLine | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var control = (TabControl)sender;
            var tab_rect = control.GetTabRect(e.Index);

            this.DrawDefaults(control, e);

            if (e.Index == control.TabCount - 1)
            {
                e.Graphics.DrawImage(Resources.AddImage,
                    CenterPosX(tab_rect, Resources.AddImage.Width),
                    CenterPosY(tab_rect, Resources.AddImage.Height));
            }
            else
            {
                e.Graphics.DrawImage(Resources.CloseImage,
                    OffsetPosX(tab_rect, Resources.CloseImage.Width),
                    CenterPosY(tab_rect, Resources.CloseImage.Height));
            }

            e.DrawFocusRectangle();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl)sender;

            if (control.SelectedIndex == control.TabCount - 1)
                CreateTabPage(control);
        }

        private void TabControl_MouseClick(object sender, MouseEventArgs e)
        {
            TabControl control = (TabControl)sender;

            if (control.SelectedIndex != control.TabPages.Count - 1)
            {
                var tab_rect = control.GetTabRect(control.SelectedIndex);

                var close_rect = new Rectangle(
                    OffsetPosX(tab_rect, Resources.CloseImage.Width),
                    CenterPosY(tab_rect, Resources.CloseImage.Height),
                    16, 16);

                if (close_rect.Contains(e.Location))
                    control.TabPages.Remove(control.SelectedTab);
            }
        }

        private void CreateTabPage(TabControl control)
        {
            var page = new TabPage("New Tab" + padding_spaces);

            if (control == this.environmentTabControl)
                page.Controls.Add(new EnvironmentTabPage(page, control.DisplayRectangle, db));
            else if (control == this.accountTabControl)
                page.Controls.Add(new AccountTabPage(page, control.DisplayRectangle, db));

            int index = control.TabPages.Count - 1;

            control.TabPages.Insert(index, page);
            control.SelectTab(index);
        }
    }
}
