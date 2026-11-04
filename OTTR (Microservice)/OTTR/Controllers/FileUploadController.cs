using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTTR.Services;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using OTTR.Models;

namespace OTTR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : Controller
    {
        private readonly IFileUploadService fileUploadService;
        private readonly ILogger<FileUploadController> logger;
        private readonly IConfiguration configuration;
        public FileUploadController(IFileUploadService fileUploadService, ILoggerFactory logger, IConfiguration configuration)
        {
            this.fileUploadService = fileUploadService;
            this.logger = logger.CreateLogger<FileUploadController>();
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("Upload/{Lob}")]
        public async Task<ActionResult> Upload([FromQuery(Name = "userEID")] string userEID, List<IFormFile> file, string Lob, String RoleList)
        {
            //logger.LogError("In Upload()");
            ResponseResource responseResource = new ResponseResource();
            object res = null;
            try
            {
                
                if (String.IsNullOrEmpty(RoleList))
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                ////logger.LogError("In Upload() RoleList => " + RoleList.ToString());
                ////logger.LogError("In Upload() Lob => " + Lob);
                ////logger.LogError("In Upload() file => " + file);
                ////logger.LogError("In Upload() userEID => " + userEID);
                List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
               // //logger.LogError("In Upload() RoleList List => "+listofFERoles.ToString());
                bool isRoleAuthorized = listofFERoles.Any(s => s.Contains("OTTR-Admin"));
                ////logger.LogError("In Upload() isRoleAuthorized => " + isRoleAuthorized);
                if (!isRoleAuthorized)
                {
                   
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                var _files = file[0];
               // //logger.LogError("In Upload() _files => "+ _files);
                ////logger.LogError("In Upload() Before Malicious check ");
                var isFileSafe = await MaliciousFileCheck(file[0]);
                if(isFileSafe!="Safe")
                {
                    
                    responseResource.code = 402;
                    responseResource.data = "Input file is corrupted or contains malicious data";
                    return Ok(responseResource);
                }
                string fname;
                fname = _files.FileName;
                ////logger.LogError("In Upload() fname => " + fname);
                var save_path = configuration.GetSection("DownloadFilesPath").Value.ToString() + "\\uploads";
                //logger.LogError("In Upload() save_path => " + save_path);
                bool exists = System.IO.Directory.Exists(save_path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(save_path);
                save_path = Path.Combine(save_path, fname);
                using (var stream = System.IO.File.Create(save_path))
                {
                    await _files.CopyToAsync(stream);
                    //logger.LogError("In Upload() stream => " + stream);
                }
                 bool success= fileUploadService.ParseFile(save_path, userEID, Lob).Result;
                //logger.LogError("In Upload() res => " + res);
               
                responseResource.code = 200;
                responseResource.data = res;
                return Ok(responseResource);
            }
            
            catch (Exception ex)
            {
                if (ex.InnerException is FileValidationException)
                {
                    responseResource.code = 400;
                    responseResource.data = ex.InnerException.Message;
                    return Ok(responseResource);
                }
                else
                {
                    responseResource.code = 400;
                    responseResource.data = "File Upload failed" + ex.ToString() + "\n" + ex.StackTrace.ToString();
                    return Ok(responseResource);
                }
            }
        }

        [HttpPost]
        public async Task<string> MaliciousFileCheck(IFormFile file)
        {
            //logger.LogError("In MaliciousFileCheck() ");
            //logger.LogError("In MaliciousFileCheck() file=> ",file);
            var fileStatus="";                
            try
            {
                if (file != null && file.Length > 0)
                {
                    //logger.LogError("In MaliciousFileCheck() file.Length=> " + file.Length);
                    using (var client = new HttpClient())
                    {
                        //logger.LogError("In MaliciousFileCheck() client=> " + client);
                        try
                        {
                            byte[] data;
                            using (var br = new BinaryReader(file.OpenReadStream()))
                                data = br.ReadBytes((int)file.OpenReadStream().Length);
                            //logger.LogError("In MaliciousFileCheck() data=> " + data);
                            ByteArrayContent bytes = new ByteArrayContent(data);
                            MultipartFormDataContent multiContent = new MultipartFormDataContent();
                            multiContent.Add(bytes, "file", file.FileName);
                            //logger.LogError("In MaliciousFileCheck() multiContent=> " + multiContent);
                            var result = client.PostAsync(configuration.GetSection("MaliciousFileValidationAPI").Value, multiContent);
                            //logger.LogError("In MaliciousFileCheck() result=> " + result);
                            fileStatus = await result.Result.Content.ReadAsStringAsync();
                            //logger.LogError("In MaliciousFileCheck() fileStatus=> " + fileStatus);
                            return fileStatus; //201 Created the request has been fulfilled, resulting in the creation of a new resource.

                        }
                        catch (Exception e)
                        {
                            //logger.LogError("In MaliciousFileCheck() catch block message=> " + e.Message);
                            //logger.LogError("In MaliciousFileCheck() catch block inner exception=> " + e.InnerException);
                            //logger.LogError("In MaliciousFileCheck() catch block stacktrace=> " + e.StackTrace);
                            return fileStatus; // 500 is generic server error
                        }
                    }
                }
                //logger.LogError("In MaliciousFileCheck() return fileSattaus=> " + fileStatus);
                return fileStatus; // 400 is bad request

            }
            catch (Exception e)
            {
                //logger.LogError("In MaliciousFileCheck() 2catch block message=> " + e.Message);
                //logger.LogError("In MaliciousFileCheck() 2catch block inner exception=> " + e.InnerException);
                //logger.LogError("In MaliciousFileCheck() 2catch block stacktrace=> " + e.StackTrace);
                return ""; // 500 is generic server error
            }
        }





    }
}
