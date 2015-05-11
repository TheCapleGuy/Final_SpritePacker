using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace SPack_MKII
{
    class SpritePacker
    {
        public List<MyImage> ImageList = new List<MyImage>();
        public int currentRow = 0;
        public void SaveToXML(string fName)
        {
            //parsing out file name to c:\\blah\"picture".png
            //only taking "picture"
            int lastIndex = fName.LastIndexOf('\\');
            string file = fName.Substring(lastIndex + 1);
            lastIndex = file.LastIndexOf('.');
            string xml = "";
            if(file != "")
            {
                xml = file.Substring(0, lastIndex) + ".xml";
            }
            //get document and atlas values set for xml
            XmlDocument doc = new XmlDocument();
            //decleration
            XmlDeclaration xmlDec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(xmlDec);
            XmlElement Atlas = doc.CreateElement("TextureAtlas");
            Atlas.SetAttribute("ImagePath", file);
            for (int i = 0; i < ImageList.Count(); i++ )
            {
                XmlElement SpriteChild = doc.CreateElement("Sprite");
                //add X value to xml
                string x = ImageList[i].X.ToString();
                SpriteChild.SetAttribute("X", x);
                //add Y value to xml
                string y = ImageList[i].Y.ToString();
                SpriteChild.SetAttribute("Y", y);
                //add Width to xml
                string width = ImageList[i].Width.ToString();
                SpriteChild.SetAttribute("Width", width);
                //add Height to xml
                string height = ImageList[i].Height.ToString();
                SpriteChild.SetAttribute("Height", height);

                Atlas.AppendChild(SpriteChild);
            }

            doc.AppendChild(Atlas);
            if(xml != "")
            {
                doc.Save(xml);
            }
            else
            {
                return;
            }
        }
        public string[] GetFileNames()
        {
            string[] fileName = new string[]{};
            //create open dialog instance
            OpenFileDialog ODlog = new OpenFileDialog();
            //set filter options
            ODlog.Filter = "All Files (*.*)| *.*";
            ODlog.FilterIndex = 1;
            ODlog.Multiselect = true;
            //call show dialog if clicked ok
            bool? userClickOk = ODlog.ShowDialog();
            //check and process if clicked
            if (userClickOk == true)
            {
                //System.IO.Stream fileStream = new System.IO.FileStream(ODlog.FileName, System.IO.FileMode.Open);
                fileName = ODlog.FileNames;
            }
            Console.WriteLine("IMPORTED");
            return fileName;
        }
        public void SaveFileName(string path, Canvas canvas)
        {
            if (null == path) return;
            //Save current canvas transform
            Transform transform = canvas.LayoutTransform;
            //reset transform in case scaled or reset
            canvas.LayoutTransform = null;
            //get size of the canvas
            Size size = new Size(canvas.Width, canvas.Height);
            //measure and arrange the canvas
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            // Create a render bitmap and push the canvas onto it
            RenderTargetBitmap renderMap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderMap.Render(canvas);
            try
            {
                using (FileStream outsream = new FileStream(path, FileMode.Create))
                {
                    //png encoder for the data
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    //push the rendered bitmap to it
                    encoder.Frames.Add(BitmapFrame.Create(renderMap));
                    //save to the stream
                    encoder.Save(outsream);
                }
            }
           catch(System.ArgumentException er)
            {
                Console.WriteLine("not a legal file name" + er);
            }
        }
        public void AddToCanvas(string fileName, Canvas canvasControl)
        {
            if (fileName != null)
            {
                BitmapImage tBImage = new BitmapImage(new Uri(fileName));
                MyImage imageToCanvas = new MyImage();
                //if first image
                if(ImageList.Count() == 0)
                {
                    imageToCanvas.X = imageToCanvas.Y = 0;
                    //adjust canvas height
                    canvasControl.Height = tBImage.Height;
                }
                else
                {
                    //if image has gone passed max width
                    if (ImageList.Last().X + Convert.ToInt32(ImageList.Last().Width) + tBImage.Width > canvasControl.MaxWidth)
                    {
                        imageToCanvas.X = 0;
                        imageToCanvas.Y += ImageList.Last().Y + GetImageHeight(currentRow, tBImage);
                        //adjust canvas height
                        canvasControl.Height += GetImageHeight(currentRow, tBImage);
                        currentRow++;
                    }
                    //rest of images
                    else
                    {
                        imageToCanvas.X = ImageList.Last().X + Convert.ToInt32(ImageList.Last().Width);
                        imageToCanvas.Y = ImageList.Last().Y;
                    }
                }
                imageToCanvas.Source = tBImage;
                imageToCanvas.Width = tBImage.Width;
                imageToCanvas.Height = tBImage.Height;
                imageToCanvas.rowPos = currentRow;
                ImageList.Add(imageToCanvas);

                Canvas.SetTop(imageToCanvas, imageToCanvas.Y);
                Canvas.SetLeft(imageToCanvas, imageToCanvas.X);
                canvasControl.Children.Add(imageToCanvas);
                UpdateCanvasWidth(canvasControl);
            }
        }
        public int GetImageHeight(int cRow, BitmapImage mImage)
        {
            int heighestSoFar;
            if(ImageList.Count() == 0)
            {
                heighestSoFar = Convert.ToInt32(mImage.PixelHeight);
                return heighestSoFar;
            }
            else
            {

                if (ImageList.Count() > 0 && ImageList.Count() < 2)
                {
                    heighestSoFar = Convert.ToInt32(ImageList.Last().Height);
                }
                else
                {
                    heighestSoFar = 0;
                        //Convert.ToInt32(ImageList.First().Height);
                    for (int i = 0; i < ImageList.Count(); i++)
                    {
                        if (ImageList[i].rowPos == cRow && ImageList[i].Height > heighestSoFar)
                        {
                            heighestSoFar = Convert.ToInt32(ImageList[i].Height);
                        }
                    }
                }
                return heighestSoFar;
            }

        }
        public void UpdateCanvasWidth(Canvas canvasControl)
        {
            //data binding to set a max Width
            //to do list 
            //canvasControl.MaxWidth = Convert.ToDouble(setMaxWidth);
            if (ImageList.Count() > 0)
            {
                //Set Canvas Width based off Width of ImageLists
                double totalImageWidth = 0;
                for (int i = 0; i < ImageList.Count(); i++)
                {
                    totalImageWidth += ImageList[i].Width;
                }
                canvasControl.Width = totalImageWidth;
            }
        }
    }
}
