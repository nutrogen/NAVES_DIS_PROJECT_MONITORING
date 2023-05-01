using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NavesPortalforWebWithCoreMvc.Controllers.AuthFromIntranetController;
using NavesPortalforWebWithCoreMvc.Data;
using NavesPortalforWebWithCoreMvc.RfSystemData;
using Microsoft.EntityFrameworkCore;
using NavesPortalforWebWithCoreMvc.Models;
using Syncfusion.EJ2.Base;
using System.Collections;

namespace NavesPortalforWebWithCoreMvc.Controllers.DIS
{
    [Authorize]
    [CheckSession]
    public class DisProjectMonitoringController : Controller
    {
        private readonly BM_NAVES_PortalContext _repository;
        private readonly IBM_NAVES_PortalContextProcedures _procedures;

        public DisProjectMonitoringController(BM_NAVES_PortalContext repository, IBM_NAVES_PortalContextProcedures procedures)
        {
            _repository = repository;
            _procedures = procedures;
        }

        /// <summary>
        /// Project Modnitoring
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        /// <summary>
        /// 비동기 데이터 소스
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="dm"></param>
        /// <returns></returns>
        public async Task<IActionResult> UrlDataSource(string SearchString, DateTime? StartDate, DateTime? EndDate, [FromBody] DataManagerRequest? dm)
        {
            try
            {
                if (SearchString is null || SearchString == String.Empty)
                {
                    SearchString = "";
                }

                //List<PNAV_DIS_GET_WORK_LIST_IN_PROJECTResult> resultList = await _procedures.PNAV_DIS_GET_WORK_LIST_IN_PROJECTAsync(SearchString, StartDate, EndDate);
                List<PNAV_DIS_GET_PROJECT_LISTResult> resultList = await _procedures.PNAV_DIS_GET_PROJECT_LISTAsync(SearchString, StartDate, EndDate);


                IEnumerable DataSource = resultList.AsEnumerable();
                DataOperations operation = new DataOperations();

                //Search
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);
                }

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }

                //Filtering
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                int count = DataSource.Cast<PNAV_DIS_GET_PROJECT_LISTResult>().Count();

                //Paging
                if (dm.Skip != 0)
                {

                    DataSource = operation.PerformSkip(DataSource, dm.Skip);
                }

                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(new { result = DataSource });
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisProjectMonitoring", returnView = "Index" });
            }
        }
    }
}
