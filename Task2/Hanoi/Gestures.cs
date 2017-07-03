using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Xml.Serialization;
using System.IO;

namespace Hanoi
{
    class Gestures 
    {        
        private List<double> templateOne = null;

        public List<double> TemplateOne
        {
            get
            {
                return templateOne;
            }
            set
            {
                if (templateOne == null) templateOne = value;
            }
        }

        private List<double> templateTwo = null;

        public List<double> TemplateTwo
        {
            get
            {
                return templateTwo;
            }
            set
            {
                if (templateTwo == null) templateTwo = value;
            }
        }

        private List<double> templateThree;

        public List<double> TemplateThree
        {
            get
            {
                return templateThree;
            }
            set
            {
                if (templateThree == null) templateThree = value;
            }
        }

        private List<double> templateClose1;

        public List<double> TemplateClose1
        {
            get
            {
                return templateClose1;
            }
            set
            {
                if (templateClose1 == null) templateClose1 = value;
            }
        }

        private List<double> templateClose2;

        public List<double> TemplateClose2
        {
            get
            {
                return templateClose2;
            }
            set
            {
                if (templateClose2 == null) templateClose2 = value;
            }
        }

        #region Initialisation

        public Gestures()
        {
            initializeTemplates();
        }

        private void initializeTemplates()
        {
            templateOne = new List<double>();
            templateTwo = new List<double>();
            templateThree = new List<double>();
            templateClose1 = new List<double>();
            templateClose2 = new List<double>();

            MyClass.Deserialize(templateOne, @"../../template1.xml");

            MyClass.Deserialize(templateTwo, @"../../template2.xml");

            MyClass.Deserialize(templateThree, @"../../template3.xml");

            MyClass.Deserialize(templateClose1, @"../../templateClose1.xml");

            MyClass.Deserialize(templateClose2, @"../../templateClose2.xml");
        }

        #endregion

        #region Getter

        public List<double> GetTemplate(int i)
        {
            switch (i)
            {
                case 1: return templateOne;
                case 2: return templateTwo;
                case 3: return templateThree;
                case 4: return templateClose1;
                case 5: return templateClose2;
                default: return new List<double>();
            }
        }

        #endregion


        internal int GetTemplateNumber(List<Point> gesturePositions)
        {
            List<double> gestureAngles = calculateAngles(gesturePositions);

            int templateNumber = classifyGesture(gestureAngles);

            return templateNumber;
        }

        public List<double> calculateAngles(List<Point> gesturePositions)
        {
            List<double> gestureAngles = new List<double>();

            for (int i = 0; i < gesturePositions.Count - 1; i++)
            {
                if (i > 1)
                {
                    // A
                    Point p1 = gesturePositions[0];
                    // B (Mitte)
                    Point p2 = gesturePositions[i];
                    // C
                    Point p3 = gesturePositions[i + 1];

                    double a, b, c;
                    // Anliegend an Mitte: a und b
                    a = Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
                    b = Math.Sqrt(Math.Pow((p2.X - p3.X), 2) + Math.Pow((p2.Y - p3.Y), 2));
                    c = Math.Sqrt(Math.Pow((p3.X - p1.X), 2) + Math.Pow((p3.Y - p1.Y), 2));

                    //1 mittlere
                    //sqrt((P1x - P2x)2 + (P1y - P2y)2)
                    //arccos((P122 + P132 - P232) / (2 * P12 * P13))

                    // Avoid NaN due to rounding errors
                    double tmp = (Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b);
                    if (tmp > 1 || tmp < -1)
                        tmp = Math.Round(tmp);

                    double angle = RadToDeg(Math.Acos(tmp));

                    gestureAngles.Add(angle);
                }
               
            }
            
            //Creating Templates in template1,2,3.xml here
            //MyClass.SerializeObject(gestureAngles, "templateClose2.xml");

            return gestureAngles;
        }

      

        private int classifyGesture(List<double> gestureAngles)
        {                     
            double localDistanceToTemplate1 = calculateLocalDistance(gestureAngles, TemplateOne);
            double localDistanceToTemplate2 = calculateLocalDistance(gestureAngles, TemplateTwo);
            double localDistanceToTemplate3 = calculateLocalDistance(gestureAngles, TemplateThree);
            double localDistanceToClose1 = calculateLocalDistance(gestureAngles, TemplateClose1);
            double localDistanceToClose2 = calculateLocalDistance(gestureAngles, TemplateClose2);

            List<double> gestures = new List<double>();
            gestures.Add(localDistanceToTemplate1);
            gestures.Add(localDistanceToTemplate2);
            gestures.Add(localDistanceToTemplate3);
            gestures.Add(localDistanceToClose1);
            gestures.Add(localDistanceToClose2);

            // sort list by distances
            var sorted = gestures
                            .Select((x, i) => new KeyValuePair<double, int>(x, i))
                            .OrderBy(x => x.Key)
                            .ToList();

            // get index of shortest distance
            List<int> idx = sorted.Select(x => x.Value).ToList();

            // inc since index starts at 0
            return idx[0]+1;



            /*int i = 0;
            if (localDistanceToTemplate1 < localDistanceToTemplate2 && localDistanceToTemplate1 < localDistanceToTemplate3) i = 1;
            else if (localDistanceToTemplate2 < localDistanceToTemplate1 && localDistanceToTemplate2 < localDistanceToTemplate3) i = 2;
            else if (localDistanceToTemplate3 < localDistanceToTemplate1 && localDistanceToTemplate3 < localDistanceToTemplate2) i = 3;

            return i;*/
        }

        private double calculateLocalDistance(List<double> gestureAngles, List<double> templateAngles)
        {

            double[,] distances = new double[gestureAngles.Count, templateAngles.Count];
             
            //Local DistanceS
            //d(i,j) =... {}

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                double gestureAngle = DegToRad(gestureAngles[i]);

                for (int j = 0; j < distances.GetLength(1); j++)
			    {
                    double angleDistance = 0;

                    
                    double templateAngle = DegToRad(templateAngles[j]);

                    if (Math.Abs(gestureAngle - templateAngle) < Math.PI) angleDistance = ((1 / Math.PI) * Math.Abs(gestureAngle - templateAngle));
                    else angleDistance = ((1 / Math.PI) * ((2 * Math.PI) - Math.Abs(gestureAngle - templateAngle)));

                    distances[i, j] = angleDistance;
                }
            }

            //DTW Distance
            //C(i, j) = min(C(i, j -1),2×C(i -1, j -1),C(i -1, j))+ d(i, j)

            double[,] localDistances = new double[gestureAngles.Count, templateAngles.Count];


            localDistances[0, 0] = distances[0, 0];

            for (int i = 1; i < distances.GetLength(0); i++)
            {
                localDistances[i, 0] = distances[i, 0] + localDistances[i-1, 0];
            }

            for (int i = 1; i < distances.GetLength(1); i++)
            {
                localDistances[0, i] = distances[0, i] + localDistances[0, i-1];
            }

            for (int i = 1; i < localDistances.GetLength(0); i++)
            {
                for (int j = 1; j < localDistances.GetLength(1); j++)
                {
                    double[] localMinimums = new double[3];


                    localMinimums[0] = localDistances[i, j - 1];
                    localMinimums[1] = 2 * localDistances[i - 1, j - 1];
                    localMinimums[2] = localDistances[i - 1, j];

                    double localMinimum = Math.Min((Math.Min(localMinimums[0], localMinimums[1])), localMinimums[2]);
                    localDistances[i, j] = localMinimum + distances[i, j];
                }
                
            }

            //Normalizing
            //C(i,j)* 1/N+M

            double normalizerVal = (1.0 / ((double) (localDistances.GetLength(0) + localDistances.GetLength(1))));

            double distanceSum = localDistances[localDistances.GetLength(0)-1, localDistances.GetLength(1)-1] * normalizerVal;


            return distanceSum;
        }



        // Transform Radiant value to Degree
        private double RadToDeg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }

        private double DegToRad(double deg)
        {

            //deg = rad* (180/Math.Pi)

            return ( deg / (180.0 / Math.PI));
        }

    }

    #region XML Handling
    public static class MyClass
    {
        public static void SerializeObject(this List<double> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<double>));
            using (var stream = File.OpenWrite(fileName))
            {
                serializer.Serialize(stream, list);

            }

        }

        public static void Deserialize(this List<double> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<double>));
            using (var stream = File.OpenRead(fileName))
            {
                var other = (List<double>)(serializer.Deserialize(stream));
                list.Clear();
                list.AddRange(other);

            }
        }
    }

    #endregion
}
