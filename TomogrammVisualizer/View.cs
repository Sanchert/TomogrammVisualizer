using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TomogrammVisualizer
{
    class View
    {
        public void SetupView(int width, int height)
        {
            // Левосторонняя система:
            //
            // начало координат - нижний левый угол:
            //     оси {X,Y,Z} -> положительные направления {вправо, вверх, веред}
            //     Matrix4.CreateRotation{X,Y,Z} вращает вокруг ОСИ против часовой стрелки

            //Matrix4.CreateRotationX(45f * (float)Math.PI / 180f, out Matrix4 rotX); - вращает вокруг оси
            //GL.Rotate(-30f * (float)Math.PI / 180f, vector); // вращает вокруг вектора 
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Frustum(0, Bin.X, 0, Bin.Y, -512, 512);
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 512);
            GL.Viewport(0, 0, width, height);
        }

        protected Color TransferFunction(short value, int min, int width)
        {
            int max = min + width;
            int newVal = Clamp((value - min) * 255 / (max - min + 1), 0, 255);
            return Color.FromArgb(newVal, newVal, newVal);
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public void DrawQuadsVertical(int layerNumber, int min, int width)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            int scale = Bin.Y / Bin.Z;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for (int x = 0; x < Bin.X - 1; x++)
                for (int y = 0; y < Bin.Z - 1; y++)
                {
                    short val;

                    val = Bin.array[x + y * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x, y * scale);

                    val = Bin.array[x + 1 + y * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x + 1, y * scale);

                    val = Bin.array[x + 1 + (y + 1) * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x + 1, (y + 1) * scale);

                    val = Bin.array[x + (y + 1) * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x, (y + 1) * scale);
                }
            GL.End();
        }


        public void DrawQuads(int layerNumber, int min, int width)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {
                    short value;

                    //1 вершина
                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord, y_coord);

                    //2 вершина
                    value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord + 1, y_coord);


                    //3 вершина
                    value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    //4 вершина
                    value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord, y_coord + 1);
                }
            GL.End();
        }
        public void DrawQuadStripVertical(int layerNumber, int min, int width)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            int scale = Bin.Y / Bin.Z;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int y = 0; y < Bin.Z - 1; y++)
            {
                GL.Begin(BeginMode.QuadStrip);
                for (int x = 0; x < Bin.X; x++)
                {
                    short val;

                    val = Bin.array[x + y * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x, y * scale);

                    val = Bin.array[x + 1 + y * Bin.X * Bin.Y + layerNumber * Bin.X];
                    GL.Color3(TransferFunction(val, min, width));
                    GL.Vertex2(x, (y + 1) * scale);

                }
                GL.End();
            }
        }

        public void DrawQuadStrip(int layerNumber, int min, int width)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
            {
                GL.Begin(BeginMode.QuadStrip);
                for (int x_coord = 0; x_coord < Bin.X; x_coord++)
                {
                    short value;

                    //1 вершина
                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord, y_coord);

                    //2 вершина
                    value = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value, min, width));
                    GL.Vertex2(x_coord, y_coord + 1);
                }
                GL.End();
            }
        }

        private Bitmap textureImage;
        private int VBOtexture;
        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0,0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMagFilter.Linear);

            ErrorCode er = GL.GetError();
            string str = er.ToString();
        }

        public void generateTextureImage(int layerNumber, int min, int width)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);

            for (int i = 0; i < Bin.X; i++)
                for (int j = 0;  j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber], min, width));
                }
        }

        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            
            GL.Color3(Color.White);
            
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}
