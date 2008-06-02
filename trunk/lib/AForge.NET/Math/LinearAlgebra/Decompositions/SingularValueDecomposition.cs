/***************************************************************************
 *                                                                         *
 *  Please note that this code is not part of the original AForge.NET      *
 *  library. This extension was created to support new features needed by  *
 *  Sinapse, a neural networking tool software. Unless otherwise advised,  *
 *  this code relies under protection of the GNU General Public License v3 *
 *                                                                         *
 ***************************************************************************/

/***************************************************************************
 *  Adapted from Lutz Roeder's Mapack for .NET, September 2000             *
 *  Adapted from Mapack for COM and Jama routines.                         *
 *  http://www.aisto.com/roeder/dotnet                                     *
 ***************************************************************************/

using System;


namespace AForge.Mathematics.LinearAlgebra.Decompositions
{

    /// <summary>
    ///   Singular Value Decomposition for a rectangular matrix.
    /// </summary>
    /// <remarks>
    ///	  For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the singular value decomposition
    ///   is an m-by-n orthogonal matrix <c>U</c>, an n-by-n diagonal matrix <c>S</c>, and
    ///   an n-by-n orthogonal matrix <c>V</c> so that <c>A = U * S * V'</c>.
    ///   The singular values, <c>sigma[k] = S[k,k]</c>, are ordered so that
    ///   <c>sigma[0] >= sigma[1] >= ... >= sigma[n-1]</c>.
    /// 
    ///   The singular value decompostion always exists, so the constructor will
    ///   never fail. The matrix condition number and the effective numerical
    ///   rank can be computed from this decomposition.
    /// </remarks>
    public class SingularValueDecomposition
    {
        private Matrix U;
        private Matrix V;
        private double[] s; // singular values
        private int m;
        private int n;

        /// <summary>Construct singular value decomposition.</summary>
        public SingularValueDecomposition(Matrix value)
            : this(value, true, true)
        {
        }

        public SingularValueDecomposition(Matrix value, bool computeLeftSingularVectors, bool computeRightSingularVectors)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Matrix copy = (Matrix)value.Clone();
            double[][] a = copy.baseArray;
            m = value.Rows;
            n = value.Columns;
            int nu = System.Math.Min(m, n);
            s = new double[System.Math.Min(m + 1, n)];
            U = new Matrix(m, nu);
            V = new Matrix(n, n);
            double[][] u = U.baseArray;
            double[][] v = V.baseArray;
            double[] e = new double[n];
            double[] work = new double[m];
            bool wantu = computeLeftSingularVectors;
            bool wantv = computeRightSingularVectors;

            // Reduce A to bidiagonal form, storing the diagonal elements in s and the super-diagonal elements in e.
            int nct = System.Math.Min(m - 1, n);
            int nrt = System.Math.Max(0, System.Math.Min(n - 2, m));
            for (int k = 0; k < System.Math.Max(nct, nrt); k++)
            {
                if (k < nct)
                {
                    // Compute the transformation for the k-th column and place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    s[k] = 0;
                    for (int i = k; i < m; i++)
                    {
                        s[k] = Tools.Hypotenuse(s[k], a[i][k]);
                    }

                    if (s[k] != 0.0)
                    {
                        if (a[k][k] < 0.0)
                        {
                            s[k] = -s[k];
                        }

                        for (int i = k; i < m; i++)
                        {
                            a[i][k] /= s[k];
                        }

                        a[k][k] += 1.0;
                    }

                    s[k] = -s[k];
                }

                for (int j = k + 1; j < n; j++)
                {
                    if ((k < nct) & (s[k] != 0.0))
                    {
                        // Apply the transformation.
                        double t = 0;
                        for (int i = k; i < m; i++)
                            t += a[i][k] * a[i][j];
                        t = -t / a[k][k];
                        for (int i = k; i < m; i++)
                            a[i][j] += t * a[i][k];
                    }

                    // Place the k-th row of A into e for the subsequent calculation of the row transformation.
                    e[j] = a[k][j];
                }

                if (wantu & (k < nct))
                {
                    // Place the transformation in U for subsequent back
                    // multiplication.
                    for (int i = k; i < m; i++)
                        u[i][k] = a[i][k];
                }

                if (k < nrt)
                {
                    // Compute the k-th row transformation and place the k-th super-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;
                    for (int i = k + 1; i < n; i++)
                    {
                        e[k] = Tools.Hypotenuse(e[k], e[i]);
                    }

                    if (e[k] != 0.0)
                    {
                        if (e[k + 1] < 0.0)
                            e[k] = -e[k];

                        for (int i = k + 1; i < n; i++)
                            e[i] /= e[k];

                        e[k + 1] += 1.0;
                    }

                    e[k] = -e[k];
                    if ((k + 1 < m) & (e[k] != 0.0))
                    {
                        // Apply the transformation.
                        for (int i = k + 1; i < m; i++)
                            work[i] = 0.0;

                        for (int j = k + 1; j < n; j++)
                            for (int i = k + 1; i < m; i++)
                                work[i] += e[j] * a[i][j];

                        for (int j = k + 1; j < n; j++)
                        {
                            double t = -e[j] / e[k + 1];
                            for (int i = k + 1; i < m; i++)
                                a[i][j] += t * work[i];
                        }
                    }

                    if (wantv)
                    {
                        // Place the transformation in V for subsequent back multiplication.
                        for (int i = k + 1; i < n; i++)
                            v[i][k] = e[i];
                    }
                }
            }

            // Set up the final bidiagonal matrix or order p.
            int p = System.Math.Min(n, m + 1);
            if (nct < n) s[nct] = a[nct][nct];
            if (m < p) s[p - 1] = 0.0;
            if (nrt + 1 < p) e[nrt] = a[nrt][p - 1];
            e[p - 1] = 0.0;

            // If required, generate U.
            if (wantu)
            {
                for (int j = nct; j < nu; j++)
                {
                    for (int i = 0; i < m; i++)
                        u[i][j] = 0.0;
                    u[j][j] = 1.0;
                }

                for (int k = nct - 1; k >= 0; k--)
                {
                    if (s[k] != 0.0)
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k; i < m; i++)
                                t += u[i][k] * u[i][j];

                            t = -t / u[k][k];
                            for (int i = k; i < m; i++)
                                u[i][j] += t * u[i][k];
                        }

                        for (int i = k; i < m; i++)
                            u[i][k] = -u[i][k];

                        u[k][k] = 1.0 + u[k][k];
                        for (int i = 0; i < k - 1; i++)
                            u[i][k] = 0.0;
                    }
                    else
                    {
                        for (int i = 0; i < m; i++)
                            u[i][k] = 0.0;
                        u[k][k] = 1.0;
                    }
                }
            }

            // If required, generate V.
            if (wantv)
            {
                for (int k = n - 1; k >= 0; k--)
                {
                    if ((k < nrt) & (e[k] != 0.0))
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k + 1; i < n; i++)
                                t += v[i][k] * v[i][j];

                            t = -t / v[k + 1][k];
                            for (int i = k + 1; i < n; i++)
                                v[i][j] += t * v[i][k];
                        }
                    }

                    for (int i = 0; i < n; i++)
                        v[i][k] = 0.0;
                    v[k][k] = 1.0;
                }
            }

            // Main iteration loop for the singular values.
            int pp = p - 1;
            int iter = 0;
            double eps = System.Math.Pow(2.0, -52.0);
            while (p > 0)
            {
                int k, kase;

                // Here is where a test for too many iterations would go.
                // This section of the program inspects for
                // negligible elements in the s and e arrays.  On
                // completion the variables kase and k are set as follows.
                // kase = 1     if s(p) and e[k-1] are negligible and k<p
                // kase = 2     if s(k) is negligible and k<p
                // kase = 3     if e[k-1] is negligible, k<p, and s(k), ..., s(p) are not negligible (qr step).
                // kase = 4     if e(p-1) is negligible (convergence).
                for (k = p - 2; k >= -1; k--)
                {
                    if (k == -1)
                        break;

                    if (System.Math.Abs(e[k]) <= eps * (System.Math.Abs(s[k]) + System.Math.Abs(s[k + 1])))
                    {
                        e[k] = 0.0;
                        break;
                    }
                }

                if (k == p - 2)
                {
                    kase = 4;
                }
                else
                {
                    int ks;
                    for (ks = p - 1; ks >= k; ks--)
                    {
                        if (ks == k)
                            break;

                        double t = (ks != p ? System.Math.Abs(e[ks]) : 0.0) + (ks != k + 1 ? System.Math.Abs(e[ks - 1]) : 0.0);
                        if (System.Math.Abs(s[ks]) <= eps * t)
                        {
                            s[ks] = 0.0;
                            break;
                        }
                    }

                    if (ks == k)
                        kase = 3;
                    else if (ks == p - 1)
                        kase = 1;
                    else
                    {
                        kase = 2;
                        k = ks;
                    }
                }

                k++;

                // Perform the task indicated by kase.
                switch (kase)
                {
                    // Deflate negligible s(p).
                    case 1:
                        {
                            double f = e[p - 2];
                            e[p - 2] = 0.0;
                            for (int j = p - 2; j >= k; j--)
                            {
                                double t = Tools.Hypotenuse(s[j], f);
                                double cs = s[j] / t;
                                double sn = f / t;
                                s[j] = t;
                                if (j != k)
                                {
                                    f = -sn * e[j - 1];
                                    e[j - 1] = cs * e[j - 1];
                                }

                                if (wantv)
                                {
                                    for (int i = 0; i < n; i++)
                                    {
                                        t = cs * v[i][j] + sn * v[i][p - 1];
                                        v[i][p - 1] = -sn * v[i][j] + cs * v[i][p - 1];
                                        v[i][j] = t;
                                    }
                                }
                            }
                        }
                        break;

                    // Split at negligible s(k).
                    case 2:
                        {
                            double f = e[k - 1];
                            e[k - 1] = 0.0;
                            for (int j = k; j < p; j++)
                            {
                                double t = Tools.Hypotenuse(s[j], f);
                                double cs = s[j] / t;
                                double sn = f / t;
                                s[j] = t;
                                f = -sn * e[j];
                                e[j] = cs * e[j];
                                if (wantu)
                                {
                                    for (int i = 0; i < m; i++)
                                    {
                                        t = cs * u[i][j] + sn * u[i][k - 1];
                                        u[i][k - 1] = -sn * u[i][j] + cs * u[i][k - 1];
                                        u[i][j] = t;
                                    }
                                }
                            }
                        }
                        break;

                    // Perform one qr step.
                    case 3:
                        {
                            // Calculate the shift.
                            double scale = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Abs(s[p - 1]), System.Math.Abs(s[p - 2])), System.Math.Abs(e[p - 2])), System.Math.Abs(s[k])), System.Math.Abs(e[k]));
                            double sp = s[p - 1] / scale;
                            double spm1 = s[p - 2] / scale;
                            double epm1 = e[p - 2] / scale;
                            double sk = s[k] / scale;
                            double ek = e[k] / scale;
                            double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
                            double c = (sp * epm1) * (sp * epm1);
                            double shift = 0.0;
                            if ((b != 0.0) | (c != 0.0))
                            {
                                shift = System.Math.Sqrt(b * b + c);
                                if (b < 0.0)
                                    shift = -shift;
                                shift = c / (b + shift);
                            }

                            double f = (sk + sp) * (sk - sp) + shift;
                            double g = sk * ek;

                            // Chase zeros.
                            for (int j = k; j < p - 1; j++)
                            {
                                double t = Tools.Hypotenuse(f, g);
                                double cs = f / t;
                                double sn = g / t;
                                if (j != k)
                                    e[j - 1] = t;
                                f = cs * s[j] + sn * e[j];
                                e[j] = cs * e[j] - sn * s[j];
                                g = sn * s[j + 1];
                                s[j + 1] = cs * s[j + 1];
                                if (wantv)
                                {
                                    for (int i = 0; i < n; i++)
                                    {
                                        t = cs * v[i][j] + sn * v[i][j + 1];
                                        v[i][j + 1] = -sn * v[i][j] + cs * v[i][j + 1];
                                        v[i][j] = t;
                                    }
                                }

                                t = Tools.Hypotenuse(f, g);
                                cs = f / t;
                                sn = g / t;
                                s[j] = t;
                                f = cs * e[j] + sn * s[j + 1];
                                s[j + 1] = -sn * e[j] + cs * s[j + 1];
                                g = sn * e[j + 1];
                                e[j + 1] = cs * e[j + 1];
                                if (wantu && (j < m - 1))
                                {
                                    for (int i = 0; i < m; i++)
                                    {
                                        t = cs * u[i][j] + sn * u[i][j + 1];
                                        u[i][j + 1] = -sn * u[i][j] + cs * u[i][j + 1];
                                        u[i][j] = t;
                                    }
                                }
                            }

                            e[p - 2] = f;
                            iter = iter + 1;
                        }
                        break;

                    // Convergence.
                    case 4:
                        {
                            // Make the singular values positive.
                            if (s[k] <= 0.0)
                            {
                                s[k] = (s[k] < 0.0 ? -s[k] : 0.0);
                                if (wantv)
                                    for (int i = 0; i <= pp; i++)
                                        v[i][k] = -v[i][k];
                            }

                            // Order the singular values.
                            while (k < pp)
                            {
                                if (s[k] >= s[k + 1])
                                    break;

                                double t = s[k];
                                s[k] = s[k + 1];
                                s[k + 1] = t;
                                if (wantv && (k < n - 1))
                                    for (int i = 0; i < n; i++)
                                    {
                                        t = v[i][k + 1];
                                        v[i][k + 1] = v[i][k];
                                        v[i][k] = t;
                                    }

                                if (wantu && (k < m - 1))
                                    for (int i = 0; i < m; i++)
                                    {
                                        t = u[i][k + 1];
                                        u[i][k + 1] = u[i][k];
                                        u[i][k] = t;
                                    }

                                k++;
                            }

                            iter = 0;
                            p--;
                        }
                        break;
                }
            }
        }

        /// <summary>Returns the condition number <c>max(S) / min(S)</c>.</summary>
        public double Condition
        {
            get { return s[0] / s[System.Math.Min(m, n) - 1]; }
        }

        /// <summary>Returns the Two norm.</summary>
        public double TwoNorm
        {
            get
            { return s[0]; }
        }

        /// <summary>Returns the effective numerical matrix rank.</summary>
        /// <value>Number of non-negligible singular values.</value>
        public int Rank
        {
            get
            {
                double eps = System.Math.Pow(2.0, -52.0);
                double tol = System.Math.Max(m, n) * s[0] * eps;
                int r = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] > tol)
                    {
                        r++;
                    }
                }

                return r;
            }
        }

        /// <summary>Returns the one-dimensional array of singular values.</summary>		
        public double[] Diagonal
        {
            get { return this.s; }
        }

        /// <summary>Returns the V matrix of Singular Vectors.</summary>		
        public Matrix RightSingularVectors
        {
            get { return V; }
        }

        /// <summary>Return the U matrix of Singular Vectors.</summary>		
        public Matrix LeftSingularVectors
        {
            get { return U; }
        }

    }
}
