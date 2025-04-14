using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResolutionTool
{
    public partial class Form1 : Form
    {
        
        List<string> files = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            List<Bitmap> bitmaps = new List<Bitmap>();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string selectedDirectory = folderBrowser.SelectedPath;
                textBoxPath.Text = selectedDirectory;
            }
        }

        void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        files.Add(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {            
            string selectedDirectory = textBoxPath.Text;
            files.Clear();
            files.AddRange(Directory.GetFiles(textBoxPath.Text));

            List<string> newResFileNames= new List<string>();
            List<string> corruptedFiles = new List<string>();

            if (files.Count <= 0)
                MessageBox.Show("There are no files: " + selectedDirectory);
            else
            {
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string fullPath = Path.Combine(selectedDirectory, fileName);

                    try
                    {
                        string tempFile = Path.Combine($"{fullPath}_temp");
                        bool isSaved;

                        using (Bitmap bitmap = new Bitmap(file))
                        {
                            isSaved = bitmap.HorizontalResolution < 299 || bitmap.VerticalResolution < 299;
                        }

                        if (isSaved)
                        {
                            List<byte[]> bmpList = SplitMultitif(file);

                            JoinTiffImages(bmpList, tempFile, EncoderValue.CompressionCCITT4);

                            File.Replace(tempFile, file, null);

                            newResFileNames.Add(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        corruptedFiles.Add(file);
                        continue;
                    }

                }
            }

            if (newResFileNames.Count > 0)
            {
                MessageBox.Show(newResFileNames.Count + $" image(s) successfully changed resolution. \n{corruptedFiles.Count} corrupted image(s) has been detected.");
                NewResolutionFiles newResolutionFilesForm = new NewResolutionFiles();
                newResolutionFilesForm.listBoxNewResFiles.Items.AddRange(newResFileNames.Select(x => $"{x} successfully changed resolution").ToArray());
                newResolutionFilesForm.listBoxNewResFiles.Items.AddRange(corruptedFiles.Select(x => $"{x} corrupted file").ToArray());

                newResolutionFilesForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No file has been changed.");
            }
        }

        public static List<byte[]> SplitMultitif(string fileFullPath)
        {
            List<byte[]> images = null;
            Bitmap[] splitedImages = null;
            try
            {
                images = new List<byte[]>();
                using (FileStream fs = File.Open(fileFullPath, FileMode.Open, FileAccess.Read))
                {
                    splitedImages = SeparateMultiTIFF2BitmapArray(new Bitmap(fs));

                    foreach (Bitmap bmp in splitedImages)
                    {
                        MemoryStream stream = new MemoryStream();

                        ImageCodecInfo imageCodecInfo = null;
                        foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders())
                        {
                            if (ice.MimeType == "image/tiff")
                            {
                                imageCodecInfo = ice;
                                break;
                            }
                        }

                        System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Compression;
                        EncoderParameters encoderParameters = new EncoderParameters(1);

                        // Save the bitmap as a TIFF file with group IV compression.
                        EncoderParameter encoderParameter = new EncoderParameter(encoder, (long)EncoderValue.CompressionCCITT4);
                        encoderParameters.Param[0] = encoderParameter;

                        bmp.Save(stream, imageCodecInfo, encoderParameters);
                        stream.Position = 0;
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, Convert.ToInt32(stream.Length));
                        images.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (splitedImages != null && splitedImages.Length != 0)
                    foreach (Bitmap bmp in splitedImages)
                        bmp.Dispose();
                GC.Collect(3, GCCollectionMode.Forced);
            }
            return images;
        }

        public static Bitmap[] SeparateMultiTIFF2BitmapArray(Bitmap bitmap)
        {
            try
            {
                int pageCount = bitmap.GetFrameCount(FrameDimension.Page);
                Bitmap[] imageArray = new Bitmap[pageCount];
                for (int i = 0; i < pageCount; i++)
                {
                    bitmap.SelectActiveFrame(FrameDimension.Page, i);

                    if (bitmap != null)
                    {
                        imageArray[i] = ConvertToBitonal(bitmap);
                        imageArray[i].SetResolution(300, 300);
                    }
                    else
                    {
                        imageArray[i] = null;
                    }
                }

                bitmap.Dispose();
                bitmap = null;

                GC.Collect(3, GCCollectionMode.Forced);

                return imageArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void JoinTiffImages(List<byte[]> images, string outFile, EncoderValue compressEncoder)
        {
            Image pages = null;
            Image bm = null;
            EncoderParameters ep = null;
            try
            {
                System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;

                ep = new EncoderParameters(2);
                ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
                ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)compressEncoder);

                int frame = 0;
                ImageCodecInfo info = GetEncoderInfo("image/tiff");

                foreach (byte[] frameBytes in images)
                {
                    if (frame == 0)
                    {
                        ImageConverter ic = new ImageConverter();
                        pages = (Image)ic.ConvertFrom(frameBytes);

                        //save the first frame
                        pages.Save(outFile, info, ep);
                    }
                    else
                    {
                        //save the intermediate frames
                        ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);

                        ImageConverter ic = new ImageConverter();
                        bm = (Image)ic.ConvertFrom(frameBytes);

                        pages.SaveAdd(bm, ep);
                        bm.Dispose();
                    }

                    if (frame == images.Count - 1)
                    {
                        //flush and close.
                        ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                        pages.SaveAdd(ep);
                    }
                    frame++;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pages != null)
                    pages.Dispose();
                if (bm != null)
                    bm.Dispose();
                if (ep != null)
                    ep.Dispose();
                GC.Collect(3, GCCollectionMode.Forced);
            }
        }

        public static void SaveToTiffCCITT4(string filePath, Image img)
        {
            Bitmap bitonalBitmap = null;
            bool dispose = true;
            dispose = !EnsureBitonal((Bitmap)img, out bitonalBitmap);

            // Get an ImageCodecInfo object that represents the TIFF codec.
            ImageCodecInfo imageCodecInfo = null;
            foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders())
            {
                if (ice.MimeType == "image/tiff")
                {
                    imageCodecInfo = ice;
                    break;
                }
            }
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Compression;
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the bitmap as a TIFF file with group IV compression.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, (long)EncoderValue.CompressionCCITT4);
            encoderParameters.Param[0] = encoderParameter;
            bitonalBitmap.Save(filePath, imageCodecInfo, encoderParameters);

            if (dispose)
            {
                bitonalBitmap.Dispose();
                bitonalBitmap = null;
            }
        }

        public static bool EnsureBitonal(Bitmap bmp, out Bitmap bitonal)
        {
            if (bmp.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                bitonal = bmp;
                return true;
            }
            else
            {
                bitonal = ConvertToBitonal(bmp);
                return false;
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }

            throw new Exception(mimeType + " mime type not found in ImageCodecInfo");
        }

        public static Bitmap ConvertToBitonal(Bitmap original)
        {
            if (original == null)
                return null;
            if (original.PixelFormat == PixelFormat.Format1bppIndexed)
                return CloneBitmap(original);

            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (original.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImageUnscaled(original, 0, 0);
                }
            }
            else
            {
                source = original;
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
            destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            GC.Collect(3, GCCollectionMode.Forced);

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Return
            return destination;
        }

        public static Bitmap CloneBitmap(Bitmap bitmap)
        {
            return CloneBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        public static Bitmap CloneBitmap(Bitmap bitmap, Rectangle cutRectangle)
        {
            try
            {
                if (cutRectangle.Width <= 0)
                {
                    cutRectangle.Width = 1;
                }
                if (cutRectangle.Height <= 0)
                {
                    cutRectangle.Height = 1;
                }
                BitmapData originalBD = bitmap.LockBits(cutRectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                Bitmap returnBitmap = new Bitmap(cutRectangle.Width, cutRectangle.Height, bitmap.PixelFormat);
                returnBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                switch (returnBitmap.PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Indexed:
                        returnBitmap.Palette = bitmap.Palette;
                        break;
                    default:
                        break;
                }
                BitmapData resultBD = returnBitmap.LockBits(new Rectangle(0, 0, returnBitmap.Width, returnBitmap.Height),
                                                            ImageLockMode.WriteOnly, bitmap.PixelFormat);

                int size = resultBD.Stride;
                byte[] PixelArr = new byte[size];
                int numOfRows = returnBitmap.Height;
                unsafe
                {
                    byte* sourcePoint = (byte*)originalBD.Scan0.ToPointer();
                    byte* destPoint = (byte*)resultBD.Scan0.ToPointer();

                    for (int i = 0; i < numOfRows; i++)
                    {
                        Marshal.Copy((IntPtr)sourcePoint, PixelArr, 0, size);
                        Marshal.Copy(PixelArr, 0, (IntPtr)destPoint, size);
                        sourcePoint += originalBD.Stride;
                        destPoint += size;
                    }
                }
                bitmap.UnlockBits(originalBD);
                returnBitmap.UnlockBits(resultBD);
                return returnBitmap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
