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
using System.Text;

using Sinapse.Data.Network;

namespace Sinapse.Data.CodeGeneration
{
    internal abstract class CodeGenerator
    {

        private NetworkContainer m_network;

        //---------------------------------------------


        #region Constructor
        protected CodeGenerator(NetworkContainer network)
	    {
            this.m_network = network;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        public NetworkContainer Network
        {
            get { return m_network; }
            set { m_network = value; }
        }
        #endregion


        //---------------------------------------------


        #region Abstract Methods
        protected abstract void build(StringBuilder codeBuilder);
        #endregion

        #region Protected Methods
        protected string disclaimer(string commentChar)
        {
            StringBuilder sB = new StringBuilder();
            sB.AppendFormat("{0}  Code generated by Sinapse Neural Networking Tool in {0}\n", DateTime.Now);
            sB.AppendFormat("{0} -----------------------------------------------------------------------------\n");
            sB.AppendFormat("{0}\n");
            sB.AppendFormat("{0}  You are free to use this code for any purpose you wish, in any application\n");
            sB.AppendFormat("{0}  under any licensing terms, but only if you add a user-visible reference to\n");
            sB.AppendFormat("{0}  the use of Sinapse inside your program and don't separate the generated code\n");
            sB.AppendFormat("{0}   from this disclaimer. Also, please pay attention to the following notice:\n");
            sB.AppendFormat("{0}\n");
            sB.AppendFormat("{0}      This code was generated in the hope that it will be useful,\n");
            sB.AppendFormat("{0}      but WITHOUT ANY WARRANTY; without even the implied warranty\n");
            sB.AppendFormat("{0}      of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.\n");
            sB.AppendFormat("{0}\n");
            sB.AppendFormat("{0}      Sinapse developer(s) are not and cannot be liable for any direct,\n");
            sB.AppendFormat("{0}      indirect, incidental, special, exemplary or consequential damages,\n");
            sB.AppendFormat("{0}      including, but not limited to, procurement of substitute goods or\n");
            sB.AppendFormat("{0}      services; loss of use, data, profits or business interruption.\n");
            sB.AppendFormat("\n");

            return sB.ToString();
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        public string Generate()
        {
            StringBuilder codeBuilder = new StringBuilder();
            this.build(codeBuilder);
            return codeBuilder.ToString();
        }

        public void Save(string path)
        {
        }
        #endregion


    }
}