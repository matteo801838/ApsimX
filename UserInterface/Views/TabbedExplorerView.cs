﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UserInterface.Commands;

namespace UserInterface.Views
{
    interface ITabbedExplorerView
    {
        event EventHandler<PopulateStartPageArgs> PopulateStartPage;

        /// <summary>
        /// Add a tab form to the tab control. Optionally select the tab if SelectTab is true.
        /// </summary>
        void AddTab(string TabText, Image TabImage, UserControl Contents, bool SelectTab);

        /// <summary>
        /// Ask user for a filename.
        /// </summary>
        string AskUserForFileName();
    }


    /// <summary>
    /// TabbedExplorerView maintains multiple explorer views in a tabbed interface. It also
    /// has a StartPageView that is shown to the use when they open a new tab.
    /// </summary>
    public partial class TabbedExplorerView : UserControl, ITabbedExplorerView
    {
        public event EventHandler<PopulateStartPageArgs> PopulateStartPage;

        /// <summary>
        /// Constructor
        /// </summary>
        public TabbedExplorerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// View has loaded
        /// </summary>
        private void OnLoad(object sender, EventArgs e)
        {
            if (PopulateStartPage != null)
                PopulateStartPageList();
        }

        /// <summary>
        /// Populate the start page.
        /// </summary>
        private void PopulateStartPageList()
        {
            PopulateStartPageArgs Args = new PopulateStartPageArgs();
            PopulateStartPage(this, Args);
            ListView.Items.Clear();
            foreach (PopulateStartPageArgs.Description Description in Args.Descriptions)
            {
                ListViewItem Item = new ListViewItem();
                Item.Text = Description.Name;
                Item.Tag = Description.OnClick;
                Item.ToolTipText = "Double click to open";

                // Load image
                int ImageIndex = ListViewImages.Images.IndexOfKey(Description.ResourceNameForImage);
                if (ImageIndex == -1)
                {
                    Bitmap Icon = Properties.Resources.ResourceManager.GetObject(Description.ResourceNameForImage) as Bitmap;
                    if (Icon != null)
                    {
                        ListViewImages.Images.Add(Description.ResourceNameForImage, Icon);
                        ImageIndex = ListViewImages.Images.Count - 1;
                    }
                }
                Item.ImageIndex = ImageIndex;
                ListView.Items.Add(Item);
            }
        }
        /// <summary>
        /// User has double clicked. Open. Open a .apsim file.
        /// </summary>
        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            EventHandler OnClick = ListView.SelectedItems[0].Tag as EventHandler;
            if (OnClick != null)
                OnClick(this, e);
        }
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ListView_DoubleClick(sender, e);
        }
        /// <summary>
        /// Tab popup menu is about to open. Enable/disable the close menu item.
        /// </summary>
        private void OnTabControlMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                for (int i = 0; i < TabControl.TabCount; ++i)
                {
                    Rectangle r = TabControl.GetTabRect(i);
                    if (r.Contains(e.Location) /* && it is the header that was clicked*/)
                    {
                        TabControl.SelectedTab = TabControl.TabPages[i];
                        CloseTabMenuItem.Enabled = TabControl.SelectedTab.Text != " ";
                        TabPopupMenu.Show(this, e.Location);

                    }
                }
            }
        }

        /// <summary>
        /// User is closing this tab.
        /// </summary>
        private void OnCloseTabClick(object sender, EventArgs e)
        {
            if (TabControl.SelectedTab.Text != " ")
                TabControl.TabPages.Remove(TabControl.SelectedTab);
        }

        /// <summary>
        /// Add a tab form to the tab control. Optionally select the tab if SelectTab is true.
        /// </summary>
        public void AddTab(string TabText, Image TabImage, UserControl Contents, bool SelectTab)
        {
            TabPage OriginalTab = TabControl.SelectedTab;
            TabPage NewTabPage = new TabPage();
            int i = TabControl.TabPages.Count - 1;
            TabControl.TabPages.Insert(i, NewTabPage);
            SetupTab(NewTabPage, TabText, TabImage, Contents);
            if (SelectTab)
            {
                // On MONO OSX: The screen doesn't redraw properly when a tab is 'inserted' in the
                // tab pages collections. The 3 lines below seem to work.
                TabControl.SelectedTab = NewTabPage;
                TabControl.SelectedTab = OriginalTab;
                TabControl.SelectedTab = NewTabPage;
            }
        }

        /// <summary>
        /// Replace the current focused tab with the specified text, image and contents.
        /// </summary>
        public void ReplaceCurrentTab(string TabText, Image TabImage, UserControl Contents)
        {
            TabPage TabPage = TabControl.SelectedTab;
            SetupTab(TabPage, TabText, TabImage, Contents);
        }

        /// <summary>
        /// Remove a tab from the tab control.
        /// </summary>
        public void RemoveTab(string TabText)
        {
            TabControl.TabPages.RemoveByKey(TabText);
        }

        /// <summary>
        /// Setup a tab according to the specified parameters.
        /// </summary>
        private void SetupTab(TabPage TabPage, string TabText, Image TabImage, UserControl Contents)
        {
            // If the tab text passed in is a filename then only show the filename (no path)
            // on the tab. The ToolTipText will still have the f ull path and name.
            if (TabText.Contains(Path.DirectorySeparatorChar.ToString()))
                TabPage.Text = Path.GetFileNameWithoutExtension(TabText);
            else
                TabPage.Text = TabText;
            TabPage.Name = TabText;
            TabPage.ToolTipText = TabText;

            // Add the specified tab image to the image list if it doesn't already exist.
            if (TabImage != null)
            {
                int TabIndex;
                for (TabIndex = 0; TabIndex < TabImageList.Images.Count; TabIndex++)
                    if (TabIndex.Equals(TabImage))
                        break;
                if (TabIndex == TabImageList.Images.Count)
                    TabImageList.Images.Add(TabImage);
                TabPage.ImageIndex = TabIndex;
            }
            else
                TabPage.ImageIndex = -1;

            // Add the TabForm passed in to the new tab page.
            TabPage.Controls.Clear();
            TabPage.Controls.Add(Contents);
            Contents.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Ask user for a filename.
        /// </summary>
        public string AskUserForFileName()
        {
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                return OpenFileDialog.FileName;
            return null;
        }
    }

    public class PopulateStartPageArgs : EventArgs
    {
        public struct Description
        {
            public string Name;
            public string ResourceNameForImage;
            public EventHandler OnClick;
        }
        
        public List<Description> Descriptions = new List<Description>();
    }

}