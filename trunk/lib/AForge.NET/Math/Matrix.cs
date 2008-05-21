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

/***************************************************************************
 *  Based on work from Lutz Roeder's Mapack for .NET, September 2000       *
 *  Further modifications by Cesar Roberto de Souza, 2008                  *
 *  Adapted from Mapack for COM and Jama routines.                         *
 *  http://www.aisto.com/roeder/dotnet                                     *
 ***************************************************************************/


using System;
using System.IO;
using AForge.Math.LinearAlgebra.Decompositions;


namespace AForge.Math
{

	/// <summary>
    ///     Matrix provides the fundamental operations of numerical linear algebra.
    /// </summary>
    /// <remarks>
    ///     In mathematics, a matrix (plural matrices) is a rectangular
    ///     table of elements (or entries), which may be numbers or, more
    ///     generally, any abstract quantities that can be added and multiplied.
    /// </remarks>
	public class Matrix
	{

        /// <summary>
        /// Optimizes Matrix operations with rows and columns. If columns are accessed 
        /// more often than rows, pass the ColumnMajor organization model as a parameter.
        /// when creating a new Matrix. Otherwise, use RowMajor as it is the C default.
        /// </summary>
        public enum OrganizationModel { RowMajor, ColumnMajor };


		private double[][] m_data;
		private int m_rows;
		private int m_columns;
        private OrganizationModel m_model;

		private static System.Random random = new System.Random();


        //---------------------------------------------


        #region Constructors
        /// <summary>Constructs an empty matrix of the given size.</summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		public Matrix(int rows, int columns)
		{
			this.m_rows = rows;
			this.m_columns = columns;
			this.m_data = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				this.m_data[i] = new double[columns];
			}
		}
	
		/// <summary>Constructs a matrix of the given size and assigns a given value to all diagonal elements.</summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		/// <param name="value">Value to assign to the diagnoal elements.</param>
        public Matrix(int rows, int columns, double value)
            : this(rows, columns)
        {
            for (int i = 0; i < rows; i++)
            {
                m_data[i][i] = value;
            }
        }

        /// <summary>Constructs an empty matrix of the given dimension.</summary>
        public Matrix(int dimension)
            : this(dimension, dimension)
        {
        }

	
		/// <summary>Constructs a matrix from the given array.</summary>
		/// <param name="value">The array the matrix gets constructed from.</param>
		public Matrix(double[][] value)
		{
			this.m_rows = value.Length;
			this.m_columns = value[0].Length;

            for (int i = 0; i < this.m_rows; i++)
            {
                this.m_data[i] = new double[this.m_columns];
                for (int j = 0; j < this.m_columns; j++)
                {
                    this.m_data[i][j] = value[i][j];
                }
            }
        }

        /// <summary> Constructs a matrix from the given matrix.</summary>
        /// <param name="value">The array the matrix gets constructed from.</param>
        public Matrix(double[,] value)
        {
            this.m_rows = value.GetLength(0);
            this.m_columns = value.GetLength(1);

            for (int i = 0; i < this.m_rows; i++)
            {
                this.m_data[i] = new double[this.m_columns];
                for (int j = 0; j < this.m_columns; j++)
                {
                    this.m_data[i][j] = value[i, j];
                }
            }
        }

        /// <summary>Constructs a new Matrix object by reading values from a data table.</summary>
        /// <param name="dataTable">The source DataTable. It must have nummeric, boolean or string type values only.</param>
        public Matrix(System.Data.DataTable dataTable)
            : this(dataTable.Rows.Count, dataTable.Columns.Count)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    if (dataTable.Columns[j].DataType == typeof(System.String))
                    {
                        this.Array[i][j] = Double.Parse((String)dataTable.Rows[i][j]);
                    }
                    else if (dataTable.Columns[j].DataType == typeof(System.Boolean))
                    {
                        this.Array[i][j] = (Boolean)dataTable.Rows[i][j] ? 1.0 : 0.0;
                    }
                    else
                    {
                        this.Array[i][j] = (Double)dataTable.Rows[i][j];
                    }
                }
            }
        }

        /// <summary>Creates a new copy of a matrix object</summary>
        /// <param name="matrix">The matrix to be copied.</param>
        public Matrix(Matrix matrix)
        {
            this.m_data = new double[matrix.Rows][];
            for (int i = 0; i < matrix.Rows; i++)
            {
                this.m_data[i] = new double[matrix.Columns];
                for (int j = 0; j < matrix.Columns; j++)
                {
                    this.m_data[i][j] = matrix.m_data[i][j];
                }
            }
        //    this.m_data = (double[][])matrix.m_data.Clone();
            this.m_columns = matrix.m_columns;
            this.m_rows = matrix.m_rows;
        }


        private Matrix()
        {
            // A private constructor is only used when creating special instances
            // of the matrix class during conversion operations. Should not be used
            // otherwise.
        }
        #endregion


        //---------------------------------------------


        #region Properties
        /// <summary></summary>
        internal protected double[][] Array
        {
            get { return this.m_data; }
            set { this.m_data = value; }
        }
	
		/// <summary>Returns the number of columns.</summary>
        public int Rows
        {
            get { return this.m_rows; }
        }

		/// <summary>Returns the number of columns.</summary>
        public int Columns
        {
            get { return this.m_columns; }
        }

		/// <summary>Returns <see langword="true"/> if the matrix is a square matrix.</summary>
        public bool Square
        {
            get { return (m_rows == m_columns); }
        }

		/// <summary>Returns <see langword="true"/> if the matrix is symmetric.</summary>
		public bool Symmetric
		{
			get
			{
				if (this.Square)
				{
					for (int i = 0; i < m_rows; i++)
					{
						for (int j = 0; j <= i; j++)
						{
							if (m_data[i][j] != m_data[j][i])
							{
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>Access the value at the given location.</summary>
        public double this[int row, int column]
        {
            set { this.m_data[row][column] = value; }
            get { return this.m_data[row][column]; }
        }

        /// <summary>Access a matrix row</summary>
        public Vector this[int row]
        {
            get { return this.GetRow(row); }
        }

        /// <summary>Determinant if matrix is square.</summary>
        public double Determinant
        {
            get { return new LuDecomposition(this).Determinant; }
        }

        /// <summary>Returns the trace of the matrix.</summary>
        /// <returns>Sum of the diagonal elements.</returns>
        public double Trace
        {
            get
            {
                double trace = 0;
                for (int i = 0; i < System.Math.Min(m_rows, m_columns); i++)
                {
                    trace += m_data[i][i];
                }
                return trace;
            }
        }

        /// <summary>Returns the One Norm for the matrix.</summary>
        /// <value>The maximum column sum.</value>
        public double OneNorm
        {
            get
            {
                double f = 0;
                for (int j = 0; j < m_columns; j++)
                {
                    double s = 0;
                    for (int i = 0; i < m_rows; i++)
                    {
                        s += System.Math.Abs(m_data[i][j]);
                    }

                    f = System.Math.Max(f, s);
                }
                return f;
            }
        }

        /// <summary>Return the Two Norm for the matrix</summary>
        public double TwoNorm
        {
            get
            {
                if ((this.m_rows != 0) && (this.m_columns != 0))
                    return new SingularValueDecomposition(this).TwoNorm;

                else return 0.0;
            }
        }

        /// <summary>Returns the Infinity Norm for the matrix.</summary>
        /// <value>The maximum row sum.</value>
        public double InfinityNorm
        {
            get
            {
                double f = 0;
                for (int i = 0; i < m_rows; i++)
                {
                    double s = 0;
                    for (int j = 0; j < m_columns; j++)
                        s += System.Math.Abs(m_data[i][j]);
                    f = System.Math.Max(f, s);
                }
                return f;
            }
        }

        /// <summary>Returns the Frobenius Norm for the matrix.</summary>
        /// <value>The square root of sum of squares of all elements.</value>
        public double FrobeniusNorm
        {
            get
            {
                double f = 0;
                for (int i = 0; i < m_rows; i++)
                {
                    for (int j = 0; j < m_columns; j++)
                    {
                        f = AForge.Math.Tools.Hypotenuse(f, m_data[i][j]);
                    }
                }

                return f;
            }
        }

        #endregion


        //---------------------------------------------


        #region Public Methods

        /// <summary>Swaps two columns of this matrix</summary>
        /// <param name="column1">The first column index to be swapped</param>
        /// <param name="column2">The second column index to be swapped</param>
        public void SwapColumns(int column1, int column2)
        {
            Vector aux = this.GetColumn(column1);
            this.SetColumn(column1, this.GetColumn(column2));
            this.SetColumn(column2, aux);
        }

        /// <summary>Swaps two rows of this matrix</summary>
        /// <param name="row1">The first row index to be swapped</param>
        /// <param name="row2">The second row index to be swapped</param>
        public void SwapRows(int row1, int row2)
        {
            Vector aux = this.GetRow(row1);
            this.SetRow(row1, this.GetRow(row2));
            this.SetRow(row2, aux);
        }

        /// <summary>Gets a entire row of the matrix as a double array</summary>
        /// <param name="index">The row's index</param>
        /// <returns>The row double array</returns>
        public Vector GetRow(int index)
        {
            return this.m_data[index];
        }

        /// <summary>Gets a entire row of the matrix as a double array</summary>
        /// <param name="index">The row's index</param>
        /// <returns>The row double array</returns>
        public Vector GetColumn(int index)
        {
            double[] col = new double[this.m_rows];

            for (int i = 0; i < col.Length; i++)
            {
                col[i] = this[i, index];
            }

            return col;
        }

        /// <summary>Sets a entire row of the matrix based on a double array</summary>
        /// <param name="index">The row's index</param>
        /// <param name="row">The row new double array</param>
        public void SetRow(int index, double[] row)
        {
            if (row.Length != this.Columns)
                throw new ArgumentException("The supplied row has a different number of columns than this matrix", "row");

            this.m_data[index] = row;
        }

        /// <summary> Sets a entire column of the matrix based on a double array</summary>
        /// <param name="index">The column's index</param>
        /// <param name="column">The column new double array</param>
        public void SetColumn(int index, double[] column)
        {
            if (column.Length != this.Rows)
                throw new ArgumentException("The supplied column has a different number of rows than this matrix", "row");

            for (int i = 0; i < column.Length; i++)
            {
                this[i, index] = column[i];
            }
        }

        public void ScaleColumns(double[] factor)
        {
            if (factor == null)
            {
                throw new ArgumentNullException("factor");
            }
            if (factor.Length != this.m_columns)
            {
                throw new ArgumentException("Dimension mismatch", "factor");
            }

            for (int j = 0; j < this.m_columns; j++)
            {
                for (int i = 0; i < this.m_rows; i++)
                {
                    this.m_data[i][j] = this.m_data[i][j] * factor[j];
                }
            }
        }

        public void ScaleRows(double[] factor)
        {
            if (factor == null)
            {
                throw new ArgumentNullException("factor");
            }
            if (factor.Length != this.m_rows)
            {
                throw new ArgumentException("Dimension mismatch", "factor");
            }

            for (int i = 0; i < this.m_rows; i++)
            {
                for (int j = 0; j < this.m_columns; j++)    
                {
                    this.m_data[i][j] = this.m_data[i][j] * factor[i];
                }
            }
        }

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="startRow">Start row index</param>
		/// <param name="endRow">End row index</param>
		/// <param name="startColumn">Start column index</param>
		/// <param name="endColumn">End column index</param>
		public Matrix Submatrix(int startRow, int endRow, int startColumn, int endColumn)
		{
			if ((startRow > endRow) || (startColumn > endColumn) ||  (startRow < 0) || (startRow >= this.m_rows) ||  (endRow < 0) || (endRow >= this.m_rows) ||  (startColumn < 0) || (startColumn >= this.m_columns) ||  (endColumn < 0) || (endColumn >= this.m_columns)) 
            { 
				throw new ArgumentException("Argument out of range."); 
			} 
			
			Matrix X = new Matrix(endRow - startRow + 1, endColumn - startColumn + 1);
			double[][] x = X.Array;
			for (int i = startRow; i <= endRow; i++)
			{
				for (int j = startColumn; j <= endColumn; j++)
				{
					x[i - startRow][j - startColumn] = m_data[i][j];
				}
			}
					
			return X;
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="rowIndexes">Array of row indices</param>
		/// <param name="columnIndexes">Array of column indices</param>
		public Matrix Submatrix(int[] rowIndexes, int[] columnIndexes)
		{
			Matrix X = new Matrix(rowIndexes.Length, columnIndexes.Length);
			double[][] x = X.Array;
			for (int i = 0; i < rowIndexes.Length; i++)
			{
				for (int j = 0; j < columnIndexes.Length; j++)
				{
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= m_rows) || (columnIndexes[j] < 0) || (columnIndexes[j] >= m_columns))
                    { 
						throw new ArgumentException("Argument out of range."); 
                    }

					x[i][j] = m_data[rowIndexes[i]][columnIndexes[j]];
				}
			}

			return X;
		}

		/// <summary> Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="i0">Starttial row index</param>
		/// <param name="i1">End row index</param>
		/// <param name="c">Array of row indices</param>
		public Matrix Submatrix(int i0, int i1, int[] c)
		{
			if ((i0 > i1) || (i0 < 0) || (i0 >= this.m_rows) || (i1 < 0) || (i1 >= this.m_rows)) 
			{ 
            	throw new ArgumentException("Argument out of range."); 
			} 
			
			Matrix X = new Matrix(i1 - i0 + 1, c.Length);
			double[][] x = X.Array;
			for (int i = i0; i <= i1; i++)
			{
				for (int j = 0; j < c.Length; j++)
				{
                    if ((c[j] < 0) || (c[j] >= m_columns)) 
                    { 
						throw new ArgumentException("Argument out of range."); 
                    } 

					x[i - i0][j] = m_data[i][c[j]];
				}
			}

			return X;
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="r">Array of row indices</param>
		/// <param name="j0">Start column index</param>
		/// <param name="j1">End column index</param>
		public Matrix Submatrix(int[] r, int j0, int j1)
		{
			if ((j0 > j1) || (j0 < 0) || (j0 >= m_columns) || (j1 < 0) || (j1 >= m_columns)) 
			{ 
				throw new ArgumentException("Argument out of range."); 
			} 
			
			Matrix X = new Matrix(r.Length, j1-j0+1);
			double[][] x = X.Array;
			for (int i = 0; i < r.Length; i++)
			{
				for (int j = j0; j <= j1; j++) 
				{
					if ((r[i] < 0) || (r[i] >= this.m_rows))
					{
						throw new ArgumentException("Argument out of range."); 
					}

					x[i][j - j0] = m_data[r[i]][j];
				}
			}

			return X;
		}



        /// <summary>Creates a deep copy of the AForge.Math.Matrix.</summary>
        public Matrix Clone()
        {
            return new Matrix(this);
        }

        /// <summary> Returns the transposed matrix.</summary>
        public Matrix Transpose()
        {
            Matrix t = new Matrix(m_columns, m_rows);

            for (int i = 0; i < this.m_rows; i++)
            {
                for (int j = 0; j < this.m_columns; j++)
                {
                    t.m_data[j][i] = this.m_data[i][j];
                }
            }

            return t;
        }

        /// <summary>Inverse of the matrix if matrix is square, pseudoinverse otherwise.</summary>
        public Matrix Inverse()
        {
            return this.Solve(Matrix.Diagonal(m_rows, 1.0));
        }

        /// <summary>Returns the LHS solution vetor if the matrix is square or the least squares solution otherwise.</summary>
        public Matrix Solve(Matrix rightHandSide)
        {
            return (m_rows == m_columns) ? new LuDecomposition(this).Solve(rightHandSide) : new QrDecomposition(this).Solve(rightHandSide);
        }

        /// <summary> Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.</summary>
        public override int GetHashCode()
        {
            return this.m_rows.GetHashCode() ^ this.m_columns.GetHashCode() ^ this.m_data.GetHashCode();
        }

        /// <summary>Returns the matrix in a textual form.</summary>
        public override string ToString()
        {
            using (StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                for (int i = 0; i < m_rows; i++)
                {
                    for (int j = 0; j < m_columns; j++)
                    {
                        writer.Write(this.m_data[i][j] + " ");
                    }

                    writer.WriteLine();
                }

                return writer.ToString();
            }
        }

        /// <summary>Converts the given Matrix into a DataTable representation.</summary>
        public virtual System.Data.DataTable ToDataTable(string name)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();

            for (int i = 0; i < this.m_columns; i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(System.Double));
            }

            for (int i = 0; i < this.m_rows; i++)
            {
                System.Data.DataRow row = dataTable.NewRow();
                for (int j = 0; j < this.Columns; j++)
                {
                    row[j] = this.m_data[i][j];
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public virtual System.Data.DataTable ToDataTable()
        {
            return this.ToDataTable("Matrix");
        }

        public double[][] ToArray()
        {
            return (double[][])this.m_data.Clone();
        }

        public double[,] ToMultidimensionalArray()
        {
            double[,] array = new double[this.m_rows, this.m_columns];

            for (int i = 0; i < m_rows; i++)
            {
                for (int j = 0; j < m_columns; j++)
                {
                    array[i,j] = this.m_data[i][j];
                }
            }
            return array;
        }
        #endregion


        //---------------------------------------------


        #region Operators

        /// <summary>Determines weather two instances are equal.</summary>
        public override bool Equals(object obj)
        {
            return Equals(this, (Matrix)obj);
        }

        /// <summary>Determines weather two instances are equal.</summary>
        public static bool Equals(Matrix left, Matrix right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left) == ((object)right))
            {
                return true;
            }

            if ((((object)left) == null) || (((object)right) == null))
            {
                return false;
            }

            if (!DimensionEquals(left, right))
            {
                return false;
            }

            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < left.Columns; j++)
                {
                    if (left[i, j] != right[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>Determines weather two instances have dimension equality.</summary>
        public bool DimensionEquals(Matrix value)
        {
            return DimensionEquals(this, value);
        }

        /// <summary>Determines weather two instances have dimension equality.</summary>
        public static bool DimensionEquals(Matrix left, Matrix right)
        {
            return (left.Rows == right.Rows && left.Columns == right.Columns);
        }

        /// <summary>Unary minus.</summary>
		public static Matrix Negate(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			int rows = value.Rows;
			int columns = value.Columns;
			double[][] data = value.Array;

			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					x[i][j] = -data[i][j];
				}
			}

			return X;
		}

		/// <summary>Unary minus.</summary>
		public static Matrix operator-(Matrix value)
		{
			return Negate(value);
		}

		/// <summary>Matrix equality.</summary>
		public static bool operator==(Matrix left, Matrix right)
		{
			return Equals(left, right);
		}

		/// <summary>Matrix inequality.</summary>
		public static bool operator!=(Matrix left, Matrix right)
		{
			return !Equals(left, right);
		}

		/// <summary>Matrix addition.</summary>
		public static Matrix Add(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}

			if (right == null)
			{
				throw new ArgumentNullException("right");
			}

            if (!DimensionEquals(left,right))
            {
                throw new ArgumentException("Matrix dimension do not match.");
            }		

			Matrix r = new Matrix(left.m_rows, left.m_columns);
			for (int i = 0; i < left.m_rows; i++)
			{
				for (int j = 0; j < left.m_columns; j++)
				{
					r[i][j] = left.m_data[i][j] + right.m_data[i][j];
				}
			}
			return r;
		}

		/// <summary>Matrix addition.</summary>
		public static Matrix operator+(Matrix left, Matrix right)
		{
			return Add(left, right);
		}

		/// <summary>Matrix subtraction.</summary>
		public static Matrix Subtract(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}

			if (right == null)
			{
				throw new ArgumentNullException("right");
			}

            if (!DimensionEquals(left, right))
            {
                throw new ArgumentException("Matrix dimension do not match.");
            }

            Matrix r = new Matrix(left.m_rows, left.m_columns);
            for (int i = 0; i < left.m_rows; i++)
            {
                for (int j = 0; j < left.m_columns; j++)
                {
                    r[i][j] = left.m_data[i][j] - right.m_data[i][j];
                }
            }
            return r;
        }

		/// <summary>Matrix subtraction.</summary>
		public static Matrix operator-(Matrix left, Matrix right)
		{
			return Subtract(left, right);
		}

		/// <summary>Matrix-scalar multiplication.</summary>
		public static Matrix Multiply(Matrix left, double right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}

			Matrix r = new Matrix(left.m_rows, left.m_columns);
			for (int i = 0; i < left.m_rows; i++)
			{
                for (int j = 0; j < left.m_columns; j++)
				{
					r.m_data[i][j] = left.m_data[i][j] * right;
				}
			}
			return r;
		}

		/// <summary>Matrix-scalar multiplication.</summary>
		public static Matrix operator*(Matrix left, double right)
		{
			return Multiply(left, right);
		}

        /// <summary>Matrix-scalar multiplication.</summary>
        public static Matrix operator*(double left, Matrix right)
        {
            return right * left;
        }

		/// <summary>Matrix-matrix multiplication.</summary>
		public static Matrix Multiply(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}

			if (right == null)
			{
				throw new ArgumentNullException("right");
			}

			int rows = left.Rows;
			double[][] data = left.Array;

			if (right.Rows != left.m_columns)
			{
				throw new ArgumentException("Matrix dimensions are not valid.");
			}

			int columns = right.Columns;
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;

			int size = left.m_columns;
			double[] column = new double[size];
			for (int j = 0; j < columns; j++)
			{
				for (int k = 0; k < size; k++)
				{
					column[k] = right[k,j];
				}
				for (int i = 0; i < rows; i++)
				{
					double[] row = data[i];
					double s = 0;
					for (int k = 0; k < size; k++)
					{
						s += row[k] * column[k];
					}
					x[i][j] = s;
				} 
			}

			return X;
		}

		/// <summary>Matrix-matrix multiplication.</summary>
		public static Matrix operator*(Matrix left, Matrix right)
		{		
			return Multiply(left, right);
        }

        /// <summary>Returns the matrix in a double[][] form. </summary>
        public static implicit operator double[][](Matrix matrix)
        {
            return matrix.m_data;
        }
        
        public static explicit operator Matrix(double[][] matrix)
        {
            Matrix r = new Matrix();
            r.m_rows = matrix.Length;
            r.m_columns = matrix[0].Length;

            for (int i = 0; i < r.m_rows; i++)
            {
                if (matrix[i].Length != r.m_columns)
                    throw new ArgumentException("The given array has rows of different dimensions", "matrix");
            }

            r.m_data = matrix;

            return r;
        }
        #endregion


        //---------------------------------------------


        #region Static Methods

        /// <summary>Returns a matrix filled with random values.</summary>
        public static Matrix Random(int rows, int columns)
        {
            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = random.NextDouble();
                }
            }
            return X;
        }

        /// <summary>Returns a square matrix filled with random values.</summary>
        public static Matrix Random(int dimension)
        {
            return Matrix.Random(dimension, dimension);
        }

        /// <summary> Returns the identity matrix of the given dimension. </summary>
        public static Matrix Identity(int dimension)
        {
            return Matrix.Diagonal(dimension, 1.0);
        }

        /// <summary>Returns a diagonal matrix of the given size.</summary>
        public static Matrix Diagonal(int rows, int columns, double value)
        {
            return new Matrix(rows, columns, value);
        }

        /// <summary>Returns a square diagonal matrix of the given dimension.</summary>
        public static Matrix Diagonal(int dimension, double value)
        {
            return Matrix.Diagonal(dimension, dimension, value);
        }

        /// <summary>Raise every member of the matrix to the given power.</summary>
        public static Matrix Pow(Matrix m, double power)
        {
            Matrix r = m.Clone();
            for (int i = 0; i < r.m_rows; i++)
            {
                for (int j = 0; j < r.m_columns; j++)
                {
                    r.m_data[i][j] = System.Math.Pow(r.m_data[i][j], power);
                }
            }
            return r;
        }


        #endregion

    }
    
}