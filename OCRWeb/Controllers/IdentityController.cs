using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OCRWeb.Controllers
{
    public class IdentityController : Controller
    {
        const string subscriptionKey = "f10e5d3aa98f4739b97983c5bdcc903c";
        const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/ocr";


        // GET: Identity
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public JsonResult UploadMe()
        {

            var file = Request.Files[0];

            var fileName = Path.GetFileName(file.FileName);

            var path = Path.Combine(Server.MapPath("~/Upload/"), fileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            file.SaveAs(path);

            Task<string> t = MakeOCRRequest(path);

            t.Wait();
            return Json(t.Result);

     
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async Task<string> MakeOCRRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response =  await client.PostAsync(uri, content).ConfigureAwait(false);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return contentString;
            }
        }


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

    }
}