using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FutsalGoal.Utility
{
    public class FileUploadUtility
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FileUploadUtility(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public bool UploadFile(string entityName, int systemId, IEnumerable<IFormFile> files, string fileName)
        {
            if (!files.Any())
                return true;
            long size = files.Sum(f => f.Length);

            var filePath = GetFilePath(entityName, systemId, fileName);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }
            return true;
        }

        public bool IsvalidImage(IEnumerable<IFormFile> files)
        {
            if (!files.Any())
                return true;

            /*  try
              {
                  var stream = files.First().OpenReadStream();
                  using (var memoryStream = new MemoryStream())
                  {
                      files.First().CopyTo(memoryStream);
                      //Read an image from the stream...
                      var i = Image.FromStream(memoryStream);

                      //Move the pointer back to the beginning of the stream
                      memoryStream.Seek(0, SeekOrigin.Begin);

                      if (ImageFormat.Jpeg.Equals(i.RawFormat))
                          return true;
                      return ImageFormat.Png.Equals(i.RawFormat) || ImageFormat.Gif.Equals(i.RawFormat);
                  }
              }
              catch
              {
                  return false;
              }*/
            return true;
        }

        public bool UploadResizeFile(string entityName, int systemId, IEnumerable<IFormFile> files)
        {
            if (!files.Any())
                return true;

          /*  long size = files.Sum(f => f.Length);

            var inputFileName = files.First().Name;
            var outputFileName = GetFilePath(entityName, systemId, files.First().FileName);

            //save original file
            string sFile_Target_Original = GetOriginalFilePath(entityName, systemId, files.First().FileName);

            using (var stream = new FileStream(sFile_Target_Original, FileMode.Create))
            {
                files.First().CopyTo(stream);
            }

            //resize now
            int width, height;
            const long quality = 50;
            var maxHeight = 128;
            var maxWidth = 50;

            Bitmap current = new Bitmap(sFile_Target_Original);
            if (current.Width > current.Height)
            {
                width = maxWidth;
                height = Convert.ToInt32(current.Height * maxHeight / (double)current.Width);
            }
            else
            {
                width = Convert.ToInt32(current.Width * maxWidth / (double)current.Height);
                height = maxHeight;
            }

            #region get resized bitmap 
            var canvas = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(current, 0, 0, width, height);

                graphics.DrawImage(current, 0, 0, maxWidth, maxHeight);

                using (var output = System.IO.File.Open(outputFileName, FileMode.Create))
                {
                    //< setup jpg >

                    var qualityParamId = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityParamId, quality);
                    //</ setup jpg >

                    //< save Bitmap as Jpg >

                    var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    current.Save(output, codec, encoderParameters);
                    //resized_Bitmap.Dispose();
                    output.Close();
                    //</ save Bitmap as Jpg >
                }
            }
            #endregion*/
            return true;
        }

        private string GetOriginalFilePath(string entityName, int systemId, string fileName)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var diretory = String.Format("{0}\\{1}\\Original\\{2}", webRootPath, "Uploads", entityName);
            if (!Directory.Exists(diretory))
                Directory.CreateDirectory(diretory);
            var filePath = webRootPath + string.Format("\\{3}\\{0}\\{0}_{1}_{2}", entityName, systemId, fileName, "Uploads\\Original");
            return filePath;
        }

        public string GetFilePath(string entityName, int systemId, string fileName)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var diretory = String.Format("{0}\\{1}\\{2}", webRootPath, "Uploads", entityName);
            if (!Directory.Exists(diretory))
                Directory.CreateDirectory(diretory);
            var filePath = webRootPath + string.Format("\\{3}\\{0}\\{0}_{1}_{2}", entityName, systemId, fileName, "Uploads");
            return filePath;
        }
        public string GetFileRelativePath(string entityName, int systemId, string fileName)
        {
            var filePath =  string.Format("//{3}//{0}//{0}_{1}_{2}", entityName, systemId, fileName, "Uploads");
            return filePath;
        }

        public void DeleteFile(string entityName, int entityId, string fileName)
        {
            var fileToDelete = GetFilePath(entityName, entityId, fileName);
            if (File.Exists(fileToDelete))
            {
                File.Delete(fileToDelete);
            }
        }
    }
}
