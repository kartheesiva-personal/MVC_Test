using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Persistence.Interface;
using NLog;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        #region Constructor And Private Members
        static Logger log = LogManager.GetCurrentClassLogger();
        private IRetailModelDao _retailModelDao;
        public HomeController(IRetailModelDao retailModelDao)
        {
            _retailModelDao = retailModelDao;
        }

        #endregion
        public ActionResult Index()
        {
            List<RetailModel> retailList = new List<RetailModel>();
            try
            {
                int year = DateTime.Now.Year;
                retailList = _retailModelDao.GetRetailByYear(year);

                _retailModelDao.SaveDisplayRetailLog(year);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
            return View(retailList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveRetail(RetailModel R)
        {
            try
            {
                if (R.UniqueId != Guid.Empty)
                {
                    R.SystemIp = GetIPAddress(HttpContext);
                    int result = _retailModelDao.SaveRetail(R);
                    if (result > 0)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

  
        public JsonResult DeleteRecord(Guid UniqueId)
        {
            try
            {
                if (UniqueId != Guid.Empty)
                {
                    RetailModel r= _retailModelDao.GetRetailByUniqueId(UniqueId);
                    r.SystemIp = GetIPAddress(HttpContext);                    
                    int result = _retailModelDao.DeleteRetail(r);
                    if (result > 0)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        public static String GetIPAddress(HttpContextBase HttpContext)
        {
            String ip = String.Empty;
            try
            {
                ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ip))
                    ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                else
                    ip = ip.Split(',')[0];
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
            return ip;
        }
    }
}