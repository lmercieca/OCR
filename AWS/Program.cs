using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System.IO;

namespace NETRekognitionConsole
{
    class Program
    {
        static void DetectText(string filename)
        {
            // Using USWest2, not the default region
            AmazonRekognitionClient rekoClient = new AmazonRekognitionClient("AKIAJ3MXTT4YJ5WCU2JA", "1Qgrgk01sRVzRkEhZ0VuB/9kO8FyZQI2qjSy76Ry", Amazon.RegionEndpoint.USWest2);

            DetectTextRequest dfr = new DetectTextRequest();

            // Request needs image butes, so read and add to request
            Amazon.Rekognition.Model.Image img = new Amazon.Rekognition.Model.Image();
            byte[] data = null;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }
            img.Bytes = new MemoryStream(data);
            dfr.Image = img;
            var outcome = rekoClient.DetectText(dfr);

            if (outcome.TextDetections.Count > 0)
            {

                foreach (var text in outcome.TextDetections)
                {
                    Console.WriteLine("Description:" + text.DetectedText + " - Confidence:" + text.Confidence );

                    //// Get the bounding box
                    //BoundingBox bb = fd.BoundingBox;
                    //Console.WriteLine("Bounding box = (" + bb.Left + ", " + bb.Top + ", " +
                    //    bb.Height + ", " + bb.Width + ")");

                }

            }
            else
                Console.WriteLine(">>> No Text found");
        }



        static void Main(string[] args)
        {
            string filename = @"C:\Users\Loui\Downloads\IMG_20171207_212819.jpg";

            DetectText(filename);

        }
    }
}