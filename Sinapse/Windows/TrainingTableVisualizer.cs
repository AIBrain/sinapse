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

namespace Sinapse.Windows
{
    public partial class TrainingTableVisualizer : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        TableDataSource dataSource;
        AdaptiveSystem adaptiveSystem;

        public TrainingTableVisualizer()
        {
            InitializeComponent();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            // Lembrete: O usu�rio nao deve poder alterar a tabela nesta janela, que
            //  � apenas um visualizador. Ent�o devemos usar c�pias de DataTables imut�veis
            
            int inputCount  = dataSource.Columns.Count(TableDataSourceColumn.ColumnRole.Input);
            int outputCount = dataSource.Columns.Count(TableDataSourceColumn.ColumnRole.Output);

            dataSource.GetData(

                outputs = adaptiveSystem.Compute(inputs);

                // Copiar o vetor de sa�da para a linha


            }

        }
    }
}