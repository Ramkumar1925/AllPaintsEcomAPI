using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Web;
using AllPaintsEcomAPI.DTO;
using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Repositories;
using AllPaintsEcomAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllPaintsEcomAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class EcomShopController : Controller
    {
        private readonly EcomShopRepository _ecomService;
        private object _dbContext;

        //public EcomShopController(EcomShopRepository ecomService)
        //{
        //    _ecomService = ecomService;
        //}

        private readonly IConfiguration Configuration;

        public EcomShopController(EcomShopRepository ecomService, IConfiguration configuration)
        {
            _ecomService = ecomService;
            Configuration = configuration;
        }


        private static string Username = string.Empty;
        private static string Password = string.Empty;
        //private static string baseAddress = "http://13.233.6.115/api/v2/auth";
        private static string baseAddress = "http://13.234.246.143/api/v2/auth";


        [Route("usage/summary")]
        [HttpGet]
        public IActionResult GetUsageSummary()
        {
            var summary = ApiUsageTracker.GetUsageSummary();
            return new JsonResult(summary);
        }


        // get data
        [Route("cart")]
        [HttpGet]
        public async Task<IActionResult> cart(string user_code)
        {
            try
            {
                string plain = "sswgBwt3AqZslpNo4Ps6jQrkyKo7x2EXNc06hMom22dfKwtQjPqtLzwS/IyTRpXr";
                //string encrypted = AesEncryption.Encrypt(plain);
                string urlSafe = AesEncryption.Decrypt(plain);


                //Interlocked.Increment(ref ApiUsageTracker.TotalRequests);
                //lock (ApiUsageTracker.UniqueUsers)
                //{
                //    ApiUsageTracker.UniqueUsers.Add(user_code);
                //}
                
                string userCode = AesEncryption.Decrypt(user_code);
                ApiUsageTracker.Track("Get Cart", userCode);

                var encryptedResult = await _ecomService.cart(userCode);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                var response1 = new ApiGetResponse
                {
                    Status = 0,
                    Message = "Your cart is currently empty.",
                    Error = ex.Message
                };
                return StatusCode(500, response1);
            }
        }


        //update data
        [Route("cart")]
        [HttpPut]
        public async Task<IActionResult> cart(updateCart prm)
        {
            try
            {        
                prm.qty           = AesEncryption.Decrypt(prm.qty);
                prm.user_code     = AesEncryption.Decrypt(prm.user_code);
                prm.materialCode  = AesEncryption.Decrypt(prm.materialCode);
                prm.RefrenceCode  = AesEncryption.Decrypt(prm.RefrenceCode);
                ApiUsageTracker.Track("Update Cart", prm.user_code);
                var encryptedResult = await _ecomService.cart(prm);

                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                var response1 = new ApiGetResponse
                {
                    Status = 0,
                    Message = "Could not update cart. Please try again.",
                    Error = ex.Message
                };

                return StatusCode(500, response1);
            }

        }


        //delete data
        [Route("cart")]
        [HttpDelete]
        public async Task<IActionResult> cart(string user_code, string ecomCode, string packSize, string RefrenceCode)
        {
            try
            { 
                user_code = AesEncryption.Decrypt(user_code);
                ecomCode = AesEncryption.Decrypt(ecomCode);
                packSize = AesEncryption.Decrypt(packSize);
                RefrenceCode = AesEncryption.Decrypt(RefrenceCode);
                ApiUsageTracker.Track("Delete Cart", user_code);

                var encryptedResult = await _ecomService.cart(user_code, ecomCode, packSize, RefrenceCode);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                var response = new ApiGetResponse
                {
                    Status = 0,
                    Message = "Unable to remove item. Please Try again.",
                    Error = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        public class EncryptedPayloadModel
        {
            public string Payload { get; set; }
        }


        [Route("cart")]
        [HttpPost]
        public async Task<IActionResult> Cart(createDtls prm)
        {
            try
            {
                 string encryptedPayload = prm.ToString();

                string decryptedJson = AesEncryption.Decrypt(encryptedPayload);

                // Step 3: Deserialize into model
                var model = JsonConvert.DeserializeObject<createDtls>(decryptedJson);

                // Step 4: Access nested values safely
                var userCode = model?.cartItem?[0]?.user_code;

                var result = await _ecomService.cart(model);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                var response = new ApiGetResponse
                {
                    Status = 0,
                    Message = "Could not update cart. Please try again.",
                    Error = ex.Message
                };

                return StatusCode(500, response);
            }
        }


        //update data
        [Route("cart1")]
        [HttpPost]
        public async Task<IActionResult> cart1(createDtls prm)
        {
            try
            {


                string json = prm.ToString();

                 string encrypted = AesEncryption.Encrypt(json);
                string urlSafe = AesEncryption.Decrypt(encrypted);
               // var model = JsonConvert.DeserializeObject<createDtls>(urlSafe);
               // var data = urlSafe.cartItem[0].user_code;

                var dcriyptData = AesEncryption.Decrypt(json);
                //   ApiUsageTracker.Track("Update Cart", dcriyptData.user_code);

                var encryptedResult = await _ecomService.cart(dcriyptData);


                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                var response1 = new ApiGetResponse
                {
                    Status = 0,
                    Message = "Could not update cart. Please try again.",
                    Error = ex.Message
                };

                return StatusCode(500, response1);
            }

        }


        [Route("GetALPNProducts_DoctypeNew")]
        [HttpPost]
        public async Task<IActionResult> GetALPNProducts_DoctypeNew(dynamic prm)
        {
            try
            {
                 
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.GetALPNProducts_DoctypeNew(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [Route("GetAllpaintprofileprocess")]
        [HttpPost]
        public async Task<IActionResult> GetAllpaintprofileprocess(dynamic prm)
        {
            try
            {
                var encryptedResult = await _ecomService.GetAllpaintprofileprocess(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [Route("getAllpaintsrgbcolor")]
        [HttpPost]
        public async Task<IActionResult> getAllpaintsrgbcolor(dynamic prm)
        {
            try
            {
                //var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                //if (string.IsNullOrWhiteSpace(jwtToken))
                //{
                //    return Unauthorized(new
                //    {
                //        status = 401,
                //        statusText = "Authorization token is missing."
                //    });
                //}

                var encryptedResult = await _ecomService.getAllpaintsrgbcolor(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [Route("FetchCustomerDtls")]
        [HttpPost]
        public async Task<IActionResult> FetchCustomerDtls(dynamic prm)
        {
            try
            {
                string data = "VFgDzgVG6gycuCVkvQoY7mFXz8+nhK6828xROirpJ8ncywEwSfubjfVS/fvEUBsMlqVuLPLOHzwPngp4xKDgZwYEGyDw2/dCI+FCGQ3QPv0Rn9nflbTO7E7iHxyeode8TswqBk1IgQ3FrukFCn6jocBljh1OXifbI37LsNiozPoMbGWH67A7dtVYHK559FDU1SJxIPA0ZUvXH4javMaP7l6WmVaeWfy0RBU0zuIZbYHFbpcKN09XpHSSLw6qH/Uvka0HSO60UUtK+78ZggKys4GpUhXwlyLJKsTHX8WVA0NE8ptSI+VuhtkpN5dj5o1qTpI7lbc/S/oZ8+6Z6oDTBuF9toQ/03B2lYvUhmxRLPTnjQSGbutseB+mVmRtVRUBqRgD09temsCfXwgBUIW8NONR65F1WchvGJcocM6sq4W/rdll6u8IaPMwJCPlFen+J93FbwNIoKfFIGyO3Q2BfohK4mzWjS9YL5EK20Cpeth0lKoheByxl8XukiPu3LxOvgv0RTmrndLpxqKMbqKKDdi4PrWHsdb7AoTQhiLtiw8eCkjWJJHYTubSBZxn0qlyAlfgP9QOQ9xrLHbdw+a+xUnxMQH0SilGH1aiPyTem7VVeOePibdRzvw0sHRplMraiY9l63+kHdzPLH6kH8dY6cYHplCVs/eztYgei2LNAuzGkzTlMEN6Q42tkG5tJYPnEnBbPjxQC5uKgApRAtx9l2tjB+SZnNgwng2R+dpcx2n4f+mRAjQxSlKlG1fCzbDqzQlxrXrKpAXjqmtmLvQhSbzU2C/JNxeexl80tnlDl3yPUltae6SMxbMsh3HV8nAEAEgUyH3NYRhWDuhXSbHNipZRx7wpFu1S4trEvVPu4oJLotnbwieXci8skmD/mKjIFXOazv4zVIZAq7HQlHl6g9cr/OMZeIZYVaD6zOtW+gGdCvI4pSIjaL0KehAnbInqE1cCiFIZAa7ZYCTA31bmgdN+fZFyIOtdHUMS+gNHPNhNcR/qYeUwpDeE+RlWbVJHW4qAH7dXg6WX9Zmb2ps9dYh+Z/LlwOukOkrJb0u6jjhsRs5CXtsuXDermLV8vVV6If13xllXljQjpDdoassebq1aiq82TZ5CcVmS1K4JZKt72i3T3f849dWnPWMXWbUXaVNWYRFRpjwVZYMnsKBeKjVqUmF6r5UuiLW5qHDxVZpSI+tfwjM9baXrJ6wAimtP8wuNOajaSfE54ad0FFDKdpA9ILYqsKVQ92MPtXDJ66KHfyyT+nj+PWb1M4QM3YGxcL7+jm5oxZRDLZw6XJvzSN2Lb15y+1CbOJuNCJXesh9Ma9pzvCHObrzt3Iat3/gsukHOXRrENjGf26NAU4tnIg==";
                var dcriyptData = AesEncryption.Decrypt(data);
                var encryptedResult = await _ecomService.FetchCustomerDtls(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [Route("OrderBookingDtls")]
        [HttpPost]
        public async Task<IActionResult> OrderBookingDtls(dynamic prm)
        {
            int maxno = 0;
            try
            {
                string json = prm.ToString();
                var dcriyptData = AesEncryption.Decrypt(json);
                var prsModel = JsonConvert.DeserializeObject<OrderMarge>(dcriyptData);

                //var prsModel = AesEncryption.Decrypt(prm);

                DataSet ds = new DataSet();
                string dsquery = "sp_Get_MaxCode";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(dsquery))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FilterValue1", prsModel.ccomcode);
                        cmd.Parameters.AddWithValue("@FilterValue2", prsModel.cloccode);
                        cmd.Parameters.AddWithValue("@FilterValue3", prsModel.corgcode);
                        cmd.Parameters.AddWithValue("@FilterValue4", prsModel.clineno);
                        cmd.Parameters.AddWithValue("@FilterValue5", prsModel.cfincode);
                        cmd.Parameters.AddWithValue("@FilterValue6", prsModel.cdoctype);
                        await con.OpenAsync();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                        await con.CloseAsync();
                    }
                }

                maxno = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                prsModel.ndocno = maxno;

                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    string query = "insert into tbl_franchise_order_booking_mst(ccomcode, cloccode, corgcode,clineno,cfincode,cdoctype," +
                        "ndocno,ldocdate,ccustomercode,ccustomername,navailablelimit,npaymentterms,cdistributorcode,cdistributorname,cprocessflag," +
                        "ntotordervalue,ntotdiscountvalue,nnetordervalue,cdiscounttype1,ndistype1value,cdiscounttype2,ndistype2value,cdiscounttype3," +
                        "ndistype3value,cdiscounttype4,ndistype4value,nsmpercent,nsmvalue,nnetmarginpercent,nnetmarginvalue,nincoterms,corderremarks," +
                        "cdeliverydate,isdeleivered,isredemption,isdelschedule,ccreatedby,lcreateddate,cmodifedby,lmodifieddate,corderchannel," +
                        "cremarks1,cremarks2,cremarks3,cexpirydate,mobileNumber,pointsAmt,userId,otpVerify) values (@ccomcode, @cloccode, @corgcode,@clineno,@cfincode,@cdoctype," +
                        "@ndocno,@ldocdate,@ccustomercode,@ccustomername,@navailablelimit,@npaymentterms,@cdistributorcode,@cdistributorname,@cprocessflag," +
                        "@ntotordervalue,@ntotdiscountvalue,@nnetordervalue,@cdiscounttype1,@ndistype1value,@cdiscounttype2,@ndistype2value," +
                        "@cdiscounttype3," +
                        "@ndistype3value,@cdiscounttype4,@ndistype4value,@nsmpercent,@nsmvalue,@nnetmarginpercent,@nnetmarginvalue,@nincoterms," +
                        "@corderremarks," +
                        "@cdeliverydate,@isdeleivered,@isredemption,@isdelschedule,@ccreatedby,@lcreateddate,@cmodifedby,@lmodifieddate," +
                        "@corderchannel," +
                        "@cremarks1,@cremarks2,@cremarks3,@cexpirydate,@mobileNumber,@pointsAmt,@userId,@otpVerify)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ccomcode", prsModel.ccomcode);
                        cmd.Parameters.AddWithValue("@cloccode", prsModel.cloccode);
                        cmd.Parameters.AddWithValue("@corgcode", prsModel.corgcode);
                        cmd.Parameters.AddWithValue("@clineno", prsModel.clineno);
                        cmd.Parameters.AddWithValue("@cfincode", prsModel.cfincode);
                        cmd.Parameters.AddWithValue("@cdoctype", prsModel.cdoctype);
                        cmd.Parameters.AddWithValue("@ndocno", maxno);
                        cmd.Parameters.AddWithValue("@ldocdate", prsModel.ldocdate);
                        cmd.Parameters.AddWithValue("@ccustomercode", prsModel.ccustomercode);
                        cmd.Parameters.AddWithValue("@ccustomername", prsModel.ccustomername);
                        cmd.Parameters.AddWithValue("@navailablelimit", prsModel.navailablelimit);
                        cmd.Parameters.AddWithValue("@npaymentterms", prsModel.npaymentterms);
                        cmd.Parameters.AddWithValue("@cdistributorcode", prsModel.cdistributorcode);
                        cmd.Parameters.AddWithValue("@cdistributorname", prsModel.cdistributorname);
                        cmd.Parameters.AddWithValue("@cprocessflag", prsModel.cprocessflag);
                        cmd.Parameters.AddWithValue("@ntotordervalue", prsModel.ntotordervalue ?? 0);
                        cmd.Parameters.AddWithValue("@ntotdiscountvalue", prsModel.ntotdiscountvalue ?? 0);
                        cmd.Parameters.AddWithValue("@nnetordervalue", prsModel.nnetordervalue ?? 0);
                        cmd.Parameters.AddWithValue("@cdiscounttype1", prsModel.cdiscounttype1);
                        cmd.Parameters.AddWithValue("@ndistype1value", prsModel.ndistype1value);
                        cmd.Parameters.AddWithValue("@cdiscounttype2", prsModel.cdiscounttype2);
                        cmd.Parameters.AddWithValue("@ndistype2value", prsModel.ndistype2value);
                        cmd.Parameters.AddWithValue("@cdiscounttype3", prsModel.cdiscounttype3);
                        cmd.Parameters.AddWithValue("@ndistype3value", prsModel.ndistype3value);
                        cmd.Parameters.AddWithValue("@cdiscounttype4", prsModel.cdiscounttype4);
                        cmd.Parameters.AddWithValue("@ndistype4value", prsModel.ndistype4value);
                        cmd.Parameters.AddWithValue("@nsmpercent", prsModel.nsmpercent);
                        cmd.Parameters.AddWithValue("@nsmvalue", prsModel.nsmvalue);
                        cmd.Parameters.AddWithValue("@nnetmarginpercent", prsModel.nnetmarginpercent);
                        cmd.Parameters.AddWithValue("@nnetmarginvalue", prsModel.nnetmarginvalue);
                        cmd.Parameters.AddWithValue("@nincoterms", prsModel.nincoterms);
                        cmd.Parameters.AddWithValue("@corderremarks", prsModel.corderremarks);
                        cmd.Parameters.AddWithValue("@cdeliverydate", prsModel.cdeliverydate);
                        cmd.Parameters.AddWithValue("@isdeleivered", prsModel.isdeleivered);
                        cmd.Parameters.AddWithValue("@isredemption", prsModel.isredemption);
                        cmd.Parameters.AddWithValue("@isdelschedule", prsModel.isdelschedule);
                        cmd.Parameters.AddWithValue("@ccreatedby", prsModel.ccreatedby);
                        cmd.Parameters.AddWithValue("@lcreateddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@cmodifedby", prsModel.cmodifedby);
                        cmd.Parameters.AddWithValue("@lmodifieddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@corderchannel", prsModel.corderchannel);
                        cmd.Parameters.AddWithValue("@cremarks1", prsModel.cremarks1);
                        cmd.Parameters.AddWithValue("@cremarks2", prsModel.cremarks2);
                        cmd.Parameters.AddWithValue("@cremarks3", prsModel.cremarks3);
                        cmd.Parameters.AddWithValue("@cexpirydate", DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"));

                        cmd.Parameters.AddWithValue("@mobileNumber", prsModel.mobileNumber ?? "");
                        cmd.Parameters.AddWithValue("@pointsAmt", prsModel.pointsAmt ?? "");
                        cmd.Parameters.AddWithValue("@userId", prsModel.userId ?? "");
                        cmd.Parameters.AddWithValue("@otpVerify", prsModel.otpVerify ?? "");

                        for (int ii = 0; ii < prsModel.OrderBookingdtlV2.Count; ii++)
                        {
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {


                                string query1 = "insert into tbl_franchise_orderbooking_grn_dtl_v1 values (@ccomcode,@cloccode,@corgcode,@clineno,@cfincode,@cdoctype," +
                                                            "@ndocno," +

                                                            "@nseqno,@niseqno,@Product_code,@Product_name,@BISMT,@Pack_size,@Quantity,@Price,@Discount_val,@pseqno,@flag)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@ccomcode", prsModel.OrderBookingdtlV2[ii].ccomcode ?? "");
                                    cmd1.Parameters.AddWithValue("@cloccode", prsModel.OrderBookingdtlV2[ii].cloccode ?? "");
                                    cmd1.Parameters.AddWithValue("@corgcode", prsModel.OrderBookingdtlV2[ii].corgcode ?? "");
                                    cmd1.Parameters.AddWithValue("@clineno", prsModel.OrderBookingdtlV2[ii].clineno ?? "");
                                    cmd1.Parameters.AddWithValue("@cfincode", prsModel.OrderBookingdtlV2[ii].cfincode ?? "");
                                    cmd1.Parameters.AddWithValue("@cdoctype", prsModel.OrderBookingdtlV2[ii].cdoctype);
                                    cmd1.Parameters.AddWithValue("@ndocno", maxno);
                                    cmd1.Parameters.AddWithValue("@nseqno", prsModel.OrderBookingdtlV2[ii].nseqno);
                                    cmd1.Parameters.AddWithValue("@niseqno", prsModel.OrderBookingdtlV2[ii].niseqno);
                                    cmd1.Parameters.AddWithValue("@Product_code", prsModel.OrderBookingdtlV2[ii].Product_code);
                                    cmd1.Parameters.AddWithValue("@Product_name", prsModel.OrderBookingdtlV2[ii].Product_name);
                                    cmd1.Parameters.AddWithValue("@BISMT", prsModel.OrderBookingdtlV2[ii].BISMT);
                                    cmd1.Parameters.AddWithValue("@Pack_size", prsModel.OrderBookingdtlV2[ii].Pack_size);
                                    cmd1.Parameters.AddWithValue("@Quantity", prsModel.OrderBookingdtlV2[ii].Quantity);
                                    cmd1.Parameters.AddWithValue("@Price", prsModel.OrderBookingdtlV2[ii].Price);
                                    cmd1.Parameters.AddWithValue("@Discount_val", prsModel.OrderBookingdtlV2[ii].Discount_val);
                                    cmd1.Parameters.AddWithValue("@pseqno", prsModel.OrderBookingdtlV2[ii].pseqno);
                                    cmd1.Parameters.AddWithValue("@flag", prsModel.OrderBookingdtlV2[ii].flag);



                                    for (int Tii = 0; Tii < prsModel.OrderBookingdtlV2[ii].OrderSchedule.Count; Tii++)
                                    {
                                        using (SqlConnection con2 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                                        {
                                            {
                                                string query2 = "insert into tbl_franchise_order_booking_Schedule_dtl values (@ccomcode,@cloccode,@corgcode," +
                                                                           "@clineno," +
                                                                           "@cfincode,@cdoctype,@ndocno,@nseqno,@niseqno,@cmaterialcode," +
                                                                           "@cproductname,@cpackcode,@nOrdQty,@nSch1Qty,@cSch1Date,@nSch2Qty,@cSch2Date," +
                                                                           "@nSch3Qty,@cSch3Date,@nSch4Qty,@cSch4Date,@ccreatedby,@ccreateddate,@cremarks)";
                                                using (SqlCommand cmd2 = new SqlCommand(query2, con2))
                                                {
                                                    cmd2.Parameters.AddWithValue("@ccomcode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].ccomcode ?? "");
                                                    cmd2.Parameters.AddWithValue("@cloccode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cloccode ?? "");
                                                    cmd2.Parameters.AddWithValue("@corgcode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].corgcode ?? "");
                                                    cmd2.Parameters.AddWithValue("@clineno", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].clineno ?? "");
                                                    cmd2.Parameters.AddWithValue("@cfincode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cfincode ?? "");
                                                    cmd2.Parameters.AddWithValue("@cdoctype", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cdoctype);
                                                    cmd2.Parameters.AddWithValue("@ndocno", maxno);
                                                    cmd2.Parameters.AddWithValue("@nseqno", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nseqno);
                                                    cmd2.Parameters.AddWithValue("@niseqno", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].niseqno);
                                                    cmd2.Parameters.AddWithValue("@cmaterialcode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cmaterialcode);
                                                    cmd2.Parameters.AddWithValue("@cproductname", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cproductname);
                                                    cmd2.Parameters.AddWithValue("@cpackcode", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cpackcode);
                                                    cmd2.Parameters.AddWithValue("@nOrdQty", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nOrdQty);
                                                    cmd2.Parameters.AddWithValue("@nSch1Qty", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nSch1Qty);
                                                    cmd2.Parameters.AddWithValue("@cSch1Date", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cSch1Date);
                                                    cmd2.Parameters.AddWithValue("@nSch2Qty", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nSch2Qty);
                                                    cmd2.Parameters.AddWithValue("@cSch2Date", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cSch2Date);
                                                    cmd2.Parameters.AddWithValue("@nSch3Qty", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nSch3Qty);
                                                    cmd2.Parameters.AddWithValue("@cSch3Date", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cSch3Date);
                                                    cmd2.Parameters.AddWithValue("@nSch4Qty", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].nSch4Qty);
                                                    cmd2.Parameters.AddWithValue("@cSch4Date", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cSch4Date);
                                                    cmd2.Parameters.AddWithValue("@ccreatedby", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].ccreatedby);
                                                    cmd2.Parameters.AddWithValue("@ccreateddate", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].ccreateddate);
                                                    cmd2.Parameters.AddWithValue("@cremarks", prsModel.OrderBookingdtlV2[ii].OrderSchedule[Tii].cremarks);

                                                    await con2.OpenAsync();
                                                    int iiii = await cmd2.ExecuteNonQueryAsync();
                                                    if (iiii > 0)
                                                    {

                                                    }
                                                    await con2.CloseAsync();
                                                }
                                            }
                                        }



                                        await con1.OpenAsync();
                                        //int iii = cmd1.ExecuteNonQuery();
                                        int iii = await cmd1.ExecuteNonQueryAsync();
                                        if (iii > 0)
                                        {

                                        }
                                        await con1.CloseAsync();
                                    }
                                }
                            }
                        }
                        await con.OpenAsync();
                        //int i = cmd.ExecuteNonQuery();
                        int i = await cmd.ExecuteNonQueryAsync();

                        if (i > 0)
                        {
                            var data = prsModel.cfincode.Split('-')[0].Split('0')[1];
                            var OrderNumber = prsModel.cloccode + "F" + data + prsModel.cdoctype + maxno + "-1";

                            var response11 = new Models.ApiResponse1
                            {
                                ndocno = maxno,
                                Message = "Order Created Sucessfully"
                            };

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    using (HttpClient client = new HttpClient())
                                    {
                                        var prm = new Param
                                        {
                                        };
                                        DataSet ds = new DataSet();
                                        string query = "SP_get_ALPN_Franchise_OrderBooking";
                                        using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                                        {
                                            using (SqlCommand cmd = new SqlCommand(query))
                                            {
                                                cmd.Connection = con;
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                                                cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                                                cmd.Parameters.AddWithValue("@FilterValue3", "Book_order1");
                                                cmd.Parameters.AddWithValue("@FilterValue4", prsModel.ndocno.ToString());
                                                cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                                                cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                                                cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                                                cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                                                cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                                                cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                                                cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                                                cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                                                cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                                                cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                                                cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                                                cmd.CommandTimeout = 80000;

                                                await con.OpenAsync();

                                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                                await Task.Run(() => adapter.Fill(ds)); // Run synchronously on a separate thread

                                                await con.CloseAsync();

                                            }
                                        }
                                        string op = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                                    }

                                    using (HttpClient client12 = new HttpClient())
                                    {
                                        Username = "sureshbv@sheenlac.in";
                                        Password = "admin123";

                                        Token token = new Token();
                                        HttpClientHandler handler = new HttpClientHandler();
                                        HttpClient client = new HttpClient(handler);
                                        var RequestBody = new Dictionary<string, string>
                                        {
                                            {"username", Username},
                                            {"password", Password},
                                        };
                                        var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(RequestBody)).Result;

                                        if (tokenResponse.IsSuccessStatusCode)
                                        {
                                            var JsonContent = tokenResponse.Content.ReadAsStringAsync().Result;

                                            JObject studentObj = JObject.Parse(JsonContent);

                                            var result = JObject.Parse(JsonContent);
                                            var items = result["data"].Children().ToList();

                                            token.access_token = (string)items[0];
                                            token.Error = null;
                                        }
                                        else
                                        {
                                            token.Error = "Not able to generate Access Token Invalid usrename or password";
                                        }

                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

                                        string myjson1 = "";

                                        var data1 = new System.Net.Http.StringContent(myjson1, Encoding.UTF8, "application/json");            // dev
                                        var url = "http://13.233.6.115/api/v2/billException/gettokendata";
                                        var response = await client.PostAsync(url, data1);
                                        string result7 = response.Content.ReadAsStringAsync().Result;

                                        var result1 = JObject.Parse(result7);
                                        var items1 = result1["data"].Children().ToList();

                                        var jsonString2 = Newtonsoft.Json.JsonConvert.SerializeObject(items1[0]);

                                        jsonString2 = jsonString2.Replace("\"access\":", "");

                                        Username = "sureshbv@sheenlac.in";
                                        Password = "admin123";

                                        var deviceIds = new List<string>
                                        {
                                            "dRjOP-TvTq6TdUlwadYoA6:APA91bHt0hozCtriFnToxC9vVBZAKoQQp_U46tLDUZEQ0nU8t8nLibV9RwEkMvvSwKQKYzwKjPoQqE9p-vDyBswyoM1KnWE2Qu7Xcuil_i9hPoEfs6iECy8"
                                        };

                                        var data3 = prsModel.cfincode.Split('-')[0].Split('0')[1];
                                        for (int j = 0; j < deviceIds.Count; j++)
                                        {
                                            mobMessage Nmodel = new mobMessage();
                                            Nmodel.token = deviceIds[j];
                                            Nmodel.notification = new dnotification();
                                            Nmodel.notification.body = "Order:" + prsModel.cloccode + "F" + data3 + prsModel.cdoctype + maxno + "-1  has been placed by " + prsModel.ccustomername + " via the AllPaints eCommerce site.\nPlease review the order history for full details regarding this order.";
                                            Nmodel.notification.title = "AllPaints - Order Confirmation";
                                            string op1 = "{message:" + JsonConvert.SerializeObject(Nmodel, Formatting.Indented) + "}";

                                            HttpClientHandler handler1 = new HttpClientHandler();
                                            HttpClient client1 = new HttpClient(handler1);
                                            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonString2);
                                            var data2 = new System.Net.Http.StringContent(op1, Encoding.UTF8, "application/json");            // dev
                                            var url1 = "https://fcm.googleapis.com/v1/projects/sheenlacnotifications/messages:send";
                                            var response1 = await client1.PostAsync(url1, data2);
                                        }


                                    }

                                    using (HttpClient client13 = new HttpClient())
                                    {
                                        var prm = new Param { };
                                        DataSet ds = new DataSet();
                                        string query = "SP_get_ALPN_Franchise_OrderBooking";
                                        using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                                        {

                                            using (SqlCommand cmd = new SqlCommand(query))
                                            {
                                                cmd.Connection = con;
                                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                                                cmd.Parameters.AddWithValue("@FilterValue2", "All_Paints_Royapettah");
                                                cmd.Parameters.AddWithValue("@FilterValue3", "Printorder_cotation");
                                                cmd.Parameters.AddWithValue("@FilterValue4", OrderNumber);
                                                cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                                                cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                                                cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                                                cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                                                cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                                                cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                                                cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                                                cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                                                cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                                                cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                                                cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                                                cmd.CommandTimeout = 80000;
                                                con.Open();


                                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                                adapter.Fill(ds);
                                                con.Close();
                                            }
                                        }

                                        string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
                                        var model2 = JsonConvert.DeserializeObject<List<emailDtls>>(op);

                                        StringBuilder rows = new StringBuilder();

                                        var tbodyBuilder = new StringBuilder();
                                        var tbodyBuilder1 = new StringBuilder();
                                        bool alternate = false;

                                        var splitName = model2[0].shipping_address.Split("~");

                                        foreach (var item in model2)
                                        {
                                            double value = double.Parse(item.total_amount);
                                            int NetValue = (int)Math.Round(value);
                                            string rowStyle = alternate ? "background-color: #f9f9f9;" : "";
                                            tbodyBuilder.AppendLine($@"
                                            <tr style='{rowStyle}'>
                                                <td style='border: 1px solid #ddd;'>{item.ProductName}</td>
                                                <td style='border: 1px solid #ddd;'>{item.Qty}</td>
                                                <td style='border: 1px solid #ddd;'>₹{NetValue}</td>
                                            </tr>");
                                            alternate = !alternate;
                                        }

                                        double totalDiscount = double.Parse(model2[0].total_discount);
                                        int totalDiscount1 = (int)Math.Round(totalDiscount);
                                        if (totalDiscount1 != 0)
                                        {
                                            tbodyBuilder1.AppendLine($@"
                                               <div style='margin: 20px auto; max-width: 550px; padding: 15px; background-color: #e6f7e6; border: 1px solid #b2d8b2; border-radius: 8px; text-align: center;'>
                                                   <p style='font-size: 16px; color: #2e7d32; margin: 0;'>
                                                       <strong>Congratulations! You have saved ₹{totalDiscount1} by choosing Allpaints</strong>
                                                   </p>
                                               </div>");
                                        }
                                        DateTime fromDate = DateTime.Now;
                                        string OrderDate = fromDate.ToString("dd-MM-yyyy HH:mm");

                                        string htmlBody = $@"
                                            <div style='background-color: #f5f5f5; padding: 40px 0;'>
                                              <div style='font-family: Arial, sans-serif; max-width: 700px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);'>
                                                <img src='https://i.ibb.co/CKzNb5XZ/allpaints-Logo-tagline.png' alt='Allpaints Logo' style='display: block; margin: 0 auto 30px auto; height: 70px;' />
                                                <p style='font-size: 16px; color: #333;'>Hi {prsModel.ccustomername},</p>
                                                <p style='font-size: 16px; color: #333;'>We’re happy to let you know that we’ve received your order <strong> {OrderNumber}</strong>, placed on <strong>{OrderDate}</strong>. Here are the details of your purchase:</p>
    
                                                <h3 style='color: #444;'>🧾 Order Summary:</h3>
                                                <table cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial, sans-serif; width: 100%; font-size: 15px;'>
                                                  <thead>
                                                    <tr style='background-color: #f0f0f0; text-align: left;'>
                                                      <th style='border: 1px solid #ddd;'>Product</th>
                                                      <th style='border: 1px solid #ddd;'>Quantity</th>
                                                      <th style='border: 1px solid #ddd;'>Net Price</th>
                                                    </tr>
                                                  </thead>
                                                  <tbody>
                                                    {tbodyBuilder}
                                                  </tbody>
                                                </table>

                                                {tbodyBuilder1}

                                                <table width='100%' cellpadding='0' cellspacing='0' style='margin-top: 20px; font-family: Arial, sans-serif; font-size: 16px;'>
                                                  <tr>
                                                    <td><strong>Total Amount:</strong>  ₹{model2[0].Final_total}</td>
                                                    <td align='right'><strong>Delivery Type:</strong>  {model2[0].plant}</td>
                                                  </tr>
                                                  <tr>
                                                    <td colspan='2' style='font-size: 14px; color: #555;'>(inclusive of all taxes)</td>
                                                  </tr>
                                                  <tr>
                                                    <td><strong>Payment Mode:</strong>   {model2[0].ordertype}</td>
                                                    <td align='right'><strong>Order Status:</strong>  Confirmed</td>
                                                  </tr>
                                                  <tr>
                                                    <td><strong>Order Type:</strong>  {model2[0].oderfrom}</td>
                                                 </tr>
                                                </table>

                                                <p style='margin-top: 25px; font-size: 16px; color: red; text-align: center;'><strong>Thank you for choosing Allpaints!</strong></p>
                                                <img src='https://i.ibb.co/KxSC7HnK/imagecaro2.png' alt='Allpaints Footer Banner' style='display: block; margin-top: 30px; width: 100%; border-radius: 6px;' />
                                              </div>
                                            </div>";

                                        MailMessage mail = new MailMessage();
                                        mail.To.Add("mohankumar@itworks1ders.com");
                                        mail.CC.Add("mohankumar@itworks1ders.com");
                                        mail.Bcc.Add("vikram@itworks1ders.com");
                                        mail.From = new MailAddress("scm@allpaints.in");
                                        mail.Subject = "Your Order " + OrderNumber + " Has Been Placed Successfully!";
                                        mail.Body = htmlBody;
                                        mail.IsBodyHtml = true;
                                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                                        smtp.EnableSsl = true;
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential("scm@allpaints.in", "gucbbbjhxsfovatf");
                                        smtp.Timeout = 60000;
                                        smtp.Send(mail);
                                        //return NoContent();

                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception in background API call: " + ex.Message);
                                }
                            });
                            string response10 = System.Text.Json.JsonSerializer.Serialize(response11);
                            var encryptedJson = AesEncryption.Encrypt(response10);
                            return new JsonResult(encryptedJson);
                        }
                        await con.CloseAsync();
                    }

                }

                return BadRequest();

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [Route("customerGenOTP")]
        [HttpPost]
        public async Task<IActionResult> customerGenOTP(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.customerGenOTP(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [Route("AllPaintsOrderData")]
        [HttpPost]
        public async Task<IActionResult> AllPaintsOrderData(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.AllPaintsOrderData(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [Route("GatwayPaymentProcess")]
        [HttpPost]
        public async Task<IActionResult> GatwayPaymentProcess(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.GatwayPaymentProcess(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [Route("GatwayPaymentDtls")]
        [HttpPost]
        public async Task<IActionResult> GatwayPaymentDtls(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.GatwayPaymentDtls(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Route("PainterGenerateOtp")]
        [HttpPost]
        public async Task<IActionResult> PainterGenerateOtp(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.PainterGenerateOtp(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("PainterOTPVerify")]
        [HttpPost]
        public async Task<IActionResult> PainterOTPVerify(dynamic prm)
        {
            try
            {
                //string json = prm.ToString();
                var encryptedResult = await _ecomService.PainterOTPVerify(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
