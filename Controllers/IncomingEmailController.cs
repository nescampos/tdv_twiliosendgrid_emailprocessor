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
    public class IncomingEmailController : ControllerBase
    {
        private const string FilesRootPath = "<path to SendGridFiles folder>";
        private List<EmailRule> _rules = new List<EmailRule>();

        public IncomingEmailController()
		{
			_rules.Add(new EmailRule { 
				AttachmentName = "finance_report.xlsx", 
				Subject = "Demo Email Finance", 
				To = "finance@<your email domain>", 
				FolderName = "Finance", 
			});
			_rules.Add(new EmailRule { 
				AttachmentName = "humanresources_report.xlsx", 
				Subject = "Demo Email Human Resources", 
				To = "humanresources@<your email domain>", 
				FolderName = "HR", 
			});
		}

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string[] emailToAddresses = Request.Form["to"];
            string emailSubject = Request.Form["Subject"];
            var files = Request.Form.Files;
            bool existSubjectInRules = _rules.Any(x => x.Subject == emailSubject);
            if(existSubjectInRules)
            {
                foreach(string emailTo in emailToAddresses)
                {
                    foreach(var file in files)
                    {
                        IEnumerable<EmailRule> rulesAvailables = _rules.Where(x => x.Subject == emailSubject && 
                                                                x.To == emailTo && x.AttachmentName == file.FileName);
                        foreach(EmailRule ruleAvailable in rulesAvailables)
                        {
                            string filePath = Path.Combine(FilesRootPath, ruleAvailable.FolderName, file.FileName);
                            using var inputStream = new FileStream(filePath, FileMode.Create);
                            // read file to stream
                            await file.CopyToAsync(inputStream);
                        }
                    }
                }
            }
            
            return Ok();

        }

        
    }
}
