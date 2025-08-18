//using AllPaintsEcomAPI.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace AllPaintsEcomAPI.DTO
{
    public class createDtls
    {

        public List<cartItem>? cartItem { get; set; }
        public List<suggestItem>? suggestItem { get; set; }
        public List<compareItem>? compareItem { get; set; }
    }
    public class cartItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class suggestItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class compareItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }


    //public class DecryptIntModelBinder : IModelBinder
    //{
    //    public Task BindModelAsync(ModelBindingContext bindingContext)
    //    {
    //        var valResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
    //        if (valResult == ValueProviderResult.None)
    //            return Task.CompletedTask;

    //        var encrypted = valResult.FirstValue;
    //        try
    //        {
    //            string decrypted = AesEncryption.Decrypt(encrypted);
    //            if (int.TryParse(decrypted.Trim(), out int qty))
    //                bindingContext.Result = ModelBindingResult.Success(qty);
    //            else
    //                bindingContext.ModelState.AddModelError(bindingContext.FieldName, "Invalid qty");
    //        }
    //        catch
    //        {
    //            bindingContext.ModelState.AddModelError(bindingContext.FieldName, "Decryption failed");
    //        }
    //        return Task.CompletedTask;
    //    }
    //}


    public class updateCart
    {
        public string? user_code { get; set; }
        public string? materialCode { get; set; }
        public string? ecomCode { get; set; }
        public string? packSize { get; set; }
        public string? qty { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class deleteCart
    {
        public string? user_code { get; set; }
        public string? materialCode { get; set; }
        public string? unique_cart { get; set; }
        public List<string>? unique_compare { get; set; }
        public string? unique_suggest { get; set; }
    }


    public class ResponseDataModel
    {
        public List<cartItem> cartItems { get; set; }
        public List<suggestItem> suggestItem { get; set; }
        public List<compareItem> compareItem { get; set; }
    }

    public class ApiGetResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int count { get; set; }
        public ResponseDataModel data { get; set; }

        public string Error { get; set; }
    }



    public static class ApiUsageTracker1
    {
        public static int TotalRequests = 0;
        public static HashSet<string> UniqueUsers = new HashSet<string>();
    }

    public class ApiUsageLog1
    {
        public int Id { get; set; }
        public string UserCode { get; set; }
        public string Endpoint { get; set; }
        public DateTime AccessTime { get; set; }
    }

    public class Param
    {
        public string? filtervalue1 { get; set; }
        public string? filtervalue2 { get; set; }
        public string? filtervalue3 { get; set; }
        public string? filtervalue4 { get; set; }
        public string? filtervalue5 { get; set; }
        public string? filtervalue6 { get; set; }
        public string? filtervalue7 { get; set; }
        public string? filtervalue8 { get; set; }
        public string? filtervalue9 { get; set; }
        public string? filtervalue10 { get; set; }
        public string? filtervalue11 { get; set; }
        public string? filtervalue12 { get; set; }
        public string? filtervalue13 { get; set; }
        public string? filtervalue14 { get; set; }
        public string? filtervalue15 { get; set; }
    }

    public class OrderMarge
    {
        public string? ccomcode { get; set; }
        public string? cloccode { get; set; }
        public string? corgcode { get; set; }
        public string? clineno { get; set; }
        public string? cfincode { get; set; }
        public string? cdoctype { get; set; }
        public int ndocno { get; set; }
        public DateTime? ldocdate { get; set; }
        public string? ccustomercode { get; set; }
        public string? ccustomername { get; set; }
        public Decimal? navailablelimit { get; set; }
        public string? npaymentterms { get; set; }
        public string? cdistributorcode { get; set; }
        public string? cdistributorname { get; set; }
        public string? cprocessflag { get; set; }
        public Decimal? ntotordervalue { get; set; }
        public Decimal? ntotdiscountvalue { get; set; }
        public Decimal? nnetordervalue { get; set; }
        public string? cdiscounttype1 { get; set; }
        public Decimal? ndistype1value { get; set; }
        public string? cdiscounttype2 { get; set; }
        public Decimal? ndistype2value { get; set; }
        public string? cdiscounttype3 { get; set; }
        public Decimal? ndistype3value { get; set; }
        public string? cdiscounttype4 { get; set; }
        public Decimal? ndistype4value { get; set; }
        public Decimal? nsmpercent { get; set; }
        public Decimal? nsmvalue { get; set; }
        public Decimal? nnetmarginpercent { get; set; }
        public Decimal? nnetmarginvalue { get; set; }
        public string? nincoterms { get; set; }
        public string? corderremarks { get; set; }
        public DateTime? cdeliverydate { get; set; }
        public string? isdeleivered { get; set; }
        public string? isredemption { get; set; }
        public string? isdelschedule { get; set; }
        public string? ccreatedby { get; set; }
        public DateTime lcreateddate { get; set; }
        public string? cmodifedby { get; set; }
        public DateTime? lmodifieddate { get; set; }
        public string? corderchannel { get; set; }
        public string? cremarks1 { get; set; }
        public string? cremarks2 { get; set; }
        public string? cremarks3 { get; set; }
        public DateTime? cexpirydate { get; set; }

        public string? mobileNumber { get; set; }
        public string? pointsAmt { get; set; }
        public string? userId { get; set; }
        public string? otpVerify { get; set; }
        public List<OrderBookingdtlV2>? OrderBookingdtlV2 { get; set; }
    }
    public class OrderBookingdtlV2
    {
        public string? ccomcode { get; set; }
        public string? cloccode { get; set; }
        public string? corgcode { get; set; }
        public string? clineno { get; set; }
        public string? cfincode { get; set; }
        public string? cdoctype { get; set; }
        public int ndocno { get; set; }
        public int nseqno { get; set; }
        public int? niseqno { get; set; }
        public string? Product_code { get; set; }
        public string? Product_name { get; set; }
        public string? BISMT { get; set; }
        public string? Pack_size { get; set; }
        public Decimal? Quantity { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? Discount_val { get; set; }
        public int pseqno { get; set; }
        public string? flag { get; set; }
        public List<OrderSchedule>? OrderSchedule { get; set; }

    }

    public class OrderSchedule
    {
        public string? ccomcode { get; set; }
        public string? cloccode { get; set; }
        public string? corgcode { get; set; }
        public string? clineno { get; set; }
        public string? cfincode { get; set; }
        public string? cdoctype { get; set; }
        public int ndocno { get; set; }
        public int nseqno { get; set; }
        public int? niseqno { get; set; }
        public string? cmaterialcode { get; set; }
        public string? cproductname { get; set; }
        public string? cpackcode { get; set; }
        public Decimal? nOrdQty { get; set; }
        public Decimal? nSch1Qty { get; set; }
        public DateTime? cSch1Date { get; set; }
        public Decimal? nSch2Qty { get; set; }
        public DateTime? cSch2Date { get; set; }
        public Decimal nSch3Qty { get; set; }
        public DateTime cSch3Date { get; set; }
        public Decimal? nSch4Qty { get; set; }
        public DateTime? cSch4Date { get; set; }
        public string? ccreatedby { get; set; }
        public DateTime? ccreateddate { get; set; }
        public string? cremarks { get; set; }

    }

    public class Token
    {
        //[JsonProperty("access_token")]
        public string access_token { get; set; }
        public string Error { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
    }

    public class emailDtls
    {
        public string? ProductName { get; set; }
        public string? Qty { get; set; }
        public string? NetAmt { get; set; }
        public string? Final_total { get; set; }
        public string? shipping_address { get; set; }
        public string? plant { get; set; }
        public string? total_discount { get; set; }
        public string? total_amount { get; set; }
        public string? oderfrom { get; set; }
        public string? ordertype { get; set; }
    }

    public class dnotification
    {
        public string body { get; set; }
        public string title { get; set; }
    }

    public class mobMessage
    {
        public string token { get; set; }
        public dnotification notification { get; set; }
    }

    public class createCustomerMadel
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? mobile { get; set; }
        public string? mobile2 { get; set; }
        public string? dateOfBirth { get; set; }
        public string? customerCode { get; set; }
        public string? id_proff { get; set; }
        public string? email { get; set; }
        public string? gender { get; set; }
        public string? address { get; set; }
        public string? state { get; set; }
        public string? city { get; set; }
        public string? pincode { get; set; }
        public string? gstNumber { get; set; }
        public string? createdBy { get; set; }
        public DateTime? createdAt { get; set; }
        public string? updatedBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? distributor_code { get; set; }
    }

    public class ApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class PainterDtls
    {
        public int Status { get; set; }
        public List<UserData> Data { get; set; }
        public string Message { get; set; }
    }

    public class UserData
    {
        public string _id { get; set; }
        public string Id { get; set; }
        public string MobileNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pincode { get; set; }
        public Address Address { get; set; }
        public string BpNumber { get; set; }
    }

    public class Address
    {
        public string Pincode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }

    public class MisResponseStatus
    {
        public string StatusCode { get; set; }
        public string Item { get; set; }
        public string ItemCount { get; set; }
        public string ItemPageing { get; set; }
        public string response { get; set; }
        public string token { get; set; }
        public string responseheader { get; set; }
        public string responsedetails { get; set; }
        public string responseattachments { get; set; }
        public string gudid { get; set; }

    }
    public class OtpVerifyed
    {
        public string? Column1 { get; set; }
    }


}
