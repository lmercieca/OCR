using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GoogleApi
{

    class Program
    {
        public static object GoogleVisionApi { get; private set; }

        private static string JsonKeypath
        {
            get
            {
                return @"c:\Users\Loui\documents\visual studio 2017\Projects\OCR\GoogleApi\IcoSocialBounty-9f2556e825f7.json";
            }
        }

        private static  GoogleCredential CreateCredential()
        {
            using (var stream = new FileStream(JsonKeypath, FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { VisionService.Scope.CloudPlatform };
                var credential = GoogleCredential.FromStream(stream);
                credential = credential.CreateScoped(scopes);
                return credential;
            }
        }


        private static VisionService CreateService(GoogleCredential credential)
        {
            var service = new VisionService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "icosocialbounty",
                GZipEnabled = true,
            });
            return service;
        }

        static void Main(string[] args)
        {
            BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
            batchRequest.Requests = new List<AnnotateImageRequest>();

            var service = CreateService(CreateCredential());

            
            var file = File.ReadAllBytes(@"C:\Users\Loui\Downloads\IMG_20171207_212819.jpg");

            batchRequest.Requests.Add(new AnnotateImageRequest()
            {
                Features = new List<Feature>() { new Feature() { Type = "TEXT_DETECTION"}, },
                ImageContext = new ImageContext() { },
                Image = new Image() { Content = Convert.ToBase64String(file) }
            });

            var annotate = service.Images.Annotate(batchRequest);
            BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();

            if (batchAnnotateImagesResponse.Responses.Any())
            {
                foreach (var res in batchAnnotateImagesResponse.Responses)
                {
                    if (res.Error != null)
                    {
                        if (res.Error.Message != null)
                            Console.WriteLine(res.Error.Message);
                    }
                    else
                    {
                        if (res.TextAnnotations != null && res.TextAnnotations.Any())
                        {
                            foreach (var text in res.TextAnnotations)
                            {
                                //text.BoundingPoly.Vertices[0].X // y
                                Console.WriteLine("Description:" + text.Description + " - Etag:" + text.ETag + " - Locale:" + text.Locale + " - Score:" + text.Score);
                            }
                        }
                    }
                }
            }
        }



    }
}
