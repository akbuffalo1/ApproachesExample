using System;
using System.Text.RegularExpressions;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;

namespace TigerApp.Shared.Services
{
    public interface IGoogleApiCloudVisionService {
        string TextDetection(byte[] imageArray);
    }
    public class GoogleApiCloudVisionService : IGoogleApiCloudVisionService
    {
        private const String CLOUD_VISION_API_KEY = "AIzaSyBqiC_8DsvGecDiPm5AbTI1hK59QeXtN4c";
        /// <summary>
        /// Creates an authorized Cloud Vision client service using Application 
        /// Default Credentials.
        /// </summary>
        public String TextDetection(byte[] imageArray)
        {
            String Answer = "";
            VisionService vision = new VisionService(new BaseClientService.Initializer
            {
                //HttpClientInitializer = credential,
                //ApplicationName = "com.byters.tigerocr",
                //GZipEnabled = true,
                ApiKey = CLOUD_VISION_API_KEY
            });

            string imageContent = Convert.ToBase64String(imageArray);

            BatchAnnotateImagesResponse result = new BatchAnnotateImagesResponse();

            result = vision.Images.Annotate(new BatchAnnotateImagesRequest()
            {
                Requests = new[] {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type =
                                "TEXT_DETECTION", MaxResults = 2000}},
                        Image = new Image() { Content = imageContent }
                    }
                }
            }).Execute();


            // Use the client to get text annotations for the given image
            //var result = DetectText(vision, imageArray);

            // Check for valid text annotations in response
            if (result.Responses[0].TextAnnotations != null)
            {
                // Loop through and output text annotations for the image
                foreach (var response in result.Responses)
                {
                    Console.WriteLine("Text found in image");
                    Console.WriteLine();
                    if (response.TextAnnotations != null || response.TextAnnotations.Count > 0) {
                        var annotation = response.TextAnnotations[0];
                        if (annotation != null)
                            Answer = annotation.Description;
                    }
                }
            }
            else
            {
                if (result.Responses[0].Error == null)
                {
                    Console.WriteLine("No text found.");
                }
                else
                {
                    Console.WriteLine("Not a valid image.");
                }
            }

            return Answer;

        }
    }
}

