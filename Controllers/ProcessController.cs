using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SendgridProcessor.Configuration;
using System.IO;

namespace SendgridProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        
        private readonly ILogger<WeatherForecastController> _logger;
        private List<EmailRule> _rules;

        public ProcessController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            FillRules();
        }

        private void FillRules()
        {
            _rules = new List<EmailRule>();
            _rules.Add(new EmailRule { AttachmentName = "demo.xlsx", Subject = "Demo Email Finance", To = "finance@<domain and subdomain>", FolderName="Finance", PathFiles=@"<root path>" });
            _rules.Add(new EmailRule { AttachmentName = "demo_hr.xlsx", Subject = "Demo Email Human Resources", To = "humanresources@<domain and subdomain>", FolderName = "HR", PathFiles=@"<root path>" });
        }

        [HttpPost]
        public async Task<string> Post()
        {
            string[] email_to_addresses = Request.Form["to"];
            string email_subject = Request.Form["Subject"];
            var files = Request.Form.Files;
            bool existSubjectInRules = _rules.Any(x => x.Subject == email_subject);
            if(existSubjectInRules)
            {
                foreach(string email_to in email_to_addresses)
                {
                    foreach(var file in files)
                    {
                        EmailRule ruleAvailable = _rules.FirstOrDefault(x => x.Subject == email_subject && 
                                                                x.To == email_to && x.AttachmentName == file.FileName);
                        if(ruleAvailable != null)
                        {
                            string filePath = System.IO.Path.Combine(ruleAvailable.PathFiles, ruleAvailable.FolderName,file.FileName);
                            using (var inputStream = new FileStream(filePath, FileMode.Create))
                            {
                                // read file to stream
                                await file.CopyToAsync(inputStream);
                                // stream to byte array
                                byte[] array = new byte[inputStream.Length];
                                inputStream.Seek(0, SeekOrigin.Begin);
                                inputStream.Read(array, 0, array.Length);
                            }
                        }
                    }
                }
            }
            
            return "Ok";
        }

        
    }
}
