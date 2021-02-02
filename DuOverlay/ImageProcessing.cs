using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DuOverlay
{
    class ImageProcessing
    {

		private const String FULLSCREEN = "Fullscreen";
		private const String WINDOWED = "Windowed";

		//Debug function
		private void saveRGB(BitmapSource image)
		{
			StreamWriter writer;
			//Logging
			try
			{
				writer = new StreamWriter("debug.txt", true);  //Set true for append mode
			}
			catch (Exception e1)
			{
				Debug.WriteLine(e1.ToString());
				return;
			}

			//	1421/580
			int heightTracker = 580;
			int baseWidth = 1421;
			while (heightTracker > 234)
			{
				Color color = getColor(image,baseWidth, heightTracker);

				int red1 = (int)(color.R);
				int red2 = (int)(color.B);
				int red3 = (int)(color.G);


				try
				{
					writer.WriteLine(red1.ToString() + "\t" + red2.ToString() + "\t" + red3.ToString());
				}
				catch (IOException e)
				{
					Debug.WriteLine(e.ToString());
				}

				heightTracker--;
			}


			try
			{
				writer.Write("\n\n\n\n");
				writer.Close();
			}
			catch (IOException e)
			{
				Debug.WriteLine(e.ToString());
			}
		}

		private int getFinalPos(int heightTracker, BitmapSource image, int baseWidth)
		{

			int count = 0;
			while (true)
			{
				heightTracker--;
				count++;
				Color color = getColor(image,baseWidth, heightTracker);
				Color nextColor = getColor(image,baseWidth, heightTracker - 1);

				double blue = color.B + nextColor.B;
				if (blue < 388)
				{
					Console.WriteLine("Count: " + count);
					return count;
				}
			}
		}

		private static Color getColor(BitmapSource bitmap, int x, int y)
		{
			Color color;
			var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
			var bytes = new byte[bytesPerPixel];
			var rect = new Int32Rect(x, y, 1, 1);

			bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

			if (bitmap.Format == PixelFormats.Bgra32)
			{
				color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
			}
			else if (bitmap.Format == PixelFormats.Bgr32)
			{
				color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
			}
			// handle other required formats
			else
			{
				color = Colors.Black;
			}

			return color;
		}

		public bool isOreRange(Color color1, Color color2)
        {
			int red = color1.R + color2.R;
			int blue = color1.B + color2.B;
			int green = color1.G + color2.G;

			if(red > 141 && red < 283 && blue >142 && blue <403 && green > 142 && green <345)
            {
				return true;
            }
			return false;
        }

		public double getOreDistance(BitmapSource image)
		{
			//16:9
			double ratioX = 2843.0 / 3840.0;
			double ratioY = 1163.0 / 2160.0;

			double height;
			int baseHeight, baseWidth, heightTracker;

			const string screenChoiceMenu = FULLSCREEN; //TODO MAKE SETTINGS!

			if (screenChoiceMenu.Equals(WINDOWED))
			{
				height = image.Height - 32;
				baseHeight = (int)(ratioY * height) + 31;
				baseWidth = (int)Math.Round(ratioX * (image.Width - 2)) + 1;
			}
			else
			{
				height = image.Height;
				baseHeight = (int)(ratioY * height);
				baseWidth = (int)Math.Round(ratioX * image.Width);
			}
			heightTracker = baseHeight;

			while (heightTracker > 1)
			{
				Color color = getColor(image,baseWidth, heightTracker);
				Color nextColor = getColor(image,baseWidth, heightTracker - 1);

				if (isOreRange(color,nextColor))
				{
					int count = getFinalPos(heightTracker, image, baseWidth);

					if (count > 3)
					{
						heightTracker -= count / 2;
						break;
					}
				}

				heightTracker--;
			}

			int finalHeight = baseHeight - heightTracker;
			double stepper = 172.0 / 2160.0 * height;
			double finalAmount = (100.0 / stepper) * finalHeight;


			if (finalAmount < 10 || finalAmount > 674)
			{
				MessageBox.Show("Scanner lines not found! Check instructions!","ERROR!", MessageBoxButton.OK,MessageBoxImage.Error);
				finalAmount = 0;
			}

			finalAmount = Math.Round(finalAmount, 5);

			return finalAmount;
		}

	}
}
