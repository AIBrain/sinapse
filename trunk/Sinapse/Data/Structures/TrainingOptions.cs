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

using Sinapse.Data;


namespace Sinapse.Data.Structures
{
    //TODO: Make this salvable together with training session

    internal enum TrainingType { ByError, ByEpoch, Manual };

    [Serializable]
    internal struct TrainingOptions
    {

        internal TrainingType TrainingType;

        internal int limEpoch;
        internal double limError;

        internal double momentum;
        internal double firstLearningRate;

        internal double? secondLearningRate;

        internal bool validateNetwork;
  //    internal bool testNetwork;

        internal TrainingVectors TrainingVectors;
        internal TrainingVectors ValidationVectors;

    }
}
