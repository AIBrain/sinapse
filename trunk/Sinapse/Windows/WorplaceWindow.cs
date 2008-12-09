/***************************************************************************
 *   Sinapse Neural Networking Tool         http://sinapse.googlecode.com  *
 *  ---------------------------------------------------------------------- *
 *   Copyright (C) 2006-2008 Cesar Roberto de Souza <cesarsouza@gmail.com> *
 *                                                                         *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 3 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sinapse.Core;
using Sinapse.Core.Systems;
using Sinapse.Core.Sources;
using Sinapse.Core.Training;

namespace Sinapse.Windows
{

    internal sealed partial class WorkplaceWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        private TreeNode nodeSources;
        private TreeNode nodeSystems;
        private TreeNode nodeTraining;
        private TreeNode rootWorkplace;

        public event WorkplaceContentDoubleClickedEventHandler WorkplaceContentDoubleClicked;


        #region Constructor
        public WorkplaceWindow()
        {
            InitializeComponent();

            nodeSources = new System.Windows.Forms.TreeNode("Sources", 1, 1);
            nodeSources.ContextMenuStrip = this.menuSource;

            nodeSystems = new System.Windows.Forms.TreeNode("Systems", 1, 1);
            nodeSystems.ContextMenuStrip = this.menuSystem;

            nodeTraining = new System.Windows.Forms.TreeNode("Trainings", 1, 1);
            nodeTraining.ContextMenuStrip = this.menuTraining;

            rootWorkplace = new System.Windows.Forms.TreeNode("Workplace", new System.Windows.Forms.TreeNode[] {
                nodeSources, nodeSystems, nodeTraining});


            Workplace.ActiveWorkplaceChanged += new EventHandler(Workplace_ActiveWorkplaceChanged);
        }
        #endregion


        #region Properties
        public WorkplaceContent SelectedItem
        {
            get
            {
                if (this.treeViewWorkplace.SelectedNode != null)
                {
                    object tag = this.treeViewWorkplace.SelectedNode.Tag;

                    if (tag is WorkplaceContent)
                        return tag as WorkplaceContent;
                }
                return null;
            }
        }

        public event TreeViewEventHandler SelectionChanged
        {
            add { this.treeViewWorkplace.AfterSelect += value; }
            remove { this.treeViewWorkplace.AfterSelect -= value; }
        }

        public new event EventHandler DoubleClick
        {
            add { this.treeViewWorkplace.DoubleClick += value; }
            remove { this.treeViewWorkplace.DoubleClick -= value; }
        }
        #endregion


        #region Form Events
        #endregion

        #region Workplace Events
        private void Workplace_ActiveWorkplaceChanged(object sender, EventArgs e)
        {
            // If we have an active workplace,
            if (Workplace.Active != null)
            {
                // Lets hide the "Nothing to show" label and populate the tree view
                this.lbNothingToShow.Hide();
                this.treeViewWorkplace.Enabled = true;
                this.populateTreeView();

                // And also register the events for the new workplace
                Workplace.Active.DataSources.ListChanged += new ListChangedEventHandler(Workplace_ContentChanged);
                Workplace.Active.TrainingSessions.ListChanged += new ListChangedEventHandler(Workplace_ContentChanged);
                Workplace.Active.AdaptiveSystems.ListChanged += new ListChangedEventHandler(Workplace_ContentChanged);
            }
            else
            {
                // Otherwise, we hide everything and show a "Nothing to show"
                //   text label on the middle of the control.
                this.treeViewWorkplace.Nodes.Clear();
                this.treeViewWorkplace.Enabled = false;
                this.lbNothingToShow.Show();
            }
        }


        private void Workplace_ContentChanged(object sender, ListChangedEventArgs e)
        {
            this.populateTreeView();
        }
        #endregion


        #region TreeView Methods
        private void populateTreeView()
        {
            this.treeViewWorkplace.SuspendLayout();
            this.treeViewWorkplace.Nodes.Clear();
            TreeNode node;

            foreach (NetworkSystem system in Workplace.Active.AdaptiveSystems)
            {
                node = new TreeNode(system.Name, 1, 1);
                node.Tag = system;
                nodeSystems.Nodes.Add(node);
            }
            foreach (DataSource source in Workplace.Active.DataSources)
            {
                node = new TreeNode(source.Name, 1, 1);
                node.Tag = source;
                nodeSources.Nodes.Add(node);
            }
            foreach (TrainingSession session in Workplace.Active.TrainingSessions)
            {
                node = new TreeNode(session.Name, 1, 1);
                node.Tag = session;
                nodeTraining.Nodes.Add(node);
            }

            this.treeViewWorkplace.Nodes.Add(rootWorkplace);
            this.treeViewWorkplace.ExpandAll();
            this.treeViewWorkplace.ResumeLayout(true);
        }
        #endregion



        private void treeViewWorkplace_DoubleClick(object sender, EventArgs e)
        {
            object tag = this.treeViewWorkplace.SelectedNode.Tag;

            if (tag is WorkplaceContent)
                this.OnWorkplaceContentDoubleClicked(
                    new WorkplaceContentDoubleClickedEventArgs(tag as WorkplaceContent));
        }

        private void treeViewWorkplace_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void OnWorkplaceContentDoubleClicked(WorkplaceContentDoubleClickedEventArgs e)
        {
            if (this.WorkplaceContentDoubleClicked != null)
                this.WorkplaceContentDoubleClicked.Invoke(this, e);
        }

        #region Menu Events
        private void menuSourceAddTable_Click(object sender, EventArgs e)
        {
            TableDataSource item = new TableDataSource("TableDataSource");
            Workplace.Active.DataSources.Add(item);

            this.OnWorkplaceContentDoubleClicked(new WorkplaceContentDoubleClickedEventArgs(item));
        }

        private void menuSystemAddNetworkActivation_Click(object sender, EventArgs e)
        {
            ActivationNetworkSystem item = new ActivationNetworkSystem();
            Workplace.Active.AdaptiveSystems.Add(item);

            // Update
            this.OnWorkplaceContentDoubleClicked(new WorkplaceContentDoubleClickedEventArgs(item));
        }

        private void menuSystemAddNetworkDistance_Click(object sender, EventArgs e)
        {

        }
        #endregion


    }

    #region Event Delegates & Arguments
    internal delegate void WorkplaceContentDoubleClickedEventHandler(object sender, WorkplaceContentDoubleClickedEventArgs e);
    internal sealed class WorkplaceContentDoubleClickedEventArgs : EventArgs
    {
        private WorkplaceContent workplaceContent;

        public WorkplaceContent WorkplaceContent
        {
            get { return workplaceContent; }
            set { workplaceContent = value; }
        }
        public WorkplaceContentDoubleClickedEventArgs(WorkplaceContent item)
        {
            this.workplaceContent = item;
        }
    }
    #endregion

}
