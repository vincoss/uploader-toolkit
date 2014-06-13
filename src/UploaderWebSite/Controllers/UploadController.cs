using System;
using System.Web.Mvc;
using UploaderWebSite.Services;


namespace UploaderWebSite.Controllers
{
    //[Authorize]
    //[RequireHttps]
    public class UploadController : AsyncController
    {
        private readonly IUploadService _uploadService;
        private const string UPLOAD_DIRECTORY_PATH = "~/Upload/{0}-{1}";

        public UploadController() : this(new UploadService())
        {
        }

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        public ActionResult Cancel(Guid? id, string name)
        {
            if (id != null && id.Value != Guid.Empty && !string.IsNullOrEmpty(name))
            {
                string path = GetUploadFilePath(id.Value, name);
                return Json(_uploadService.CancelUpload(path));
            }
            return new HttpStatusCodeResult(400);
        }

        #region Private methods

        [NonAction]
        public string GetUploadFilePath(Guid id, string name)
        {
            return this.HttpContext.Server.MapPath(string.Format(UPLOAD_DIRECTORY_PATH, id, name));
        } 

        #endregion
    }
}