using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Graphics
{
    class Camera
    {
        public float mAngleX = 0;
        public float mAngleY = 0;
        vec3 mDirection;
      public  vec3 mPosition;
        public vec3 mCenter;
        vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;
        public Camera()
        {
            Reset(0, 0, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 10000000);
        }

        public vec3 GetLookDirection()
        {
            return mDirection;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }

        public vec3 GetCameraTarget()
        {
            return mCenter;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            mCenter = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);
            mCenter.y = 205;
            mPosition = eyePos;
            mDirection = mCenter - mPosition;
            mRight = glm.cross(mDirection, upVec);
            mUp = upVec;
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);

            mViewMatrix = glm.lookAt(mPosition, mCenter, mUp);
        }
        public void SetHeight(float y)
        {
            mCenter.y = y;
        }
        public void UpdateViewMatrix()
        {
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , (float)(Math.Sin(mAngleY))
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            mUp = glm.cross(mRight, mDirection);

            mPosition = mCenter - (mDirection) * 6;
            
            //mPosition.y += 5;

            mViewMatrix = glm.lookAt(mPosition, mCenter, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }


        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Pitch(float angleDegrees)
        {
            mAngleY += angleDegrees;
        }

        public void Walk(float dist)
        {
            
           if(!Collided(mCenter + dist * mDirection))
                mCenter += dist * mDirection;

            valid();
        }
        public void Strafe(float dist)
        {
            if (!Collided(mCenter + dist * mRight))
                 mCenter += dist * mRight;
            valid();
        }
        public void Fly(float dist)
        {
            if (!Collided(mCenter + dist * mUp))
                mCenter += dist * mUp;
            valid();
            //  valid();
        }
        public void valid()
        {
            if (mCenter.y > 500)
                mCenter.y = 500;
            if (mCenter.y < 5)
                mCenter.y = 5;
            if (mCenter.x > 24200)
                mCenter.x = 24200;
            if (mCenter.x < -24200)
                mCenter.x = -24200;
            if (mCenter.z > 24200)
                mCenter.z = 24200;
            if (mCenter.z < -24200)
                mCenter.z = -24200;
        }

        double calc_distance(vec3 first, vec3 second)
        {
            return Math.Sqrt(Math.Pow((first.x - second.x), 2) + Math.Pow((first.y - second.y), 2) + Math.Pow((first.z - second.z), 2));
        }
        public bool Collided(vec3 mCenter)
        {
            for (int i = 0; i < Renderer.Obstacles.Count; i++)
            {
                vec3 curpos = Renderer.Obstacles[i].position;
                //MessageBox.Show("el distance = " + calc_distance(curpos, mCenter).ToString() +  "el radius = " + Renderer.Obstacles[i].radius.ToString());
                if (calc_distance(curpos, mCenter) < Renderer.Obstacles[i].radius)
                    return true;
            }

            return false;
        }
    }
}
