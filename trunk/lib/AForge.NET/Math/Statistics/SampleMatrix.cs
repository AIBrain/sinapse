/***************************************************************************
 *                                                                         *
 *  Copyright (C) 2006-2008 Cesar Roberto de Souza <cesarsouza@gmail.com>  *
 *                                                                         *
 *  Please note that this code is not part of the original AForge.NET      *
 *  library. This extension was created to support new features needed by  *
 *  Sinapse, a neural networking tool software. Unless otherwise advised,  *
 *  this code relies under protection of the GNU General Public License v3 *
 *                                                                         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using AForge.Math;


namespace AForge.Statistics
{

    /// <summary>
    ///     SampleMatrix provides operations of sample statistics
    /// </summary>
    /// <remarks>
    ///     In statistics, a sample is a subset of a population.
    /// </remarks>
    public class SampleMatrix : AForge.Math.Matrix
    {

        public enum DataModel { RowsAsObservations, ColumnsAsObservations };

        private string[] m_colNames;
        private string m_title;


        //---------------------------------------------


        #region Constructors
        /// <summary>
        /// Creates a new SampleMatrix object, where each columns corresponds
        /// to a variable and each row to an observation of those variables.
        /// </summary>
        /// <param name="matrix">The base matrix</param>
        /// <param name="model">The matrix model</param>
        public SampleMatrix(double[][] matrix, DataModel model)
            : base(matrix)
        {
            if (model == DataModel.ColumnsAsObservations)
                this.Array = (double[][])this.Transpose();

            this.generateDefaultNames();
        }

        public SampleMatrix(int rows, int columns)
            : base(rows, columns)
        {
            this.generateDefaultNames();
        }

        public SampleMatrix(System.Data.DataTable dataTable) : base (dataTable.Rows.Count, dataTable.Columns.Count)
        {
            dataTable.AcceptChanges();

            this.m_colNames = new string[dataTable.Columns.Count];
            
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    if (dataTable.Columns[j].DataType == typeof(System.String))
                    {
                        this.Array[i][j] = Double.Parse((string)dataTable.Rows[i][j]);
                    }
                    else
                    {
                        this.Array[i][j] = (Double)dataTable.Rows[i][j];
                    }
                    this.ColumnNames[j] = dataTable.Columns[j].Caption;
                }
            }
        }

        /// <summary>
        /// Creates a new SampleMatrix object, where each columns corresponds
        /// to a variable and each row to an observation of those variables.
        /// </summary>
        /// <param name="matrix">The base matrix</param>
        /// <param name="model">The matrix model</param>
        public SampleMatrix(double[,] matrix, DataModel model)
            : base(matrix)
        {
            if (model == DataModel.ColumnsAsObservations)
                this.Array = (double[][])this.Transpose();

            this.generateDefaultNames();
        }

        /// <summary>
        /// Creates a new SampleMatrix object, where each columns corresponds
        /// to a variable and each row to an observation of those variables.
        /// </summary>
        /// <param name="matrix">The base matrix</param>
        /// <param name="model">The matrix model</param>
        public SampleMatrix(Matrix matrix, DataModel model)
            : base(matrix)
        {
            if (model == DataModel.ColumnsAsObservations)
                this.Array = (double[][])this.Transpose();

            this.generateDefaultNames();
        }

        public SampleMatrix(Matrix matrix)
            : this(matrix, DataModel.RowsAsObservations)
        {
        }

        /// <summary>
        /// Creates a new SampleMatrix object, where each columns corresponds
        /// to a variable and each row to an observation of those variables.
        /// </summary>
        /// <param name="matrix">The matrix to be copied.</param>
        public SampleMatrix(SampleMatrix matrix)
            : base((Matrix)matrix)
        {
            this.m_colNames = matrix.m_colNames;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        /// <summary>The number of observations contained on this sample</summary>
        public int Observations
        {
            get { return base.Rows; }
        }

        /// <summary>The number of variables (data dimension) of this sample</summary>
        public int Variables
        {
            get { return base.Columns; }
        }

        /// <summary>The column (variables) labels of this sample.</summary>
        public string[] ColumnNames
        {
            get { return this.m_colNames; }
        }

        /// <summary>Retrieves a item from the sample</summary>
        /// <param name="variable">The variable index of the requested item</param>
        /// <param name="observation">The observation index of the requested item</param>
        public new double this[int variable, int observation]
        {
            get
            {
                // Always consider columns as variables, rows as observations
                return base[observation, variable];
            }
            set
            {
                base[observation, variable] = value;
            }
        }

        /// <summary>Retrieves a item from the sample</summary>
        /// <param name="variable">The variable label of the requested item</param>
        /// <param name="observation">The observation index of the requested item</param>
        public double this[string variable, int observation]
        {
            get
            {
                int index = System.Array.IndexOf(this.m_colNames, variable);
                return this[index, observation];
            }
            set
            {
                int index = System.Array.IndexOf(this.m_colNames, variable);
                this[index, observation] = value;
            }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>Converts this sample matrix to an ordinary matrix</summary>
        public Matrix ToMatrix()
        {
            return this.ToMatrix(DataModel.RowsAsObservations);
        }

        /// <summary>Converts this sample matrix to an ordinary matrix, using the given data organization model</summary>
        /// <param name="model">The data model for the generated matrix</param>
        public Matrix ToMatrix(DataModel model)
        {
            Matrix m = (Matrix)this;

            if (model == DataModel.RowsAsObservations)
                return m;

            else return m.Transpose();
        }

        public System.Data.DataTable ToDataTable()
        {
            System.Data.DataTable dataTable = new System.Data.DataTable(this.m_title);

            foreach (string colName in this.ColumnNames)
            {
                dataTable.Columns.Add(colName, typeof(double));
            }

            for (int i = 0; i < this.Rows; i++)
            {
                System.Data.DataRow row = dataTable.NewRow();
                for (int j = 0; j < this.Columns; j++)
                {
                    row[j] = this.Array[i][j];
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>Converts this sample matrix to a string representation</summary>
        public string ToString(bool includeHeaders)
        {
            if (includeHeaders)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < this.Variables; i++)
                {
                    sb.Append(this.m_colNames[i]);

                    if (i < this.Variables - 1)
                        sb.Append("\t");
                }

                sb.AppendLine();
                sb.Append(base.ToString());
                return sb.ToString();
            }
            else
            {
                return base.ToString();
            }
        }

        /// <summary>Creates a shallow copy of this object</summary>
        public new SampleMatrix Clone()
        {
            return new SampleMatrix(this);
        }
        #endregion


        //---------------------------------------------


        #region Static Methods

        /// <summary>Finds the empirical mean along each dimension of the matrix</summary>
        public static Vector Mean(SampleMatrix m)
        {
            return SampleMatrix.Mean((Matrix)m);
        }

        /// <summary>Finds the empirical mean along each column of the matrix</summary>
        public static Vector Mean(Matrix m)
        {
            Vector mean = new Vector(m.Columns);
            for (int j = 0; j < m.Columns; j++)
            {
                for (int i = 0; i < m.Rows; i++)
                {
                    mean[j] += m[i, j];
                }

                mean[j] /= (double)m.Columns;
            }
            return mean;
        }

        /// <summary>Calculates the empirical standard deviation along each dimension of the matrix</summary>
        public static Vector StandardDeviation(SampleMatrix m)
        {
            return SampleMatrix.StandardDeviation((Matrix)m);
        }

        public static Vector StandardDeviation(Matrix m)
        {
            Vector mean = SampleMatrix.Mean(m);
            Vector std = new Vector(m.Columns);

            for (int j = 0; j < m.Columns; j++)
            {
                for (int i = 0; i < m.Rows;i++)
                {
                    std[j] += System.Math.Pow((m[i, j] - mean[j]), 2);
                }

                std[j] /= (double)m.Rows;
                std[j] = System.Math.Sqrt(std[j]);

                // The following in an inelegant but usual way to handle
                //   near-zero std. dev. values, which below would cause a zero-divide.

                if (std[j] <= Double.Epsilon)
                    std[j] = 1.0;
            }
            return std;
        }

        /// <summary>Centers column data, subtracting the empirical mean from each variable.</summary>
        public static void Center(SampleMatrix m)
        {
            SampleMatrix.Center((Matrix)m);
        }

        /// <summary>Centers column data, subtracting the empirical mean from each variable.</summary>
        /// <param name="m">A matrix where each column represent a variable and each row represent a observation.</param>
        public static void Center(Matrix m)
        {
            Vector mean = SampleMatrix.Mean(m);

            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    m[i, j] -= mean[j];
                }
            }
        }

        /// <summary>Standardizes column data, removing the empirical standard deviation from each variable.</summary>
        public static void Standardize(SampleMatrix m)
        {
            SampleMatrix.Standardize((Matrix)m);
        }

        /// <summary>Standardizes column data, removing the empirical standard deviation from each variable.</summary>
        /// <param name="m">A matrix where each column represent a variable and each row represent a observation.</param>
        public static void Standardize(Matrix m)
        {
            Vector std = SampleMatrix.StandardDeviation(m);

            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    m[i, j] /= System.Math.Sqrt(m.Rows) * std[j];
                }
            }
        }

        /// <summary>Calculates the covariance matrix of a sample matrix, returning a new matrix object</summary>
        /// <remarks>
        ///   In statistics and probability theory, the covariance matrix is a matrix of
        ///   covariances between elements of a vector. It is the natural generalization
        ///   to higher dimensions of the concept of the variance of a scalar-valued
        ///   random variable.
        /// </remarks>
        /// <returns>The covariance matrix.</returns>
        public static Matrix Covariance(SampleMatrix m)
        {
            SampleMatrix r = m.Clone();
            SampleMatrix.Center(r);
            return (1.0 / (r.Observations - 1)) * r.Transpose() * (Matrix)r;
        }

        /// <param name="m">A matrix where each column represent a variable and each row represent a observation.</param>
        public static Matrix Covariance(Matrix m)
        {
            return Covariance((SampleMatrix)m);
        }     
        
        /// <summary>Calculates the correlation matrix of this samples, returning a new matrix object</summary>
        /// <remarks>
        /// In statistics and probability theory, the correlation matrix is the same
        /// as the covariance matrix of the standardized random variables.
        /// </remarks>
        /// <returns>The correlation matrix</returns>
        public static Matrix Correlation(SampleMatrix m)
        {
            SampleMatrix r = m.Clone();
            SampleMatrix.Center(r);
            SampleMatrix.Standardize(r);
            return (1.0 / (r.Observations - 1)) * r.Transpose() * (Matrix)r;
        }

        /// <param name="m">A matrix where each column represent a variable and each row represent a observation.</param>
        public static Matrix Correlation(Matrix m)
        {
            return Correlation((SampleMatrix)m);
        }
        #endregion


        //---------------------------------------------


        #region Operators

        #endregion


        //---------------------------------------------


        #region Private Methods
        private void generateDefaultNames()
        {
            m_colNames = new string[this.Variables];

            for (int i = 0; i < this.Variables; i++)
                m_colNames[i] = "Column " + i;
        }
        #endregion


    }

}
