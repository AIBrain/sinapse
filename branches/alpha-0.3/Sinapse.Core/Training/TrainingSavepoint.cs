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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using AForge.Neuro;

using Sinapse.Core;
using Sinapse.Core.Networks;


namespace Sinapse.Core.Training
{
    /// <summary>
    /// Stores a network state
    /// </summary>
    [Serializable]
    public sealed class TrainingSavepoint
    {

        private MemoryStream m_memoryStream;
        private TrainingStatus m_networkStatus;
        private DateTime m_creationTime;


        //---------------------------------------------


        #region Constructor
        public TrainingSavepoint(Network network, TrainingStatus networkStatus)
        {
            this.m_memoryStream = new MemoryStream();

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(m_memoryStream, network);
            this.m_memoryStream.Seek(0, SeekOrigin.Begin);

            this.m_networkStatus = networkStatus;
            this.m_creationTime = DateTime.Now;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        public Network Network
        {
            get
            {
                BinaryFormatter bf = new BinaryFormatter();
                m_memoryStream.Seek(0, SeekOrigin.Begin);
                ActivationNetwork network = bf.Deserialize(m_memoryStream) as ActivationNetwork;
                return network;
            }
        }

        public TrainingStatus NetworkStatus
        {
            get { return this.m_networkStatus; }
            set { this.m_networkStatus = value; }
        }

        public DateTime CreationTime
        {
            get { return this.m_creationTime; }
        }

        #endregion


        //---------------------------------------------


        #region Private Methods
        #endregion


    }



    [Serializable]
    internal sealed class TrainingSavepointCollection : System.ComponentModel.BindingList<TrainingSavepoint>
    {

        private TrainingSavepoint m_currentSavepoint;
        private ActivationNetworkContainer m_networkContainer;

        [NonSerialized]
        public EventHandler CurrentChanged;

        [NonSerialized]
        public EventHandler SavepointRegistered;

        [NonSerialized]
        public EventHandler SavepointRestored;


        //---------------------------------------------


        #region Constructor
        public TrainingSavepointCollection(ActivationNetworkContainer networkContainer)
        {
            this.m_networkContainer = networkContainer;
            this.m_currentSavepoint = null;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        public TrainingSavepoint Current
        {
            get { return this.m_currentSavepoint; }
        }

        public TrainingSavepoint Optimal
        {
            get
            {
                TrainingSavepoint bestSavepoint = null;

                foreach (TrainingSavepoint sp in this)
                {
                    if (bestSavepoint == null /*|| sp.ErrorValidation <= bestSavepoint.ErrorValidation*/)
                        bestSavepoint = sp;
                }

                return bestSavepoint;
            }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        public void Restore(TrainingSavepoint networkSavepoint)
        {
            this.m_currentSavepoint = networkSavepoint;

            this.OnCurrentSavepointChanged();
            this.OnSavepointRestored();
        }

        public void Add(TrainingStatus trainingStatus)
        {
            this.m_currentSavepoint = new TrainingSavepoint(m_networkContainer.Network, trainingStatus);
            this.Add(m_currentSavepoint);

            this.OnCurrentSavepointChanged();
            this.OnSavepointRegistered();
        }
        #endregion


        //---------------------------------------------


        #region Private Methods
        private void OnCurrentSavepointChanged()
        {
            if (this.CurrentChanged != null)
                this.CurrentChanged.Invoke(this, EventArgs.Empty);
        }

        private void OnSavepointRegistered()
        {
            if (this.SavepointRegistered != null)
                this.SavepointRegistered.Invoke(this, EventArgs.Empty);
        }

        private void OnSavepointRestored()
        {
            if (this.SavepointRestored != null)
                this.SavepointRestored.Invoke(this, EventArgs.Empty);
        }
        #endregion


    }    

}