using System;
using Numerics;

namespace Sample3DViewer
{
    public class MatrixTransformation
    {
        MatrixMN _M = new MatrixMN(4, 4);

        public MatrixTransformation()
        {
            _M.SetIdentity();
        }

        public void Reset()
        {
            _M.SetIdentity();
        }

        public void Scale(float dx, float dy, float dz)
        {
            MatrixMN A = new MatrixMN(4, 4);

            A.SetIdentity();
            A[0, 0] = dx;
            A[1, 1] = dy;
            A[2, 2] = dz;

            _M = _M * A;
        }

        public void Translate(float dx, float dy, float dz)
        {
            MatrixMN A = new MatrixMN(4, 4);

            A.SetIdentity();
            A[0, 3] = dx;
            A[1, 3] = dy;
            A[2, 3] = dz;

            _M = _M * A;
        }

        public void Translate(VectorN v)
        {
            MatrixMN A = new MatrixMN(4, 4);
            A.SetIdentity();
            A[0, 3] = v[0];
            A[1, 3] = v[1];
            A[2, 3] = v[2];
            _M = _M * A;
        }

        public VectorN Transform(float a, float b, float c, float d)
        {
            VectorN v = new VectorN(4);
            v[0] = a; v[1] = b; v[2] = c; v[3] = d;
            return _M * v;
        }

        public void PerspectiveFovRH(float fieldOfViewVertical,
                                float nearViewPlane, float farViewPlane,
                                float viewportWidth, float viewportHeight)
        {
            _M.SetZero();

            float focalLength = (float)(1.0 / Math.Tan(fieldOfViewVertical / 2.0));
            float alpha = (focalLength * viewportHeight);
            _M[0, 0] = alpha;
            _M[1, 1] = alpha;
            _M[2, 2] = farViewPlane / (farViewPlane - nearViewPlane);
            _M[3, 2] = 1.0f;
            _M[2, 3] = -nearViewPlane * farViewPlane / (farViewPlane - nearViewPlane);

            _M[0, 2] = (float)(viewportWidth / 2.0);
            _M[1, 2] = (float)(viewportHeight / 2.0);
        }

        public float[] ToArray()
        {
            return _M.ToArray();
        }
    }

    public class Camera
    {
        MatrixMN _rotations = new MatrixMN(4, 4);
        MatrixMN _translations = new MatrixMN(4, 4);
        VectorN _lookAt;

        VectorN _initialLocation;
        float _initialTheta;
        float _initialPsi;

        public Camera(VectorN lookAt, VectorN location, float theta, float psi)
        {
            _lookAt = lookAt;
            _initialLocation = location;
            _initialTheta = theta;
            _initialPsi = psi;

            Reset();
        }

        public void Reset()
        {
            _rotations.SetIdentity();
            Rotate(_initialTheta, _initialPsi);
            _translations.SetIdentity();
            Translate(_initialLocation);
        }

        public void Rotate(float dTheta, float dPsi)
        {
            MatrixMN R1 = new MatrixMN(4, 4);
            R1.SetIdentity();

            //Rotate around Y
            R1[0, 0] = (float)Math.Cos(dTheta); R1[0, 2] = -(float)Math.Sin(dTheta);
            R1[2, 0] = (float)Math.Sin(dTheta); R1[2, 2] = (float)Math.Cos(dTheta);

            MatrixMN R2 = new MatrixMN(4, 4);
            R2.SetIdentity();

            //Rotate around X
            R2[1, 1] = (float)Math.Cos(dPsi); R2[1, 2] = -(float)Math.Sin(dPsi);
            R2[2, 1] = (float)Math.Sin(dPsi); R2[2, 2] = (float)Math.Cos(dPsi);

            _rotations = R1 * R2 * _rotations;
        }

        public void Translate(VectorN d)
        {
            _translations[0, 3] = d[0];
            _translations[1, 3] = d[1];
            _translations[2, 3] = d[2];
        }

        public void TranslateX(float dx) { _translations[0, 3] = dx; }
        public void TranslateY(float dy) { _translations[1, 3] = dy; }
        public void TranslateZ(float dz) { _translations[2, 3] = dz; }

        public void DollyZoom(float dz)
        {
            if (_translations[2, 3] - dz > 0)
                _translations[2, 3] -= dz;
        }

        public float[] GetTransform()
        {
            float[] ret = new float[16];
            MatrixMN M = new MatrixMN(4, 4);
            M.SetIdentity();

            MatrixMN N = new MatrixMN(4, 4);
            N.SetIdentity();

            N[0, 3] = -_lookAt[0];
            N[1, 3] = -_lookAt[1];
            N[2, 3] = -_lookAt[2];

            M = _translations * _rotations;
            M = M * N;
            for (int i = 0; i < 16; i++)
            {
                ret[i] = M[i / 4, i % 4];
            }
            return ret;
        }
    }
}
