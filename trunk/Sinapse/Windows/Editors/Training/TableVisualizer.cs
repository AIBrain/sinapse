using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Sinapse.Core;
using Sinapse.Core.Training;
using Sinapse.Core.Sources;
using Sinapse.Core.Systems;

namespace Sinapse.WinForms
{
    public partial class TrainingTableVisualizer : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        TableDataSource dataSource;
        AdaptiveSystem adaptiveSystem;
        DataView currentView;

        public TrainingTableVisualizer()
        {
            InitializeComponent();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            // Lembrete: O usu�rio nao deve poder alterar a tabela nesta janela, que
            //  � apenas um visualizador. Ent�o devemos usar c�pias de DataTables imut�veis

            int inputCount = dataSource.Columns.GetCount(DataSourceRole.Input);
            int outputCount = dataSource.Columns.GetCount(DataSourceRole.Output);



            foreach (DataRow row in currentView.Table.Rows)
            {
                object[] inputs = dataSource.GetData(row, DataSourceRole.Input);
                object[] outputs = dataSource.GetData(row, DataSourceRole.Output);
                double[] rawOutputs;
                double[] deviations;

                adaptiveSystem.Test(inputs, outputs, out rawOutputs, out deviations);

                dataSource.SetData(row, DataSourceRole.Input, inputs);
                dataSource.SetData(row, DataSourceRole.Output, outputs);

            }


        }


    }
}