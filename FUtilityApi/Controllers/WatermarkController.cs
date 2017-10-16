using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using FUtilityApi.Models;
using System.Web;
using System.Web.Http.Cors;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace FUtilityApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WatermarkController : ApiController
    {
        [HttpPost]
        [Route("api/watermark/createWatermark")]
        public IHttpActionResult createWatermark(UrlImage url)
        {
            DefaultResponse def = new DefaultResponse();
            string domain = ConfigurationManager.AppSettings["domain"].ToString();

            string path1 = Path.Combine(HttpContext.Current.Server.MapPath(url.url1));
            string path2 = Path.Combine(HttpContext.Current.Server.MapPath(url.url2));

            Image imgPhoto = Image.FromFile(path2);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            Image imgWatermark = new Bitmap(path1);
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            grPhoto.DrawImage(
                imgPhoto,                               
                new Rectangle(0, 0, phWidth, phHeight), 
                0,                                      
                0,                                      
                wmWidth,                                
                phHeight,                                
                GraphicsUnit.Pixel);

            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grWatermark = Graphics.FromImage(bmWatermark);

            ImageAttributes imageAttributes = new ImageAttributes();

            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);

            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);

            int xPosOfWm = ((phWidth - wmWidth) - 10);
            int yPosOfWm = phHeight - wmHeight;

            grWatermark.DrawImage(imgWatermark,
                new Rectangle(0, yPosOfWm, wmWidth, wmHeight),  
                0,                  
                0,                  
                wmWidth,            
                wmHeight,           
                GraphicsUnit.Pixel, 
                imageAttributes);

            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();

            DateTime now = DateTime.Now;
            var urlImage = "~/Uploads/result" + now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            var link = HttpContext.Current.Server.MapPath(urlImage);
            var urlRes = domain + "/Uploads/result" + now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            def.meta = new Meta(200, "Success");
            def.data = urlRes;
            imgPhoto.Save(link);
            imgPhoto.Dispose();
            imgWatermark.Dispose();
            return Ok(def);

        }

        [HttpPost]
        [Route("api/watermark/uploadImage")]
        public IHttpActionResult uploadImage()
        {
            string domain = ConfigurationManager.AppSettings["domain"].ToString();
            DefaultResponse def = new DefaultResponse();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                List<String> files = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);


                    var postedFile = httpRequest.Files[file];

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 4096 * 4096 * 1; //Size = 16 MB

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = Utils.unsignString(postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.') - 1)).Replace(" ", "_").ToLower();
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg, .jpeg, .gif, .png, .bmp.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 16 mb.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(postedFile.InputStream))
                            {
                                DateTime now = DateTime.Now;
                                fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                                var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension);
                                Utils.createThumb(500, filePath, fileData);
                                string img = "/Uploads/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                                string rel = "/Uploads/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                                files.Add(img);
                            }
                        }
                    }
                }
                def.meta = new Meta(200, "Success");
                def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [HttpPost]
        [Route("api/imageCp")]
        public IHttpActionResult imageCp([FromUri] string imageSource, [FromUri] string imageMark)
        {
            string domain = ConfigurationManager.AppSettings["domain"].ToString();
            string urlImgSource = upToMark(imageSource);
            var urlImgMark = upToMark(imageMark);
            Image img = Image.FromFile(urlImgMark);
            var rsWidght = 100;
            var rsHeight = rsWidght * img.Height / img.Width;
            Bitmap resized = new Bitmap(img, new Size(rsWidght, rsHeight));
            using (Image image = Image.FromFile(urlImgSource))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (TextureBrush watermarkBrush = new TextureBrush(resized))
            {
                DateTime now = DateTime.Now;
                int x = 25;
                int y = (image.Height - resized.Height - x);
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(resized.Width, resized.Height)));
                var url = "~/Uploads/Result" + now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                var filePath = HttpContext.Current.Server.MapPath(url);
                var linkResponse = domain + "/Uploads/Result" + now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                image.Save(filePath);
                return Ok(linkResponse);
            }
        }

        public string upToMark(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] dataImgS = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(dataImgS))
                {
                    using (var yourImage = Image.FromStream(mem))
                    {
                        DateTime now = DateTime.Now;
                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/imgUp-"+ now.ToString("yyyyMMddHHmmssfff") +".jpg");
                        yourImage.Save(filePath);
                        return filePath;
                    }
                }
            }
        }

    }
}
 