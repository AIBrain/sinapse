using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Sinapse.Core.Training;

using WeifenLuo.WinFormsUI.Docking;

namespace Sinapse.Documents
{
    public partial class AdaptativeSystemTrainer :  WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public AdaptativeSystemTrainer(TrainingSession session)
        {
            InitializeComponent();
        }
    }
}