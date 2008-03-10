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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Sinapse.Data;
using Sinapse.Data.Network;
using Sinapse.Data.Structures;
using Sinapse.Forms.Dialogs;


namespace Sinapse.Forms
{

    internal sealed partial class MainForm : Form
    {

        private NetworkContainer m_networkContainer;
        private NetworkDatabase m_networkDatabase;
        private NetworkWorkplace m_networkWorkplace;


        //---------------------------------------------


        #region Constructor & Destructor
        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer,
                          true);

            this.lbVersion.Text = "v" + Application.ProductVersion;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        public NetworkContainer CurrentNetworkContainer
        {
            get { return this.m_networkContainer; }
            set
            {
                this.m_networkContainer = value;

                this.tabControlSide.NetworkContainer = value;
                this.tabControlMain.NetworkContainer = value;

                bool notNull = (value != null);

                if (notNull)
                {
                    this.lbNeuronCount.Text = m_networkContainer.Layout;

                    this.m_networkContainer.ObjectSaved += new FileSystemEventHandler(currentNetwork_NetworkSaved);

                    if (this.m_networkContainer.IsSaved)
                        this.MenuNetworkSaveAs.Enabled = true;
                }
                else
                {
                    this.lbNeuronCount.Text = "00";
                }

                this.updateButtons();
                this.btnNetworkSave.Enabled = notNull;
                this.MenuFileCloseNetwork.Enabled = notNull;
                this.MenuNetworkSave.Enabled = notNull;
                this.MenuNetworkSaveAs.Enabled = notNull;
                this.MenuNetworkWeights.Enabled = notNull;

            }
        }

        public NetworkDatabase CurrentNetworkDatabase
        {
            get { return this.m_networkDatabase; }
            set
            {
                this.m_networkDatabase = value;

                this.tabControlSide.NetworkDatabase = value;
                this.tabControlMain.NetworkDatabase = value;

                bool notNull = (value != null);

                if (notNull)
                {
                    this.lbInputCount.Text = this.m_networkDatabase.Schema.InputColumns.Length.ToString();
                    this.lbOutputCount.Text = this.m_networkDatabase.Schema.OutputColumns.Length.ToString();

                    this.m_networkDatabase.ObjectSaved += new FileSystemEventHandler(currentDatabase_DatabaseSaved);

                    if (this.m_networkDatabase.IsSaved)
                        this.MenuDatabaseSaveAs.Enabled = true;
                }
                else
                {
                    this.lbInputCount.Text = "00";
                    this.lbOutputCount.Text = "00";
                }

                this.updateButtons();
                this.btnDatabaseSave.Enabled = notNull;
                this.MenuFileCloseDatabase.Enabled = notNull;
                this.MenuDatabaseSave.Enabled = notNull;
                this.MenuDatabaseSaveAs.Enabled = notNull;
                this.MenuDatabaseEdit.Enabled = notNull;

            }
        }

        public NetworkWorkplace CurrentNetworkWorkplace
        {
            get { return this.m_networkWorkplace; }
            set
            {
                this.m_networkWorkplace = value;

                bool notNull = (value != null);

                if (notNull)
                {
                    this.m_networkWorkplace.ObjectSaved += new FileSystemEventHandler(currentWorkplace_WorkplaceSaved);
                }
                else
                {
                }

                this.MenuFileCloseWorkplace.Enabled = notNull;
            }
        }
       
        #endregion


        //---------------------------------------------


        #region MainForm Events
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.DesignMode)
            {
                // Set form Sizing and Location
                if (!Properties.Settings.Default.main_FirstLoad)
                {
                    this.Size = Properties.Settings.Default.main_Size;
                    this.Location = Properties.Settings.Default.main_Location;
                    this.WindowState = Properties.Settings.Default.main_WindowState;
                }

                // Wire up controls and events
                this.tabControlSide.TrainerControl.GraphControl = this.tabControlMain.GraphControl;
                this.tabControlMain.GraphControl.NetworkTrainer = this.tabControlSide.TrainerControl;

                this.tabControlSide.TrainerControl.StatusChanged += sideTrainerControl_StatusChanged;
                this.tabControlSide.TrainerControl.TrainingComplete += sideTrainerControl_TrainingComplete;

                this.CurrentNetworkContainer = null;
                this.CurrentNetworkDatabase = null;
                this.CurrentNetworkWorkplace = null;

                HistoryListener.Write("Waiting data");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            HistoryListener.Write("Exiting...");

            if (this.tabControlSide.TrainerControl.IsTraining)
                this.tabControlSide.TrainerControl.Stop();

            // Save settings before closing
            Properties.Settings.Default.main_FirstLoad = false;
            Properties.Settings.Default.main_WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.main_Size = this.Size;
                Properties.Settings.Default.main_Location = this.Location;
            }
            else
            {
                Properties.Settings.Default.main_Size = this.RestoreBounds.Size;
                Properties.Settings.Default.main_Location = this.RestoreBounds.Location;
            }
        }
        #endregion


        //---------------------------------------------


        #region Main Tab Control Events
        private void tabControlMain_SelectionChanged(object sender, EventArgs e)
        {
            this.statusBarControl.UpdateSelectedItems(tabControlMain.SelectedItemCount, tabControlMain.ItemCount);
        }

        private void tabControlMain_SelectedControlChanged(object sender, EventArgs e)
        {
           
        }
        #endregion


        //---------------------------------------------


        #region Network Trainer Control Events
        private void sideTrainerControl_StatusChanged(object sender, EventArgs e)
        {
            this.statusBarControl.UpdateNetworkState(this.tabControlSide.TrainerControl.NetworkState);
        }

        private void sideTrainerControl_TrainingComplete(object sender, EventArgs e)
        {
            if (MessageBox.Show("Training completed. Would you like to start querying the Network?",
                "Done", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                this.MenuNetworkQuery_Click(this, EventArgs.Empty);
            }
        }
        #endregion


        //---------------------------------------------


        #region Object Events
        private void currentNetwork_NetworkSaved(object sender, FileSystemEventArgs e)
        {
            this.openNetworkDialog.FileName = e.FullPath;
            this.saveNetworkDialog.FileName = e.FullPath;

            this.MenuNetworkSaveAs.Enabled = true;
        }

        private void currentDatabase_DatabaseSaved(object sender, FileSystemEventArgs e)
        {
            this.openDatabaseDialog.FileName = e.FullPath;
            this.saveDatabaseDialog.FileName = e.FullPath;

            this.MenuDatabaseSaveAs.Enabled = true;
        }

        private void currentWorkplace_WorkplaceSaved(object sender, FileSystemEventArgs e)
        {
        }
        #endregion


        //---------------------------------------------


        #region Menu File
        private void MenuFileWizard_Click(object sender, EventArgs e)
        {
            ImportWizard importWizard = new ImportWizard();
            if (importWizard.ShowDialog(this) == DialogResult.OK)
            {
                this.CurrentNetworkDatabase = importWizard.GetNetworkData();
                if (this.CurrentNetworkContainer == null)
                {
                    HistoryListener.Write("Database imported!");

                    if (MessageBox.Show("Data imported successfuly. Would you like to create" +
                        " the neural network now?", "Import Complete", MessageBoxButtons.YesNo)
                        == DialogResult.Yes)
                    {
                        this.MenuNetworkNew_Click(this, e);
                    }
                }
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            MenuDatabaseSave_Click(sender, e);
            MenuNetworkSave_Click(sender, e);
        }

        private void MenuFileCloseNetwork_Click(object sender, EventArgs e)
        {
            this.CurrentNetworkContainer = null;
            HistoryListener.Write("Network Closed");
        }

        private void MenuFileCloseDatabase_Click(object sender, EventArgs e)
        {
            this.CurrentNetworkDatabase = null;
            HistoryListener.Write("Database Closed");
        }

        private void MenuFileCloseWorkplace_Click(object sender, EventArgs e)
        {
            this.CurrentNetworkWorkplace = null;
            HistoryListener.Write("Workplace Closed");
        }
        #endregion


        #region Menu Database
        private void MenuDatabaseSave_Click(object sender, EventArgs e)
        {
            if (this.CurrentNetworkDatabase.IsSaved)
                this.databaseSave(this.CurrentNetworkDatabase.LastSavePath);

            else this.saveDatabaseDialog.ShowDialog(this);
        }


        private void MenuDatabaseSaveAs_Click(object sender, EventArgs e)
        {
            this.saveDatabaseDialog.ShowDialog(this);
        }


        private void MenuDatabaseOpen_Click(object sender, EventArgs e)
        {
            this.openDatabaseDialog.ShowDialog(this);
        }

        private void MenuDatabaseEdit_Click(object sender, EventArgs e)
        {

        }
        #endregion


        #region Menu Network
        private void MenuNetworkNew_Click(object sender, EventArgs e)
        {
            if (this.m_networkDatabase == null)
            {
                MessageBox.Show("Please import or create a database schema before creating the Network.");
                return;
            }

            if (this.CurrentNetworkContainer == null || 
                (this.CurrentNetworkContainer != null &&
                MessageBox.Show("Would you like to overwrite your current network?" +
                                "\nAny unsaved training sessions will be lost",
                                "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                NetworkCreationDialog creationDlg = new NetworkCreationDialog(m_networkDatabase.Schema);
                if (creationDlg.ShowDialog(this) == DialogResult.OK)
                {
                    this.CurrentNetworkContainer = creationDlg.CreateNetworkContainer();
                    HistoryListener.Write("Network created!");

                    if (Properties.Settings.Default.main_AutoSwitchToTrainingTab)
                    {
                        this.tabControlSide.TrainerControl.ShowTab();
                    }
                }
            }
        }

        private void MenuNetworkSave_Click(object sender, EventArgs e)
        {
            if (this.CurrentNetworkContainer.IsSaved)
                this.networkSave(this.CurrentNetworkContainer.LastSavePath);

            else this.saveNetworkDialog.ShowDialog(this);
        }


        private void MenuNetworkSaveAs_Click(object sender, EventArgs e)
        {
            this.saveNetworkDialog.ShowDialog(this);
        }


        private void MenuNetworkOpen_Click(object sender, EventArgs e)
        {
            this.openNetworkDialog.ShowDialog(this);
        }

        private void MenuNetworkQuery_Click(object sender, EventArgs e)
        {
            this.tabControlMain.QueryControl.ShowTab();
        }

        private void MenuNetworkShowWeight_Click(object sender, EventArgs e)
        {
            new NetworkInspectorDialog(CurrentNetworkContainer).ShowDialog(this);
        }
        #endregion


        #region Menu Help
        private void MenuHelpAbout_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void MenuHelpContents_Click(object sender, EventArgs e)
        {

        }
        #endregion


        //---------------------------------------------


        #region ToolStripMenu Training
        private void btnTrainStart_Click(object sender, EventArgs e)
        {
            this.tabControlSide.TrainerControl.Start();
        }

        private void btnTrainStop_Click(object sender, EventArgs e)
        {
            this.tabControlSide.TrainerControl.Stop();
        }

        private void btnTrainPause_Click(object sender, EventArgs e)
        {
            this.tabControlSide.TrainerControl.Pause();
        }

        private void btnTrainForget_Click(object sender, EventArgs e)
        {
            this.tabControlSide.TrainerControl.Forget();
        }

        private void btnTrainNext_Click(object sender, EventArgs e)
        {
            //   this.tabControlSide.TrainerControl.Start();
        }

        private void btnTrainBack_Click(object sender, EventArgs e)
        {

        }

        private void btnTrainGraph_Click(object sender, EventArgs e)
        {
            this.tabControlMain.GraphControl.ShowTab();
        }
        #endregion


        #region ToolStripMenu Testing
        private void btnTestCompute_Click(object sender, EventArgs e)
        {
            if (this.tabControlMain.SelectedControl != this.tabControlMain.TestingSetControl)
                this.tabControlMain.TestingSetControl.ShowTab();

            this.tabControlMain.TestingSetControl.Compute();
        }

        private void btnTestReport_Click(object sender, EventArgs e)
        {
            this.tabControlMain.TestingSetControl.Compare();
        }

        private void btnTestRound_Click(object sender, EventArgs e)
        {
            if (this.tabControlMain.SelectedControl != this.tabControlMain.TestingSetControl)
                this.tabControlMain.TestingSetControl.ShowTab();

            ToolStripMenuItem item = (sender as ToolStripMenuItem);
            
            if (item.Tag is Single)
            {
                float value = (float)item.Tag;

                if (this.tabControlMain.SelectedControl is Controls.MainTabControl.TabPageTesting)
                    this.tabControlMain.TestingSetControl.NetworkDatabase.Round(true, value);
                else if (this.tabControlMain.SelectedControl is Controls.MainTabControl.TabPageQuery)
                    this.tabControlMain.QueryControl.NetworkDatabase.Round(false, value);
            }
        }
        #endregion


        //---------------------------------------------


        #region File Dialogs
        private void openNetworkDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.networkOpen(openNetworkDialog.FileName);
        }

        private void saveNetworkDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.networkSave(saveNetworkDialog.FileName);
        }

        private void openDatabaseDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.databaseOpen(openDatabaseDialog.FileName);
        }

        private void saveDatabaseDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.databaseSave(saveDatabaseDialog.FileName);
        }

        private void mruProviderDatabase_MenuItemClicked(string filename)
        {
            this.databaseOpen(filename);
        }

        private void mruProviderNetwork_MenuItemClicked(string filename)
        {
            this.networkOpen(filename);
        }
        #endregion


        //---------------------------------------------


        #region Open & Save Network
        private void networkSave(string path)
        {
            try
            {
                NetworkContainer.Serialize(this.CurrentNetworkContainer, path);
                this.mruProviderNetwork.Insert(path);
                HistoryListener.Write("Network Saved");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error saving network");
            }
        }

        private void networkOpen(string path)
        {
            NetworkContainer neuralNetwork = null;

            try
            {
                neuralNetwork = NetworkContainer.Deserialize(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error opening network");
            }
            finally
            {
                if (neuralNetwork != null)
                {
                    this.CurrentNetworkContainer = neuralNetwork;
                    this.mruProviderNetwork.Insert(path);
                    HistoryListener.Write("Network Loaded");

                    if (m_networkDatabase == null && File.Exists(Path.ChangeExtension(path,".sdo")))
                    {
                        if (MessageBox.Show("Sinapse detected a database with the same name as this network. " +
                            "Would you like to load this database too?", "Matching database found", MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                        {
                            databaseOpen(Path.ChangeExtension(path, ".sdo"));
                        }
                    }
                }
            }
        }
        #endregion


        #region Open & Save Database
        private void databaseSave(string path)
        {
            try
            {
                NetworkDatabase.Serialize(this.CurrentNetworkDatabase, path);
                this.mruProviderDatabase.Insert(path);
                HistoryListener.Write("Database Saved");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error saving database");
#if DEBUG
                throw e;
#endif
            }
         
        }

        private void databaseOpen(string path)
        {
            NetworkDatabase networkDatabase = null;

            try
            {
                networkDatabase = NetworkDatabase.Deserialize(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error opening database");
#if DEBUG
                throw e;
#endif
            }
            finally
            {
                if (networkDatabase != null)
                {
                    this.CurrentNetworkDatabase = networkDatabase;
                    this.mruProviderDatabase.Insert(path);
                    HistoryListener.Write("Database Loaded");

                    if (m_networkContainer == null && File.Exists(Path.ChangeExtension(path, ".ann")))
                    {
                        if (MessageBox.Show("Sinapse detected a network with the same name as this database. " +
                            "Would you like to load this network too?", "Matching network found", MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                        {
                            networkOpen(Path.ChangeExtension(path, ".ann"));
                        }
                    }
                }
            }
        }
        #endregion


        #region Open & Save Workplace
        private void workplaceSave(string path)
        {
            try
            {
                NetworkWorkplace.Serialize(this.CurrentNetworkWorkplace, path);
                this.mruProviderWorkplace.Insert(path);
                HistoryListener.Write("Workplace Saved");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error saving workplace");
#if DEBUG
                throw e;
#endif
            }

        }

        private void workplaceOpen(string path)
        {
            NetworkWorkplace networkWorkplace = null;

            try
            {
                networkWorkplace = NetworkWorkplace.Deserialize(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error opening workplace");
#if DEBUG
                throw e;
#endif
            }
            finally
            {
                if (networkWorkplace != null)
                {
                    this.CurrentNetworkWorkplace = networkWorkplace;
                    this.mruProviderWorkplace.Insert(path);
                    HistoryListener.Write("Workplace Loaded");
                }
            }
        }
     
        #endregion


        //---------------------------------------------


        #region Private Methods
        private void updateButtons()
        {
            if (this.m_networkDatabase != null && m_networkContainer != null)
            {
                this.toolStripTraining.Enabled = true;
                this.toolStripTesting.Enabled = true;
            }
            else
            {
                this.toolStripTraining.Enabled = false;
                this.toolStripTesting.Enabled = false;
            }
        }
        #endregion


    }
}