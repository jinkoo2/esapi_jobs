using System;

namespace Numerics
{
    class MatrixMN
    {
        float[,] _elements;

        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public MatrixMN(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            _elements = new float[rows, cols];
        }

        public void SetIdentity()
        {
            SetZero();

            for (int i = 0; i < Rows; ++i)
            {
                _elements[i, i] = 1.0f;
            }
        }

        public void SetZero()
        {
            for (int i = 0; i < Rows; ++i)
                for (int j = 0; j < Cols; ++j)
                {
                    _elements[i, j] = 0.0f;
                }
        }


        public float this[int i, int j]
        {
            get { return _elements[i, j]; }
            set { _elements[i, j] = value; }
        }

        public static VectorN operator *(MatrixMN lhs, VectorN v)
        {
            VectorN rv = new VectorN(lhs.Rows);
            for (int i = 0; i < lhs.Rows; i++)
            {
                float sum = 0;
                for (int j = 0; j < lhs.Cols; j++)
                    sum += lhs[i, j] * v[j];
                rv[i] = sum;
            }
            return rv;
        }

        public static MatrixMN operator *(MatrixMN lhs, MatrixMN rhs)
        {
            MatrixMN rv = new MatrixMN(lhs.Rows, rhs.Cols);

            for (int i = 0; i < lhs.Rows; i++)
            {
                for (int j = 0; j < rhs.Cols; j++)
                {
                    float sum = 0.0f;

                    for (int k = 0; k < lhs.Cols; k++)
                        sum += lhs[i, k] * rhs[k, j];

                    rv[i, j] = sum;
                }
            }
            return rv;
        }

        public static void CopyRect(MatrixMN dst, MatrixMN src, int nDstRow, int nDstCol)
        {
            int rows = Math.Min(dst.Rows - nDstRow, src.Rows);
            int cols = Math.Min(dst.Cols - nDstCol, src.Cols);

            for (int i = nDstRow; i < nDstRow + rows; i++)
            {
                for (int j = nDstCol; j < nDstCol + cols; j++)
                    dst[i, j] = src[i - nDstRow, j - nDstCol];
            }
        }

        public float[] ToArray()
        {
            float[] p = new float[Rows * Cols];
            int k = 0;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    p[k++] = _elements[i, j];

            return p;
        }
    }

    public class VectorN
    {
        float[] _elements;

        public int Length { get { return _elements.Length; } }

        public VectorN(float[] el)
        {
            _elements = el;
        }

        public VectorN(int count)
        {
            _elements = new float[count];
        }

        public void SetZero()
        {
            for (int j = 0; j < _elements.Length; ++j)
            {
                _elements[j] = 0.0f;
            }
        }

        public float this[int i]
        {
            get { return _elements[i]; }
            set { _elements[i] = value; }
        }

        public static VectorN operator +(VectorN lhs, VectorN rhs)
        {
            VectorN rv = new VectorN(lhs.Length);

            for (int i = 0; i < lhs.Length; i++)
                rv[i] = lhs[i] + rhs[i];
            return rv;
        }

        public void MultiplyByScaler(float s)
        {
            for (int i = 0; i < Length; i++)
            {
                _elements[i] *= s;
            }
        }
    }
}
