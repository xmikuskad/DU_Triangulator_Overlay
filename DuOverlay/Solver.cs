using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace DuOverlay
{
	public class Solver
	{
		private const int FIELD_COUNT = 4;
		private StreamWriter writer;
		private string reader;
		private const string fileName = "log.txt";
		private const string readerFileName = "resources/readerFile.txt";

		private List<List<Double>> positions = new List<List<Double>>();
		private List<Double> other = new List<Double>();

		//Prve pocty
		private double[] sValue = new double[FIELD_COUNT];
		private double[] aValue = new double[FIELD_COUNT];
		private double[] bValue = new double[FIELD_COUNT];
		private double[] cValue = new double[FIELD_COUNT];

		private double x1, y1, z1, a, b, c, R;

		//Main function
		public void startSolve(List<String> varList, MainWindow con)
		{
			//Logging
			try
			{
				checkFileSize();
				writer = new StreamWriter(fileName, true);  //Set true for append mode
				writeToFile("\nNew logging", DateTime.Now.ToString());
			}
			catch (Exception e1)
			{
				con.hideClipMsg();
				Debug.WriteLine(e1.ToString());
				return;
			}

			//Opening reader file
			try
			{
				reader = Properties.Resources.readerFile.ToString();
			}
			catch (Exception e2)
			{
				Debug.WriteLine(e2.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
				con.hideClipMsg();
				writeToFile("ERROR!", "Cannot find fileReader.txt");
				MessageBox.Show("Cannot find fileReader.txt", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			//Checking input and loading variables
			try
			{
				checkInput(varList, con);
			}
			catch (Exception e)
			{
				con.setResult("");
				MessageBox.Show("Input problem!\nProblem: " + e.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
				con.hideClipMsg();
				writeToFile("ERROR!", "Input problem! MSG: " + e.ToString());

				try
				{
					writer.Close();
				}
				catch (Exception e2)
				{
					Debug.WriteLine(e2.ToString());
				}
				return;
			}


			//Making calculations
			try
			{
				prepare();

				makeCalculations();

				finishIt(x1, y1, z1, con);

				con.showClipMsg();
			}
			catch (Exception e)
			{
				MessageBox.Show("Calculation problem!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
				con.setResult("");
				con.hideClipMsg();
				writeToFile("ERROR", "Calculation problem! " + e.ToString());
			}
			finally
			{
				try
				{
					writer.Close();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
			}

			void finishIt(double x, double y, double z, MainWindow con)
			{
				double s = Math.Sqrt(x * x + y * y + z * z);
				double v = Math.Sqrt(x * x + y * y);
				a = Math.Acos(z / s) * 180 / Math.PI - 90;
				b = Math.Acos(x / v) * 180 / Math.PI - 180;
				c = s - R;

				if (y < 0)
				{
					b = -b;
				}

				a = decimal.ToDouble(decimal.Round(new decimal(a), 4, MidpointRounding.ToEven));
				b = decimal.ToDouble(decimal.Round(new decimal(b), 4, MidpointRounding.ToEven));
				c = decimal.ToDouble(decimal.Round(new decimal(c), 4, MidpointRounding.ToEven));

				writeToFile("s", s.ToString());
				writeToFile("a", a.ToString());
				writeToFile("b", b.ToString());
				writeToFile("c", c.ToString());

				String result = "::pos{" + positions[0][0] + "," + positions[0][1] + "," + a + "," + b + "," + c + "}";

				writeToFile("Final pos is", result);

				con.setResult(result);

				Clipboard.SetText(result);

			}

			void prepare()
			{
				for (int i = 0; i < FIELD_COUNT; i++)
				{

					sValue[i] = R + positions[i][4];
					aValue[i] = sValue[i] * Math.Sin(positions[i][2] * Math.PI / 180 + Math.PI / 2) * Math.Cos(positions[i][3] * Math.PI / 180 + Math.PI);
					bValue[i] = sValue[i] * Math.Sin(positions[i][2] * Math.PI / 180 + Math.PI / 2) * Math.Sin(positions[i][3] * Math.PI / 180 + Math.PI);
					cValue[i] = sValue[i] * Math.Cos(positions[i][2] * Math.PI / 180 + Math.PI / 2);

				}

				for (int i = 0; i < FIELD_COUNT; i++)
					writeToFile("A", aValue[i].ToString());

				for (int i = 0; i < FIELD_COUNT; i++)
					writeToFile("B", bValue[i].ToString());

				for (int i = 0; i < FIELD_COUNT; i++)
					writeToFile("C", cValue[i].ToString());

				for (int i = 0; i < FIELD_COUNT; i++)
					writeToFile("S", sValue[i].ToString());

			}

			void makeCalculations()
			{
				double[] deltaA = new double[3];
				double[] deltaB = new double[3];
				double[] deltaC = new double[3];
				double p1, p2, p3;
				double D1, D2, A1, A2, B1, B2;

				for (int i = 0; i < 3; i++)
				{
					deltaA[i] = aValue[0] - aValue[1 + i];
					deltaB[i] = bValue[0] - bValue[1 + i];
					deltaC[i] = cValue[0] - cValue[1 + i];
				}

				p1 = other[0] * other[0] - aValue[0] * aValue[0] - bValue[0] * bValue[0] - cValue[0] * cValue[0] - other[1] * other[1] + aValue[1] * aValue[1] + bValue[1] * bValue[1] + cValue[1] * cValue[1];
				p2 = other[0] * other[0] - aValue[0] * aValue[0] - bValue[0] * bValue[0] - cValue[0] * cValue[0] - other[2] * other[2] + aValue[2] * aValue[2] + bValue[2] * bValue[2] + cValue[2] * cValue[2];
				p3 = other[0] * other[0] - aValue[0] * aValue[0] - bValue[0] * bValue[0] - cValue[0] * cValue[0] - other[3] * other[3] + aValue[3] * aValue[3] + bValue[3] * bValue[3] + cValue[3] * cValue[3];

				A1 = -4 * deltaA[0] * deltaC[1] + 4 * deltaA[1] * deltaC[0];
				B1 = -4 * deltaB[0] * deltaC[1] + 4 * deltaB[1] * deltaC[0];
				A2 = -4 * deltaA[0] * deltaC[2] + 4 * deltaA[2] * deltaC[0];
				B2 = -4 * deltaB[0] * deltaC[2] + 4 * deltaB[2] * deltaC[0];
				D1 = (2 * p1 * deltaC[1] - 2 * p2 * deltaC[0]);
				D2 = (2 * p1 * deltaC[2] - 2 * p3 * deltaC[0]);

				writeToFile("A1", A1.ToString());
				writeToFile("B1", B1.ToString());
				writeToFile("A2", A2.ToString());
				writeToFile("B2", B2.ToString());
				writeToFile("D1", D1.ToString());
				writeToFile("D2", D2.ToString());

				x1 = (D1 * B2 - D2 * B1) / (A1 * B2 - A2 * B1);
				y1 = (D1 - A1 * x1) / B1;
				z1 = (p1 + 2 * deltaA[0] * x1 + 2 * deltaB[0] * y1) / (-2 * deltaC[0]);

				writeToFile("x1", x1.ToString());
				writeToFile("y1", y1.ToString());
				writeToFile("z1", z1.ToString());

				//Check for bullshit
				double max = Math.Max(Math.Max(other[0], other[1]), Math.Max(other[2], other[3]));

				double min = 1000;
				int min_index = 0;
				for (int i = 0; i < FIELD_COUNT; i++)
				{
					if (other[i] < min)
					{
						min = other[i];
						min_index = i;
					}
				}

				double tmp = Math.Pow(x1 - aValue[min_index], 2) + Math.Pow(y1 - bValue[min_index], 2) + Math.Pow(z1 - cValue[min_index], 2);

				if (tmp > max * max)
				{
					writeToFile("Bullshit check failed, sorry", "");
					throw new Exception("BULLSHIT CHECK!");
				}
			}

			double getRValue(String value1, String value2)
			{
				List<String> list = new List<String>(reader.Split("\n"));
				String tmp;

				/*while ((tmp = reader.ReadLine()) != null)
				{
					list.Add(tmp);
				}*/

				double value = -1;

				foreach (String str in list)
				{
					String tmpValue = str.Split("=")[1];
					String val1 = str.Split("=")[0].Split(",")[0];
					String val2 = str.Split("=")[0].Split(",")[1];

					if (value1.Equals(val1) && value2.Equals(val2))
					{
						Double foundValue = double.Parse(tmpValue);
						value = Math.Sqrt(foundValue / (4 * Math.PI));
						break;
					}
				}

				return value;
			}

			void checkInput(List<String> varList, MainWindow con)
            {
                for (int i = 0; i < FIELD_COUNT; i++)
				{

					positions.Add(new List<Double>());

					String pos = varList[i * 2];
					String otherVar = varList[i * 2 + 1];

					if (string.IsNullOrEmpty(pos))
					{
						throw new Exception("Position " + (i + 1) + " is empty");
					}

					if (string.IsNullOrEmpty(otherVar))
					{
						throw new Exception("Ore distance " + (i + 1) + " is empty!");
					}

					//Get inputs from string
					List<String> tmp = new List<String>(pos.Split("{"));

					if (tmp.Count <= 1)
					{
						throw new Exception("Position " + (i + 1) + " is badly formatted!");
					}

					String positionsRaw = tmp[1].Split("}")[0];

					List<String> tmpList = new List<String>(positionsRaw.Split(","));

					//Saving values to list
					try
					{
						foreach (String str in tmpList)
						{
							positions[i].Add(Double.Parse(str));
						}
					}
					catch (Exception)
					{
						throw new Exception("Position " + (i + 1) + " is badly formatted!");
					}

					try
					{
						other.Add(Double.Parse(otherVar));
					}
					catch (Exception)
					{
						throw new Exception("Ore distance " + (i + 1) + " is badly formatted!");
					}

					//Only calculate once
					if (i == 0)
					{
						//Prepping R value
						if (reader == null)
						{
							throw new Exception("Reader file not found");
						}
						R = getRValue(tmpList[0], tmpList[1]);
						if (R < 0)
						{
							R = 120000;
							MessageBox.Show("Calculations for this planet may be inaccurate!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
						}
					}
				}

				double pos1 = positions[0][0];
				double pos2 = positions[0][1];

				for (int i = 1; i < FIELD_COUNT; i++)
				{
					if (pos1 != positions[i][0] || pos2 != positions[i][1])
						throw new Exception("All positions have to be on same planet!");
				}

				reorderBySmallest();

			}

			void reorderBySmallest()
			{
				for (int i = 0; i < 3; i++)
				{
					double min = 99999.0;
					int minIndex = -1;
					for (int j = i; j < 4; j++)
					{
						if (other[j] < min)
						{
							min = other[j];
							minIndex = j;
						}
					}

					if (minIndex < 0)
						throw new Exception("Ore distances are too big!");

					double tmp = other[i];
					other[i] = min;
					other[minIndex] = tmp;

					List<Double> tmpList = positions[i];
					positions[i] = positions[minIndex];
					positions[minIndex] = tmpList;
				}

			}

			void checkFileSize()
			{
				FileInfo file = new FileInfo(fileName);
				if (file == null || !file.Exists) return;

				//If log file is bigger than 20mb, we should delete it
				if (getFileSizeMegaBytes(file) > 20)
				{
					File.Delete(fileName);
				}
			}

			double getFileSizeMegaBytes(FileInfo file)
			{
				return (double)file.Length / (1024 * 1024);
			}

			//Used for logging
			void writeToFile(String varName, String value)
			{
				try
				{
					writer.WriteLine(varName + " " + value);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
				}
			}

		}
	}
}
