using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace DuOverlay
{
    class ImageProcessing
    {
		//Debug function
		private void saveRGB(Bitmap image)
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
				Color color = image.GetPixel(baseWidth, heightTracker);

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

		private int getFinalPos(int heightTracker, Bitmap image, int baseWidth)
		{
			int count = 0;
			while (heightTracker > 1)
			{
				heightTracker--;
				count++;
				Color color = image.GetPixel(baseWidth, heightTracker);
				Color nextColor = image.GetPixel(baseWidth, heightTracker - 1);

				if (!isOreRange(color,nextColor))
				{
					return count;
				}
			}

			return 0;
		}

		public bool isOreRange(Color color1, Color color2)
        {
			int red = color1.R + color2.R;
			int blue = color1.B + color2.B;
			int green = color1.G + color2.G;

			if(red > 141 && red < 283 && blue >142 && blue <403 && green > 142 && green <345)
            {
				return false;
            }
			return true;
        }

		public double getOreDistance(Bitmap image, bool showWarning)
		{
			//16:9
			double ratioX = 2843.0 / 3840.0;
			double ratioY = 1163.0 / 2160.0;

			double height;
			int baseHeight, baseWidth, heightTracker;

			SingletonSettings. DISPLAY_MODES screenChoiceMenu = SingletonSettings.Instance.getDisplayMode();

			if (SingletonSettings.DISPLAY_MODES.WINDOWED == screenChoiceMenu)
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
				Color color = image.GetPixel(baseWidth, heightTracker);
				Color nextColor = image.GetPixel(baseWidth, heightTracker - 1);

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
				if(showWarning)
					MessageBox.Show("Scanner lines not found! Check instructions!","ERROR!", MessageBoxButton.OK,MessageBoxImage.Error);
				finalAmount = 0;
			}

			finalAmount = Math.Round(finalAmount, 5);

			return finalAmount;
		}

	}
}
