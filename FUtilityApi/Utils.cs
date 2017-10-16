using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;

namespace FUtilityApi
{
    public class Utils
    {
        private static readonly string[] VietnameseSigns = new string[]

        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        public static string unsignString(string str)
        {
            try
            {
                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }
                Regex r = new Regex("(?:[^a-z0-9 @]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(str, String.Empty).ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string unsignString2(string str)
        {
            try
            {
                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }
                Regex r = new Regex("(?:[^a-z0-9 ,@]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(str, String.Empty).ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampMilisecondToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static long DateTimeToUnixTimeStamp(DateTime date)
        {
            try
            {
                return (long)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
            catch (Exception ex)
            {
                return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }

        public static void GenerateThumbnail(int thumbWidth, int thumbHeight, string fileThumbnailName, byte[] input)
        {
            MemoryStream ms = new MemoryStream(input);
            //Image returnImage = Image.FromStream(ms);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            double srcWidth = image.Width;
            double srcHeight = image.Height;
            var percWidth = ((float)thumbWidth / (float)srcWidth); // 0.2
            var percHeight = ((float)thumbHeight / (float)srcHeight); // 0.25
            var percentage = Math.Max(percHeight, percWidth); // 0.25

            var width = (int)Math.Max(srcWidth * percentage, thumbWidth); // 250
            var height = (int)Math.Max(srcHeight * percentage, thumbHeight); // 200

            using (var resizedBmp = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage((Image)resizedBmp))
                {
                    graphics.InterpolationMode = InterpolationMode.Default;
                    graphics.DrawImage(image, 0, 0, width, height);
                }

                //work out the coordinates of the top left pixel for cropping
                var x = (width - thumbWidth) / 2; // 25
                var y = (height - thumbHeight) / 2; // 0

                //create the cropping rectangle
                var rectangle = new Rectangle(x, y, thumbWidth, thumbHeight); // 25, 0, 200, 200

                //crop
                using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                {
                    croppedBmp.Save(fileThumbnailName);
                }
            }

            image.Dispose();
        }

        public static void createThumb(int thumbWidth, string fileThumbnailName, byte[] input)
        {
            //extract path
            int lastIndex = fileThumbnailName.LastIndexOf("\\");
            string path = fileThumbnailName.Substring(0, lastIndex);
            Directory.CreateDirectory(path);
            MemoryStream ms = new MemoryStream(input);
            //Image returnImage = Image.FromStream(ms);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            double srcWidth = image.Width;
            double srcHeight = image.Height;
            double thumbHeight = (srcHeight / srcWidth) * thumbWidth;
            Bitmap bmp = new Bitmap(thumbWidth, (int)thumbHeight);

            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, (int)thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, (int)srcWidth, (int)srcHeight, GraphicsUnit.Pixel);

            bmp.Save(fileThumbnailName);

            bmp.Dispose();
            image.Dispose();
        }

        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        public static async void sendNotifyEmail(string sToEmail, string sToDisplayName, string sSubject, string sBody)
        {
            var fromAddress = new MailAddress("noreply.fbsale", "FB.SALE");
            var toAddress = new MailAddress(sToEmail, sToDisplayName);

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Timeout = 30000,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "fbsale@#123")
            })
            {
                using (var message = new MailMessage(fromAddress, toAddress))
                {
                    message.IsBodyHtml = true;
                    message.Subject = sSubject;
                    message.Body = sBody;
                    try
                    {
                        smtp.Send(message);
                        message.Dispose();
                        smtp.Dispose();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            smtp.Send(message);
                            message.Dispose();
                            smtp.Dispose();
                        }
                        catch (Exception ex1)
                        {
                        }
                        finally
                        {
                            message.Dispose();
                            smtp.Dispose();
                        }
                    }
                    finally
                    {
                        message.Dispose();
                        smtp.Dispose();
                    }
                }
            }
        }
    }
}